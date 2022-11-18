using demo.level;

using libsm64sharp;

using OpenTK;

using Quad64;
using Quad64.Scripts;
using Quad64.src.JSON;
using Quad64.src.LevelInfo;
using Quad64.src.Scripts;


namespace demo {
  public static class Quad64LevelMeshLoader {
    public static Level LoadLevel(LevelId levelId) {
      ROM rom = ROM.Instance;

      var pathToAutoLoadRom = "rom/sm64.z64";
      rom.readFile(pathToAutoLoadRom);

      Globals.objectComboEntries.Clear();
      Globals.behaviorNameEntries.Clear();
      BehaviorNameFile.parseBehaviorNames(
          Globals.getDefaultBehaviorNamesPath());
      ModelComboFile.parseObjectCombos(Globals.getDefaultObjectComboPath());
      rom.setSegment(0x15, Globals.seg15_location[0], Globals.seg15_location[1],
                     false, null);
      rom.setSegment(0x02, Globals.seg02_location[0], Globals.seg02_location[1],
                     rom.isSegmentMIO0(0x02, null), rom.Seg02_isFakeMIO0,
                     rom.Seg02_uncompressedOffset, null);

      var level = new Level((ushort) levelId, 1);
      LevelScripts.parse(ref level, 0x15, 0);
      level.sortAndAddNoModelEntries();
      level.CurrentAreaID = level.Areas[0].AreaID;

      return level;
    }

    public static ISm64StaticCollisionMesh UpdateCollisionMesh(
        ISm64Context sm64Context,
        Area area) {
      var sm64StaticCollisionMeshBuilder =
          sm64Context.CreateStaticCollisionMesh();

      var terrainType = (Sm64TerrainType) area.DefaultTerrainType;
      CopyIntoBuilder<ISm64StaticCollisionMeshBuilder,
          ISm64StaticCollisionMesh>(
          area.collision,
          sm64StaticCollisionMeshBuilder,
          terrainType);

      return sm64StaticCollisionMeshBuilder.Build();
    }

    public static void CopyIntoBuilder<TCollisionMeshBuilder,
                                       TCollisionMesh>(
        CollisionMap collisionMap,
        TCollisionMeshBuilder collisionMeshBuilder,
        Sm64TerrainType terrainType = Sm64TerrainType.TERRAIN_GRASS)
        where TCollisionMeshBuilder : ISm64CollisionMeshBuilder<
            TCollisionMeshBuilder, TCollisionMesh>
        where TCollisionMesh : ISm64CollisionMesh {
      foreach (var collisionTriangleList in collisionMap.triangles) {
        var surfaceType = (Sm64SurfaceType) collisionTriangleList.id;

        var vertices = collisionMap.verts;

        var indices = collisionTriangleList.indices;
        for (var i = 0; i < indices.Length; i += 3) {
          var vertex1 =
              Quad64LevelMeshLoader.ConvertVector_(vertices[indices[i]]);
          var vertex2 =
              Quad64LevelMeshLoader.ConvertVector_(vertices[indices[i + 1]]);
          var vertex3 =
              Quad64LevelMeshLoader.ConvertVector_(vertices[indices[i + 2]]);

          collisionMeshBuilder.AddTriangle(
              surfaceType,
              terrainType,
              vertex1, vertex2, vertex3
          );
        }
      }
    }


    private static (int, int, int) ConvertVector_(Vector3 vector)
      => ((int) vector.X, (int) vector.Y, (int) vector.Z);
  }
}