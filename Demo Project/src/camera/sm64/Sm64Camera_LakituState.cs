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
      public Vec3f curFocus = new();
      /**
       * Lakitu's focus, which (when CAM_FLAG_SMOOTH_MOVEMENT is set), approaches his goalFocus every frame.
       */
      /*0x0C*/
      public Vec3f curPos = new();
      /**
       * The focus point that lakitu turns towards every frame.
       * If CAM_FLAG_SMOOTH_MOVEMENT is unset, this is the same as curFocus.
       */
      /*0x18*/
      public Vec3f goalFocus = new();
      /**
       * The point that lakitu flies towards every frame.
       * If CAM_FLAG_SMOOTH_MOVEMENT is unset, this is the same as curPos.
       */
      /*0x24*/
      public Vec3f goalPos = new();

      /*0x30*/
      public byte[] filler1 = new byte[12]; // extra unused Vec3f?

      /// Copy of the active camera mode
      /*0x3C*/
      public byte mode;
      /// Copy of the default camera mode
      /*0x3D*/
      public byte defMode;

      /*0x3E*/
      public byte[] filler2 = new byte[10];

      /*0x48*/
      public float focusDistance; // unused
      /*0x4C*/
      public short oldPitch; // unused
      /*0x4E*/
      public short oldYaw;   // unused
      /*0x50*/
      public short oldRoll;  // unused

      /// The angular offsets added to lakitu's pitch, yaw, and roll
      /*0x52*/
      public Vec3s shakeMagnitude = new();

      // shake pitch, yaw, and roll phase: The progression through the camera shake (a cosine wave).
      // shake pitch, yaw, and roll vel: The speed of the camera shake.
      // shake pitch, yaw, and roll decay: The shake's deceleration.
      /*0x58*/
      public short shakePitchPhase;
      /*0x5A*/
      public short shakePitchVel;
      /*0x5C*/
      public short shakePitchDecay;

      /*0x60*/
      public Vec3f unusedVec1 = new();
      /*0x6C*/
      public Vec3s unusedVec2 = new();
      /*0x72*/
      public byte[] filler3 = new byte[8];

      /// Used to rotate the screen when rendering.
      /*0x7A*/
      public short roll;
      /// Copy of the camera's yaw.
      /*0x7C*/
      public short yaw;
      /// Copy of the camera's next yaw.
      /*0x7E*/
      public short nextYaw;
      /// The actual focus point the game uses to render.
      /*0x80*/
      public Vec3f focus = new();
      /// The actual position the game is rendered from.
      /*0x8C*/
      public Vec3f pos = new();

      // Shake variables: See above description
      /*0x98*/
      public short shakeRollPhase;
      /*0x9A*/
      public short shakeRollVel;
      /*0x9C*/
      public short shakeRollDecay;
      /*0x9E*/
      public short shakeYawPhase;
      /*0xA0*/
      public short shakeYawVel;
      /*0xA2*/
      public short shakeYawDecay;

      // focH,Vspeed: how fast lakitu turns towards his goalFocus.
      /// By default HSpeed is 0.8, so lakitu turns 80% of the horz distance to his goal each frame.
      /*0xA4*/
      public float focHSpeed;
      /// By default VSpeed is 0.3, so lakitu turns 30% of the vert distance to his goal each frame.
      /*0xA8*/
      public float focVSpeed;

      // posH,Vspeed: How fast lakitu flies towards his goalPos.
      /// By default they are 0.3, so lakitu will fly 30% of the way towards his goal each frame.
      /*0xAC*/
      public float posHSpeed;
      /*0xB0*/
      public float posVSpeed;

      /// The roll offset applied during part of the key dance cutscene
      /*0xB4*/
      public short keyDanceRoll;
      /// Mario's action from the previous frame. Only used to determine if Mario just finished a dive.
      /*0xB8*/
      public uint lastFrameAction;
      /*0xBC*/
      public short unused;
    }
  }
}
