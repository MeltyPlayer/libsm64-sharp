using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  public static class LibSm64Interop {
    private const string SM64_X86_DLL = "lib\\sm64-x86.dll";

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_global_init(
        IntPtr rom,
        IntPtr outTexture,
        IntPtr debugPrintFunctionPtr);

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_global_terminate();

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_static_surfaces_load(
        LowLevelSm64Surface[] surfaces,
        ulong numSurfaces);

    [DllImport(SM64_X86_DLL)]
    public static extern uint sm64_mario_create(
        short marioX,
        short marioY,
        short marioZ);

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_mario_tick(
        uint marioId,
        ref LowLevelSm64MarioInputs inputs,
        ref LowLevelSm64MarioState outState,
        ref LowLevelSm64MarioGeometryBuffers outBuffers);

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_mario_delete(uint marioId);

    [DllImport(SM64_X86_DLL)]
    public static extern uint sm64_surface_object_create(
        ref LowLevelSm64SurfaceObject surfaceObject);

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_surface_object_move(
        uint objectId,
        ref LowLevelSm64ObjectTransform transform);

    [DllImport(SM64_X86_DLL)]
    public static extern void sm64_surface_object_delete(uint objectId);
  }
}