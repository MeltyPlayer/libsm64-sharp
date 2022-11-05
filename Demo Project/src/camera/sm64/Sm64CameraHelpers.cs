namespace demo.camera.sm64 {
  public class Sm64CameraHelpers {
    /**
     * Approaches a value by a given increment, returns FALSE if the target is
     * reached. Appears to be a strange way of implementing
     * approach_float_symmetric from object_helpers.c. It could possibly be an
     * older version of the function
     */
    public static bool camera_approach_float_symmetric_bool(ref float current, float target, float increment) {
      var dist = target - current;

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
      return !(Math.Abs(current - target) < .001);
    }
  }
}
