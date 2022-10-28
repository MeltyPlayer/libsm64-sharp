using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  public static class LibSm64Interop {
#if WIN64
    private const string SM64_DLL = "lib\\sm64-x64.dll";
#else
    private const string SM64_DLL = "lib\\sm64-x86.dll";
#endif

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
    public static extern uint sm64_surface_object_create(
        ref LowLevelSm64SurfaceObject surfaceObject);

    [DllImport(SM64_DLL)]
    public static extern void sm64_surface_object_move(
        uint objectId,
        ref LowLevelSm64ObjectTransform transform);

    [DllImport(SM64_DLL)]
    public static extern void sm64_surface_object_delete(uint objectId);
  }
}