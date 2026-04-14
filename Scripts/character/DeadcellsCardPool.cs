using BaseLib.Abstracts;
using Godot;

namespace Deadcells.Scripts.character;

public sealed class DeadcellsCardPool : CustomCardPoolModel
{
    public override string Title => "Beheaded";

    //public override string EnergyColorName => "beheaded";

    //public override string CardFrameMaterialPath => "card_frame_red";
    public override float H => 1f; //Hue; changes the color. 
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness

    public override Color DeckEntryCardColor => Colors.White;

    public override Color EnergyOutlineColor => Colors.DarkBlue; //4C0099

    public override bool IsColorless => false;
    public override string? BigEnergyIconPath => "res://Deadcells/images/ui/combat/beheaded_energy_icon.png";
    public override string? TextEnergyIconPath => "res://Deadcells/images/ui/combat/text_beheaded_energy_icon.png";
}
