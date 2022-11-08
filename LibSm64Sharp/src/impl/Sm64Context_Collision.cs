using libsm64sharp.lowlevel;


namespace libsm64sharp {
  public sealed partial class Sm64Context {
    public int FindWallCollision(ref float xPtr,
                                 ref float yPtr,
                                 ref float zPtr,
                                 float offsetY,
                                 float radius)
      => LibSm64Interop.sm64_surface_find_wall_collision(
          ref xPtr, ref yPtr, ref zPtr, offsetY, radius);

    public int FindWallCollisions(ref LowLevelSm64WallCollisionData colData)
      => LibSm64Interop.sm64_surface_find_wall_collisions(ref colData);

    public unsafe float FindCeil(float posX,
                                 float posY,
                                 float posZ,
                                 out LowLevelSm64SurfaceInternal? pceil) {
      LowLevelSm64SurfaceInternal* pceilPtr = null;

      var returnValue =
          LibSm64Interop.sm64_surface_find_ceil(posX, posY, posZ, ref pceilPtr);
      pceil = MarshalUtil.MarshalRef(pceilPtr);

      return returnValue;
    }

    public unsafe float FindFloorHeightAndData(float xPos,
                                               float yPos,
                                               float zPos,
                                               out LowLevelSm64FloorGeometry?
                                                   floorGeo) {
      LowLevelSm64FloorGeometry* floorGeoPtr = null;

      var returnValue =
          LibSm64Interop.sm64_surface_find_floor_height_and_data(
              xPos, yPos, zPos, ref floorGeoPtr);
      floorGeo = MarshalUtil.MarshalRef(floorGeoPtr);

      return returnValue;
    }

    public float FindFloorHeight(float x, float y, float z)
      => LibSm64Interop.sm64_surface_find_floor_height(x, y, z);

    public unsafe float FindFloor(
        float xPos,
        float yPos,
        float zPos,
        out LowLevelSm64SurfaceInternal? pfloor) {
      LowLevelSm64SurfaceInternal* pfloorPtr = null;

      var returnValue = LibSm64Interop.sm64_surface_find_floor(
          xPos, yPos, zPos, ref pfloorPtr);
      pfloor = MarshalUtil.MarshalRef(pfloorPtr);

      return returnValue;
    }

    public float FindWaterLevel(float x, float z)
      => LibSm64Interop.sm64_surface_find_water_level(x, z);

    public float FindPoisonGasLevel(float x, float z)
      => LibSm64Interop.sm64_surface_find_poison_gas_level(x, z);
  }
}