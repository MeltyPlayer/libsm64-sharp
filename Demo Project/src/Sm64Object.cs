using demo.camera;
using demo.mesh;

using libsm64sharp;

using Quad64.src.LevelInfo;
using Quad64;
using Quad64.Scripts;
using Quad64.src.Scripts;


namespace demo;

public class Sm64Object : IDisposable, IRenderable {
  private readonly Quad64ObjectRenderer renderer_;
  private readonly ISm64DynamicCollisionMesh? dynamicCollisionMesh_;
  private readonly Object3D object_;

  public Sm64Object(
      ISm64Context context,
      Level level,
      Object3D obj,
      ICamera camera
  ) {
    CollisionMap? collisionMap = null;
    var scale = 1f;

    var billboard = false;

    var scripts = obj.ParseBehavior();
    foreach (var script in scripts) {
      if (script.Command == BehaviorCommand.load_collision_data) {
        var collisionAddress = BitLogic.BytesToInt(script.data, 4, 4);
        collisionMap = CollisionMapLoader.Load(collisionAddress);
      }

      if (script.Command == BehaviorCommand.SCALE) {
        var rawScale = BitLogic.BytesToInt(script.data, 2, 2);
        scale = rawScale / 100f;
      }

      if (script.Command == BehaviorCommand.billboard) {
        billboard = true;
      }
    }

    if (collisionMap != null) {
      var collisionBuilder = context.CreateDynamicCollisionMesh(scale)
                                    .SetPosition(
                                        obj.xPos, obj.yPos, obj.zPos);
      Quad64LevelMeshLoader.CopyIntoBuilder<ISm64DynamicCollisionMeshBuilder,
          ISm64DynamicCollisionMesh>(
          collisionMap,
          collisionBuilder);
      this.dynamicCollisionMesh_ = collisionBuilder.Build();
    }
    this.renderer_ =
        new Quad64ObjectRenderer(level, obj, camera, scale, billboard);
    this.object_ = obj;
  }

  ~Sm64Object() => this.ReleaseUnmanagedResources_();

  public void Dispose() {
    this.ReleaseUnmanagedResources_();
    GC.SuppressFinalize(this);
  }

  private void ReleaseUnmanagedResources_() {
    this.dynamicCollisionMesh_?.Dispose();
  }

  public void Tick() {
    this.dynamicCollisionMesh_?.SetPosition(this.object_.xPos,
                                            this.object_.yPos,
                                            this.object_.zPos);
  }

  public void Render() {
    this.renderer_.Render();
  }
}