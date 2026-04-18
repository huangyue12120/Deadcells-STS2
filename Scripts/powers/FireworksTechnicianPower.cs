using BaseLib.Abstracts;
using BaseLib.Extensions;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using static Deadcells.Scripts.character.DeadcellsCardModel;

namespace Deadcells.Scripts.powers;

//燃爆大师
//手雷伤害提高X点
public sealed class FireworksTechnicianPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
    };

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (props.IsPoweredAttack_() && cardSource != null && cardSource is DeadcellsCardModel deadcellsCard && deadcellsCard.DCCDTags.Contains(DeadcellsCardTag.Grenade) && dealer == base.Owner && cardSource.Owner.Creature == base.Owner)
        {
            return this.Amount;
        }
        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource);
    }
}
