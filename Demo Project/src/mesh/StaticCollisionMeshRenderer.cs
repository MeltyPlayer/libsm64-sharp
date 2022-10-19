using demo.gl;

using libsm64sharp;

using OpenTK.Graphics.OpenGL;


namespace demo.mesh {
  public class StaticCollisionMeshRenderer {
    private readonly ISm64StaticCollisionMesh collisionMesh_;
    private readonly float minY_ = float.MaxValue;
    private readonly float maxY_ = float.MinValue;

    private GlShaderProgram? texturelessShaderProgram_;
    private int minYLocation_;
    private int maxYLocation_;

    public StaticCollisionMeshRenderer(ISm64StaticCollisionMesh collisionMesh) {
      this.collisionMesh_ = collisionMesh;

      foreach (var triangle in this.collisionMesh_.Triangles) {
        foreach (var vertex in triangle.Vertices) {
          var y = vertex.Y;
          this.minY_ = Math.Min(this.minY_, y);
          this.maxY_ = Math.Max(this.maxY_, y);
        }
      }
    }

    public void Render() {
      this.InitShaderIfNull_();
      this.RenderCollisionMesh_();
    }

    private void InitShaderIfNull_() {
      if (this.texturelessShaderProgram_ != null) {
        return;
      }

      var vertexShaderSrc = @"
# version 120
varying vec3 vertexPosition;
varying vec4 vertexColor;
void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexPosition = gl_Vertex.xyz;
    vertexColor = gl_Color;
}";

      var fragmentShaderSrc = @$"
# version 130 
uniform float minY;
uniform float maxY;
out vec4 fragColor;
in vec3 vertexPosition;
in vec4 vertexColor;
void main() {{
    fragColor = vertexColor;
    fragColor.rgb *= (vertexPosition.y - minY) / (maxY - minY);
}}";

      this.texturelessShaderProgram_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

      this.minYLocation_ =
          this.texturelessShaderProgram_.GetUniformLocation("minY");
      this.maxYLocation_ =
          this.texturelessShaderProgram_.GetUniformLocation("maxY");
    }

    private void RenderCollisionMesh_() {
      this.texturelessShaderProgram_!.Use();
      GL.Uniform1(this.minYLocation_, this.minY_);
      GL.Uniform1(this.maxYLocation_, this.maxY_);

      GL.Begin(PrimitiveType.Triangles);

      GL.Color3(0, 1f, 0);
      foreach (var triangle in this.collisionMesh_.Triangles) {
        foreach (var vertex in triangle.Vertices) {
          GL.Vertex3(vertex.X, vertex.Y, vertex.Z);
        }
      }

      GL.End();
    }
  }
}