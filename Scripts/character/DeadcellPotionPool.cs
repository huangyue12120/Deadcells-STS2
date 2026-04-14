using BaseLib.Abstracts;
using Godot;

namespace Deadcells.Scripts.character;

public sealed class DeadcellPotionPool : CustomPotionPoolModel
{
    //public override string EnergyColorName => "JokerMod:joker";

    public override string? BigEnergyIconPath => "res://Deadcells/images/ui/combat/beheaded_energy_icon.png";
    public override string? TextEnergyIconPath => "res://Deadcells/images/ui/combat/text_beheaded_energy_icon.png";

    public override Color LabOutlineColor => new Color("F4C95D");
}
