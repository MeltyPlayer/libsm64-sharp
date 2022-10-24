namespace demo.audio.impl.al {
  public partial class AlAudioManager {
    public IBufferAudioStream<short> CreateBufferAudioStream(
        IAudioBuffer<short> buffer)
      => new AlBufferAudioStream(buffer);

    private class AlBufferAudioStream : IBufferAudioStream<short> {
      public AlBufferAudioStream(IAudioBuffer<short> buffer) {
        this.Buffer = buffer;
      }

      public AudioChannelsType AudioChannelsType
        => this.Buffer.AudioChannelsType;

      public int Frequency => this.Buffer.Frequency;
      public int SampleCount => this.Buffer.SampleCount;

      public short GetPcm(AudioChannelType channelType, int sampleOffset) {
        sampleOffset = !this.Reversed
                           ? sampleOffset
                           : this.Buffer.SampleCount - sampleOffset;

        return this.Buffer.GetPcm(channelType, sampleOffset);
      }

      public IAudioBuffer<short> Buffer { get; }
      public bool Reversed { get; set; }
    }
  }
}