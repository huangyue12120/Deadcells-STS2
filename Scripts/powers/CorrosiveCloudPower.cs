using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.powers;

public sealed class CorrosiveCloudPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BleedingPower>(),
        HoverTipFactory.FromPower<PoisonPower>()
    };

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is BleedingPower && power.Owner.Side != base.Owner.Side && applier == base.Owner && amount >= 3)
        {
            this.Flash();
            decimal poison = decimal.Floor(amount / 3) * this.Amount;
            await PowerCmd.Apply<PoisonPower>(choiceContext, power.Owner, poison, base.Owner, null);
        }
    }
}
