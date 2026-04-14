using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//渴血盾
//获得 !B! 点 格挡 . NL 下个回合当你受到伤害时,给予所有敌人 !M! 点 流血 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class BloodthirstyShield() : DeadcellsCardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
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
        HoverTipFactory.FromPower<BloodthirstyShieldPower>(),
        HoverTipFactory.FromPower<BleedingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(12, ValueProp.Move),
        new PowerVar<BloodthirstyShieldPower>(4)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<BloodthirstyShieldPower>(base.Owner.Creature, base.DynamicVars["BloodthirstyShieldPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4);
        base.DynamicVars["BloodthirstyShieldPower"].UpgradeValueBy(1);
    }

}
