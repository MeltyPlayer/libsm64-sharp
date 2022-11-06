﻿namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private const float TOLERANCE = .001f;

    /**
     * Return the value 'current' after it tries to approach target, going up at
     * most 'inc' and going down at most 'dec'.
     */
    float approach_float(float current, float target, float inc, float dec) {
      if (current < target) {
        current += inc;
        if (current > target) {
          current = target;
        }
      } else {
        current -= dec;
        if (current < target) {
          current = target;
        }
      }
      return current;
    }

    /**
     * Approaches an float value by taking the difference between the target
     * and current value and adding a fraction of that to the current value.
     * Edits the current value directly, returns TRUE if the target has been
     * reached, FALSE otherwise.
     */
    static bool approach_float_asymptotic_bool(ref float current, float target, float multiplier) {
      if (multiplier > 1f) {
        multiplier = 1f;
      }
      current += (target - current) * multiplier;
      return !(Math.Abs(current - target) < TOLERANCE);
    }

    /**
     * Nearly the same as the above function, returns new value instead.
     */
    static float approach_float_asymptotic(float current, float target, float multiplier) {
      current += (target - current) * multiplier;
      return current;
    }


    /**
     * Approaches an short value in the same fashion as
     * approach_float_asymptotic_bool, returns TRUE if target
     * is reached. Note: Since this function takes integers as parameters, the
     * last argument is the reciprocal of what it would be in the previous two
     * functions.
     */
    static bool approach_short_asymptotic_bool(ref short current, short target, short divisor) {
      current = approach_short_asymptotic(current, target, divisor);
      return current != target;
    }

    /**
     * Approaches an short value in the same fashion as approach_float_asymptotic, returns the new value.
     * Note: last parameter is the reciprocal of what it would be in the float functions
     */
    static short approach_short_asymptotic(short current, short target, short divisor) {
      var temp = current;

      if (divisor == 0) {
        current = target;
      } else {
        temp -= target;
        temp -= (short)(temp / divisor);
        temp += target;
        current = temp;
      }
      return current;
    }
  }
}
