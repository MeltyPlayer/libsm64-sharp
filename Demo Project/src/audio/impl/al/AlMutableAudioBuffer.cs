using demo.util.asserts;
using System;


namespace demo.audio.impl.al {
  public partial class AlAudioManager {
    public IMutableAudioBuffer<short> CreateMutableBuffer()
      => new AlMutableAudioBuffer();

    private class AlMutableAudioBuffer : IMutableAudioBuffer<short> {
      private short[][] channels_;

      public AudioChannelsType AudioChannelsType { get; private set; }

      public int Frequency { get; set; }

      public int SampleCount { get; private set; }

      public void SetPcm(short[][] channelSamples) {
        switch (channelSamples.Length) {
          case 1: {
            this.SetMonoPcm(channelSamples[0]);
            break;
          }
          case 2: {
            this.SetStereoPcm(channelSamples[0], channelSamples[1]);
            break;
          }
          default: throw new NotFiniteNumberException();
        }
      }


      public void SetMonoPcm(short[] samples) {
        this.AudioChannelsType = AudioChannelsType.MONO;
        this.SampleCount = samples.Length;
        this.channels_ = new[] {samples};
      }

      public void SetStereoPcm(short[] leftChannelSamples,
                               short[] rightChannelSamples) {
        Asserts.Equal(leftChannelSamples.Length, rightChannelSamples.Length,
                      "Expected the left/right channels to have the same number of samples!");

        this.AudioChannelsType = AudioChannelsType.STEREO;
        this.SampleCount = leftChannelSamples.Length;
        this.channels_ = new[] { leftChannelSamples, rightChannelSamples };
      }

      public short GetPcm(AudioChannelType channelType, int sampleOffset)
        => this.channels_[channelType switch {
            AudioChannelType.MONO         => 0,
            AudioChannelType.STEREO_LEFT  => 0,
            AudioChannelType.STEREO_RIGHT => 1
        }][sampleOffset];
    }
  }
}