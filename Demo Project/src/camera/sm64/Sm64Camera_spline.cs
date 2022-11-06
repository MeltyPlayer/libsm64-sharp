namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Information for a control point in a spline segment.
     */
    class CutsceneSplinePoint {
      /* The index of this point in the spline. Ignored except for -1, which ends the spline.
         An index of -1 should come four points after the start of the last segment. */
      public sbyte index;
      /* Roughly controls the number of frames it takes to progress through the spline segment.
         See move_point_along_spline() in camera.c */
      public byte speed;
      public Vec3s point = new();
    }

    /**
     * Produces values using a cubic b-spline curve. Basically Q is the used output,
     * u is a value between 0 and 1 that represents the position along the spline,
     * and a0-a3 are parameters that define the spline.
     *
     * The spline is described at www2.cs.uregina.ca/~anima/408/Notes/Interpolation/UniformBSpline.htm
     */
    void evaluate_cubic_spline(float u, Vec3f Q, Vec3f a0, Vec3f a1, Vec3f a2, Vec3f a3) {
      float[] B = new float[4];
      float x;
      float y;
      float z;

      if (u > 1f) {
        u = 1f;
      }

      B[0] = (1f - u) * (1f - u) * (1f - u) / 6f;
      B[1] = u * u * u / 2f - u * u + 0.6666667f;
      B[2] = -u * u * u / 2f + u * u / 2f + u / 2f + 0.16666667f;
      B[3] = u * u * u / 6f;

      Q[0] = B[0] * a0[0] + B[1] * a1[0] + B[2] * a2[0] + B[3] * a3[0];
      Q[1] = B[0] * a0[1] + B[1] * a1[1] + B[2] * a2[1] + B[3] * a3[1];
      Q[2] = B[0] * a0[2] + B[1] * a1[2] + B[2] * a2[2] + B[3] * a3[2];

      // Unused code
      B[0] = -0.5f * u * u + u - 0.33333333f;
      B[1] = 1.5f * u * u - 2f * u - 0.5f;
      B[2] = -1.5f * u * u + u + 1f;
      B[3] = 0.5f * u * u - 0.16666667f;

      x = B[0] * a0[0] + B[1] * a1[0] + B[2] * a2[0] + B[3] * a3[0];
      y = B[0] * a0[1] + B[1] * a1[1] + B[2] * a2[1] + B[3] * a3[1];
      z = B[0] * a0[2] + B[1] * a1[2] + B[2] * a2[2] + B[3] * a3[2];
    }

    /**
     * Computes the point that is `progress` percent of the way through segment `splineSegment` of `spline`,
     * and stores the result in `p`. `progress` and `splineSegment` are updated if `progress` becomes >= 1.0.
     *
     * When neither of the next two points' speeds == 0, the number of frames is between 1 and 255. Otherwise
     * it's infinite.
     *
     * To calculate the number of frames it will take to progress through a spline segment:
     * If the next two speeds are the same and nonzero, it's 1.0 / firstSpeed.
     *
     * s1 and s2 are short hand for first/secondSpeed. The progress at any frame n is defined by a recurrency relation:
     *      p(n+1) = (s2 - s1 + 1) * p(n) + s1
     * Which can be written as
     *      p(n) = (s2 * ((s2 - s1 + 1)^(n) - 1)) / (s2 - s1)
     *
     * Solving for the number of frames:
     *      n = log(((s2 - s1) / s1) + 1) / log(s2 - s1 + 1)
     *
     * @return 1 if the point has reached the end of the spline, when `progress` reaches 1.0 or greater, and
     * the 4th CutsceneSplinePoint in the current segment away from spline[splineSegment] has an index of -1.
     */
    int move_point_along_spline(Vec3f p, CutsceneSplinePoint[] spline, ref short splineSegment, ref float progress) {
      int finished = 0;
      var controlPoints = new Vec3f[4];
      int i = 0;
      float u = progress;
      float progressChange;
      float firstSpeed = 0;
      float secondSpeed = 0;
      int segment = splineSegment;

      if (splineSegment < 0) {
        segment = 0;
        u = 0;
      }
      if (spline[segment].index == -1 || spline[segment + 1].index == -1 || spline[segment + 2].index == -1) {
        return 1;
      }

      for (i = 0; i < 4; i++) {
        controlPoints[i][0] = spline[segment + i].point[0];
        controlPoints[i][1] = spline[segment + i].point[1];
        controlPoints[i][2] = spline[segment + i].point[2];
      }
      evaluate_cubic_spline(u, p, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);

      if (spline[splineSegment + 1].speed != 0) {
        firstSpeed = 1.0f / spline[splineSegment + 1].speed;
      }
      if (spline[splineSegment + 2].speed != 0) {
        secondSpeed = 1.0f / spline[splineSegment + 2].speed;
      }
      progressChange = (secondSpeed - firstSpeed) * progress + firstSpeed;

      if (1 <= (progress += progressChange)) {
        (splineSegment)++;
        if (spline[splineSegment + 3].index == -1) {
          splineSegment = 0;
          finished = 1;
        }
          (progress)--;
      }
      return finished;
    }
  }
}
