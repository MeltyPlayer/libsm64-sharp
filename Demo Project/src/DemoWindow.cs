using System.Runtime.InteropServices;

using demo.audio;
using demo.audio.impl.al;
using demo.camera;
using demo.controller;
using demo.gl;
using demo.mesh;

using libsm64sharp;
using libsm64sharp.lowlevel;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace demo;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;

  private readonly ISm64Mario sm64Mario_;
  private readonly MarioController marioController_;
  private readonly MarioMeshRenderer marioMeshRenderer_;

  private readonly StaticAssimpSceneRenderer staticAssimpSceneRenderer_;
  private readonly StaticCollisionMeshRenderer staticCollisionMeshRenderer_;

  private MarioOrbitingCamera camera_;
  private MarioOrbitingCameraController cameraController_;

  private IAudioManager<short> audioManager_;
  private IBufferedSound<short> bufferedSound_;
  private IAudioBuffer<short> audioBuffer_;
  private IAudioSource<short> audioSource_;
  private IActiveSound<short> activeSound_;

  private bool isGlInit_;

  private const int AUDIO_FREQUENCY_ = 22050;
  private const int AUDIO_BUFFER_SIZE_ = 544;

  public DemoWindow() {
    Sm64Context.RegisterDebugPrintFunction(text => {
      //Debug.WriteLine(text);
    });

    Sm64Context.RegisterPlaySoundFunction(
        args => {
          // TODO: Play sounds
          ;
        });

    var sm64RomBytes = File.ReadAllBytes("rom\\sm64.z64");
    this.sm64Context_ = Sm64Context.InitFromRom(sm64RomBytes);

    {
      var audioBanks = this.sm64Context_.LoadAudioBanks();

      this.audioManager_ = new AlAudioManager();

      this.bufferedSound_ =
          this.audioManager_.CreateBufferedSound(
              AudioChannelsType.STEREO, 22050 / 2 * 3, 544, 5);
      this.bufferedSound_.Play();

      /*var firstInstrument =
     audioBanks.CtlEntries.Skip(8).First().Instruments.Skip(1).First();
 var state = firstInstrument.NormalNotesSound.Sample.Loop.State;
 this.audioBuffer_ =
     AifcAudioDecoder.Decode(this.audioManager_,
                             firstInstrument.NormalNotesSound);

 this.audioSource_ = this.audioManager_.CreateAudioSource();

 this.activeSound_ = this.audioSource_.Create(this.audioBuffer_);
 this.activeSound_.Looping = true;
 this.activeSound_.Play();*/
    }

    var (assimpScene, staticCollisionMesh) =
        new LevelMeshLoader().LoadAndCreateCollisionMesh(this.sm64Context_);
    this.staticAssimpSceneRenderer_ =
        new StaticAssimpSceneRenderer(assimpScene);
    this.staticCollisionMeshRenderer_ =
        new StaticCollisionMeshRenderer(staticCollisionMesh);

    this.sm64Mario_ = this.sm64Context_.CreateMario(0, 900, 0);
    this.camera_ = new MarioOrbitingCamera(this.sm64Mario_);

    this.marioController_ =
        new MarioController(this.sm64Mario_, this.camera_, this);
    this.marioMeshRenderer_ = new MarioMeshRenderer(this.sm64Mario_.Mesh);

    this.cameraController_ =
        new MarioOrbitingCameraController(this.camera_, this);
  }

  private void InitGL_() {
    if (this.isGlInit_) {
      return;
    }

    this.isGlInit_ = true;

    GL.ShadeModel(ShadingModel.Smooth);
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

    GL.ClearColor(.5f, .5f, .5f, 1);
  }


  protected override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    this.marioController_.BeforeTick();
    this.sm64Mario_.Tick();

    var singleChannelLength = AUDIO_BUFFER_SIZE_;
    var singleChannelLengthInternal = 2 * singleChannelLength;

    var audioBuffer = new short[2 * singleChannelLengthInternal];
    {
      var audioBufferHandle = GCHandle.Alloc(audioBuffer, GCHandleType.Pinned);
      LibSm64Interop.sm64_tick_audio(audioBufferHandle.AddrOfPinnedObject());
      audioBufferHandle.Free();
    }

    this.bufferedSound_.PopulateNextBufferPcm(audioBuffer);
  }

  protected override void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);
    this.InitGL_();

    var width = this.Width;
    var height = this.Height;
    GL.Viewport(0, 0, width, height);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    this.RenderPerspective_();

    GL.Flush();
    this.SwapBuffers();
  }

  private void RenderPerspective_() {
    var width = this.Width;
    var height = this.Height;

    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GlUtil.Perspective(this.camera_.FovY, 1.0 * width / height, 1, 50000);
      GlUtil.LookAt(this.camera_);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
    }

    this.marioMeshRenderer_.Render();
    this.staticAssimpSceneRenderer_.Render();
    //this.staticCollisionMeshRenderer_.Render();
  }
}