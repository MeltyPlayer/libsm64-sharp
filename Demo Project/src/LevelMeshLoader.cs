using Assimp;

using libsm64sharp;


namespace demo;

public class LevelMeshLoader {
  public (Scene, ISm64StaticCollisionMesh) LoadAndCreateCollisionMesh(
      ISm64Context sm64Context) {
    var assimpContext = new AssimpContext();
    var assimpScene =
        assimpContext.ImportFile("resources/mesh/Bob-omb Battlefield.obj");

    var sm64StaticCollisionMeshBuilder =
        sm64Context.CreateStaticCollisionMesh();

    var scale = Constants.LEVEL_SCALE;

    foreach (var assimpMesh in assimpScene.Meshes) {
      foreach (var assimpFace in assimpMesh.Faces) {
        var assimpVertex0 = assimpMesh.Vertices[assimpFace.Indices[0]];
        var assimpVertex1 = assimpMesh.Vertices[assimpFace.Indices[1]];
        var assimpVertex2 = assimpMesh.Vertices[assimpFace.Indices[2]];

        sm64StaticCollisionMeshBuilder.AddTriangle(
            Sm64SurfaceType.SURFACE_DEFAULT,
            Sm64TerrainType.TERRAIN_GRASS,
            LevelMeshLoader.ConvertVector_(assimpVertex0, scale),
            LevelMeshLoader.ConvertVector_(assimpVertex2, scale),
            LevelMeshLoader.ConvertVector_(assimpVertex1, scale)
        );
      }
    }

    new AssimpNormalSmoother().SmoothNormalsInScene(assimpScene);

    return (assimpScene, sm64StaticCollisionMeshBuilder.Build());
  }

  private static (short, short, short) ConvertVector_(
      Vector3D vector,
      float scale)
    => ((short) (vector.X * scale),
        (short) (vector.Y * scale),
        (short) (vector.Z * scale));
}