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

//钢管
//获得 !B! 点 格挡 . 在你的下一个回合开始时对所有敌人造成你剩余格挡的伤害. NL 结束你的回合.
[Pool(typeof(DeadcellsCardPool))]
public sealed class IronStaff() : DeadcellsCardModel(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Red => true;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<IronStaffPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(24, ValueProp.Move),
        new PowerVar<IronStaffPower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<IronStaffPower>(base.Owner.Creature, base.DynamicVars["IronStaffPower"].BaseValue, base.Owner.Creature, this);
        await Task.Run(() => PlayerCmd.EndTurn(base.Owner, canBackOut: false));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(6);
    }

}
