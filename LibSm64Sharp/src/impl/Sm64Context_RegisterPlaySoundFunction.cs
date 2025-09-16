using System.Runtime.InteropServices;

using libsm64sharp.lowlevel;


namespace libsm64sharp;

public partial class Sm64Context {
  private static bool hasInitPlaySoundHandler_;
  private static PlaySoundFuncDelegate? playSoundHandler_;
  private static lowlevel.PlaySoundFuncDelegate? playSoundFuncDelegate_;

  public class PlaySoundArgs {
    public PlaySoundArgs(Sm64SoundId soundId,
                         byte priority,
                         byte soundStatus,
                         byte bitFlags1,
                         byte bitFlags2,
                         IReadOnlySm64Vector3f position) {
      this.SoundId = soundId;
      this.Priority = priority;
      this.SoundStatus = soundStatus;
      this.BitFlags1 = bitFlags1;
      this.BitFlags2 = bitFlags2;
      this.Position = position;
    }

    public Sm64SoundId SoundId { get; private set; }
    public byte Priority { get; private set; }
    public byte SoundStatus { get; private set; }
    public byte BitFlags1 { get; private set; }
    public byte BitFlags2 { get; private set; }
    public IReadOnlySm64Vector3f Position { get; private set; }
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

    Sm64Context.playSoundHandler_(
        new PlaySoundArgs(soundId, priority, soundStatus, bitFlags1, bitFlags2,
                          position
        ));
  }

  public static void RegisterPlaySoundFunction(
      PlaySoundFuncDelegate handler) {
    Sm64Context.playSoundHandler_ = handler;

    if (!Sm64Context.hasInitPlaySoundHandler_) {
      Sm64Context.hasInitPlaySoundHandler_ = true;
      Sm64Context.playSoundFuncDelegate_ = PlaySoundFuncDelegateWrapper_;
      LibSm64Interop.sm64_register_play_sound_function(
          Marshal.GetFunctionPointerForDelegate(
              Sm64Context.playSoundFuncDelegate_));
    }
  }
}