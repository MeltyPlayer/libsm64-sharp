using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

public static partial class LibSm64Interop {
  [DllImport(SM64_DLL)]
  public static extern void sm64_set_sound_volume(float volume);
}