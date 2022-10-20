using System.Runtime.InteropServices;

using libsm64sharp.lowlevel;


namespace libsm64sharp {
  public partial class Sm64Context {
    private static bool hasInitPlaySoundHandler_;
    private static PlaySoundFuncDelegate? playSoundHandler_;

    public class PlaySoundArgs {
      public Sm64SoundId SoundId { get; init; }
      public byte Priority { get; init; }
      public byte SoundStatus { get; init; }
      public byte BitFlags1 { get; init; }
      public byte BitFlags2 { get; init; }
      public IReadOnlySm64Vector3<float> Position { get; init; }
    }

    public delegate void PlaySoundFuncDelegate(PlaySoundArgs args);

    private static void PlaySoundFuncDelegateWrapper_(
        uint soundBits,
        ref LowLevelSm64Vector3f position) {
      if (Sm64Context.playSoundHandler_ == null) {
        return;
      }

      var firstByte = (byte) (soundBits >> 24);
      var secondByte = (byte) (soundBits >> 16);
      var thirdByte = (byte) (soundBits >> 8);
      var fourthByte = (byte) soundBits;

      var soundBank = (byte) (firstByte >> 4);
      var bitFlags1 = (byte) (firstByte & 0xF);

      var soundIdInBank = secondByte;
      var priority = thirdByte;

      var bitFlags2 = (byte) (fourthByte >> 4);
      var soundStatus = (byte) (fourthByte & 0xF);

      var soundId = (Sm64SoundId) ((soundBank << 8) | soundIdInBank);

      Sm64Context.playSoundHandler_(new PlaySoundArgs {
          SoundId = soundId,
          Priority = priority,
          SoundStatus = soundStatus,
          BitFlags1 = bitFlags1,
          BitFlags2 = bitFlags2,
          Position = position
      });
    }

    public static void RegisterPlaySoundFunction(
        PlaySoundFuncDelegate handler) {
      Sm64Context.playSoundHandler_ = handler;

      if (!Sm64Context.hasInitPlaySoundHandler_) {
        Sm64Context.hasInitPlaySoundHandler_ = true;
        lowlevel.PlaySoundFuncDelegate wrapper = PlaySoundFuncDelegateWrapper_;
        LibSm64Interop.sm64_register_play_sound_function(
            Marshal.GetFunctionPointerForDelegate(wrapper));
      }
    }
  }
}