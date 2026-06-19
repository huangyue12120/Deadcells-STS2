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
//油腻之剑
//造成 !D! 点伤害. NL 如果目标有 燃烧 改为造成 !HD! 点伤害 . NL 给予 !M! 层 油 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class OiledSword() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BurnsPower>(),
        HoverTipFactory.FromPower<OilPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6, ValueProp.Move),
        new ExtraDamageVar(6),
        new PowerVar<OilPower>(3)
    };

    protected override bool ShouldGlowGoldInternal => base.CombatState.HittableEnemies.Any(e => e.HasPower<BurnsPower>());

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target.HasPower<BurnsPower>())
        {
            decimal totalDmg = base.DynamicVars.Damage.BaseValue + base.DynamicVars.ExtraDamage.BaseValue;
            await DamageCmd.Attack(totalDmg)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        await PowerCmd.Apply<OilPower>(choiceContext, cardPlay.Target, base.DynamicVars["OilPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }

}
