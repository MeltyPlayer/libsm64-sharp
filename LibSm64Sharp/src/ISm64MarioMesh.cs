using System.Numerics;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;


namespace libsm64sharp;

public interface ISm64MarioMesh {
  Image<Rgba32> Texture { get; }

  ISm64MarioMeshTrianglesData? TriangleData { get; }
}

public interface ISm64MarioMeshTrianglesData {
  int TriangleCount { get; }

  IReadOnlyList<Vector3> Positions { get; }
  IReadOnlyList<Vector3> Normals { get; }
  IReadOnlyList<Vector3> Colors { get; }
  IReadOnlyList<Vector2> Uvs { get; }
}