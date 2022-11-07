using libsm64sharp;
using libsm64sharp.lowlevel;


namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private const int CELL_HEIGHT_LIMIT = 20000;
    private const int FLOOR_LOWER_LIMIT = -11000;

    /**
     * Set the camera's y coordinate to goalHeight, respecting floors and ceilings in the way
     */
    void set_camera_height(ref Camera c, float goalHeight) {
      float marioFloorHeight;
      float marioCeilHeight;
      float camFloorHeight;
      float baseOff = 125f;
      float camCeilHeight = find_ceil(c.pos[0], gLakituState.goalPos[1] - 50f,
                                      c.pos[2], out var surface);

      if ((sMarioCamState.action & PlayerAction.ACT_FLAG_HANGING) != 0) {
        marioCeilHeight = sMarioGeometry.currCeilHeight;
        marioFloorHeight = sMarioGeometry.currFloorHeight;

        if (marioFloorHeight < marioCeilHeight - 400f) {
          marioFloorHeight = marioCeilHeight - 400f;
        }

        goalHeight = marioFloorHeight +
                     (marioCeilHeight - marioFloorHeight) * 0.4f;

        if (sMarioCamState.pos[1] - 400 > goalHeight) {
          goalHeight = sMarioCamState.pos[1] - 400;
        }

        approach_camera_height(ref c, goalHeight, 5f);
      } else {
        camFloorHeight =
            find_floor(c.pos[0], c.pos[1] + 100f, c.pos[2], out surface) +
            baseOff;
        marioFloorHeight = baseOff + sMarioGeometry.currFloorHeight;

        if (camFloorHeight < marioFloorHeight) {
          camFloorHeight = marioFloorHeight;
        }
        if (goalHeight < camFloorHeight) {
          goalHeight = camFloorHeight;
          c.pos[1] = goalHeight;
        }
        // Warp camera to goalHeight if further than 1000 and Mario is stuck in the ground
        if (sMarioCamState.action == PlayerAction.ACT_BUTT_STUCK_IN_GROUND ||
            sMarioCamState.action == PlayerAction.ACT_HEAD_STUCK_IN_GROUND ||
            sMarioCamState.action == PlayerAction.ACT_FEET_STUCK_IN_GROUND) {
          if (Math.Abs(c.pos[1] - goalHeight) > 1000f) {
            c.pos[1] = goalHeight;
          }
        }
        approach_camera_height(ref c, goalHeight, 20f);
        if (camCeilHeight != CELL_HEIGHT_LIMIT) {
          camCeilHeight -= baseOff;
          if ((c.pos[1] > camCeilHeight &&
               sMarioGeometry.currFloorHeight + baseOff < camCeilHeight)
              || (sMarioGeometry.currCeilHeight != CELL_HEIGHT_LIMIT
                  && sMarioGeometry.currCeilHeight > camCeilHeight &&
                  c.pos[1] > camCeilHeight)) {
            c.pos[1] = camCeilHeight;
          }
        }
      }
    }


    /**
     * Make the camera's y coordinate approach `goal`,
     * unless smooth movement is off, in which case the y coordinate is simply set to `goal`
     */
    void approach_camera_height(ref Camera c, float goal, float inc) {
      if ((sStatusFlags & (int) CamFlags.CAM_FLAG_SMOOTH_MOVEMENT) != 0) {
        if (c.pos[1] < goal) {
          if ((c.pos[1] += inc) > goal) {
            c.pos[1] = goal;
          }
        } else {
          if ((c.pos[1] -= inc) < goal) {
            c.pos[1] = goal;
          }
        }
      } else {
        c.pos[1] = goal;
      }
    }

    /**
     * Pitch the camera down when the camera is facing down a slope
     */
    short look_down_slopes(short camYaw) {
      float floorDY;
// Default pitch
      short pitch = 0x05B0;
// x and z offsets towards the camera
      float xOff = sMarioCamState.pos[0] + sins(camYaw) * 40f;
      float zOff = sMarioCamState.pos[2] + coss(camYaw) * 40f;

      floorDY = find_floor(xOff, sMarioCamState.pos[1], zOff, out var floor) -
                sMarioCamState.pos[1];

      if (floor != null) {
        if (floor.Value.type != (short) Sm64SurfaceType.SURFACE_WALL_MISC &&
            floorDY > 0) {
          if (floor.Value.normal.Z == 0f && floorDY < 100f) {
            pitch = 0x05B0;
          } else {
            // Add the slope's angle of declination to the pitch
            pitch += atan2s(40f, floorDY);
          }
        }
      }

      return pitch;
    }
  }
}