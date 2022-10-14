using libsm64sharp;

using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace demo;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;
  private readonly ISm64Mario sm64Mario_;

  public DemoWindow(GameWindowSettings gameWindowSettings,
                    NativeWindowSettings nativeWindowSettings) : base(
      gameWindowSettings, nativeWindowSettings) {
    var sm64RomBytes = File.ReadAllBytes("sm64.z64");

    this.sm64Context_ = new Sm64Context(sm64RomBytes);
    this.sm64Mario_ = this.sm64Context_.CreateMario(0, 0, 0);
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

    GL.Enable(EnableCap.Texture2D);
    GL.Enable(EnableCap.Normalize);

    GL.Enable(EnableCap.CullFace);
    GL.CullFace(CullFaceMode.Back);

    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
  }

  protected override void OnUpdateFrame(FrameEventArgs args) {
    this.sm64Mario_.Tick();
  }

  protected override void OnRenderFrame(FrameEventArgs args) {
    this.ResetGl_();

    GL.ClearColor(1, 0, 0, 1);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    var marioMesh = this.sm64Mario_.Mesh;
    var marioMeshTriangleData = marioMesh.TriangleData;
    if (marioMeshTriangleData != null) {
      GL.Begin(PrimitiveType.Triangles);

      for (var i = 0; i < marioMeshTriangleData.TriangleCount; ++i) {
        for (var v = 0; v < 3; ++v) {
          var offset = 3 * i + v;

          var vertexPosition = marioMeshTriangleData.Positions[offset];
          GL.Vertex3(vertexPosition.X, vertexPosition.Y, vertexPosition.Z);

          var vertexColor = marioMeshTriangleData.Colors[offset];
          GL.Color3(vertexColor.X, vertexColor.Y, vertexColor.Z);
        }
      }

      GL.End();
    }

    GL.Flush();
    this.SwapBuffers();
  }
}