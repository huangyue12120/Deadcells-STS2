using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//翻滚
//获得 !B! 点 格挡 . NL 抽取3张牌,丢弃掉其中的 *攻击牌 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class Roll() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5, ValueProp.Move),
        new CardsVar(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        IEnumerable<CardModel> cards = (await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner)).Where((CardModel c) => c.Type == CardType.Attack);
        await CardCmd.Discard(choiceContext, cards);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3);
    }

}
