using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//巨人杀手
//造成 !D! 点伤害 !M! 次. NL 每次使用都会改变效果. NL 对精英和boss额外 NL 造成 !CN! 点伤害.
//1.5D2R，
[Pool(typeof(DeadcellsCardPool))]
public sealed class Giantkiller() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private int playCount = 0;

    private bool IsBossOrElite => (base.CombatState != null && base.CombatState.RunState != null && base.CombatState.RunState.CurrentRoom != null && (base.CombatState.RunState.CurrentRoom.RoomType == RoomType.Elite || base.CombatState.RunState.CurrentRoom.RoomType == RoomType.Boss));
    protected override bool Red => true;
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
        new IntVar("ToEliteDamage",3)
    };



    protected override bool ShouldGlowGoldInternal => IsBossOrElite;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        this.playCount++;
        decimal dmgAmt = (IsBossOrElite && !cardPlay.Target.HasPower<MinionPower>()) ? base.DynamicVars.Damage.BaseValue + base.DynamicVars["ToEliteDamage"].BaseValue : base.DynamicVars.Damage.BaseValue;
        string atkFx = this.playCount % 2 == 0 ? "vfx/vfx_attack_blunt" : "vfx/vfx_attack_slash";
        await DamageCmd.Attack(dmgAmt).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this).Targeting(cardPlay.Target).WithHitFx(atkFx).Execute(choiceContext);
        await ChangeStatus();
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ToEliteDamage"].UpgradeValueBy(3);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber <= 1)
        {
            this.playCount = 0;
        }
        return base.AfterSideTurnStart(side, participants, combatState);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        this.playCount = 0;
        base.DynamicVars.Damage.ResetToBase();
        base.EnergyCost.SetCustomBaseCost(1);
        base.DynamicVars.Repeat.ResetToBase();
        return base.AfterCombatVictory(room);
    }

    private Task ChangeStatus()
    {
        if (this.playCount % 2 != 0)
        {
            if (IsBossOrElite)
            {
                base.DynamicVars["ToEliteDamage"].UpgradeValueBy(3);
                if (this.IsUpgraded)
                {
                    base.DynamicVars["ToEliteDamage"].UpgradeValueBy(3);
                }
            }
            base.DynamicVars.Damage.UpgradeValueBy(13);
            base.EnergyCost.UpgradeBy(1);
            base.DynamicVars.Repeat.UpgradeValueBy(-1);
        }
        else
        {
            if (IsBossOrElite)
            {
                base.DynamicVars["ToEliteDamage"].UpgradeValueBy(-3);
                if (this.IsUpgraded)
                {
                    base.DynamicVars["ToEliteDamage"].UpgradeValueBy(-3);
                }
            }
            base.DynamicVars.Damage.UpgradeValueBy(-13);
            base.EnergyCost.UpgradeBy(-1);
            base.DynamicVars.Repeat.UpgradeValueBy(1);
        }
        return Task.CompletedTask;
    }
}
