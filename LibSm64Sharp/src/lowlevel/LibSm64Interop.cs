using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

public static partial class LibSm64Interop {
  /// <summary>
  ///   Shamelessly stolen from:
  ///   https://stackoverflow.com/a/30646096
  /// </summary>
  static LibSm64Interop() {
    var myPath = new Uri(typeof(LibSm64Interop).Assembly.CodeBase).LocalPath;
    var myFolder = Path.GetDirectoryName(myPath);

    var is64 = IntPtr.Size == 8;
    var subfolder = is64 ? "win-x64" : "win-x86";

    LoadLibrary(Path.Combine(myFolder, "runtimes", subfolder, "native") +
                "runtimes" + subfolder + "MyDll.dll");
  }

  [DllImport("kernel32.dll")]
  private static extern IntPtr LoadLibrary(string dllToLoad);

  private const string SM64_DLL = "sm64.dll";

  [DllImport(SM64_DLL)]
  public static extern void sm64_register_debug_print_function(
      IntPtr debugPrintFunctionPtr);

  [DllImport(SM64_DLL)]
  public static extern void sm64_register_play_sound_function(
      IntPtr playSoundFunctionPtr);

  [DllImport(SM64_DLL)]
  public static extern void sm64_global_init(
      IntPtr rom,
      IntPtr outTexture);

  [DllImport(SM64_DLL)]
  public static extern void sm64_global_terminate();

  [DllImport(SM64_DLL)]
  public static extern void sm64_audio_init(IntPtr rom);

  [DllImport(SM64_DLL)]
  public static extern uint sm64_audio_tick(uint numQueuedSamples,
                                            uint numDesiredSamples,
                                            IntPtr audioBuffer);

  [DllImport(SM64_DLL)]
  public static extern void sm64_static_surfaces_load(
      LowLevelSm64Surface[] surfaces,
      ulong numSurfaces);
}