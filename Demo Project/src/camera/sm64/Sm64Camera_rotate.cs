﻿using System.Numerics;

namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Rotates a vector in the horizontal plane and copies it to a new vector.
     */
    public void rotate_in_xz(ref Vector3 dst, Vector3 src, short yaw) {
      dst[0] = src[2] * sins(yaw) + src[0] * coss(yaw);
      dst[1] = src[1];
      dst[2] = src[2] * coss(yaw) - src[0] * sins(yaw);
    }

    /**
     * Rotates a vector in the YZ plane and copies it to a new vector.
     *
     * Note: This function also flips the Z axis, so +Z moves forward, not backward like it would in world
     * space. If possible, use vec3f_set_dist_and_angle()
     */
    public void rotate_in_yz(ref Vector3 dst, Vector3 src, short pitch) {
      dst[2] = -(src[2] * coss(pitch) - src[1] * sins(pitch));
      dst[1] = src[2] * sins(pitch) + src[1] * coss(pitch);
      dst[0] = src[0];
    }
  }
}
