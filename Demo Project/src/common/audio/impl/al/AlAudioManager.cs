using OpenTK.Audio;


namespace demo.common.audio.impl.al {
  public partial class AlAudioManager : IAudioManager<short> {
    private readonly AudioContext context_ = new AudioContext();
  }
}