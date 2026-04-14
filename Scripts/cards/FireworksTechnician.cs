using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.cards;

//燃爆大师
//你的手雷伤害提高 4/6 点。
[Pool(typeof(DeadcellsCardPool))]
public sealed class FireworksTechnician() : DeadcellsCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<GrenadePower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<FireworksTechnicianPower>(4)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FireworksTechnicianPower>(base.Owner.Creature, base.DynamicVars["FireworksTechnicianPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FireworksTechnicianPower"].UpgradeValueBy(2);
    }

}
