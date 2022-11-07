namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    int clamp_pitch(Vec3f from, Vec3f to, short maxPitch, short minPitch) {
      int outOfRange = 0;

      vec3f_get_dist_and_angle(
          from,
          to,
          out var dist, 
          out var pitch,
          out var yaw);
      if (pitch > maxPitch) {
        pitch = maxPitch;
        outOfRange++;
      }
      if (pitch < minPitch) {
        pitch = minPitch;
        outOfRange++;
      }
      vec3f_set_dist_and_angle(from, to, dist, pitch, yaw);
      return outOfRange;
    }
  }
}