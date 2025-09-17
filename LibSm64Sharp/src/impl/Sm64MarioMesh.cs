using libsm64sharp.lowlevel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;


namespace libsm64sharp;

public partial class Sm64Context {
  private class Sm64MarioMesh : ISm64MarioMesh {
    private readonly Sm64MarioMeshTrianglesData triangleData_;

    public Sm64MarioMesh(Image<Rgba32> texture) {
      this.Texture = texture;
      this.triangleData_ = new Sm64MarioMeshTrianglesData(
          this.PositionsBuffer,
          this.NormalsBuffer,
          this.ColorsBuffer,
          this.UvsBuffer);
    }

    public Image<Rgba32> Texture { get; }
    public ISm64MarioMeshTrianglesData? TriangleData { get; private set; }

    public void UpdateTriangleDataFromBuffers(int triangleCount) {
      this.triangleData_.TriangleCount = triangleCount;
      this.TriangleData = this.triangleData_;
    }

    private const int SM64_GEO_MAX_TRIANGLES_ = 1024;
    private const int SM64_GEO_MAX_VERTICES_ = 3 * SM64_GEO_MAX_TRIANGLES_;

    public Vector3[] PositionsBuffer { get; } =
      new Vector3[SM64_GEO_MAX_VERTICES_];

    public Vector3[] NormalsBuffer { get; } =
      new Vector3[SM64_GEO_MAX_VERTICES_];

    public Vector3[] ColorsBuffer { get; } =
      new Vector3[SM64_GEO_MAX_VERTICES_];

    public Vector2[] UvsBuffer { get; } =
      new Vector2[SM64_GEO_MAX_VERTICES_];
  }


  private class Sm64MarioMeshTrianglesData : ISm64MarioMeshTrianglesData {
    public int TriangleCount { get; set; }

    public Sm64MarioMeshTrianglesData(
        IReadOnlyList<Vector3> positions,
        IReadOnlyList<Vector3> normals,
        IReadOnlyList<Vector3> colors,
        IReadOnlyList<Vector2> uvs) {
      this.Positions = positions;
      this.Normals = normals;
      this.Colors = colors;
      this.Uvs = uvs;
    }

    public IReadOnlyList<Vector3> Positions { get; }
    public IReadOnlyList<Vector3> Normals { get; }
    public IReadOnlyList<Vector3> Colors { get; }
    public IReadOnlyList<Vector2> Uvs { get; }
  }
}