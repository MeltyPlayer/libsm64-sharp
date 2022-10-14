using libsm64sharp.lowlevel;


namespace libsm64sharp {
  public partial class Sm64Context {
    public ISm64Mario CreateMario(short x, short y, short z)
      => new Sm64Mario(x, y, z);

    private class Sm64Mario : ISm64Mario {
      private readonly uint id_;

      public Sm64Mario(short x, short y, short z) {
        this.id_ = LibSm64Interop.sm64_mario_create(x, y, z);
      }

      ~Sm64Mario() {
        this.ReleaseUnmanagedResources_();
      }

      public void Dispose() {
        this.ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_()
        => LibSm64Interop.sm64_mario_delete(this.id_);
    }
  }
}