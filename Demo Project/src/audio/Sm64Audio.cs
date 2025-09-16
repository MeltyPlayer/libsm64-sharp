using System.Diagnostics;

using demo.common.audio;

using libsm64sharp;


namespace demo.audio;

public static class Sm64Audio {
  private static ICircularQueueActiveSound<short> circularQueueActiveSound_;

  private const int AUDIO_FREQUENCY_ = 32000;
  private const int AUDIO_BUFFER_SIZE_ = 544;

  public static void Start(ISm64Context sm64Context,
                           IAudioManager<short> audioManager) {
    Task.Run(() => {
      var stopwatch = new Stopwatch();

      try {
        Sm64Audio.circularQueueActiveSound_ =
            audioManager.CreateBufferedSound(
                AudioChannelsType.STEREO, AUDIO_FREQUENCY_, 2);

        var singleChannelLength = 2 * AUDIO_BUFFER_SIZE_;
        var singlePassBufferLength = 2 * singleChannelLength;

        // The more passes included in a single buffer, the longer the delay
        // but less stuttering.
        var passIndex = 0;
        var passCount = 2;

        var passLengths = new uint[passCount];
        var audioBuffers = new short[passCount][];

        for (var p = 0; p < passCount; ++p) {
          audioBuffers[p] = new short[singlePassBufferLength];
        }

        while (true) {
          stopwatch.Restart();

          var audioBuffer = audioBuffers[passIndex];
          var numSamples = sm64Context.TickAudio(
              Sm64Audio.circularQueueActiveSound_.QueuedSamples,
              1100,
              audioBuffer);

          passLengths[passIndex] = 2 * 2 * numSamples;

          if (passIndex == passCount - 1) {
            passIndex = 0;

            var totalAudioBufferLength = passLengths.Sum(v => v);
            var totalAudioBuffer = new short[totalAudioBufferLength];

            int totalIndex = 0;
            for (var bufferIndex = 0;
                 bufferIndex < audioBuffers.Length;
                 ++bufferIndex) {
              var buffer = audioBuffers[bufferIndex];
              var passLength = passLengths[bufferIndex];

              for (var s = 0; s < passLength; ++s) {
                totalAudioBuffer[totalIndex++] = buffer[s];
              }
            }

            Sm64Audio.circularQueueActiveSound_.PopulateNextBufferPcm(
                totalAudioBuffer);
          } else {
            passIndex++;
          }

          var targetSeconds = 1.0 / 30;
          var targetTicks = targetSeconds * Stopwatch.Frequency;

          // Expensive, but more accurate than Thread.sleep
          var i = 0;
          while (stopwatch.ElapsedTicks < targetTicks) {
            ++i;
          }
        }
      } catch (Exception ex) {
        ;
      }
    });
  }
}