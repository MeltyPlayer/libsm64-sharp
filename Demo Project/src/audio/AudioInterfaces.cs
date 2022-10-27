using System.Numerics;


namespace demo.audio {
  public interface IAudioManager<TNumber> where TNumber : INumber<TNumber> {
    // TODO: Add method for creating mutable buffer
    // TODO: Add method for creating mutable circular buffers
    // TODO: Add support for looping a certain section of audio

    IMutableAudioBuffer<TNumber> CreateMutableBuffer();

    IBufferAudioStream<TNumber> CreateBufferAudioStream(
        IAudioBuffer<TNumber> buffer);

    IAudioSource<TNumber> CreateAudioSource();

    IBufferedSound<TNumber> CreateBufferedSound(
        AudioChannelsType audioChannelsType,
        int frequency,
        int bufferSize,
        int bufferCount);
  }


  public enum AudioChannelsType {
    UNDEFINED,
    MONO,
    STEREO,
  }

  public enum AudioChannelType {
    UNDEFINED,
    MONO,
    STEREO_LEFT,
    STEREO_RIGHT,
  }


  public interface IAudioFormat<out TNumber> where TNumber : INumber<TNumber> {
    AudioChannelsType AudioChannelsType { get; }
    int Frequency { get; }
  }

  public interface IStaticAudioFormat<out TNumber>
      : IAudioFormat<TNumber> where TNumber : INumber<TNumber> {
    int SampleCount { get; }
  }


  public interface IAudioData<out TNumber> : IStaticAudioFormat<TNumber>
      where TNumber : INumber<TNumber> {
    TNumber GetPcm(AudioChannelType channelType, int sampleOffset);
  }


  /// <summary>
  ///   Type for storing static audio data, e.g. a loaded audio file.
  /// </summary>
  public interface IAudioBuffer<out TNumber> : IAudioData<TNumber>
      where TNumber : INumber<TNumber> { }


  public interface IMutableAudioBuffer<TNumber> : IAudioBuffer<TNumber>
      where TNumber : INumber<TNumber> {
    new int Frequency { get; set; }

    void SetPcm(TNumber[][] channelSamples);

    void SetMonoPcm(TNumber[] samples);

    void SetStereoPcm(TNumber[] leftChannelSamples,
                      TNumber[] rightChannelSamples);
  }


  /// <summary>
  ///   Type that streams out audio data. Can be used as an input for other
  ///   streams to apply effects, or played out to the speakers via an audio
  ///   source.
  /// </summary>
  public interface IAudioStream<out TNumber>
      : IAudioData<TNumber> where TNumber : INumber<TNumber> { }

  public interface IBufferAudioStream<TNumber> : IAudioStream<TNumber>
      where TNumber : INumber<TNumber> {
    IAudioBuffer<TNumber> Buffer { get; }
    bool Reversed { get; set; }
  }


  public interface IAudioSource<TNumber> where TNumber : INumber<TNumber> {
    IActiveSound<TNumber> Create(IAudioBuffer<TNumber> buffer);
    IActiveSound<TNumber> Play(IAudioBuffer<TNumber> buffer);

    IActiveSound<TNumber> Create(IAudioStream<TNumber> stream);
    IActiveSound<TNumber> Play(IAudioStream<TNumber> stream);
  }

  public enum SoundState {
    UNDEFINED,
    STOPPED,
    PLAYING,
    PAUSED,
    DISPOSED,
  }

  public interface IActiveSound<out TNumber>
      : IStaticAudioFormat<TNumber>, IDisposable
      where TNumber : INumber<TNumber> {
    IAudioStream<TNumber> Stream { get; }

    SoundState State { get; }

    void Play();
    void Stop();
    void Pause();

    int SampleOffset { get; set; }
    TNumber GetPcm(AudioChannelType channelType);

    float Volume { get; set; }
    bool Looping { get; set; }
  }

  public interface IBufferedSound<TNumber>
      : IAudioFormat<TNumber>, IDisposable where TNumber : INumber<TNumber> {
    SoundState State { get; }

    void Play();
    void Stop();
    void Pause();

    TNumber GetPcm(AudioChannelType channelType);

    float Volume { get; set; }

    int BufferSize { get; }
    int BufferCount { get; }

    void PopulateNextBufferPcm(TNumber[] data);
    void PopulateNextBufferPcm(TNumber[][] data);
  }
}