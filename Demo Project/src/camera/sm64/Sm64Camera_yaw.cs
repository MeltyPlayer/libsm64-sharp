namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    short sYawSpeed = 0x400;

    /**
     * The yaw passed here is the yaw of the direction FROM Mario TO Lakitu.
     *
     * wallYaw always has 90 degrees added to it before this is called -- it's parallel to the wall.
     *
     * @return the new yaw from Mario to rotate towards.
     *
     * @warning this is jank. It actually returns the yaw that will rotate further INTO the wall. So, the
     *          developers just add 180 degrees to the result.
     */
    int calc_avoid_yaw(short yawFromMario, short wallYaw) {
      short yawDiff;

      yawDiff = (short) (wallYaw - yawFromMario + DEGREES(90));

      if (yawDiff < 0) {
        // Deflect to the right
        yawFromMario = wallYaw;
      } else {
        // Note: this favors the left side if the wall is exactly perpendicular to the camera.
        // Deflect to the left
        yawFromMario = (short)(wallYaw + DEGREES(180));
      }
      return yawFromMario;
    }

    int clamp_positions_and_find_yaw(Vec3f pos, Vec3f origin, float xMax, float xMin, float zMax, float zMin) {
      short yaw = gCamera.nextYaw;

      if (pos[0] >= xMax) {
        pos[0] = xMax;
      }
      if (pos[0] <= xMin) {
        pos[0] = xMin;
      }
      if (pos[2] >= zMax) {
        pos[2] = zMax;
      }
      if (pos[2] <= zMin) {
        pos[2] = zMin;
      }
      yaw = calculate_yaw(origin, pos);
      return yaw;
    }
  }
}
