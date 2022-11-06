namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private readonly CameraFovStatus sFOVState = new();

    enum CameraFovFunc : byte {
      CAM_FOV_SET_45,
      CAM_FOV_SET_29,
      CAM_FOV_ZOOM_30,
      CAM_FOV_DEFAULT,
      CAM_FOV_BBH,
      CAM_FOV_APP_45,
      CAM_FOV_SET_30,
      CAM_FOV_APP_20,
      CAM_FOV_APP_80,
      CAM_FOV_APP_30,
      CAM_FOV_APP_60
    }

    /**
     * Info for the camera's field of view and the FOV shake effect.
     */
    class CameraFovStatus {
      /// The current function being used to set the camera's field of view (before any fov shake is applied).
      /*0x00*/
      public CameraFovFunc fovFunc;
      /// The current field of view in degrees
      /*0x04*/
      public float fov;

      // Fields used by shake_camera_fov()

      /// The amount to change the current fov by in the fov shake effect.
      /*0x08*/
      public float fovOffset;
      /// A bool set in fov_default() but unused otherwise
      /*0x0C*/
      public uint unusedIsSleeping;
      /// The range in degrees to shake fov
      /*0x10*/
      public float shakeAmplitude;
      /// Used to calculate fovOffset, the phase through the shake's period.
      /*0x14*/
      public short shakePhase;
      /// How much to progress through the shake period
      /*0x16*/
      public short shakeSpeed;
      /// How much to decrease shakeAmplitude each frame.
      /*0x18*/
      public short decay;
    }

    /**
     * Change the camera's FOV mode.
     *
     * @see geo_camera_fov
     */
    void set_fov_function(CameraFovFunc func) {
      this.sFOVState.fovFunc = func;
    }

    void set_fov_30() {
      sFOVState.fov = 30f;
    }

    void approach_fov_20() {
      camera_approach_float_symmetric_bool(ref sFOVState.fov, 20f, 0.3f);
    }

    void set_fov_45() {
      sFOVState.fov = 45f;
    }

    void set_fov_29() {
      sFOVState.fov = 29f;
    }

    void zoom_fov_30() {
      // Pretty sure approach_float_asymptotic_bool would do a much nicer job here, but you do you,
      // Nintendo.
      camera_approach_float_symmetric_bool(ref sFOVState.fov, 30f, (30f - sFOVState.fov) / 60f);
    }

    void approach_fov_30() {
      camera_approach_float_symmetric_bool(ref this.sFOVState.fov, 30f, 1f);
    }

    void approach_fov_60() {
      camera_approach_float_symmetric_bool(ref this.sFOVState.fov, 60f, 1f);
    }

    void approach_fov_45() {
      float targetFoV = sFOVState.fov;

      targetFoV = 45f;

      sFOVState.fov = approach_float(sFOVState.fov, targetFoV, 2f, 2f);
    }

    void approach_fov_80() {
      camera_approach_float_symmetric_bool(ref sFOVState.fov, 80f, 3.5f);
    }
  }
}
