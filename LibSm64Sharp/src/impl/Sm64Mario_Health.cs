using libsm64sharp.lowlevel;

using System.Numerics;


namespace libsm64sharp;

public partial class Sm64Context {
  private partial class Sm64Mario : ISm64Mario {
    public void Damage(uint damage,
                       DamageType damageType,
                       Vector3 position)
      => LibSm64Interop.sm64_mario_take_damage(
          this.id_, damage, damageType, position.X, position.Y, position.Z);

    public void Heal(byte healCounter)
      => LibSm64Interop.sm64_mario_heal(this.id_, healCounter);

    public void Kill() => LibSm64Interop.sm64_mario_kill(this.id_);
  }
}