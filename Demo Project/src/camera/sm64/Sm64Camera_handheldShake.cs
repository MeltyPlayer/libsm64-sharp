namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * A point that's used in a spline, controls the direction to move the camera in
     * during the shake effect.
     */
    class HandheldShakePoint {
      /*0x08*/
      public Vec3s point = new();
    }

    enum HandCamShake {
      HAND_CAM_SHAKE_OFF = 0,
      HAND_CAM_SHAKE_CUTSCENE = 1,
      HAND_CAM_SHAKE_UNUSED = 2,
      HAND_CAM_SHAKE_HANG_OWL = 3,
      HAND_CAM_SHAKE_HIGH = 4,
      HAND_CAM_SHAKE_STAR_DANCE = 5,
      HAND_CAM_SHAKE_LOW = 6,
    }

    HandheldShakePoint[] sHandheldShakeSpline = {
      new(),
      new(),
      new(),
      new(),
    };
    short sHandheldShakeMag;
    float sHandheldShakeTimer;
    float sHandheldShakeInc;
    short sHandheldShakePitch;
    short sHandheldShakeYaw;
    short sHandheldShakeRoll;

    /**
     * Enables the handheld shake effect for this frame.
     *
     * @see shake_camera_handheld()
     */
    void set_handheld_shake(HandCamShake mode) {
      switch (mode) {
        // They're not in numerical order because that would be too simple...
        case HandCamShake.HAND_CAM_SHAKE_CUTSCENE: // Lowest increment
          sHandheldShakeMag = 0x600;
          sHandheldShakeInc = 0.04f;
          break;
        case HandCamShake.HAND_CAM_SHAKE_LOW: // Lowest magnitude
          sHandheldShakeMag = 0x300;
          sHandheldShakeInc = 0.06f;
          break;
        case HandCamShake.HAND_CAM_SHAKE_HIGH: // Highest mag and inc
          sHandheldShakeMag = 0x1000;
          sHandheldShakeInc = 0.1f;
          break;
        case HandCamShake.HAND_CAM_SHAKE_UNUSED: // Never used
          sHandheldShakeMag = 0x600;
          sHandheldShakeInc = 0.07f;
          break;
        case HandCamShake.HAND_CAM_SHAKE_HANG_OWL: // exactly the same as UNUSED...
          sHandheldShakeMag = 0x600;
          sHandheldShakeInc = 0.07f;
          break;
        case HandCamShake.HAND_CAM_SHAKE_STAR_DANCE: // Slightly steadier than HANG_OWL and UNUSED
          sHandheldShakeMag = 0x400;
          sHandheldShakeInc = 0.07f;
          break;
        default:
          sHandheldShakeMag = 0x0;
          sHandheldShakeInc = 0f;
          break;
      }
    }

    /**
     * When sHandheldShakeMag is nonzero, this function adds small random offsets to `focus` every time
     * sHandheldShakeTimer increases above 1.0, simulating the camera shake caused by unsteady hands.
     *
     * This function must be called every frame in order to actually apply the effect, since the effect's
     * mag and inc are set to 0 every frame at the end of this function.
     */
    void shake_camera_handheld(Vec3f pos, Vec3f focus) {
      int i;
      Vec3f shakeOffset = new();
      var shakeSpline = new Vec3f[4];

      if (sHandheldShakeMag == 0) {
        vec3f_set(shakeOffset, 0f, 0f, 0f);
      } else {
        for (i = 0; i < 4; i++) {
          shakeSpline[i][0] = sHandheldShakeSpline[i].point[0];
          shakeSpline[i][1] = sHandheldShakeSpline[i].point[1];
          shakeSpline[i][2] = sHandheldShakeSpline[i].point[2];
        }
        evaluate_cubic_spline(sHandheldShakeTimer, shakeOffset, shakeSpline[0],
                              shakeSpline[1], shakeSpline[2], shakeSpline[3]);
        if (1f <= (sHandheldShakeTimer += sHandheldShakeInc)) {
          // The first 3 control points are always (0,0,0), so the random spline is always just a
          // straight line
          for (i = 0; i < 3; i++) {
            vec3s_copy(sHandheldShakeSpline[i].point, sHandheldShakeSpline[i + 1].point);
          }
          random_vec3s(sHandheldShakeSpline[3].point, sHandheldShakeMag, sHandheldShakeMag, (short)(sHandheldShakeMag / 2));
          sHandheldShakeTimer -= 1f;

          // Code dead, this is set to be 0 before it is used.
          sHandheldShakeInc = random_float() * 0.5f;
          if (sHandheldShakeInc < 0.02f) {
            sHandheldShakeInc = 0.02f;
          }
        }
      }

      approach_short_asymptotic_bool(ref sHandheldShakePitch, (short)shakeOffset[0], 0x08);
      approach_short_asymptotic_bool(ref sHandheldShakeYaw, (short)shakeOffset[1], 0x08);
      approach_short_asymptotic_bool(ref sHandheldShakeRoll, (short)shakeOffset[2], 0x08);

      if ((sHandheldShakePitch | sHandheldShakeYaw) != 0) {
        vec3f_get_dist_and_angle(pos, focus, out var dist, out var pitch, out var yaw);
        pitch += sHandheldShakePitch;
        yaw += sHandheldShakeYaw;
        vec3f_set_dist_and_angle(pos, focus, dist, pitch, yaw);
      }

      // Unless called every frame, the effect will stop after the first time.
      sHandheldShakeMag = 0;
      sHandheldShakeInc = 0f;
    }
  }
}
