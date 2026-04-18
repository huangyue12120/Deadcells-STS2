using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

//钢管反击
//在你的回合开始时对所有敌人造成你当前格挡的伤害1次
public sealed class IronStaffPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    private int BlockNumber { get; set; }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        this.Flash();

        for (int i = 0; i < this.Amount; i++)
        {
            foreach (Creature enemy in base.CombatState.HittableEnemies)
            {
                await CreatureCmd.Damage(choiceContext, enemy, this.BlockNumber, ValueProp.Move, base.Owner, null);
            }
        }
        await PowerCmd.Apply<IronStaffPower>(base.Owner, -this.Amount, base.Owner, null);
    }

    public override Task BeforeTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        this.BlockNumber = base.Owner.Block;
        return base.BeforeTurnEndVeryEarly(choiceContext, side);
    }
}
