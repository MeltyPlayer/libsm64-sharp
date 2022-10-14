using System.Runtime.InteropServices;

using libsm64sharp.lowlevel;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace libsm64sharp {
  public sealed partial class Sm64Context : ISm64Context {
    private const int SM64_TEXTURE_WIDTH = 64 * 11;
    private const int SM64_TEXTURE_HEIGHT = 64;
    private Image<Rgba32> marioTextureImage_;

    public Sm64Context(byte[] romBytes,
                       Action<string> debugPrintCallback) {
      var callbackDelegate = new DebugPrintFuncDelegate(debugPrintCallback);
      var romHandle = GCHandle.Alloc(romBytes, GCHandleType.Pinned);
      var textureData = new byte[4 * SM64_TEXTURE_WIDTH * SM64_TEXTURE_HEIGHT];
      var textureDataHandle = GCHandle.Alloc(textureData, GCHandleType.Pinned);

      LibSm64Interop.sm64_global_init(romHandle.AddrOfPinnedObject(),
                                      textureDataHandle.AddrOfPinnedObject(),
                                      Marshal.GetFunctionPointerForDelegate(
                                          callbackDelegate));

      this.marioTextureImage_ =
          new Image<Rgba32>(SM64_TEXTURE_WIDTH, SM64_TEXTURE_HEIGHT);
      {
        var frame = this.marioTextureImage_.Frames[0];
        for (var ix = 0; ix < SM64_TEXTURE_WIDTH; ix++) {
          for (var iy = 0; iy < SM64_TEXTURE_HEIGHT; iy++) {
            var pixel = frame[ix, iy];

            var pixelOffset = 4 * (ix + SM64_TEXTURE_WIDTH * iy);
            pixel.R = textureData[pixelOffset + 0];
            pixel.G = textureData[pixelOffset + 1];
            pixel.B = textureData[pixelOffset + 2];
            pixel.A = textureData[pixelOffset + 3];

            frame[ix, iy] = pixel;
          }
        }
      }

      romHandle.Free();
      textureDataHandle.Free();
    }

    ~Sm64Context() {
      this.ReleaseUnmanagedResources_();
    }

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_()
      => LibSm64Interop.sm64_global_terminate();
  }
}