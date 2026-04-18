using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.relics;

//生存卷轴
[Pool(typeof(DeadcellRelicPool))]
public sealed class Alive : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new MaxHpVar(7),
        new HealVar(3)
    };

    // 小图标
    public override string PackedIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(base.Owner.Creature, base.DynamicVars.MaxHp.BaseValue);
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber <= 1)
        {
            await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
        }
    }
}
