namespace libsm64sharp;

public interface ISm64CollisionMeshBuilder<out TSelf, out TBuildType>
    where TSelf : ISm64CollisionMeshBuilder<TSelf, TBuildType>
    where TBuildType : ISm64CollisionMesh {
  TBuildType Build();

  TSelf AddTriangle(
      Sm64SurfaceType surfaceType,
      Sm64TerrainType terrainType,
      (int x, int y, int z) vertex1,
      (int x, int y, int z) vertex2,
      (int x, int y, int z) vertex3);

  TSelf AddQuad(
      Sm64SurfaceType surfaceType,
      Sm64TerrainType terrainType,
      (int x, int y, int z) vertex1,
      (int x, int y, int z) vertex2,
      (int x, int y, int z) vertex3,
      (int x, int y, int z) vertex4);
}

public interface ISm64CollisionMesh {
  IReadOnlyList<ISm64Triangle> Triangles { get; }
}