using BaseLib.Abstracts;
using Deadcells.Scripts.cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.powers;

public sealed class AttackDefendPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ThrowingKnife>()
    };

    private bool CanGive { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var entries = CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().ToList();
        if (entries.Count >= 2)
        {
            CardModel lastCard = entries[^2].CardPlay.Card;
            if (this.CanGive)
            {
                if (cardPlay.Card.Type is CardType.Attack && lastCard.Type is CardType.Skill)
                {
                    for (int i = 0; i < this.Amount; i++)
                    {
                        ThrowingKnife knife = CombatState.CreateCard<ThrowingKnife>(base.Owner.Player);
                        await CardPileCmd.Add(knife, PileType.Hand);
                    }
                    this.Flash();
                    this.CanGive = false;
                }
                if (cardPlay.Card.Type is CardType.Skill && lastCard.Type is CardType.Attack)
                {
                    for (int i = 0; i < this.Amount; i++)
                    {
                        ThrowingKnife knife = CombatState.CreateCard<ThrowingKnife>(base.Owner.Player);
                        await CardPileCmd.Add(knife, PileType.Hand);
                    }
                    this.Flash();
                    this.CanGive = false;
                }
            }
            else
            {
                this.CanGive = true;
            }
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        this.CanGive = true;
        return base.AfterPlayerTurnStart(choiceContext, player);
    }
}