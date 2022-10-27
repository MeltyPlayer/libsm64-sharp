using OpenTK.Audio.OpenAL;


namespace demo.audio.impl.al {
  public partial class AlAudioManager {
    public IBufferedSound<short> CreateBufferedSound(
        AudioChannelsType audioChannelsType,
        int frequency,
        int bufferCount)
      => new AlBufferedSound(
          audioChannelsType,
          frequency,
          bufferCount);

    private class AlBufferedSound : IBufferedSound<short> {
      private readonly List<SingleBuffer> allBuffers_ = new();

      private readonly Queue<SingleBuffer> readyForDataBuffers_ = new();

      private readonly Dictionary<uint, SingleBuffer> buffersById_ = new();

      private class SingleBuffer : IDisposable {
        public uint alBufferId_;
        public int bufferSize;

        private bool isDisposed_;

        private readonly AudioChannelsType audioChannelsType_;
        private readonly int frequency_;

        public SingleBuffer(
            AudioChannelsType audioChannelsType,
            int frequency) {
          this.audioChannelsType_ = audioChannelsType;
          this.frequency_ = frequency;

          AL.GenBuffer(out this.alBufferId_);
        }

        ~SingleBuffer() => this.ReleaseUnmanagedResources_();

        public void Dispose() {
          this.AssertNotDisposed_();

          this.ReleaseUnmanagedResources_();
          GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources_() {
          this.isDisposed_ = true;
          AL.DeleteBuffer(ref this.alBufferId_);
        }

        private void AssertNotDisposed_() {
          if (this.isDisposed_) {
            throw new Exception("Expected active sound to not be disposed");
          }
        }

        public void PopulateAndQueueUpInSource(
            short[] shortBufferData,
            uint sourceId) {
          this.AssertNotDisposed_();

          ALFormat bufferFormat = default;
          switch (this.audioChannelsType_) {
            case AudioChannelsType.MONO: {
              this.bufferSize = shortBufferData.Length;
              bufferFormat = ALFormat.Mono16;
              break;
            }
            case AudioChannelsType.STEREO: {
              this.bufferSize = shortBufferData.Length / 2;
              bufferFormat = ALFormat.Stereo16;
              break;
            }
          }

          var byteCount = 2 * shortBufferData.Length;
          var byteBufferData = new byte[byteCount];
          Buffer.BlockCopy(shortBufferData, 0, byteBufferData, 0,
                           byteCount);

          AL.BufferData((int) this.alBufferId_,
                        bufferFormat,
                        byteBufferData,
                        byteCount,
                        this.frequency_);
          AssertNoError_();

          AL.SourceQueueBuffer((int) sourceId, (int) this.alBufferId_);
          AssertNoError_();
        }
      }

      private bool isDisposed_;
      private uint alSourceId_;

      public AlBufferedSound(
          AudioChannelsType audioChannelsType,
          int frequency,
          int bufferCount) {
        this.AudioChannelsType = audioChannelsType;
        this.Frequency = frequency;
        this.BufferCount = bufferCount;

        AL.GenSource(out this.alSourceId_);

        for (var i = 0; i < bufferCount; ++i) {
          var buffer = new SingleBuffer(audioChannelsType, frequency);
          this.allBuffers_.Add(buffer);
          this.readyForDataBuffers_.Enqueue(buffer);
          this.buffersById_[buffer.alBufferId_] = buffer;
        }
      }

      ~AlBufferedSound() => this.ReleaseUnmanagedResources_();

      public void Dispose() {
        this.AssertNotDisposed_();

        this.ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        this.isDisposed_ = true;
        AL.DeleteSource(ref this.alSourceId_);
      }

      private void AssertNotDisposed_() {
        if (this.State == SoundState.DISPOSED) {
          throw new Exception("Expected active sound to not be disposed");
        }
      }

      public AudioChannelsType AudioChannelsType { get; }
      public int Frequency { get; }

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
        AssertNoError_();
      }

      public void Stop() {
        this.AssertNotDisposed_();
        AL.SourceStop(this.alSourceId_);
        AssertNoError_();
      }

      public void Pause() {
        this.AssertNotDisposed_();
        AL.SourcePause(this.alSourceId_);
        AssertNoError_();
      }

      public short GetPcm(AudioChannelType channelType) {
        throw new NotImplementedException();
      }

      public float Volume { get; set; }

      public uint QueuedSamples {
        get {
          AL.GetSource(this.alSourceId_, ALGetSourcei.SampleOffset,
                       out var currentSample);

          this.FreeUpProcessedBuffers();

          var queuedBufferCount =
              this.allBuffers_.Count - this.readyForDataBuffers_.Count;

          var totalQueuedSamples = 0;
          foreach (var buffer in this.allBuffers_) {
            if (!this.readyForDataBuffers_.Contains(buffer)) {
              if (queuedBufferCount == 1) {
                var remainingSamples = buffer.bufferSize - currentSample;
                totalQueuedSamples += remainingSamples;
              } else {
                totalQueuedSamples += buffer.bufferSize;
              }
            }
          }

          return (uint) totalQueuedSamples;
        }
      }

      public int BufferCount { get; }

      public void FreeUpProcessedBuffers() {
        AL.GetSource(this.alSourceId_, ALGetSourcei.BuffersProcessed,
                     out var numBuffersProcessed);
        AssertNoError_();

        if (numBuffersProcessed > 0) {
          var unqueuedBuffers =
              AL.SourceUnqueueBuffers((int) this.alSourceId_,
                                      numBuffersProcessed);
          AssertNoError_();

          foreach (var unqueuedBuffer in unqueuedBuffers) {
            this.readyForDataBuffers_.Enqueue(
                this.buffersById_[(uint) unqueuedBuffer]);
          }
        }
      }

      public void PopulateNextBufferPcm(short[] data) {
        this.FreeUpProcessedBuffers();

        if (this.readyForDataBuffers_.TryDequeue(out var nextBuffer)) {
          nextBuffer.PopulateAndQueueUpInSource(data, this.alSourceId_);

          if (this.State != SoundState.PLAYING) {
            this.Play();
          }
        }
      }

      private static void AssertNoError_() {
        var error = AL.GetError();
        if (error != ALError.NoError) {
          ;
        }
      }
    }
  }
}