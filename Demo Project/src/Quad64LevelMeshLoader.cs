using Quad64;
using Quad64.Scripts;
using Quad64.src.JSON;
using Quad64.src.LevelInfo;


namespace demo {
  public static class Quad64LevelMeshLoader {
    public static Level LoadAndCreateCollisionMesh() {
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

      var level = new Level(0x10, 1);
      LevelScripts.parse(ref level, 0x15, 0);
      level.sortAndAddNoModelEntries();
      level.CurrentAreaID = level.Areas[0].AreaID;

      return level;
    }
  }
}