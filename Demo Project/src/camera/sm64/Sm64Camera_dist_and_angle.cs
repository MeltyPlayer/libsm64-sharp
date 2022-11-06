using System.Numerics;

namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Take the vector starting at 'from' pointed at 'to' an retrieve the length
     * of that vector, as well as the yaw and pitch angles.
     * Basically it converts the direction to spherical coordinates.
     */
    void vec3f_get_dist_and_angle(Vector3 from, Vector3 to, out float dist, out short pitch, out short yaw) {
      var x = to[0] - from[0];
      var y = to[1] - from[1];
      var z = to[2] - from[2];

      dist = MathF.Sqrt(x * x + y * y + z * z);
      pitch = atan2s(MathF.Sqrt(x * x + z * z), y);
      yaw = atan2s(z, x);
    }

    /**
     * Construct the 'to' point which is distance 'dist' away from the 'from' position,
     * and has the angles pitch and yaw.
     */
    void vec3f_set_dist_and_angle(Vector3 from, ref Vector3 to, float dist, short pitch, short yaw) {
      to[0] = from[0] + dist * coss(pitch) * sins(yaw);
      to[1] = from[1] + dist * sins(pitch);
      to[2] = from[2] + dist * coss(pitch) * coss(yaw);
    }
  }
}
