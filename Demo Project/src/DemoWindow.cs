using System.Diagnostics;

using demo.gl;

using libsm64sharp;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace demo;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;
  private readonly ISm64Mario sm64Mario_;

  private bool isGlInit_;

  private GlShaderProgram texturedShaderProgram_;
  private int texture0Location_;
  private int useLightingLocation_;

  private GlShaderProgram texturelessShaderProgram_;

  private GlTexture marioTexture_;

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

    this.marioTexture_ = new GlTexture(this.sm64Mario_.Mesh.Texture);
  }

  protected override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);
    this.sm64Mario_.Tick();
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
    var fovY = 30;

    var cameraXNormal = 1;
    var cameraYNormal = -.2f;
    var cameraZNormal = 0;

    var cameraDistance = 500f;

    var marioPos = this.sm64Mario_.Position;
    var centerX = marioPos.X;
    var centerY = marioPos.Y;
    var centerZ = marioPos.Z;

    var cameraX = centerX - cameraDistance * cameraXNormal;
    var cameraY = centerY - cameraDistance * cameraYNormal;
    var cameraZ = centerZ - cameraDistance * cameraZNormal;

    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GlUtil.Perspective(fovY, 1.0 * width / height, 1, 10000);
      GlUtil.LookAt(cameraX, cameraY, cameraZ,
                    centerX, centerY, centerZ,
                    0, 1, 0);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
    }

    var marioMesh = this.sm64Mario_.Mesh;
    var marioMeshTriangleData = marioMesh.TriangleData;
    if (marioMeshTriangleData != null) {
      this.texturedShaderProgram_.Use();
      GL.Uniform1(this.texture0Location_, 0);
      GL.Uniform1(this.useLightingLocation_, 1f);

      this.marioTexture_.Bind();
      GL.Begin(PrimitiveType.Triangles);

      for (var i = 0; i < marioMeshTriangleData.TriangleCount; ++i) {
        for (var v = 0; v < 3; ++v) {
          var offset = 3 * i + v;

          var vertexNormal = marioMeshTriangleData.Normals[offset];
          GL.Normal3(vertexNormal.X, vertexNormal.Y, vertexNormal.Z);

          var vertexColor = marioMeshTriangleData.Colors[offset];
          var sc = 1;
          GL.Color3(sc * vertexColor.X, sc * vertexColor.Y, sc * vertexColor.Z);

          var vertexUv = marioMeshTriangleData.Uvs[offset];
          GL.TexCoord2(vertexUv.X, vertexUv.Y);

          var vertexPosition = marioMeshTriangleData.Positions[offset];
          GL.Vertex3(vertexPosition.X, vertexPosition.Y, vertexPosition.Z);
        }
      }

      GL.End();
      this.marioTexture_.Unbind();
    }

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