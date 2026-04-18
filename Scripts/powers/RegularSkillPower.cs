using BaseLib.Abstracts;
using Deadcells.Scripts.cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.powers;
//常备技巧
//回合开始时将 1 张带有保留与消耗的翻滚置入你的手牌。
public sealed class RegularSkillPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<RegularSkill>(),
        HoverTipFactory.FromCard<Roll>(),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        this.Flash();
        for (int i = 0; i < this.Amount; i++)
        {
            Roll roll = CombatState.CreateCard<Roll>(base.Owner.Player);
            roll.AddKeyword(CardKeyword.Retain);
            roll.AddKeyword(CardKeyword.Exhaust);
            await CardPileCmd.Add(roll, PileType.Hand);
        }
    }
}
