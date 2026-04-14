using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.powers;
//无情
//你每对敌人施加 2 次负面效果，抽 1 张牌。
public sealed class RuthlessPower : CustomPowerModel
{
    private int _debuffCounter = 0;
    private int _triggerThreshold = 2;
    private int _triggerCountThisTurn = 0; // 本回合已触发次数
    private int _maxTriggersPerTurn = 5;  // 每回合最大触发次数
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Debuff", 2),
    };

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Type == PowerType.Debuff && power.Owner != applier && power.Applier == applier && applier == base.Owner && amount > 0)//&& _triggerCountThisTurn < _maxTriggersPerTurn)
        {
            _debuffCounter++;

            // 检查是否达到触发阈值（每2次）
            if (_debuffCounter % _triggerThreshold == 0)
            {
                await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), this.Amount, base.Owner.Player);
                //_triggerCountThisTurn++;
            }
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        _debuffCounter = 0;
        return base.AfterPlayerTurnStart(choiceContext, player);
    }
}
