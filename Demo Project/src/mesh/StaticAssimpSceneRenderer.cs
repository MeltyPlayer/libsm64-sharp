using Assimp;

using demo.gl;

using OpenTK.Graphics.OpenGL;

using SixLabors.ImageSharp.PixelFormats;

using Image = SixLabors.ImageSharp.Image;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;


namespace demo.mesh {
  public class StaticAssimpSceneRenderer {
    private readonly Scene assimpScene_;

    private GlShaderProgram? texturedShaderProgram_;
    private int texture0Location_;
    private int useLightingLocation_;

    private readonly GlDisplayList glDisplayList_;
    private readonly GlTexture[] glTextures_;

    public StaticAssimpSceneRenderer(Scene assimpScene) {
      this.assimpScene_ = assimpScene;
      this.glDisplayList_ = new GlDisplayList(this.RenderAssimpScene_);
      this.glTextures_ =
          assimpScene.Materials.Select(assimpMaterial => {
                       var filePath = assimpMaterial.TextureDiffuse.FilePath;

                       if (filePath == null) {
                         return GlTexture.FromColor(Color.White);
                       }

                       // TODO: Fix this so it's not hard-coded
                       var texturePath = Path.Join("resources/mesh", filePath);
                       var textureImage = Image.Load<Rgba32>(texturePath);

                       return GlTexture.FromImage(textureImage);
                     })
                     .ToArray();
    }

    public void Render() {
      this.InitShaderIfNull_();
      this.glDisplayList_.CompileOrRender();
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
    fragColor = vertexColor * texColor;

    vec3 diffuseLightNormal = normalize(vec3(.5, -1, .5));
    float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);
    float ambientLightAmount = .3;
    float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);
    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);

    if (fragColor.a < .95) {{
      discard;
    }}
}}";

      this.texturedShaderProgram_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

      this.texture0Location_ =
          this.texturedShaderProgram_.GetUniformLocation("texture0");
      this.useLightingLocation_ =
          this.texturedShaderProgram_.GetUniformLocation("useLighting");
    }

    private void RenderAssimpScene_() {
      this.texturedShaderProgram_!.Use();
      GL.Uniform1(this.texture0Location_, 0f);
      GL.Uniform1(this.useLightingLocation_, 1f);

      var scale = Constants.LEVEL_SCALE;

      GL.Color3(1f, 1f, 1f);

      var indexOrder = new[] {0, 2, 1};
      foreach (var mesh in this.assimpScene_.Meshes) {
        var texture = this.glTextures_[mesh.MaterialIndex];
        texture.Bind();

        GL.Begin(PrimitiveType.Triangles);

        var uvs = mesh.TextureCoordinateChannels[0];

        foreach (var face in mesh.Faces) {
          foreach (var i in indexOrder) {
            var vertexIndex = face.Indices[i];

            var normal = mesh.Normals[vertexIndex];
            GL.Normal3(-normal.X, -normal.Y, -normal.Z);

            var uv = uvs[vertexIndex];
            GL.TexCoord2(uv.X, 1 - uv.Y);

            var vertex = mesh.Vertices[vertexIndex];
            GL.Vertex3(vertex.X * scale, vertex.Y * scale, vertex.Z * scale);
          }
        }

        GL.End();

        texture.Unbind();
      }
    }
  }
}