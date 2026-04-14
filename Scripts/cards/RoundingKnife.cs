using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.cards;
//圆舞飞刀
//予所有敌人 !M! 层 虚弱 . NL 给予敌人3层 流血 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class RoundingKnife() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Basic, TargetType.AllEnemies)
{
    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<BleedingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<WeakPower>(1),
        new PowerVar<BleedingPower>(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(enemy, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<BleedingPower>(enemy, base.DynamicVars["BleedingPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BleedingPower"].UpgradeValueBy(3);
    }

}
