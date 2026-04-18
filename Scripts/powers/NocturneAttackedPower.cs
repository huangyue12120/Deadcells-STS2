using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

//夜歌标记
//这名角色在下1次被攻击时受到1倍伤害。
public sealed class NocturneAttackedPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("DamageMultiple", 2)
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
        decimal num = base.DynamicVars["DamageMultiple"].BaseValue;

        return num;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        this.Flash();
        await PowerCmd.TickDownDuration(this);
    }
}