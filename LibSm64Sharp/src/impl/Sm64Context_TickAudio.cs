using libsm64sharp.lowlevel;

using System.Runtime.InteropServices;


namespace libsm64sharp;

public sealed partial class Sm64Context {
  public uint TickAudio(uint numQueuedSamples,
                        uint numDesiredSamples,
                        short[] audioBuffer) {
    uint numSamples;
    {
      var audioBufferHandle =
          GCHandle.Alloc(audioBuffer, GCHandleType.Pinned);
      numSamples = LibSm64Interop.sm64_audio_tick(
          numQueuedSamples,
          numDesiredSamples,
          audioBufferHandle.AddrOfPinnedObject());
      audioBufferHandle.Free();
    }

    return numSamples;
  }
}