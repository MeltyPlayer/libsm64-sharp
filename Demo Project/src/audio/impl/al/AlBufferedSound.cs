using OpenTK.Audio.OpenAL;


namespace demo.audio.impl.al {
  public partial class AlAudioManager {
    public IBufferedSound<short> CreateBufferedSound(
        AudioChannelsType audioChannelsType,
        int frequency,
        int bufferSize,
        int bufferCount)
      => new AlBufferedSound(
          audioChannelsType,
          frequency,
          bufferSize,
          bufferCount);

    private class AlBufferedSound : IBufferedSound<short> {
      private readonly List<SingleBuffer> allBuffers_ = new();
      private readonly Queue<SingleBuffer> readyForDataBuffers_ = new();
      private readonly Dictionary<uint, SingleBuffer> buffersById_ = new();

      private class SingleBuffer : IDisposable {
        public uint alBufferId_;

        private bool isDisposed_;

        private readonly AudioChannelsType audioChannelsType_;
        private readonly int frequency_;
        private readonly int bufferSize_;

        public SingleBuffer(
            AudioChannelsType audioChannelsType,
            int frequency,
            int bufferSize) {
          this.audioChannelsType_ = audioChannelsType;
          this.frequency_ = frequency;
          this.bufferSize_ = bufferSize;

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
              bufferFormat = ALFormat.Mono16;
              break;
            }
            case AudioChannelsType.STEREO: {
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

          AL.SourceQueueBuffer((int) sourceId, (int) this.alBufferId_);
        }

        public void PopulateAndQueueUpInSource(
            short[][] data,
            uint sourceId) {
          this.AssertNotDisposed_();

          ALFormat bufferFormat = default;
          short[] shortBufferData = default!;
          switch (this.audioChannelsType_) {
            case AudioChannelsType.MONO: {
              bufferFormat = ALFormat.Mono16;
              shortBufferData = new short[1 * this.bufferSize_];

              for (var i = 0; i < shortBufferData.Length; ++i) {
                shortBufferData[i] = data[0][i];
              }

              break;
            }
            case AudioChannelsType.STEREO: {
              bufferFormat = ALFormat.Stereo16;
              shortBufferData = new short[2 * this.bufferSize_];

              // TODO: Is this correct, are they interleaved?
              for (var i = 0; i < shortBufferData.Length / 2; ++i) {
                shortBufferData[2 * i] = data[0][i];
                shortBufferData[2 * i] = data[1][i];
              }

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

          AL.SourceQueueBuffer((int) sourceId, (int) this.alBufferId_);
        }
      }

      private bool isDisposed_;
      private uint alSourceId_;

      public AlBufferedSound(
          AudioChannelsType audioChannelsType,
          int frequency,
          int bufferSize,
          int bufferCount) {
        this.AudioChannelsType = audioChannelsType;
        this.Frequency = frequency;
        this.BufferSize = bufferSize;
        this.BufferCount = bufferCount;

        AL.GenSource(out this.alSourceId_);

        for (var i = 0; i < bufferCount; ++i) {
          var buffer =
              new SingleBuffer(audioChannelsType, frequency, bufferSize);
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
      }

      public void Stop() {
        this.AssertNotDisposed_();
        AL.SourceStop(this.alSourceId_);
      }

      public void Pause() {
        this.AssertNotDisposed_();
        AL.SourcePause(this.alSourceId_);
      }

      public short GetPcm(AudioChannelType channelType) {
        throw new NotImplementedException();
      }

      public float Volume { get; set; }
      public int BufferSize { get; }
      public int BufferCount { get; }

      public void PopulateNextBufferPcm(short[] data) {
        AL.GetSource(this.alSourceId_, ALGetSourcei.BuffersProcessed,
                     out var numBuffersProcessed);

        if (numBuffersProcessed > 0) {
          var unqueuedBuffers =
              AL.SourceUnqueueBuffers((int) this.alSourceId_,
                                      numBuffersProcessed);
          foreach (var unqueuedBuffer in unqueuedBuffers) {
            this.readyForDataBuffers_.Enqueue(
                this.buffersById_[(uint) unqueuedBuffer]);
          }
        }

        if (this.readyForDataBuffers_.TryDequeue(out var nextBuffer)) {
          nextBuffer.PopulateAndQueueUpInSource(data, this.alSourceId_);
          this.Play();
        }
      }

      public void PopulateNextBufferPcm(short[][] data) {
        AL.GetSource(this.alSourceId_, ALGetSourcei.BuffersProcessed,
                     out var numBuffersProcessed);

        if (numBuffersProcessed > 0) {
          var unqueuedBuffers =
              AL.SourceUnqueueBuffers((int) this.alSourceId_,
                                      numBuffersProcessed);
          foreach (var unqueuedBuffer in unqueuedBuffers) {
            this.readyForDataBuffers_.Enqueue(
                this.buffersById_[(uint) unqueuedBuffer]);
          }
        }

        var nextBuffer = this.readyForDataBuffers_.Dequeue();
        nextBuffer.PopulateAndQueueUpInSource(data, this.alSourceId_);
        this.Play();
      }
    }
  }
}