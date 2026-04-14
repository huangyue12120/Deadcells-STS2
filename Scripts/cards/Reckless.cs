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
//莽撞
//随机打出 !M! 张你抽牌堆中的攻击牌并使其 消耗 . 
[Pool(typeof(DeadcellsCardPool))]
public sealed class Reckless() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cardsToDraw = from c in PileType.Draw.GetPile(base.Owner).Cards where c.Type == CardType.Attack select c;
        IEnumerable<CardModel> cardsToAutoPlay = cardsToDraw.ToList().UnstableShuffle(base.Owner.RunState.Rng.CombatCardSelection).Take(base.DynamicVars.Cards.IntValue);
        foreach (CardModel cardModel in cardsToAutoPlay)
        {
            cardModel.AddKeyword(CardKeyword.Exhaust);
            await CardCmd.AutoPlay(choiceContext, cardModel, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }

}