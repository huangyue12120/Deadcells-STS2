using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Deadcells.Scripts.powers;
//夜光
//战斗结束时额外获得1组卡牌奖励 
public sealed class NightLightPower : CustomPowerModel
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
        room.AddExtraReward(base.Owner.Player, new CardReward(CardCreationOptions.ForRoom(base.Owner.Player, room.RoomType), 3, base.Owner.Player));
        return Task.CompletedTask;
    }
}