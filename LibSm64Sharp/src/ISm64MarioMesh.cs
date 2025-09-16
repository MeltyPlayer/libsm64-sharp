using libsm64sharp.lowlevel;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;


namespace libsm64sharp;

public interface ISm64MarioMesh {
  Image<Rgba32> Texture { get; }

  ISm64MarioMeshTrianglesData? TriangleData { get; }
}

public interface ISm64MarioMeshTrianglesData {
  int TriangleCount { get; }

  IReadOnlyList<LowLevelSm64Vector3f> Positions { get; }
  IReadOnlyList<LowLevelSm64Vector3f> Normals { get; }
  IReadOnlyList<LowLevelSm64Vector3f> Colors { get; }
  IReadOnlyList<LowLevelSm64Vector2f> Uvs { get; }
}