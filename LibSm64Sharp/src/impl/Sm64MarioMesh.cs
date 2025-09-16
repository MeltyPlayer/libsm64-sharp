using libsm64sharp.lowlevel;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;


namespace libsm64sharp;

public partial class Sm64Context {
  private class Sm64MarioMesh : ISm64MarioMesh {
    private readonly Sm64MarioMeshTrianglesData triangleData_;

    public Sm64MarioMesh(Image<Rgba32> texture) {
      this.Texture = texture;
      this.triangleData_ = new Sm64MarioMeshTrianglesData {
          Positions = this.PositionsBuffer,
          Normals = this.NormalsBuffer,
          Colors = this.ColorsBuffer,
          Uvs = this.UvsBuffer,
      };
    }

    public Image<Rgba32> Texture { get; }
    public ISm64MarioMeshTrianglesData? TriangleData { get; private set; }

    public void UpdateTriangleDataFromBuffers(int triangleCount) {
      this.triangleData_.TriangleCount = triangleCount;
      this.TriangleData = this.triangleData_;
    }

    private const int SM64_GEO_MAX_TRIANGLES_ = 1024;
    private const int SM64_GEO_MAX_VERTICES_ = 3 * SM64_GEO_MAX_TRIANGLES_;

    public LowLevelSm64Vector3f[] PositionsBuffer { get; } =
      new LowLevelSm64Vector3f[SM64_GEO_MAX_VERTICES_];

    public LowLevelSm64Vector3f[] NormalsBuffer { get; } =
      new LowLevelSm64Vector3f[SM64_GEO_MAX_VERTICES_];

    public LowLevelSm64Vector3f[] ColorsBuffer { get; } =
      new LowLevelSm64Vector3f[SM64_GEO_MAX_VERTICES_];

    public LowLevelSm64Vector2f[] UvsBuffer { get; } =
      new LowLevelSm64Vector2f[SM64_GEO_MAX_VERTICES_];
  }


  private class Sm64MarioMeshTrianglesData : ISm64MarioMeshTrianglesData {
    public int TriangleCount { get; set; }

    public IReadOnlyList<LowLevelSm64Vector3f> Positions { get; init; }
    public IReadOnlyList<LowLevelSm64Vector3f> Normals { get; init; }
    public IReadOnlyList<LowLevelSm64Vector3f> Colors { get; init; }
    public IReadOnlyList<LowLevelSm64Vector2f> Uvs { get; init; }
  }
}