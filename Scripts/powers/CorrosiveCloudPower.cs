using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.powers;

public sealed class CorrosiveCloudPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BleedingPower>(),
        HoverTipFactory.FromPower<PoisonPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("PoisonNumber", 0)
    };

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power.Target != applier && power.Applier == applier && applier == base.Owner && power is BleedingPower && amount > 0)
        {
            this.Flash();
            await Task.Run(() => base.DynamicVars["PoisonNumber"].UpgradeValueBy(Mathf.FloorToInt((float)amount / 3)));
            await PowerCmd.Apply<PoisonPower>(power.Owner, base.DynamicVars["PoisonNumber"].IntValue * this.Amount, base.Owner, null);
            await Task.Run(() => base.DynamicVars["PoisonNumber"].ResetToBase());
        }
    }
}