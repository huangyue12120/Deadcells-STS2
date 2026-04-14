using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Deadcells.Scripts.powers;

//掠夺
//战斗结束时额外获得1金
public sealed class RobPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    public override Task AfterCombatEnd(CombatRoom room)
    {
        room.AddExtraReward(base.Owner.Player, new GoldReward(base.Amount, base.Owner.Player));
        return Task.CompletedTask;
    }
}
