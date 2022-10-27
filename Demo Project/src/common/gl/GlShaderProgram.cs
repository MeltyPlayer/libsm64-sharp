using OpenTK.Graphics.OpenGL;


namespace demo.common.gl {
  public class GlShaderProgram : IDisposable {
    private const int UNDEFINED_ID = -1;

    private int vertexShaderId_ = UNDEFINED_ID;
    private int fragmentShaderId_ = UNDEFINED_ID;

    public static GlShaderProgram
        FromShaders(string vertexShaderSrc, string fragmentShaderSrc)
      => new(vertexShaderSrc, fragmentShaderSrc);

    private GlShaderProgram(string vertexShaderSrc,
                            string fragmentShaderSrc) {
      this.vertexShaderId_ =
          CreateAndCompileShader_(vertexShaderSrc, ShaderType.VertexShader);
      this.fragmentShaderId_ =
          CreateAndCompileShader_(fragmentShaderSrc, ShaderType.FragmentShader);

      this.ProgramId = GL.CreateProgram();

      GL.AttachShader(this.ProgramId, this.vertexShaderId_);
      GL.AttachShader(this.ProgramId, this.fragmentShaderId_);
      GL.LinkProgram(this.ProgramId);
    }

    ~GlShaderProgram() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      GL.DeleteProgram(this.ProgramId);
      if (this.vertexShaderId_ != UNDEFINED_ID) {
        GL.DeleteShader(this.vertexShaderId_);
      }
      if (this.fragmentShaderId_ != UNDEFINED_ID) {
        GL.DeleteShader(this.fragmentShaderId_);
      }

      this.ProgramId =
          this.vertexShaderId_ = this.fragmentShaderId_ = UNDEFINED_ID;
    }

    private static int CreateAndCompileShader_(string src,
                                               ShaderType shaderType) {
      var shaderId = GL.CreateShader(shaderType);
      GL.ShaderSource(shaderId, 1, new[] {src}, (int[]) null);
      GL.CompileShader(shaderId);

      // TODO: Throw/return this error
      var bufferSize = 10000;
      GL.GetShaderInfoLog(
          shaderId,
          bufferSize,
          out var shaderErrorLength,
          out var shaderError);

      if (shaderError?.Length > 0) {
        ;
      }

      return shaderId;
    }

    public int ProgramId { get; private set; } = UNDEFINED_ID;

    public void Use() => GL.UseProgram(this.ProgramId);

    public int GetUniformLocation(string name) =>
        GL.GetUniformLocation(this.ProgramId, name);

    public int GetAttribLocation(string name) =>
        GL.GetAttribLocation(this.ProgramId, name);
  }
}