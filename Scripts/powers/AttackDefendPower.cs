using BaseLib.Abstracts;
using Deadcells.Scripts.cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Deadcells.Scripts.powers;

public sealed class AttackDefendPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ThrowingKnife>()
    };

    private CardType? LastCardType { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner.Player)
        {
            return;
        }

        CardType currentType = cardPlay.Card.Type;
        if (currentType is not (CardType.Attack or CardType.Skill))
        {
            this.LastCardType = null;
            return;
        }

        if (this.LastCardType == null || this.LastCardType == currentType)
        {
            this.LastCardType = currentType;
            return;
        }

        this.LastCardType = null;
        for (int index = 0; index < this.Amount; index++)
        {
            ThrowingKnife knife = CombatState.CreateCard<ThrowingKnife>(base.Owner.Player);
            await CardPileCmd.Add(knife, PileType.Hand);
        }
        this.Flash();
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner.Player)
        {
            this.LastCardType = null;
        }
        return base.AfterPlayerTurnStart(choiceContext, player);
    }
}
