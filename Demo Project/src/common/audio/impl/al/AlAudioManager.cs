using OpenTK.Audio;


namespace demo.common.audio.impl.al;

public partial class AlAudioManager : IAudioManager<short> {
  private readonly AudioContext context_ = new();

  public void Dispose() {
    this.ReleaseUnmanagedResources_();
    GC.SuppressFinalize(this);
  }

  private void ReleaseUnmanagedResources_() => this.context_.Dispose();
}