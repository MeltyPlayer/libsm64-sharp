namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Look ahead to the left or right in the direction the player is facing
     * The calculation for pan[0] could be simplified to:
     *      yaw = -yaw;
     *      pan[0] = sins(sMarioCamState->faceAngle[1] + yaw) * sins(0xC00) * dist;
     * Perhaps, early in development, the pan used to be calculated for both the x and z directions
     *
     * Since this function only affects the camera's focus, Mario's movement direction isn't affected.
     */
    void pan_ahead_of_player(Camera c) {
      var pan = new Vec3f();

      var sm64MarioPos = this.sm64Mario_.Position;
      var marioPos = new Vec3f(sm64MarioPos.X, sm64MarioPos.Y, sm64MarioPos.Z);

      // Get distance and angle from camera to Mario.
      vec3f_get_dist_and_angle(position, marioPos, out var dist, out var pitch, out var yaw);

      // The camera will pan ahead up to about 30% of the camera's distance to Mario.
      pan[2] = sins(0xC00) * dist;

      rotatef_in_xz(pan, pan, this.sm64Mario_.FaceAngle);
      // rotate in the opposite direction
      yaw = (short)(-yaw);
      rotate_in_xz(pan, pan, yaw);
      // Only pan left or right
      pan[2] = 0f;

      // If Mario is long jumping, or on a flag pole (but not at the top), then pan in the opposite direction
      if (sMarioCamState.action == PlayerAction.ACT_LONG_JUMP ||
          (sMarioCamState.action != PlayerAction.ACT_TOP_OF_POLE &&
           ((int)sMarioCamState.action & (int)PlayerAction.ACT_FLAG_ON_POLE) != 0)) {
        pan[0] = -pan[0];
      }

      // Slowly make the actual pan, sPanDistance, approach the calculated pan
      // If Mario is sleeping, then don't pan
      /*if (sStatusFlags & CAM_FLAG_SLEEPING) {
        approach_float_asymptotic_bool(ref sPanDistance, 0f, 0.025f);
      } else {*/
      approach_float_asymptotic_bool(ref sPanDistance, pan[0], 0.025f);
      //}

      // Now apply the pan. It's a dir vector to the left or right, rotated by the camera's yaw to Mario
      pan[0] = sPanDistance;
      yaw = (short)(-yaw);
      rotate_in_xz(pan, pan, yaw);
      vec3f_add(c.focus, pan);
    }
  }
}
