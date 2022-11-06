using System.Numerics;

namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    class Vector3<TNumber> where TNumber : INumber<TNumber> {
      private readonly TNumber[] impl_ = new TNumber[3];

      public TNumber this[int index] {
        set => this.impl_[index] = value;
        get => this.impl_[index];
      }
    }

    class Vec3f : Vector3<float> { }
    class Vec3s : Vector3<short> { }

    /// Copy vector 'src' to 'dest'
    void vec3f_copy(Vec3f dest, Vec3f src) {
      dest[0] = src[0];
      dest[1] = src[1];
      dest[2] = src[2];
    }

    /// Set vector 'dest' to (x, y, z)
    void vec3f_set(Vec3f dest, float x, float y, float z) {
      dest[0] = x;
      dest[1] = y;
      dest[2] = z;
    }

    /// Add vector 'a' to 'dest'
    void vec3f_add(Vec3f dest, Vec3f a) {
      dest[0] += a[0];
      dest[1] += a[1];
      dest[2] += a[2];
    }

    /// Make 'dest' the sum of vectors a and b.
    void vec3f_sum(Vec3f dest, Vec3f a, Vec3f b) {
      dest[0] = a[0] + b[0];
      dest[1] = a[1] + b[1];
      dest[2] = a[2] + b[2];
    }

    /// Copy vector src to dest
    void vec3s_copy(Vec3s dest, Vec3s src) {
      dest[0] = src[0];
      dest[1] = src[1];
      dest[2] = src[2];
    }

    /// Set vector 'dest' to (x, y, z)
    void vec3s_set(Vec3s dest, short x, short y, short z) {
      dest[0] = x;
      dest[1] = y;
      dest[2] = z;
    }

    /// Add vector a to 'dest'
    void vec3s_add(Vec3s dest, Vec3s a) {
      dest[0] += a[0];
      dest[1] += a[1];
      dest[2] += a[2];
    }

    /// Make 'dest' the sum of vectors a and b.
    void vec3s_sum(Vec3s dest, Vec3s a, Vec3s b) {
      dest[0] = (short)(a[0] + b[0]);
      dest[1] = (short)(a[1] + b[1]);
      dest[2] = (short)(a[2] + b[2]);
    }

    /// Subtract vector a from 'dest'
    void vec3s_sub(Vec3s dest, Vec3s a) {
      dest[0] -= a[0];
      dest[1] -= a[1];
      dest[2] -= a[2];
    }

    /// Convert short vector a to float vector 'dest'
    void vec3s_to_vec3f(Vec3f dest, Vec3s a) {
      dest[0] = a[0];
      dest[1] = a[1];
      dest[2] = a[2];
    }

    /**
     * Convert float vector a to a short vector 'dest' by rounding the components
     * to the nearest integer.
     */
    void vec3f_to_vec3s(Vec3s dest, Vec3f a) {
      // add/subtract 0.5 in order to round to the nearest s32 instead of truncating
      dest[0] = (short)(a[0] + ((a[0] > 0) ? 0.5f : -0.5f));
      dest[1] = (short)(a[1] + ((a[1] > 0) ? 0.5f : -0.5f));
      dest[2] = (short)(a[2] + ((a[2] > 0) ? 0.5f : -0.5f));
    }
  }
}
