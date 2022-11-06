namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Rotates a vector in the horizontal plane and copies it to a new vector.
     */
    void rotate_in_xz(Vec3f dst, Vec3f src, short yaw) {
      var src0 = src[0];
      var src1 = src[1];
      var src2 = src[2];

      dst[0] = src2 * sins(yaw) + src[0] * coss(yaw);
      dst[1] = src1;
      dst[2] = src2 * coss(yaw) - src0 * sins(yaw);
    }

    /**
     * Rotates a vector in the YZ plane and copies it to a new vector.
     *
     * Note: This function also flips the Z axis, so +Z moves forward, not backward like it would in world
     * space. If possible, use vec3f_set_dist_and_angle()
     */
    void rotate_in_yz(Vec3f dst, Vec3f src, short pitch) {
      var src0 = src[0];
      var src1 = src[1];
      var src2 = src[2];

      dst[2] = -(src2 * coss(pitch) - src1 * sins(pitch));
      dst[1] = src2 * sins(pitch) + src1 * coss(pitch);
      dst[0] = src0;
    }

    /**
     * Rotates a vector in the horizontal plane and copies it to a new vector.
     */
    void rotatef_in_xz(Vec3f dst, Vec3f src, float yaw) {
      var src0 = src[0];
      var src1 = src[1];
      var src2 = src[2];

      yaw *= 180 / MathF.PI;

      dst[0] = src2 * MathF.Sin(yaw) + src0 * MathF.Cos(yaw);
      dst[1] = src1;
      dst[2] = src2 * MathF.Cos(yaw) - src0 * MathF.Sin(yaw);
    }

    /**
     * Rotates a vector in the YZ plane and copies it to a new vector.
     *
     * Note: This function also flips the Z axis, so +Z moves forward, not backward like it would in world
     * space. If possible, use vec3f_set_dist_and_angle()
     */
    void rotatef_in_yz(Vec3f dst, Vec3f src, float pitch) {
      var src0 = src[0];
      var src1 = src[1];
      var src2 = src[2];

      pitch *= 180 / MathF.PI;

      dst[2] = -(src2 * MathF.Cos(pitch) - src1 * MathF.Sin(pitch));
      dst[1] = src2 * MathF.Sin(pitch) + src1 * MathF.Cos(pitch);
      dst[0] = src0;
    }
  }
}
