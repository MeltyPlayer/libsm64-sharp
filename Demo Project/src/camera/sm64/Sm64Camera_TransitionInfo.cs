namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    /**
     * Struct containing info that is used when transition_next_state() is called. Stores the intermediate
     * distances and angular displacements from lakitu's goal position and focus.
     */
    class TransitionInfo {
      /*0x00*/
      public short posPitch;
      /*0x02*/
      public short posYaw;
      /*0x04*/
      public float posDist;
      /*0x08*/
      public short focPitch;
      /*0x0A*/
      public short focYaw;
      /*0x0C*/
      public float focDist;
      /*0x10*/
      public int framesLeft;
      /*0x14*/
      public Vec3f marioPos = new();
      /*0x20*/
      public byte unused; // for the structs to align, there has to be an extra unused variable here. type is unknown.
    };
  }
}
