using BaseLib.Abstracts;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using static Deadcells.Scripts.character.DeadcellsCardModel;

namespace Deadcells.Scripts.powers;

public sealed class BleedingSpreadPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BleedingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver != base.Owner || target == null || target.Side == base.Owner.Side || power is not BleedingPower || amount <= 0)
            return base.ModifyPowerAmountGivenAdditive(power, giver, amount, target, cardSource);
        return this.Amount;
    }

    //0.103.2版本原内容
    //public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    //{
    //    if (giver != base.Owner || target == base.Owner || power is not BleedingPower || amount <= 0) return base.ModifyPowerAmountGiven(power, giver, amount, target, cardSource);
    //    if (power is BleedingPower && amount > 0 && giver != null)
    //    {
    //        if(cardSource != null && cardSource is DeadcellsCardModel dc && dc.DCCDTags.Contains(DeadcellsCardTag.SingleBlood))
    //        {
    //            _isSpreading = true;
    //        }
    //        return amount + this.Amount;
    //    }
    //    return base.ModifyPowerAmountGiven(power, giver, amount, target, cardSource);
    //}

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not BleedingPower || applier != base.Owner || power.Owner.Side == base.Owner.Side ||
            amount <= this.Amount || cardSource is not DeadcellsCardModel card ||
            !card.DCCDTags.Contains(DeadcellsCardTag.SingleBlood))
            return;

        this.Flash();
        decimal bleeding = amount - this.Amount;
        foreach (Creature enemy in base.CombatState.HittableEnemies.ToList())
        {
            if (enemy != power.Owner)
            {
                await PowerCmd.Apply<BleedingPower>(choiceContext, enemy, bleeding, base.Owner, null);
            }
        }
    }
}
