using libsm64sharp.lowlevel;

namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    bool gCheckingSurfaceCollisionsForCamera;
    PlayerGeometry sMarioGeometry;

    /**
     * Struct containing the nearest floor and ceiling to the player, as well as the previous floor and
     * ceiling. It also stores their distances from the player's position.
     */
    struct PlayerGeometry {
      /*0x00*/
      public LowLevelSm64SurfaceInternal? currFloor;

      /*0x04*/
      public float currFloorHeight;

      /*0x08*/
      public short currFloorType;

      /*0x0C*/
      public LowLevelSm64SurfaceInternal? currCeil;

      /*0x10*/
      public short currCeilType;

      /*0x14*/
      public float currCeilHeight;

      /*0x18*/
      public LowLevelSm64SurfaceInternal? prevFloor;

      /*0x1C*/
      public float prevFloorHeight;

      /*0x20*/
      public short prevFloorType;

      /*0x24*/
      public LowLevelSm64SurfaceInternal? prevCeil;

      /*0x28*/
      public float prevCeilHeight;

      /*0x2C*/
      public short prevCeilType;

      /// Unused, but recalculated every frame
      /*0x30*/
      public float waterHeight;
    }

    /**
     * Stores type and height of the nearest floor and ceiling to Mario in `pg`
     *
     * Note: Also finds the water level, but waterHeight is unused
     */
    void find_mario_floor_and_ceil(ref PlayerGeometry pg) {
      bool tempCheckingSurfaceCollisionsForCamera = gCheckingSurfaceCollisionsForCamera;
      gCheckingSurfaceCollisionsForCamera = true;

      LowLevelSm64SurfaceInternal? surf;
      if (find_floor(sMarioCamState.pos[0], sMarioCamState.pos[1] + 10f,
                     sMarioCamState.pos[2], out surf) != FLOOR_LOWER_LIMIT) {
        pg.currFloorType = surf!.Value.type;
      } else {
        pg.currFloorType = 0;
      }

      if (find_ceil(sMarioCamState.pos[0], sMarioCamState.pos[1] - 10f,
                    sMarioCamState.pos[2], out surf) != CELL_HEIGHT_LIMIT) {
        pg.currCeilType = surf!.Value.type;
      } else {
        pg.currCeilType = 0;
      }

      gCheckingSurfaceCollisionsForCamera = false;
      pg.currFloorHeight = find_floor(sMarioCamState.pos[0],
                                       sMarioCamState.pos[1] + 10f,
                                       sMarioCamState.pos[2], out pg.currFloor);
      pg.currCeilHeight = find_ceil(sMarioCamState.pos[0],
                                     sMarioCamState.pos[1] - 10f,
                                     sMarioCamState.pos[2], out pg.currCeil);
      pg.waterHeight = find_water_level(sMarioCamState.pos[0], sMarioCamState.pos[2]);
      gCheckingSurfaceCollisionsForCamera = tempCheckingSurfaceCollisionsForCamera;
    }
  }
}