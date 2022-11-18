using demo.mesh;

using libsm64sharp;

using Quad64.src.LevelInfo;
using Quad64;
using Quad64.Scripts;


namespace demo {
  public class Sm64Object : IDisposable, IRenderable {
    private readonly Quad64ObjectRenderer renderer_;
    private readonly ISm64DynamicCollisionMesh? dynamicCollisionMesh_;

    public Sm64Object(
        ISm64Context context,
        Level level,
        Object3D obj) {
      var scripts = obj.ParseBehavior();
      foreach (var script in scripts) {
        if (script.Command == 0x2A) {
          var collisionAddress = BitLogic.BytesToInt(script.data, 4, 4);
          var cmap = CollisionMapLoader.Load(collisionAddress);
          var collisionBuilder = context.CreateDynamicCollisionMesh()
                                        .SetPosition(obj.xPos, obj.yPos, obj.zPos);
          Quad64LevelMeshLoader.CopyIntoBuilder<ISm64DynamicCollisionMeshBuilder,
              ISm64DynamicCollisionMesh>(
              cmap,
              collisionBuilder);
          this.dynamicCollisionMesh_ = collisionBuilder.Build();

          break;
        }
      }

      this.renderer_ = new Quad64ObjectRenderer(level, obj);
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
      //this.dynamicCollisionMesh_?.SetPosition()
    }

    public void Render() {
      this.renderer_.Render();
    }
  }
}