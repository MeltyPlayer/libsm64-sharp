using OpenTK.Graphics.OpenGL;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Color = System.Drawing.Color;


namespace demo.common.gl {
  public class GlTexture : IDisposable {
    private const int UNDEFINED_ID = -1;

    private int id_ = UNDEFINED_ID;

    public static GlTexture FromColor(Color color) {
      var image = new Image<Rgba32>(1, 1);
      var frame = image.Frames[0];

      var pixel = frame[0, 0];
      pixel.R = color.R;
      pixel.G = color.G;
      pixel.B = color.B;
      pixel.A = color.A;
      frame[0, 0] = pixel;

      return FromImage(image);
    }

    public static GlTexture FromImage(Image<Rgba32> image) => new(image);

    private GlTexture(Image<Rgba32> image) {
      GL.GenTextures(1, out int id);
      this.id_ = id;

      var target = TextureTarget.Texture2D;
      GL.BindTexture(target, this.id_);
      {
        this.LoadImageIntoTexture_(image);
      }

      GL.TexParameter(target, TextureParameterName.TextureMinFilter,
                      (int) TextureMinFilter.Nearest);
      GL.TexParameter(target, TextureParameterName.TextureMagFilter,
                      (int) TextureMagFilter.Linear);

      GL.BindTexture(target, UNDEFINED_ID);
    }

    private void LoadImageIntoTexture_(Image<Rgba32> image) {
      var imageWidth = image.Width;
      var imageHeight = image.Height;

      var rgba = new byte[4 * imageWidth * imageHeight];
      var frame = image.Frames[0];
      for (var y = 0; y < imageHeight; y++) {
        for (var x = 0; x < imageWidth; x++) {
          var pixel = frame[x, y];

          var outI = 4 * (y * imageWidth + x);
          rgba[outI] = pixel.R;
          rgba[outI + 1] = pixel.G;
          rgba[outI + 2] = pixel.B;
          rgba[outI + 3] = pixel.A;
        }
      }

      // TODO: Use different formats
      GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    imageWidth, imageHeight,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    rgba);
    }

    ~GlTexture() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      var id = this.id_;
      GL.DeleteTextures(1, ref id);

      this.id_ = UNDEFINED_ID;
    }

    public void Bind(int textureIndex = 0) {
      GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
      GL.BindTexture(TextureTarget.Texture2D, this.id_);
    }

    public void Unbind(int textureIndex = 0) {
      GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
      GL.BindTexture(TextureTarget.Texture2D, UNDEFINED_ID);
    }
  }
}