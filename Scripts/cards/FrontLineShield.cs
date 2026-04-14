using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//前线盾
//获得 6/9 点 格挡 . 将 2 张 虚无 与 消耗 *攻击牌 置入你的手牌
[Pool(typeof(DeadcellsCardPool))]
public sealed class FrontLineShield() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override bool Red => true;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Shield];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6, ValueProp.Move),
        new CardsVar(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        IEnumerable<CardModel> grenadeCards = CardFactory.GetForCombat(base.Owner, from c in ModelDb.CardPool<DeadcellsCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                                                                                   where c.Type == CardType.Attack
                                                                                   select c, base.DynamicVars.Cards.IntValue, base.Owner.RunState.Rng.CombatCardGeneration);
        foreach (CardModel cardModel in grenadeCards)
        {
            cardModel.AddKeyword(CardKeyword.Exhaust);
            cardModel.AddKeyword(CardKeyword.Ethereal);
        }
        await CardPileCmd.Add(grenadeCards, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3);
    }

}
