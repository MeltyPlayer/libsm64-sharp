namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    enum CameraMode {
      CAMERA_MODE_NONE = 0x00,
      CAMERA_MODE_RADIAL = 0x01,
      CAMERA_MODE_OUTWARD_RADIAL = 0x02,
      CAMERA_MODE_BEHIND_MARIO = 0x03,
      CAMERA_MODE_CLOSE = 0x04, // Inside Castle / Big Boo's Haunt
      CAMERA_MODE_C_UP = 0x06,
      CAMERA_MODE_WATER_SURFACE = 0x08,
      CAMERA_MODE_SLIDE_HOOT = 0x09,
      CAMERA_MODE_INSIDE_CANNON = 0x0A,
      CAMERA_MODE_BOSS_FIGHT = 0x0B,
      CAMERA_MODE_PARALLEL_TRACKING = 0x0C,
      CAMERA_MODE_FIXED = 0x0D,
      CAMERA_MODE_8_DIRECTIONS = 0x0E, // AKA Parallel Camera, Bowser Courses & Rainbow Ride
      CAMERA_MODE_FREE_ROAM = 0x10,
      CAMERA_MODE_SPIRAL_STAIRS = 0x11,
    }
  }
}
