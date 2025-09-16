using Assimp;

using demo.common.gl;

using OpenTK.Graphics.OpenGL;

using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;


namespace demo.mesh;

public class StaticAssimpSceneRenderer : IRenderable {
  private readonly Scene assimpScene_;

  private GlShaderProgram? texturedShaderProgram_;
  private int texture0Location_;
  private int useLightingLocation_;

  private readonly GlDisplayList glDisplayList_;
  private readonly GlTexture[] glTextures_;

  public StaticAssimpSceneRenderer(AssimpSceneData assimpSceneData) {
    this.assimpScene_ = assimpSceneData.Scene;
    this.glDisplayList_ = new GlDisplayList(this.RenderAssimpScene_);
    this.glTextures_ =
        this.assimpScene_.Materials.Select(assimpMaterial => {
              var textureImage =
                  assimpSceneData.TexturesByMaterial[assimpMaterial];

              return textureImage != null
                         ? GlTexture.FromImage(textureImage, WrapMode.REPEAT, WrapMode.REPEAT)
                         : GlTexture.FromColor(Color.White);
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

    // With 3-point bilinear filtering copied shamelessly from:
    // https://www.shadertoy.com/view/Ws2fWV
    var fragmentShaderSrc = @$"
# version 130 
uniform sampler2D texture0;
uniform float useLighting;
out vec4 fragColor;
in vec4 vertexColor;
in vec3 vertexNormal;
in vec2 uv0;


vec2 norm2denorm(sampler2D tex, vec2 uv)
{{
    return uv * vec2(textureSize(tex, 0)) - 0.5;
}}

ivec2 denorm2idx(vec2 d_uv)
{{
    return ivec2(floor(d_uv));
}}

ivec2 norm2idx(sampler2D tex, vec2 uv)
{{
    return denorm2idx(norm2denorm(tex, uv));
}}

vec2 idx2norm(sampler2D tex, ivec2 idx)
{{
    vec2 denorm_uv = vec2(idx) + 0.5;
    vec2 size = vec2(textureSize(tex, 0));
    return denorm_uv / size;
}}

vec4 texel_fetch(sampler2D tex, ivec2 idx)
{{
    vec2 uv = idx2norm(tex, idx);
    return texture(tex, uv);
}}

#if 0
float find_mipmap_level(in vec2 texture_coordinate) // in texel units
{{
    vec2  dx_vtc        = dFdx(texture_coordinate);
    vec2  dy_vtc        = dFdy(texture_coordinate);
    float delta_max_sqr = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
    float mml = 0.5 * log2(delta_max_sqr);
    return max( 0, mml ); // Thanks @Nims
}}
#endif


/*
 * Unlike Nintendo's documentation, the N64 does not use
 * the 3 closest texels.
 * The texel grid is triangulated:
 *
 *     0 .. 1        0 .. 1
 *   0 +----+      0 +----+
 *     |   /|        |\   |
 *   . |  / |        | \  |
 *   . | /  |        |  \ |
 *     |/   |        |   \|
 *   1 +----+      1 +----+
 *
 * If the sampled point falls above the diagonal,
 * The top triangle is used; otherwise, it's the bottom.
 */

vec4 texture_3point(sampler2D tex, vec2 uv)
{{
    vec2 denorm_uv = norm2denorm(tex, uv);
    ivec2 idx_low = denorm2idx(denorm_uv);
    vec2 ratio = denorm_uv - vec2(idx_low);

#define FLIP_DIAGONAL

#ifndef FLIP_DIAGONAL
    // this uses one diagonal orientation
    #if 0
    // using conditional, might not be optimal
    bool lower_flag = 1.0 < ratio.s + ratio.t;
    ivec2 corner0 = lower_flag ? ivec2(1, 1) : ivec2(0, 0);
    #else
    // using step() function, might be faster
    int lower_flag = int(step(1.0, ratio.s + ratio.t));
    ivec2 corner0 = ivec2(lower_flag, lower_flag);
    #endif
    ivec2 corner1 = ivec2(0, 1);
    ivec2 corner2 = ivec2(1, 0);
#else
    // orient the triangulated mesh diagonals the other way
    #if 0
    bool lower_flag = ratio.s - ratio.t > 0.0;
    ivec2 corner0 = lower_flag ? ivec2(1, 0) : ivec2(0, 1);
    #else
    int lower_flag = int(step(0.0, ratio.s - ratio.t));
    ivec2 corner0 = ivec2(lower_flag, 1 - lower_flag);
    #endif
    ivec2 corner1 = ivec2(0, 0);
    ivec2 corner2 = ivec2(1, 1);
#endif
    ivec2 idx0 = idx_low + corner0;
    ivec2 idx1 = idx_low + corner1;
    ivec2 idx2 = idx_low + corner2;

    vec4 t0 = texel_fetch(tex, idx0);
    vec4 t1 = texel_fetch(tex, idx1);
    vec4 t2 = texel_fetch(tex, idx2);

    // This is standard (Crammer's rule) barycentric coordinates calculation.
    vec2 v0 = vec2(corner1 - corner0);
    vec2 v1 = vec2(corner2 - corner0);
    vec2 v2 = ratio   - vec2(corner0);
    float den = v0.x * v1.y - v1.x * v0.y;
    /*
     * Note: the abs() here is necessary because we don't guarantee
     * the proper order of vertices, so some signed areas are negative.
     * But since we only interpolate inside the triangle, the areas
     * are guaranteed to be positive, if we did the math more carefully.
     */
    float lambda1 = abs((v2.x * v1.y - v1.x * v2.y) / den);
    float lambda2 = abs((v0.x * v2.y - v2.x * v0.y) / den);
    float lambda0 = 1.0 - lambda1 - lambda2;

    return lambda0*t0 + lambda1*t1 + lambda2*t2;
}}


void main() {{
    vec4 texColor = texture_3point(texture0, uv0);
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