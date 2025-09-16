using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

public static partial class LibSm64Interop {
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