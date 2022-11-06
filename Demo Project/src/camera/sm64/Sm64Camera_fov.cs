namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    public enum CameraFovFunc {
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
    public class CameraFovStatus {
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
    };

  }
}
