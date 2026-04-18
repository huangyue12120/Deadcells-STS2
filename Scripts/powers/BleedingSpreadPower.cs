using BaseLib.Abstracts;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
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

    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver != base.Owner || target == base.Owner || power is not BleedingPower || amount <= 0) return base.ModifyPowerAmountGiven(power, giver, amount, target, cardSource);
        if (power is BleedingPower && amount > 0 && giver != null)
        {
            if(cardSource != null && cardSource is DeadcellsCardModel dc && dc.DCCDTags.Contains(DeadcellsCardTag.SingleBlood))
            {
                _isSpreading = true;
            }
            return amount + this.Amount;
        }
        return base.ModifyPowerAmountGiven(power, giver, amount, target, cardSource);
    }

    private bool _isSpreading = false;

    public override async Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        if (power is not BleedingPower) return;
        if (power.Applier != base.Owner) return;
        if (power.Owner == base.Owner) return;

        if (power is BleedingPower && _isSpreading)
        {
            try
            {
                this.Flash();
                foreach (Creature m in base.CombatState.HittableEnemies)
                {
                    if (m != power.Owner)
                    {
                        await PowerCmd.Apply<BleedingPower>(m, power.Amount, null, null);
                    }
                }
            }
            finally
            {
                this._isSpreading = false;
            }
        }
    }
}
