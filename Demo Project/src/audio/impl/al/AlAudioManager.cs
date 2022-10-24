using OpenTK.Audio;


namespace demo.audio.impl.al {
  public partial class AlAudioManager : IAudioManager<short> {
    private readonly AudioContext context_ = new AudioContext();
  }
}