using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.cards;

//血花四溅
//流血 会扩散给其他敌人. NL 当你给予 流血 时,额外给与 !M! 层.
[Pool(typeof(DeadcellsCardPool))]
public sealed class BleedingSpread() : DeadcellsCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BleedingSpreadPower>(),
        HoverTipFactory.FromPower<BleedingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BleedingSpreadPower>(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BleedingSpreadPower>(base.Owner.Creature, base.DynamicVars["BleedingSpreadPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BleedingSpreadPower"].UpgradeValueBy(1);
    }

}
