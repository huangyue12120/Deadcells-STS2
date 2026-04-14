using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

public sealed class BurnsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("DamageIncrease", 25),
    };

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return 1;
        }
        if (!props.IsPoweredAttack_())
        {
            return 1;
        }
        decimal num = 1 + (base.DynamicVars["DamageIncrease"].BaseValue / 100);

        return num;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != null && props.IsPoweredAttack_() && target == base.Owner && amount >= 0)
        {
            int dmgIncrease = Mathf.FloorToInt(this.Amount / 3);
            return dmgIncrease;
        }
        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource);
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side)
        {
            return;
        }
        this.Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unpowered, null, null);
        if (base.Owner.IsAlive)
        {
            await PowerCmd.Apply<BurnsPower>(base.Owner, -this.Amount, base.Owner, null);
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }
}
