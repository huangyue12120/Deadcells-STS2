using BaseLib.Utils;
using Deadcells.Scripts.character;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//狂暴之刃
//造成 !D! 点伤害,如果你的生命小于一半,改为造成 !HD! 点伤害.
[Pool(typeof(DeadcellsCardPool))]
public sealed class FranticSword() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
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

    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.CurrentHp <= Mathf.FloorToInt(base.Owner.Creature.MaxHp / 2);

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8, ValueProp.Move),
        new ExtraDamageVar(6)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.Owner.Creature.CurrentHp <= Mathf.FloorToInt(base.Owner.Creature.MaxHp / 2))
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue + base.DynamicVars.ExtraDamage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, null)
                .Execute(choiceContext);
        }
        else
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, null)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }

}
