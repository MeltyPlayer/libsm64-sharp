using OpenTK.Audio.OpenAL;


namespace demo.audio.impl.al {
  public partial class AlAudioManager {
    private partial class AlAudioSource {
      public IActiveSound<short> Create(IAudioBuffer<short> buffer)
        => Create(this.manager_.CreateBufferAudioStream(buffer));

      public IActiveSound<short> Create(IAudioStream<short> stream)
        => new AlActiveSound(stream);
    }

    private class AlActiveSound : IActiveSound<short> {
      private bool isDisposed_;
      private uint alBufferId_;
      private uint alSourceId_;

      public AlActiveSound(IAudioStream<short> stream) {
        this.Stream = stream;

        AL.GenBuffer(out this.alBufferId_);

        ALFormat bufferFormat = default;
        short[] shortBufferData = default!;
        switch (stream.AudioChannelsType) {
          case AudioChannelsType.MONO: {
            bufferFormat = ALFormat.Mono16;
            shortBufferData = new short[1 * stream.SampleCount];

            for (var i = 0; i < stream.SampleCount; ++i) {
              shortBufferData[i] = stream.GetPcm(AudioChannelType.MONO, i);
            }

            break;
          }
          case AudioChannelsType.STEREO: {
            bufferFormat = ALFormat.Stereo16;
            shortBufferData = new short[2 * stream.SampleCount];

            // TODO: Is this correct, are they interleaved?
            for (var i = 0; i < stream.SampleCount; ++i) {
              shortBufferData[2 * i] =
                  stream.GetPcm(AudioChannelType.STEREO_LEFT, i);
              shortBufferData[2 * i + 1] =
                  stream.GetPcm(AudioChannelType.STEREO_RIGHT, i);
            }

            break;
          }
        }

        var byteCount = 2 * shortBufferData.Length;
        var byteBufferData = new byte[byteCount];
        Buffer.BlockCopy(shortBufferData, 0, byteBufferData, 0,
                         byteCount);

        AL.BufferData((int)this.alBufferId_,
                      bufferFormat,
                      byteBufferData,
                      byteCount,
                      stream.Frequency);

        AL.GenSource(out this.alSourceId_);
        AL.BindBufferToSource(this.alSourceId_, this.alBufferId_);
      }

      ~AlActiveSound() => this.ReleaseUnmanagedResources_();

      public void Dispose() {
        this.AssertNotDisposed_();

        this.ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        this.isDisposed_ = true;
        AL.DeleteBuffer(ref this.alBufferId_);
        AL.DeleteSource(ref this.alSourceId_);
      }

      private void AssertNotDisposed_() {
        if (this.State == SoundState.DISPOSED) {
          throw new Exception("Expected active sound to not be disposed");
        }
      }

      public IAudioStream<short> Stream { get; }

      public AudioChannelsType AudioChannelsType
        => this.Stream.AudioChannelsType;

      public int Frequency => this.Stream.Frequency;
      public int SampleCount => this.Stream.SampleCount;

      public SoundState State
        => this.isDisposed_
               ? SoundState.DISPOSED
               : AL.GetSourceState(this.alSourceId_) switch {
                   ALSourceState.Initial => SoundState.STOPPED,
                   ALSourceState.Playing => SoundState.PLAYING,
                   ALSourceState.Paused => SoundState.PAUSED,
                   ALSourceState.Stopped => SoundState.STOPPED,
                   _ => throw new ArgumentOutOfRangeException()
               };

      public void Play() {
        this.AssertNotDisposed_();
        AL.SourcePlay(this.alSourceId_);
      }

      public void Stop() {
        this.AssertNotDisposed_();
        AL.SourceStop(this.alSourceId_);
      }

      public void Pause() {
        this.AssertNotDisposed_();
        AL.SourcePause(this.alSourceId_);
      }

      public int SampleOffset {
        get {
          this.AssertNotDisposed_();

          AL.GetSource(this.alSourceId_,
                       ALGetSourcei.SampleOffset,
                       out var sampleOffset);
          return sampleOffset;
        }
        set {
          this.AssertNotDisposed_();

          AL.Source(this.alSourceId_, ALSourcei.SampleOffset, (int)value);
        }
      }

      public short GetPcm(AudioChannelType channelType)
        => this.Stream.GetPcm(channelType, this.SampleOffset);

      public float Volume {
        get {
          this.AssertNotDisposed_();

          AL.GetSource(this.alSourceId_, ALSourcef.Gain, out var gain);
          return gain;
        }
        set {
          this.AssertNotDisposed_();
          AL.Source(this.alSourceId_, ALSourcef.Gain, value);
        }
      }

      public bool Looping {
        get {
          this.AssertNotDisposed_();

          AL.GetSource(this.alSourceId_, ALSourceb.Looping, out var looping);
          return looping;
        }
        set {
          this.AssertNotDisposed_();
          AL.Source(this.alSourceId_, ALSourceb.Looping, value);
        }
      }
    }
  }
}