using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.cards;

//故技重施
//打出你上一张牌的复制. NL (不会打出 *故技重施 )
[Pool(typeof(DeadcellsCardPool))]
public sealed class InASimilar() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override bool IsPlayable => CombatManager.Instance.History.CardPlaysStarted.Count() > 0;

    private IEnumerable<CardModel> GetCardsLastPlay()
    {
        var playerCardPlays = CombatManager.Instance.History.CardPlaysStarted
            .Where(e => e.CardPlay.Card.Owner == base.Owner && e.CardPlay.Card is not InASimilar)
            .ToList();

        int count = this.IsUpgraded ? 2 : 1;

        // 从最后开始向前获取指定数量的卡牌
        for (int i = 1; i <= Math.Min(count, playerCardPlays.Count); i++)
        {
            yield return playerCardPlays[playerCardPlays.Count - i].CardPlay.Card;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cardsSelect = GetCardsLastPlay();
        if (cardsSelect.Any())
        {
            foreach (CardModel card in cardsSelect)
            {
                var autoPlay = card.CreateClone();
                if (card.IsUpgraded)
                {
                    CardCmd.Upgrade(autoPlay);
                }
                autoPlay.AddKeyword(CardKeyword.Exhaust);
                await CardCmd.AutoPlay(choiceContext, autoPlay, null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }

}