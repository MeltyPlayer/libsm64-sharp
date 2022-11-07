﻿namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private Camera gCamera = new();

    enum DoorStatus : byte {
      DOOR_DEFAULT = 0,
      DOOR_LEAVING_SPECIAL = 1,
      DOOR_ENTER_LOBBY = 2,
    }

    enum CamMove {
      CAM_MOVE_RETURN_TO_MIDDLE = 0x0001,
      CAM_MOVE_ZOOMED_OUT = 0x0002,
      CAM_MOVE_ROTATE_RIGHT = 0x0004,
      CAM_MOVE_ROTATE_LEFT = 0x0008,
      CAM_MOVE_ENTERED_ROTATE_SURFACE = 0x0010,
      CAM_MOVE_METAL_BELOW_WATER = 0x0020,
      CAM_MOVE_FIX_IN_PLACE = 0x0040,
      CAM_MOVE_UNKNOWN_8 = 0x0080,
      CAM_MOVING_INTO_MODE = 0x0100,
      CAM_MOVE_STARTED_EXITING_C_UP = 0x0200,
      CAM_MOVE_UNKNOWN_11 = 0x0400,
      CAM_MOVE_INIT_CAMERA = 0x0800,
      CAM_MOVE_ALREADY_ZOOMED_OUT = 0x1000,
      CAM_MOVE_C_UP_MODE = 0x2000,
      CAM_MOVE_SUBMERGED = 0x4000,
      CAM_MOVE_PAUSE_SCREEN = 0x8000,
    }

    enum CamFlags {
      CAM_FLAG_SMOOTH_MOVEMENT = 0x0001,
      CAM_FLAG_BLOCK_SMOOTH_MOVEMENT = 0x0002,
      CAM_FLAG_FRAME_AFTER_CAM_INIT = 0x0004,
      CAM_FLAG_CHANGED_PARTRACK_INDEX = 0x0008,
      CAM_FLAG_CCM_SLIDE_SHORTCUT = 0x0010,
      CAM_FLAG_CAM_NEAR_WALL = 0x0020,
      CAM_FLAG_SLEEPING = 0x0040,
      CAM_FLAG_UNUSED_7 = 0x0080,
      CAM_FLAG_UNUSED_8 = 0x0100,
      CAM_FLAG_COLLIDED_WITH_WALL = 0x0200,
      CAM_FLAG_START_TRANSITION = 0x0400,
      CAM_FLAG_TRANSITION_OUT_OF_C_UP = 0x0800,
      CAM_FLAG_BLOCK_AREA_PROCESSING = 0x1000,
      CAM_FLAG_UNUSED_13 = 0x2000,
      CAM_FLAG_UNUSED_CUTSCENE_ACTIVE = 0x4000,
      CAM_FLAG_BEHIND_MARIO_POST_DOOR = 0x8000,
    }

    /**
     * The main camera struct. Gets updated by the active camera mode and the current level/area. In
     * update_lakitu, its pos and focus are used to calculate lakitu's next position and focus, which are
     * then used to render the game.
     */
    class Camera {
      /*0x00*/
      public CameraMode mode; // What type of mode the camera uses (see defines above)

      /*0x01*/
      public byte defMode;

      /**
       * Determines what direction Mario moves in when the analog stick is moved.
       *
       * @warning This is NOT the camera's xz-rotation in world space. This is the angle calculated from the
       *          camera's focus TO the camera's position, instead of the other way around like it should
       *          be. It's effectively the opposite of the camera's actual yaw. Use
       *          vec3f_get_dist_and_angle() if you need the camera's yaw.
       */
      /*0x02*/
      public short yaw;

      /*0x04*/
      public Vec3f focus = new();

      /*0x10*/
      public Vec3f pos = new();

      /*0x1C*/
      public Vec3f unusedVec1 = new();

      /// The x coordinate of the "center" of the area. The camera will rotate around this point.
      /// For example, this is what makes the camera rotate around the hill in BoB
      /*0x28*/
      public float areaCenX;

      /// The z coordinate of the "center" of the area. The camera will rotate around this point.
      /// For example, this is what makes the camera rotate around the hill in BoB
      /*0x2C*/
      public float areaCenZ;

      /*0x30*/
      public byte cutscene;

      /*0x31*/
      public byte[] filler1 = new byte[8];

      /*0x3A*/
      public short nextYaw;

      /*0x3C*/
      public byte[] filler2 = new byte[40];

      /*0x64*/
      public DoorStatus doorStatus;

      /// The y coordinate of the "center" of the area. Unlike areaCenX and areaCenZ, this is only used
      /// when paused. See zoom_out_if_paused_and_outside
      /*0x68*/
      public float areaCenY;
    }
  }
}