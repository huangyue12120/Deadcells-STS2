using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//施虐者的匕首
//造成 !D! 点伤害 !M! 次. NL 目标每有种 *减益 便增加一点伤害和段数.
[Pool(typeof(DeadcellsCardPool))]
public sealed class SadistStiletto() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    private static bool ShouldCountPower(PowerModel power)
    {
        if (power.TypeForCurrentAmount == PowerType.Debuff)
        {
            return power is not ITemporaryPower;
        }
        return false;
    }

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3, ValueProp.Move),
        new ExtraDamageVar(0),
        new CalculationBaseVar(1),
        new CalculationExtraVar(1),
        new CalculatedVar("RepeatTime").WithMultiplier((card, target) => target?.Powers.Count(ShouldCountPower) ?? 0)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int debuffNum = cardPlay.Target.Powers.Count(ShouldCountPower);
        await Task.Run(() => base.DynamicVars.ExtraDamage.UpgradeValueBy(debuffNum));
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .WithHitCount((int)base.DynamicVars["RepeatTime"].PreviewValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        await Task.Run(() => base.DynamicVars.ExtraDamage.UpgradeValueBy(-debuffNum));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1);
        base.DynamicVars["RepeatTime"].UpgradeValueBy(1);
    }

}