using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//巨镰左爪
//对所有敌人造成 !D! 点伤害. NL 给予所有敌人 !M! 层 易伤 . NL 洗入 *巨镰右爪. NL 消耗 .

[Pool(typeof(DeadcellsCardPool))]
public sealed class ScytheClaw() : DeadcellsCardModel(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ScytheClawTwo>(base.IsUpgraded),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14, ValueProp.Move),
        new PowerVar<VulnerablePower>(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        }

        ScytheClawTwo clawTwo = CombatState.CreateCard<ScytheClawTwo>(base.Owner);
        if (this.IsUpgraded)
        {
            CardCmd.Upgrade(clawTwo);
        }
        IEnumerable<CardModel> enumerable = [clawTwo];
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Draw, cardPlay.Card.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(6);
    }

}


//巨镰右爪
//对所有敌人造成 !D! 点伤害. NL 如果至少造成 !M! 未被格挡的伤害 则敌人被击晕. NL 洗入 *巨镰左爪. NL 消耗 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class ScytheClawTwo() : DeadcellsCardModel(3, CardType.Attack, CardRarity.Token, TargetType.AllEnemies, false)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ScytheClaw>(base.IsUpgraded),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(28, ValueProp.Move),
        new IntVar("TargetDamage", 42)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Attack", base.Owner.Character.AttackAnimDelay);
        int num = (await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage.BaseValue, ValueProp.Move, base.Owner.Creature, this)).Sum((DamageResult result) => result.UnblockedDamage);
        if (num >= base.DynamicVars["TargetDamage"].IntValue)
        {
            foreach (Creature c in CombatState.Enemies)
            {
                string NextMoveID = c.Monster.NextMove.Id;
                await CreatureCmd.Stun(c, NextMoveID);
            }
        }
        ScytheClaw claw = CombatState.CreateCard<ScytheClaw>(base.Owner);
        if (this.IsUpgraded)
        {
            CardCmd.Upgrade(claw);
        }
        IEnumerable<CardModel> enumerable = [claw];
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Draw, cardPlay.Card.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4);
        base.DynamicVars["TargetDamage"].UpgradeValueBy(6);
    }

}
