using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//平底锅
//造成 !D! 点伤害. NL 如果敌人的意图是 *攻击 . NL 再造成 !D! 点伤害.（upg：1weak 1vul）
[Pool(typeof(DeadcellsCardPool))]
public sealed class Saucepan() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(7, ValueProp.Move),
        new RepeatVar(1),
        new PowerVar<WeakPower>(1)
    };

    private bool IntendsToAttack
    {
        get
        {
            foreach (Creature enemy in base.CombatState.Enemies)
            {
                if (enemy == null) continue;
                if (enemy.Monster.IntendsToAttack)
                {
                    return true;
                }
            }
            return false;
        }
    }

    protected override bool ShouldGlowGoldInternal => IntendsToAttack;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target.Monster.IntendsToAttack)
        {
            await Task.Run(() => base.DynamicVars.Repeat.UpgradeValueBy(1));
        }
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .WithHitCount(base.DynamicVars.Repeat.IntValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

        if (this.IsUpgraded)
        {
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        }
        await Task.Run(() => base.DynamicVars.Repeat.ResetToBase());
    }

    protected override void OnUpgrade()
    {

    }

}
