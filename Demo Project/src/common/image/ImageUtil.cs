using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace demo.common.image {
  public static class ImageUtil {
    public static bool IsImageTransparent(Image<Rgba32> image) {
      var imageWidth = image.Width;
      var imageHeight = image.Height;

      var frame = image.Frames[0];
      for (var y = 0; y < imageHeight; y++) {
        for (var x = 0; x < imageWidth; x++) {
          var pixel = frame[x, y];
          if (pixel.A != 255) {
            return true;
          }
        }
      }

      return false;
    }
  }
}