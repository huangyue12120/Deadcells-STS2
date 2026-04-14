using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//致命卡组
//造成 !D! 点伤害 NL 从弃牌堆里随机抽取一张技能牌(upg: drawpile 1 atk)
[Pool(typeof(DeadcellsCardPool))]
public sealed class KillingDeck() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool Green => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3, ValueProp.Move),
        new CardsVar(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

        IEnumerable<CardModel> cardsInDiscard = from c in PileType.Discard.GetPile(base.Owner).Cards where c != cardPlay.Card && c.Type == CardType.Skill select c;

        IEnumerable<CardModel> discardToMove = cardsInDiscard.ToList().UnstableShuffle(base.Owner.RunState.Rng.CombatCardSelection).Take(base.DynamicVars.Cards.IntValue);
        await CardPileCmd.Add(discardToMove, PileType.Hand, CardPilePosition.Bottom);
        if (this.IsUpgraded)
        {
            IEnumerable<CardModel> cardsInDraw = from c in PileType.Draw.GetPile(base.Owner).Cards where c != cardPlay.Card && c.Type == CardType.Attack select c;
            IEnumerable<CardModel> drawToMove = cardsInDraw.ToList().UnstableShuffle(base.Owner.RunState.Rng.CombatCardSelection).Take(base.DynamicVars.Cards.IntValue);
            await CardPileCmd.Add(drawToMove, PileType.Hand, CardPilePosition.Bottom);
        }
    }

    protected override void OnUpgrade()
    {

    }

}
