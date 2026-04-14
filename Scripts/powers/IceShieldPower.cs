using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

public sealed class IceShieldPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    private bool IsPlayerTurn = false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Enemy)
        {
            this.IsPlayerTurn = false;
        }
        else
        {
            this.IsPlayerTurn = true;
            await PowerCmd.Apply<IceShieldPower>(base.Owner, -base.Amount, base.Owner, null);
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (IsPlayerTurn) return;
        if (!this.IsPlayerTurn && target == base.Owner && dealer != base.Owner && props.IsPoweredAttack_())
        {
            this.Flash();
            foreach (Creature enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<IceShieldLoseStrPower>(enemy, base.Amount, base.Owner, null);
                await PowerCmd.Apply<StrengthPower>(enemy, -base.Amount, base.Owner, null);
            }
        }
    }
}

public sealed class IceShieldLoseStrPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    private int _turnsUntilEffect = 1;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Enemy)
            return;

        if (_turnsUntilEffect > 0)
        {
            _turnsUntilEffect--;
            return;
        }

        await PowerCmd.Apply<StrengthPower>(base.Owner, this.Amount, null, null);
        await PowerCmd.Apply<IceShieldLoseStrPower>(base.Owner, -this.Amount, null, null);
    }
}
