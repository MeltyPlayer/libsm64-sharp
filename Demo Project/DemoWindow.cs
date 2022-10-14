using libsm64sharp;

using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using System.Diagnostics;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;

  public DemoWindow(GameWindowSettings gameWindowSettings,
                    NativeWindowSettings nativeWindowSettings) : base(
      gameWindowSettings, nativeWindowSettings) {
    var sm64RomBytes = File.ReadAllBytes("sm64.z64");

    this.sm64Context_ = new Sm64Context(sm64RomBytes);
  }

  private void ResetGl_() {
    GL.Enable(EnableCap.PointSmooth);
    GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

    GL.ClearDepth(5.0F);

    GL.DepthFunc(DepthFunction.Lequal);
    GL.Enable(EnableCap.DepthTest);
    GL.DepthMask(true);

    GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

    GL.Enable(EnableCap.Texture2d);
    GL.Enable(EnableCap.Normalize);

    GL.Enable(EnableCap.CullFace);
    GL.CullFace(CullFaceMode.Back);

    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
  }

  protected override void OnRenderFrame(FrameEventArgs args) {
    this.ResetGl_();

    GL.ClearColor(1, 0, 0, 1);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


    GL.Flush();
    this.SwapBuffers();
  }
}