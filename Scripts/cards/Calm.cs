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

//冷静
//获得 [E] [E] .随机丢弃 !M! 张 *攻击牌 
[Pool(typeof(DeadcellsCardPool))]
public sealed class Calm() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.ForEnergy(this)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(2),
        new CardsVar(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        IEnumerable<CardModel> atkCards = from card in PileType.Hand.GetPile(base.Owner).Cards where card.Type == CardType.Attack select card;
        IEnumerable<CardModel> cardsToDiscard = atkCards.ToList().UnstableShuffle(base.Owner.RunState.Rng.CombatCardSelection).Take(base.DynamicVars.Cards.IntValue);
        await CardCmd.Discard(choiceContext, cardsToDiscard);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(-1);
    }

}