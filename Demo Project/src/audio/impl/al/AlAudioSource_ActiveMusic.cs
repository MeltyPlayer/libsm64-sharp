using OpenTK.Audio.OpenAL;


namespace demo.audio.impl.al {
  public partial class AlAudioManager {
    private partial class AlAudioSource : IAudioSource<short> {
      public IActiveMusic<short> CreateMusic(
          IAudioBuffer<short> introBuffer,
          IAudioBuffer<short> loopBuffer)
        => CreateMusic(this.manager_.CreateBufferAudioStream(introBuffer),
                       this.manager_.CreateBufferAudioStream(loopBuffer));

      public IActiveMusic<short> CreateMusic(
          IAudioStream<short> introStream,
          IAudioStream<short> loopStream)
        => new AlActiveMusic(introStream, loopStream);
    }

    private class AlActiveMusic : IActiveMusic<short> {
      private bool isDisposed_;
      private uint alSourceId_;
      private uint alIntroBufferId_;
      private uint alLoopBufferId_;

      public AlActiveMusic(IAudioStream<short> introStream,
                           IAudioStream<short> loopStream) {
        this.IntroStream = introStream;
        this.LoopStream = loopStream;

        AL.GenSource(out this.alSourceId_);

        {
          AL.GenBuffer(out this.alIntroBufferId_);

          ALFormat bufferFormat = default;
          short[] shortBufferData = default!;
          switch (introStream.AudioChannelsType) {
            case AudioChannelsType.MONO: {
              bufferFormat = ALFormat.Mono16;
              shortBufferData = new short[1 * introStream.SampleCount];

              for (var i = 0; i < introStream.SampleCount; ++i) {
                shortBufferData[i] =
                    introStream.GetPcm(AudioChannelType.MONO, i);
              }

              break;
            }
            case AudioChannelsType.STEREO: {
              bufferFormat = ALFormat.Stereo16;
              shortBufferData = new short[2 * introStream.SampleCount];

              // TODO: Is this correct, are they interleaved?
              for (var i = 0; i < introStream.SampleCount; ++i) {
                shortBufferData[2 * i] =
                    introStream.GetPcm(AudioChannelType.STEREO_LEFT, i);
                shortBufferData[2 * i + 1] =
                    introStream.GetPcm(AudioChannelType.STEREO_RIGHT, i);
              }

              break;
            }
          }

          var byteCount = 2 * shortBufferData.Length;
          var byteBufferData = new byte[byteCount];
          Buffer.BlockCopy(shortBufferData, 0, byteBufferData, 0,
                           byteCount);

          AL.BufferData((int) this.alIntroBufferId_,
                        bufferFormat,
                        byteBufferData,
                        byteCount,
                        introStream.Frequency);

          AL.SourceQueueBuffer((int) this.alSourceId_,
                               (int) this.alIntroBufferId_);
        }

        {
          AL.GenBuffer(out this.alLoopBufferId_);

          ALFormat bufferFormat = default;
          short[] shortBufferData = default!;
          switch (loopStream.AudioChannelsType) {
            case AudioChannelsType.MONO: {
              bufferFormat = ALFormat.Mono16;
              shortBufferData = new short[1 * loopStream.SampleCount];

              for (var i = 0; i < loopStream.SampleCount; ++i) {
                shortBufferData[i] =
                    loopStream.GetPcm(AudioChannelType.MONO, i);
              }

              break;
            }
            case AudioChannelsType.STEREO: {
              bufferFormat = ALFormat.Stereo16;
              shortBufferData = new short[2 * loopStream.SampleCount];

              // TODO: Is this correct, are they interleaved?
              for (var i = 0; i < loopStream.SampleCount; ++i) {
                shortBufferData[2 * i] =
                    loopStream.GetPcm(AudioChannelType.STEREO_LEFT, i);
                shortBufferData[2 * i + 1] =
                    loopStream.GetPcm(AudioChannelType.STEREO_RIGHT, i);
              }

              break;
            }
          }

          var byteCount = 2 * shortBufferData.Length;
          var byteBufferData = new byte[byteCount];
          Buffer.BlockCopy(shortBufferData, 0, byteBufferData, 0,
                           byteCount);

          AL.BufferData((int) this.alLoopBufferId_,
                        bufferFormat,
                        byteBufferData,
                        byteCount,
                        loopStream.Frequency);

          AL.SourceQueueBuffer((int) this.alSourceId_,
                               (int) this.alLoopBufferId_);
        }

        Task.Run(() => {
          int processedBufferCount;
          do {
            AL.GetSource(this.alSourceId_, ALGetSourcei.BuffersProcessed,
                         out processedBufferCount);
            Thread.Sleep(1000);
          } while (processedBufferCount == 0);

          AL.SourceUnqueueBuffers((int) this.alSourceId_, processedBufferCount);
          AL.Source(this.alSourceId_, ALSourceb.Looping, true);
        });
      }

      ~AlActiveMusic() => this.ReleaseUnmanagedResources_();

      public void Dispose() {
        this.AssertNotDisposed_();

        this.ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        this.isDisposed_ = true;
        AL.DeleteBuffer(ref this.alIntroBufferId_);
        AL.DeleteBuffer(ref this.alLoopBufferId_);
        AL.DeleteSource(ref this.alSourceId_);
      }

      private void AssertNotDisposed_() {
        if (this.State == SoundState.DISPOSED) {
          throw new Exception("Expected active sound to not be disposed");
        }
      }


      public IAudioStream<short> IntroStream { get; }
      public IAudioStream<short> LoopStream { get; }

      public AudioChannelsType AudioChannelsType
        => throw new NotImplementedException();

      public int Frequency => throw new NotImplementedException();
      public int SampleCount => throw new NotImplementedException();

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

      public void Pause() {
        this.AssertNotDisposed_();
        AL.SourcePause(this.alSourceId_);
      }

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
    }
  }
}