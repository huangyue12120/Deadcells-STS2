using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//雷之盾
//获得 !B! 点 格挡 . NL 获得 !M! 点临时 力量 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class ThunderShield() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Green => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Shield];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(7, ValueProp.Move),
        new PowerVar<StrengthPower>(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<SetupStrikePower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2);
        base.DynamicVars.Strength.UpgradeValueBy(1);
    }

}
