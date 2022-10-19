using demo.camera;
using demo.gl;
using demo.mesh;

using libsm64sharp;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace demo;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;
  private readonly ISm64Mario sm64Mario_;

  private bool isGlInit_;

  private readonly MarioMeshRenderer marioMeshRenderer_;

  private GlShaderProgram texturedShaderProgram_;
  private int texture0Location_;
  private int useLightingLocation_;

  private GlShaderProgram texturelessShaderProgram_;

  private  FlyingCamera camera_ = new();
  private readonly float fovY_ = 30;

  private bool isMouseDown_ = false;
  private (int, int)? prevMousePosition_ = null;

  private bool isForwardDown_ = false;
  private bool isBackwardDown_ = false;
  private bool isLeftwardDown_ = false;
  private bool isRightwardDown_ = false;

  public DemoWindow() {
    var sm64RomBytes = File.ReadAllBytes("rom\\sm64.z64");

    this.sm64Context_ = new Sm64Context(sm64RomBytes);

    short floorZ = 0;
    this.sm64Context_.CreateStaticCollisionMesh()
        .AddTriangle(
            Sm64SurfaceType.SURFACE_DEFAULT,
            Sm64TerrainType.TERRAIN_GRASS,
            (2000, floorZ, 0),
            (-2000, floorZ, 0),
            (0, floorZ, 2000)
        )
        .Build();

    this.sm64Mario_ = this.sm64Context_.CreateMario(0, 0, 0);
    this.marioMeshRenderer_ = new MarioMeshRenderer(this.sm64Mario_.Mesh);

    this.MouseDown += (_, args) => {
      if (args.Button == MouseButton.Left) {
        isMouseDown_ = true;
        this.prevMousePosition_ = null;
      }
    };
    this.MouseUp += (_, args) => {
      if (args.Button == MouseButton.Left) {
        isMouseDown_ = false;
      }
    };
    this.MouseMove += (_, args) => {
      if (this.isMouseDown_) {
        var mouseLocation = (args.X, args.Y);

        if (this.prevMousePosition_ != null) {
          var (prevMouseX, prevMouseY) = this.prevMousePosition_.Value;
          var (mouseX, mouseY) = mouseLocation;

          var deltaMouseX = mouseX - prevMouseX;
          var deltaMouseY = mouseY - prevMouseY;

          var fovY = this.fovY_;
          var fovX = fovY / this.Height * this.Width;

          var deltaXFrac = 1f * deltaMouseX / this.Width;
          var deltaYFrac = 1f * deltaMouseY / this.Height;

          var mouseSpeedX = 1;
          var mouseSpeedY = 1;

          this.camera_.Pitch += deltaYFrac * fovY * mouseSpeedY;
          this.camera_.Yaw -= deltaXFrac * fovX * mouseSpeedX;
        }

        this.prevMousePosition_ = mouseLocation;
      }
    };

    this.KeyDown += (_, args) => {
      switch (args.Key) {
        case Key.W: {
          this.isForwardDown_ = true;
          break;
        }
        case Key.S: {
          this.isBackwardDown_ = true;
          break;
        }
        case Key.A: {
          this.isLeftwardDown_ = true;
          break;
        }
        case Key.D: {
          this.isRightwardDown_ = true;
          break;
        }
        case Key.Space: {
          this.sm64Mario_.Gamepad.ScheduleAButton(true);
          break;
        }
      }
    };

    this.KeyUp += (_, args) => {
      switch (args.Key) {
        case Key.W: {
          this.isForwardDown_ = false;
          break;
        }
        case Key.S: {
          this.isBackwardDown_ = false;
          break;
        }
        case Key.A: {
          this.isLeftwardDown_ = false;
          break;
        }
        case Key.D: {
          this.isRightwardDown_ = false;
          break;
        }
        case Key.Space: {
          this.sm64Mario_.Gamepad.ScheduleAButton(false);
          break;
        }
      }
    };
  }

  private void InitGL_() {
    if (this.isGlInit_) {
      return;
    }

    this.isGlInit_ = true;

    GL.ShadeModel(ShadingModel.Smooth);
    GL.Enable(EnableCap.PointSmooth);
    GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

    GL.ClearDepth(5.0F);

    GL.DepthFunc(DepthFunction.Lequal);
    GL.Enable(EnableCap.DepthTest);
    GL.DepthMask(true);

    GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

    GL.Enable(EnableCap.Texture2D);
    GL.Enable(EnableCap.Normalize);

    GL.Enable(EnableCap.CullFace);
    GL.CullFace(CullFaceMode.Back);

    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

    GL.ClearColor(.5f, .5f, .5f, 1);

    var vertexShaderSrc = @"
# version 120
in vec2 in_uv0;
varying vec4 vertexColor;
varying vec3 vertexNormal;
varying vec2 uv0;
void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    vertexColor = gl_Color;
    uv0 = gl_MultiTexCoord0.st;
}";

    var fragmentShaderSrc = @$"
# version 130 
uniform sampler2D texture0;
uniform float useLighting;
out vec4 fragColor;
in vec4 vertexColor;
in vec3 vertexNormal;
in vec2 uv0;
void main() {{
    vec4 texColor = texture(texture0, uv0);
    fragColor = vertexColor;

    if (texColor.a > .95) {{
      fragColor.rgb = texColor.rgb;
    }}

    vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
    float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);
    float ambientLightAmount = .3;
    float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);
    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);
}}";

    this.texturedShaderProgram_ =
        GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

    this.texture0Location_ =
        this.texturedShaderProgram_.GetUniformLocation("texture0");
    this.useLightingLocation_ =
        this.texturedShaderProgram_.GetUniformLocation("useLighting");

    this.texturelessShaderProgram_ =
        GlShaderProgram.FromShaders(@"
# version 120

varying vec4 vertexColor;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
}", @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor;

void main() {
    fragColor = vertexColor;
}");
  }

  protected override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    this.sm64Mario_.Tick();

    var forwardVector =
        (this.isForwardDown_ ? 1 : 0) - (this.isBackwardDown_ ? 1 : 0);
    var rightwardVector =
        (this.isRightwardDown_ ? 1 : 0) - (this.isLeftwardDown_ ? 1 : 0);
    this.camera_.Move(forwardVector, rightwardVector, 15);
  }

  protected override void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);
    this.InitGL_();

    var width = this.Width;
    var height = this.Height;
    GL.Viewport(0, 0, width, height);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    this.RenderPerspective_();

    GL.Flush();
    this.SwapBuffers();
  }

  private void RenderPerspective_() {
    var width = this.Width;
    var height = this.Height;

    /*var cameraXNormal = 1;
    var cameraYNormal = -.2f;
    var cameraZNormal = 0;

    var cameraDistance = 500f;

    var marioPos = this.sm64Mario_.Position;
    var centerX = marioPos.X;
    var centerY = marioPos.Y;
    var centerZ = marioPos.Z;

    var cameraX = centerX - cameraDistance * cameraXNormal;
    var cameraY = centerY - cameraDistance * cameraYNormal;
    var cameraZ = centerZ - cameraDistance * cameraZNormal;*/

    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GlUtil.Perspective(this.fovY_, 1.0 * width / height, 1, 10000);
      GlUtil.LookAt(this.camera_);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
    }

    this.marioMeshRenderer_.Render();

    {
      this.texturelessShaderProgram_.Use();

      GL.Color3(1f, 1f, 1f);

      var floorZ = .5f;
      GL.Begin(PrimitiveType.Triangles);
      GL.Vertex3(2000, floorZ, 0);
      GL.Vertex3(-2000, floorZ, 0);
      GL.Vertex3(0, floorZ, 2000);
      GL.End();
    }
  }
}