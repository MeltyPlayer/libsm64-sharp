using Assimp;

using demo.common.image;
using demo.mesh;

using libsm64sharp;


namespace demo;

public class LevelMeshLoader {
  public (AssimpSceneData, ISm64StaticCollisionMesh) LoadAndCreateCollisionMesh(
      ISm64Context sm64Context) {
    var assimpSceneData =
        AssimpSceneData.Load("resources/mesh/Bob-omb Battlefield.obj");

    var sm64StaticCollisionMeshBuilder =
        sm64Context.CreateStaticCollisionMesh();

    var scale = Constants.LEVEL_SCALE;

    var assimpScene = assimpSceneData.Scene;
    foreach (var assimpMesh in assimpScene.Meshes) {
      var assimpMaterial = assimpScene.Materials[assimpMesh.MaterialIndex];
      var texture = assimpSceneData.TexturesByMaterial[assimpMaterial];

      var isTextureTransparent = ImageUtil.IsImageTransparent(texture);

      foreach (var assimpFace in assimpMesh.Faces) {
        var assimpVertex0 = assimpMesh.Vertices[assimpFace.Indices[0]];
        var assimpVertex1 = assimpMesh.Vertices[assimpFace.Indices[1]];
        var assimpVertex2 = assimpMesh.Vertices[assimpFace.Indices[2]];

        var sm64Vertex0 = LevelMeshLoader.ConvertVector_(assimpVertex0, scale);
        var sm64Vertex1 = LevelMeshLoader.ConvertVector_(assimpVertex1, scale);
        var sm64Vertex2 = LevelMeshLoader.ConvertVector_(assimpVertex2, scale);

        // Add "outside"
        sm64StaticCollisionMeshBuilder.AddTriangle(
            Sm64SurfaceType.SURFACE_DEFAULT,
            Sm64TerrainType.TERRAIN_GRASS,
            sm64Vertex0,
            sm64Vertex2,
            sm64Vertex1
        );

        // TODO: Add "inside" for transparent textures?
        if (isTextureTransparent) {
        }
      }
    }

    new AssimpNormalSmoother().SmoothNormalsInScene(assimpScene);

    return (assimpSceneData, sm64StaticCollisionMeshBuilder.Build());
  }

  private static (int, int, int) ConvertVector_(
      Vector3D vector,
      float scale)
    => ((int) (vector.X * scale),
        (int) (vector.Y * scale),
        (int) (vector.Z * scale));
}