using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.cards;

//A计划
//丢弃掉手中的技能牌,从抽牌堆中抽取等量攻击牌,获得 [E] .
[Pool(typeof(DeadcellsCardPool))]
public sealed class APlan() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
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
        HoverTipFactory.ForEnergy(this)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var skillCards = PileType.Hand.GetPile(base.Owner).Cards
        .Where(c => c.Type == CardType.Skill)
        .ToList();

        var attackCardsInDraw = PileType.Draw.GetPile(base.Owner).Cards
            .Where(c => c.Type == CardType.Attack)
            .ToList();

        int discardNumber = skillCards.Count;

        await CardCmd.Discard(choiceContext, skillCards);

        for (int i = 0; i < attackCardsInDraw.Count && i < discardNumber; i++)
        {
            await CardPileCmd.Add(attackCardsInDraw[i], PileType.Hand);
        }

        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }

}
