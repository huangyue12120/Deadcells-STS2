using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;


[Pool(typeof(DeadcellsCardPool))]
public sealed class ClusterGrenade() : DeadcellsCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Sly
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Grenade];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<GrenadePower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10, ValueProp.Move),
        new ExtraDamageVar(5),
        new RepeatVar(5)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        for (int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
        {
            Creature enemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
            if (enemy == null)
            {
                continue;
            }
            await CreatureCmd.Damage(choiceContext, enemy, base.DynamicVars.ExtraDamage.BaseValue, ValueProp.Move, this);
        }
        await base.OnPlay(choiceContext, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4);
    }

}
