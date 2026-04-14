using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//隼之拳套
//造成 !D! 点伤害 !M! 次. NL 如果目标的生命小于等于40%. NL 额外造成 !HD! 点伤害 !M! 次.  NL 如果该牌费用不为0则 *使用后会返回手牌.
[Pool(typeof(DeadcellsCardPool))]
public sealed class HayabusaGauntlets() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override bool ShouldGlowGoldInternal => base.CombatState.HittableEnemies.Any(e => e.CurrentHp <= e.MaxHp * 0.4);
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3, ValueProp.Move),
        new ExtraDamageVar(6),
        new RepeatVar(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (cardPlay.Target.CurrentHp <= cardPlay.Target.MaxHp * 0.4)
        {
            await DamageCmd.Attack(base.DynamicVars.ExtraDamage.BaseValue)
                .WithHitCount(base.DynamicVars.Repeat.IntValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        if (this.EnergyCost.GetAmountToSpend() != 0)
        {
            await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }

}
