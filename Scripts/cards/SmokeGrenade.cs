using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.cards;

//烟雾弹
//获得 !CN! 层 隐匿 . NL 你的下一张攻击牌伤害增加 !M! 点. NL 消耗 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class SmokeGrenade() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Sly,
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Grenade];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<GrenadePower>(),
        HoverTipFactory.FromPower<SmokeBombPower>(),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<SmokeBombPower>(1),
        new PowerVar<VigorPower>(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SmokeBombPower>(base.Owner.Creature, base.DynamicVars["SmokeBombPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<VigorPower>(base.Owner.Creature, base.DynamicVars["VigorPower"].BaseValue, base.Owner.Creature, this);
        await base.OnPlay(choiceContext, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["VigorPower"].UpgradeValueBy(3);
    }

}
