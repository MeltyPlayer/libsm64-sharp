using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

public static partial class LibSm64Interop {
  [DllImport(SM64_DLL)]
  public static extern int sm64_surface_find_wall_collision(
      ref float xPtr,
      ref float yPtr,
      ref float zPtr,
      float offsetY,
      float radius);

  [DllImport(SM64_DLL)]
  public static extern int sm64_surface_find_wall_collisions(
      ref LowLevelSm64WallCollisionData colData);

  [DllImport(SM64_DLL)]
  public static extern unsafe float sm64_surface_find_ceil(
      float posX,
      float posY,
      float posZ,
      ref LowLevelSm64SurfaceInternal* pceil);

  [DllImport(SM64_DLL)]
  public static extern unsafe float sm64_surface_find_floor_height_and_data(
      float xPos,
      float yPos,
      float zPos,
      ref LowLevelSm64FloorGeometry* floorGeo);

  [DllImport(SM64_DLL)]
  public static extern float sm64_surface_find_floor_height(
      float x,
      float y,
      float z);

  [DllImport(SM64_DLL)]
  public static extern unsafe float sm64_surface_find_floor(
      float xPos,
      float yPos,
      float zPos,
      ref LowLevelSm64SurfaceInternal* pfloor);

  [DllImport(SM64_DLL)]
  public static extern float sm64_surface_find_water_level(float x, float z);

  [DllImport(SM64_DLL)]
  public static extern float sm64_surface_find_poison_gas_level(
      float x,
      float z);
}