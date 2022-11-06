namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private Camera gCamera = new();

    /**
     * The main camera struct. Gets updated by the active camera mode and the current level/area. In
     * update_lakitu, its pos and focus are used to calculate lakitu's next position and focus, which are
     * then used to render the game.
     */
    class Camera {
      /*0x00*/
      public byte mode; // What type of mode the camera uses (see defines above)
      /*0x01*/
      public byte defMode;
      /**
       * Determines what direction Mario moves in when the analog stick is moved.
       *
       * @warning This is NOT the camera's xz-rotation in world space. This is the angle calculated from the
       *          camera's focus TO the camera's position, instead of the other way around like it should
       *          be. It's effectively the opposite of the camera's actual yaw. Use
       *          vec3f_get_dist_and_angle() if you need the camera's yaw.
       */
      /*0x02*/
      public short yaw;
      /*0x04*/
      public Vec3f focus = new();
      /*0x10*/
      public Vec3f pos = new();
      /*0x1C*/
      public Vec3f unusedVec1 = new();
      /// The x coordinate of the "center" of the area. The camera will rotate around this point.
      /// For example, this is what makes the camera rotate around the hill in BoB
      /*0x28*/
      public float areaCenX;
      /// The z coordinate of the "center" of the area. The camera will rotate around this point.
      /// For example, this is what makes the camera rotate around the hill in BoB
      /*0x2C*/
      public float areaCenZ;
      /*0x30*/
      public byte cutscene;
      /*0x31*/
      public byte[] filler1 = new byte[8];
      /*0x3A*/
      public short nextYaw;
      /*0x3C*/
      public byte[] filler2 = new byte[40];
      /*0x64*/
      public byte doorStatus;
      /// The y coordinate of the "center" of the area. Unlike areaCenX and areaCenZ, this is only used
      /// when paused. See zoom_out_if_paused_and_outside
      /*0x68*/
      public float areaCenY;
    }
  }
}
