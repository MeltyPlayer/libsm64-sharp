using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

public static partial class LibSm64Interop {
  [DllImport(SM64_DLL)]
  public static extern void sm64_set_mario_health(
      int marioId,
      ushort health);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_take_damage(
      int marioId,
      uint damage,
      DamageType damageType,
      float x,
      float y,
      float z);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_heal(int marioId, byte healCounter);

  [DllImport(SM64_DLL)]
  public static extern void sm64_mario_kill(int marioId);
}