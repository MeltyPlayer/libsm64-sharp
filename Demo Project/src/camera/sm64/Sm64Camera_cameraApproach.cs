namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Approaches a value by a given increment, returns FALSE if the target is
     * reached. Appears to be a strange way of implementing
     * approach_float_symmetric from object_helpers.c. It could possibly be an
     * older version of the function
     */
    bool camera_approach_f32_symmetric_bool(ref float current,
                                            float target,
                                            float increment) {
      current = camera_approach_f32_symmetric(current, target, increment);
      return !(Math.Abs(current - target) < TOLERANCE);
    }

    /**
     * Nearly the same as the above function, this one returns the new value in place of a bool.
     */
    float camera_approach_f32_symmetric(float current,
                                        float target,
                                        float increment) {
      float dist = target - current;

      if (increment < 0) {
        increment = -1 * increment;
      }
      if (dist > 0) {
        dist -= increment;
        if (dist > 0) {
          current = target - dist;
        } else {
          current = target;
        }
      } else {
        dist += increment;
        if (dist < 0) {
          current = target - dist;
        } else {
          current = target;
        }
      }
      return current;
    }


    bool camera_approach_s16_symmetric_bool(ref short current,
                                              short target,
                                              short increment) {
      current = camera_approach_s16_symmetric(current, target, increment);
      return current != target;
    }

    short camera_approach_s16_symmetric(short current,
                                          short target,
                                          short increment) {
      var dist = (short) (target - current);

      if (increment < 0) {
        increment = (short) (-1 * increment);
      }
      if (dist > 0) {
        dist -= increment;
        if (dist >= 0) {
          current = (short) (target - dist);
        } else {
          current = target;
        }
      } else {
        dist += increment;
        if (dist <= 0) {
          current = (short) (target - dist);
        } else {
          current = target;
        }
      }
      return current;
    }
  }
}