using demo.gl;

using libsm64sharp;

using OpenTK.Graphics.OpenGL;


namespace demo.mesh {
  public class MarioMeshRenderer {
    private readonly ISm64MarioMesh mesh_;

    private GlShaderProgram? texturedShaderProgram_;
    private int texture0Location_;
    private int useLightingLocation_;

    private GlTexture? marioTexture_;

    public MarioMeshRenderer(ISm64MarioMesh mesh) {
      this.mesh_ = mesh;
    }

    public void Render() {
      this.InitShaderIfNull_();
      this.InitTextureIfNull_();

      this.RenderMario_();
    }

    private void InitShaderIfNull_() {
      if (this.texturedShaderProgram_ != null) {
        return;
      }

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
    }

    private void InitTextureIfNull_() {
      if (this.marioTexture_ != null) {
        return;
      }

      this.marioTexture_ = GlTexture.FromImage(this.mesh_.Texture);
    }

    private void RenderMario_() {
      var triangleData = this.mesh_.TriangleData;
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
  }
}