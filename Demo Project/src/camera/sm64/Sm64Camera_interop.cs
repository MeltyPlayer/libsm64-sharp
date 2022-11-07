using libsm64sharp.lowlevel;


namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    int f32_find_wall_collision(ref float xPtr,
                                ref float yPtr,
                                ref float zPtr,
                                float offsetY,
                                float radius) {
      return LibSm64Interop.sm64_surface_find_wall_collision(
          ref xPtr, ref yPtr, ref zPtr, offsetY, radius);
    }

    int find_wall_collisions(ref LowLevelSm64WallCollisionData colData) {
      return LibSm64Interop.sm64_surface_find_wall_collisions(ref colData);
    }

    unsafe float find_ceil(float posX,
                           float posY,
                           float posZ,
                           out LowLevelSm64SurfaceInternal? pceil) {
      LowLevelSm64SurfaceInternal* pceilPtr = null;

      var returnValue =
          LibSm64Interop.sm64_surface_find_ceil(posX, posY, posZ, ref pceilPtr);
      pceil = MarshalUtil.MarshalRef(pceilPtr);

      return returnValue;
    }

    unsafe float find_floor_height_and_data(float xPos,
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

    float find_floor_height(float x, float y, float z) {
      return LibSm64Interop.sm64_surface_find_floor_height(x, y, z);
    }

    unsafe float find_floor(float xPos,
                     float yPos,
                     float zPos,
                     out LowLevelSm64SurfaceInternal? pfloor) {
      LowLevelSm64SurfaceInternal* pfloorPtr = null;

      var returnValue = LibSm64Interop.sm64_surface_find_floor(
          xPos, yPos, zPos, ref pfloorPtr);
      pfloor = MarshalUtil.MarshalRef(pfloorPtr);

      return returnValue;
    }

    float find_water_level(float x, float z) {
      return LibSm64Interop.sm64_surface_find_water_level(x, z);
    }

    float find_poison_gas_level(float x, float z) {
      return LibSm64Interop.sm64_surface_find_poison_gas_level(x, z);
    }
  }
}