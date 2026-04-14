using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;


//对称长枪
//对所有敌人造成 !D! 点伤害 !M! 次. NL 如果第一段击中至少两 NL 名敌人则后续造成两倍 NL 伤害.
[Pool(typeof(DeadcellsCardPool))]
public sealed class SymmetricalSpear() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5, ValueProp.Move),
        new RepeatVar(2),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = (await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage.BaseValue, ValueProp.Move, base.Owner.Creature, this)).Count((DamageResult result) => result.TotalDamage > 0);
        if (num >= 2)
        {
            await Task.Run(() => base.DynamicVars.Damage.UpgradeValueBy(5));
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .WithHitCount((base.DynamicVars.Repeat.IntValue - 1))
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await Task.Run(() => base.DynamicVars.Damage.UpgradeValueBy(-5));
        }
        else
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .WithHitCount((base.DynamicVars.Repeat.IntValue - 1))
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(1);
    }

}