using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//乌鸦之翼
//获得翱翔. NL 你在本回合每受到一次 *攻击 ,对攻击者造成 !D! 点伤害. NL (受攻击加成的影响)
[Pool(typeof(DeadcellsCardPool))]
public sealed class CrowFeathers() : DeadcellsCardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<SoarPower>(),
        HoverTipFactory.FromPower<NextTurnLoseFlightPower>(),
        HoverTipFactory.FromPower<FlameBarrierPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6, ValueProp.Move),
        new PowerVar<SoarPower>(1),
        new PowerVar<NextTurnLoseFlightPower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SoarPower>(base.Owner.Creature, base.DynamicVars["SoarPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<NextTurnLoseFlightPower>(base.Owner.Creature, base.DynamicVars["NextTurnLoseFlightPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<FlameBarrierPower>(base.Owner.Creature, base.DynamicVars.Damage.PreviewValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
