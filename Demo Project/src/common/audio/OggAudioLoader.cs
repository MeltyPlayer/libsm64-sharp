using NVorbis;


namespace demo.common.audio;

public class OggAudioLoader {
  public IAudioBuffer<short> LoadAudio(
      IAudioManager<short> audioManager,
      string oggFilePath) {
    using var ogg = new VorbisReader(oggFilePath);

    var mutableBuffer = audioManager.CreateMutableBuffer();
    mutableBuffer.Frequency = ogg.SampleRate;

    {
      var sampleCount = (int)ogg.TotalSamples;

      var channelCount = ogg.Channels;
      var floatCount = channelCount * sampleCount;
      var floatPcm = new float[floatCount];
      ogg.ReadSamples(floatPcm, 0, floatCount);

      var channels = new short[channelCount][];
      for (var c = 0; c < channelCount; ++c) {
        channels[c] = new short[sampleCount];
      }

      for (var i = 0; i < sampleCount; ++i) {
        for (var c = 0; c < channelCount; ++c) {
          var floatSample = floatPcm[2 * i + c];

          var floatMin = -1f;
          var floatMax = 1f;

          var normalizedFloatSample =
              (MathF.Max(floatMin, Math.Min(floatSample, floatMax)) -
               floatMin) / (floatMax - floatMin);

          float shortMin = short.MinValue;
          float shortMax = short.MaxValue;

          var shortSample =
              (short)Math.Round(shortMin +
                                normalizedFloatSample *
                                (shortMax - shortMin));

          channels[c][i] = shortSample;
        }
      }

      mutableBuffer.SetPcm(channels);
    }

    return mutableBuffer;
  }
}