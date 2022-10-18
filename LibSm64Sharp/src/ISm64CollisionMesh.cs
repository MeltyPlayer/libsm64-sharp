namespace libsm64sharp {
  public interface ISm64CollisionMeshBuilder<TSelf, TBuildType>
      where TSelf : ISm64CollisionMeshBuilder<TSelf, TBuildType>
      where TBuildType : ISm64CollisionMesh {
    TBuildType Build();

    TSelf AddTriangle(
        Sm64SurfaceType surfaceType,
        Sm64TerrainType terrainType,
        (short x, short y, short z) vertex1,
        (short x, short y, short z) vertex2,
        (short x, short y, short z) vertex3);

    TSelf AddQuad(
        Sm64SurfaceType surfaceType,
        Sm64TerrainType terrainType,
        (short x, short y, short z) vertex1,
        (short x, short y, short z) vertex2,
        (short x, short y, short z) vertex3,
        (short x, short y, short z) vertex4);
  }

  public interface ISm64CollisionMesh {
    IReadOnlyList<ISm64Triangle> Triangles { get; }
  }
}