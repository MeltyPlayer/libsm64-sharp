using demo.common.gl;

using libsm64sharp;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Vector3 = System.Numerics.Vector3;


namespace demo.mesh;

public class MarioMeshRenderer {
  private readonly ISm64Context context_;
  private readonly ISm64Mario mario_;

  private GlShaderProgram? texturedShaderProgram_;
  private int texture0Location_;
  private int useLightingLocation_;

  private GlTexture? marioTexture_;
  private GlTexture? shadowTexture_;

  public MarioMeshRenderer(
      ISm64Context context,
      ISm64Mario mario) {
    this.context_ = context;
    this.mario_ = mario;
  }

  public void Render() {
    this.InitShaderIfNull_();
    this.InitTextureIfNull_();

    this.RenderMario_();
    this.RenderMarioShadow_();
  }

  private void InitShaderIfNull_() {
    if (this.texturedShaderProgram_ != null) {
      return;
    }

    var vertexShaderSrc = @"
# version 120
varying vec4 vertexColor;
varying vec3 vertexNormal;
varying vec2 uv0;
void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    vertexColor = gl_Color;
    uv0 = gl_MultiTexCoord0.st;
}";

    var fragmentShaderSrc = @"
# version 130 
uniform sampler2D texture0;
uniform float useLighting;
out vec4 fragColor;
in vec4 vertexColor;
in vec3 vertexNormal;
in vec2 uv0;
void main() {{
    vec4 texColor = texture(texture0, uv0);

    fragColor.rgb = mix(vertexColor.rgb, texColor.rgb, texColor.a);
    fragColor.a = max(vertexColor.a, texColor.a);

    vec3 diffuseLightNormal = normalize(vec3(.5, -1, .5));
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
  }

  private void InitTextureIfNull_() {
    if (this.marioTexture_ != null) {
      return;
    }

    this.marioTexture_ =
        GlTexture.FromImage(this.mario_.Mesh.Texture, WrapMode.CLAMP,
                            WrapMode.CLAMP);
    this.shadowTexture_ =
        GlTexture.FromFile("resources/shadow.png",
                           WrapMode.CLAMP,
                           WrapMode.CLAMP);
  }

  private void RenderMario_() {
    var triangleData = this.mario_.Mesh.TriangleData;
    if (triangleData == null) {
      return;
    }

    this.texturedShaderProgram_!.Use();
    GL.Uniform1(this.texture0Location_, 0);
    GL.Uniform1(this.useLightingLocation_, 1f);

    this.marioTexture_!.Bind();
    GL.Begin(PrimitiveType.Triangles);

    for (var i = 0; i < triangleData.TriangleCount; ++i) {
      for (var v = 0; v < 3; ++v) {
        var offset = 3 * i + v;

        var vertexNormal = triangleData.Normals[offset];
        GL.Normal3(vertexNormal.X, vertexNormal.Y, vertexNormal.Z);

        var vertexColor = triangleData.Colors[offset];
        var sc = 1;
        GL.Color3(sc * vertexColor.X, sc * vertexColor.Y, sc * vertexColor.Z);

        var vertexUv = triangleData.Uvs[offset];
        GL.TexCoord2(vertexUv.X, vertexUv.Y);

        var vertexPosition = triangleData.Positions[offset];
        GL.Vertex3(vertexPosition.X, vertexPosition.Y, vertexPosition.Z);
      }
    }

    GL.End();
    this.marioTexture_!.Unbind();
  }

  private void RenderMarioShadow_() {
    var position = this.mario_.Position;

    var marioX = position.X;
    var marioY = position.Y;
    var marioZ = position.Z;

    var floorY = this.context_.FindFloor(marioX, marioY, marioZ,
                                         out var floor);
    var shadowY = 3f;

    if (floor == null) {
      return;
    }

    GL.LoadIdentity();
    GL.Translate(marioX, floorY + shadowY, marioZ);

    this.RotateTowardsNormal(floor.Value.normal);

    GL.Color4(0, 0, 0, 0);

    var size = 64;

    this.shadowTexture_.Bind();
    GL.Begin(PrimitiveType.Quads);
    {
      GL.TexCoord2(0, 0);
      GL.Vertex3(-size, 0, -size);

      GL.TexCoord2(0, 1);
      GL.Vertex3(-size, 0, size);

      GL.TexCoord2(1, 1);
      GL.Vertex3(size, 0, size);

      GL.TexCoord2(1, 0);
      GL.Vertex3(size, 0, -size);
    }
    GL.End();
    this.shadowTexture_.Unbind();

    GL.LoadIdentity();
  }

  void RotateTowardsNormal(IReadOnlySm64Vector3f normal) {
    var v1 = new Vector3(0, 1, 0);
    var v2 = new Vector3(normal.X, normal.Y, normal.Z);

    var a = Vector3.Cross(v1, v2);

    var w = MathF.Sqrt(v1.LengthSquared() * v2.LengthSquared()) +
            Vector3.Dot(v1, v2);

    var q = new Quaternion(a.X, a.Y, a.Z, w);

    var m = Matrix4.CreateFromQuaternion(q);

    GL.MultMatrix(ref m);
  }
}