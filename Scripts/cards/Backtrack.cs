using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.cards;

//返回
//将你的其他手牌弃入 *抽牌堆 . NL 从你的 *弃牌堆 中抽取等量卡牌. NL 消耗 
[Pool(typeof(DeadcellsCardPool))]
public sealed class Backtrack() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Red => true;

    protected override bool Purple => true;

    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = PileType.Hand.GetPile(base.Owner).Cards
            .Where(c => c != cardPlay.Card)
            .ToList();

        var discardCards = PileType.Discard.GetPile(base.Owner).Cards
            .Where(c => c != cardPlay.Card)
            .ToList();

        if (handCards.Count > 0)
        {
            await CardPileCmd.Add(handCards, PileType.Draw, CardPilePosition.Random);
        }

        if (discardCards.Count > 0)
        {
            if (discardCards.Count >= handCards.Count)
            {
                var shuffledDiscard = discardCards.UnstableShuffle(base.Owner.RunState.Rng.CombatCardSelection);
                var cardsToMove = shuffledDiscard.Take(handCards.Count).ToList();
                await CardPileCmd.Add(cardsToMove, PileType.Hand, CardPilePosition.Bottom);
            }
            else
            {
                await CardPileCmd.Add(discardCards, PileType.Hand, CardPilePosition.Bottom);
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }

}
