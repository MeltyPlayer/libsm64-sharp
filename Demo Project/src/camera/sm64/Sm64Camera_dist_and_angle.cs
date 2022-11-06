namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Take the vector starting at 'from' pointed at 'to' an retrieve the length
     * of that vector, as well as the yaw and pitch angles.
     * Basically it converts the direction to spherical coordinates.
     */
    void vec3f_get_dist_and_angle(Vec3f from, Vec3f to, out float dist, out short pitch, out short yaw) {
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
    void vec3f_set_dist_and_angle(Vec3f from, Vec3f to, float dist, short pitch, short yaw) {
      to[0] = from[0] + dist * coss(pitch) * sins(yaw);
      to[1] = from[1] + dist * sins(pitch);
      to[2] = from[2] + dist * coss(pitch) * coss(yaw);
    }

    short calculate_pitch(Vec3f from, Vec3f to) {
      var dx = to[0] - from[0];
      var dy = to[1] - from[1];
      var dz = to[2] - from[2];
      var pitch = atan2s(MathF.Sqrt(dx * dx + dz * dz), dy);
      return pitch;
    }

    short calculate_yaw(Vec3f from, Vec3f to) {
      var dx = to[0] - from[0];
      var dz = to[2] - from[2];
      var yaw = atan2s(dz, dx);

      return yaw;
    }

    /**
     * Calculates the pitch and yaw between two vectors.
     */
    void calculate_angles(Vec3f from, Vec3f to, out short pitch, out short yaw) {
      var dx = to[0] - from[0];
      var dy = to[1] - from[1];
      var dz = to[2] - from[2];

      pitch = atan2s(MathF.Sqrt(dx * dx + dz * dz), dy);
      yaw = atan2s(dz, dx);
    }


    /**
     * Finds the distance between two vectors.
     */
    float calc_abs_dist(Vec3f a, Vec3f b) {
      var distX = b[0] - a[0];
      var distY = b[1] - a[1];
      var distZ = b[2] - a[2];
      var distAbs = MathF.Sqrt(distX * distX + distY * distY + distZ * distZ);

      return distAbs;
    }

    /**
     * Finds the horizontal distance between two vectors.
     */
    float calc_hor_dist(Vec3f a, Vec3f b) {
      var distX = b[0] - a[0];
      var distZ = b[2] - a[2];
      var distHor = MathF.Sqrt(distX * distX + distZ * distZ);

      return distHor;
    }
  }
}
