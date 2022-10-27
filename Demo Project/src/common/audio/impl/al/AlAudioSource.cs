using OpenTK.Audio.OpenAL;


namespace demo.common.audio.impl.al {
  public partial class AlAudioManager {
    public IAudioSource<short> CreateAudioSource() => new AlAudioSource(this);

    private partial class AlAudioSource : IAudioSource<short> {
      private readonly AlAudioManager manager_;

      public AlAudioSource(AlAudioManager manager) {
        this.manager_ = manager;
      }
    }
  }
}