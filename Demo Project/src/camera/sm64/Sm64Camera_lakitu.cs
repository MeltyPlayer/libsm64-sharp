namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private LakituState gLakituState = new();

    /**
     * A struct containing info pertaining to lakitu, such as his position and focus, and what
     * camera-related effects are happening to him, like camera shakes.
     *
     * This struct's pos and focus are what is actually used to render the game.
     *
     * @see update_lakitu()
     */
    class LakituState {
      /**
       * Lakitu's position, which (when CAM_FLAG_SMOOTH_MOVEMENT is set), approaches his goalPos every frame.
       */
      /*0x00*/
      Vec3f curFocus = new();
      /**
       * Lakitu's focus, which (when CAM_FLAG_SMOOTH_MOVEMENT is set), approaches his goalFocus every frame.
       */
      /*0x0C*/
      Vec3f curPos = new();
      /**
       * The focus point that lakitu turns towards every frame.
       * If CAM_FLAG_SMOOTH_MOVEMENT is unset, this is the same as curFocus.
       */
      /*0x18*/
      Vec3f goalFocus = new();
      /**
       * The point that lakitu flies towards every frame.
       * If CAM_FLAG_SMOOTH_MOVEMENT is unset, this is the same as curPos.
       */
      /*0x24*/
      Vec3f goalPos = new();

      /*0x30*/
      byte[] filler1 = new byte[12]; // extra unused Vec3f?

      /// Copy of the active camera mode
      /*0x3C*/
      byte mode;
      /// Copy of the default camera mode
      /*0x3D*/
      byte defMode;

      /*0x3E*/
      byte[] filler2 = new byte[10];

      /*0x48*/
      float focusDistance; // unused
      /*0x4C*/
      short oldPitch; // unused
      /*0x4E*/
      short oldYaw;   // unused
      /*0x50*/
      short oldRoll;  // unused

      /// The angular offsets added to lakitu's pitch, yaw, and roll
      /*0x52*/
      Vec3s shakeMagnitude;

      // shake pitch, yaw, and roll phase: The progression through the camera shake (a cosine wave).
      // shake pitch, yaw, and roll vel: The speed of the camera shake.
      // shake pitch, yaw, and roll decay: The shake's deceleration.
      /*0x58*/
      short shakePitchPhase;
      /*0x5A*/
      short shakePitchVel;
      /*0x5C*/
      short shakePitchDecay;

      /*0x60*/
      Vec3f unusedVec1 = new();
      /*0x6C*/
      Vec3s unusedVec2 = new();
      /*0x72*/
      byte[] filler3 = new byte[8];

      /// Used to rotate the screen when rendering.
      /*0x7A*/
      short roll;
      /// Copy of the camera's yaw.
      /*0x7C*/
      short yaw;
      /// Copy of the camera's next yaw.
      /*0x7E*/
      short nextYaw;
      /// The actual focus point the game uses to render.
      /*0x80*/
      Vec3f focus = new();
      /// The actual position the game is rendered from.
      /*0x8C*/
      Vec3f pos = new();

      // Shake variables: See above description
      /*0x98*/
      short shakeRollPhase;
      /*0x9A*/
      short shakeRollVel;
      /*0x9C*/
      short shakeRollDecay;
      /*0x9E*/
      short shakeYawPhase;
      /*0xA0*/
      short shakeYawVel;
      /*0xA2*/
      short shakeYawDecay;

      // focH,Vspeed: how fast lakitu turns towards his goalFocus.
      /// By default HSpeed is 0.8, so lakitu turns 80% of the horz distance to his goal each frame.
      /*0xA4*/
      float focHSpeed;
      /// By default VSpeed is 0.3, so lakitu turns 30% of the vert distance to his goal each frame.
      /*0xA8*/
      float focVSpeed;

      // posH,Vspeed: How fast lakitu flies towards his goalPos.
      /// By default they are 0.3, so lakitu will fly 30% of the way towards his goal each frame.
      /*0xAC*/
      float posHSpeed;
      /*0xB0*/
      float posVSpeed;

      /// The roll offset applied during part of the key dance cutscene
      /*0xB4*/
      short keyDanceRoll;
      /// Mario's action from the previous frame. Only used to determine if Mario just finished a dive.
      /*0xB8*/
      uint lastFrameAction;
      /*0xBC*/
      short unused;
    }
  }
}
