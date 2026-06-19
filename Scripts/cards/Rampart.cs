using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.cards;
//壁垒
//直到下一回合,你无法受到任何 *攻击 伤害. NL 丢弃掉手牌中所有的技能牌. 消耗 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class Rampart() : DeadcellsCardModel(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Shield];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<RampartPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<RampartPower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RampartPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RampartPower"].BaseValue, base.Owner.Creature, this);
        IEnumerable<CardModel> cards = from c in PileType.Hand.GetPile(base.Owner).Cards where c.Type == CardType.Skill select c;
        await CardCmd.Discard(choiceContext, cards);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

}
