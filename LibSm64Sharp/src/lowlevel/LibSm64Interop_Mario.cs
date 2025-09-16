using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

public static partial class LibSm64Interop {
  [DllImport(SM64_DLL)]
  public static extern int sm64_mario_create(
      float marioX,
      float marioY,
      float marioZ);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_tick(
      int marioId,
      ref LowLevelSm64MarioInputs inputs,
      ref LowLevelSm64MarioOutState outState,
      ref LowLevelSm64MarioGeometryBuffers outBuffers);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_delete(int marioId);


  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_animation(
      int marioId,
      MarioAnimId animId);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_anim_frame(
      int marioId,
      short animFrame);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_position(
      int marioId,
      float x,
      float y,
      float z);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_angle(
      int marioId,
      float x,
      float y,
      float z);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_faceangle(
      int marioId,
      float y);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_velocity(
      int marioId,
      float x,
      float y,
      float z);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_forward_velocity(
      int marioId,
      float vel);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_invincibility(
      int marioId,
      short timer);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_water_level(int marioId, int level);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_gas_level(int marioId, int level);

  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_health(
      int marioId,
      ushort health);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_heal(int marioId, byte healCounter);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_kill(int marioId);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_extend_cap(int marioId, ushort capTime);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_attack(
      int marioId,
      float x,
      float y,
      float z,
      float hitboxHeight);
}