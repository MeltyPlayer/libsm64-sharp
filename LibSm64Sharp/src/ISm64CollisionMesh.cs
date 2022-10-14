﻿namespace libsm64sharp {
  public interface ISm64CollisionMeshBuilder<TSelf, TBuildType>
      where TSelf : ISm64CollisionMeshBuilder<TSelf, TBuildType>
      where TBuildType : ISm64CollisionMesh {
    TBuildType Build();

    TSelf AddTriangle(
        Sm64SurfaceType surfaceType,
        Sm64TerrainType terrainType,
        IReadOnlyList<(float x, float y, float z)> vertices);
  }

  public interface ISm64CollisionMesh {
    IReadOnlyList<ISm64Triangle> Triangles { get; }
  }
}