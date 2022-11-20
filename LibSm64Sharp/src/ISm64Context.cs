using libsm64sharp.lowlevel;


namespace libsm64sharp {
  public interface ISm64Context : IDisposable {
    uint TickAudio(uint numQueuedSamples,
                   uint numDesiredSamples,
                   short[] audioBuffer);

    ISm64Mario CreateMario(float x, float y, float z);

    // TODO: Are builders the right design for this?
    ISm64DynamicCollisionMeshBuilder CreateDynamicCollisionMesh(float scale = 1);
    ISm64StaticCollisionMeshBuilder CreateStaticCollisionMesh();


    int FindWallCollision(ref float xPtr,
                          ref float yPtr,
                          ref float zPtr,
                          float offsetY,
                          float radius);

    int FindWallCollisions(ref LowLevelSm64WallCollisionData colData);

    float FindCeil(float posX,
                   float posY,
                   float posZ,
                   out LowLevelSm64SurfaceInternal? pceil);

    float FindFloorHeightAndData(float xPos,
                                 float yPos,
                                 float zPos,
                                 out LowLevelSm64FloorGeometry? floorGeo);

    float FindFloorHeight(float x, float y, float z);

    float FindFloor(float xPos,
                    float yPos,
                    float zPos,
                    out LowLevelSm64SurfaceInternal? pfloor);

    float FindWaterLevel(float x, float z);
    float FindPoisonGasLevel(float x, float z);
  }
}