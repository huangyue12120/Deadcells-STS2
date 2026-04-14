using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.relics;

//被诅咒的文物
//
[Pool(typeof(DeadcellRelicPool))]
public sealed class CorruptedArtifact : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(666),
        new IntVar("Curse", 0)
    };

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner.Creature)
        {
            return 1;
        }
        if (base.DynamicVars["Curse"].IntValue == 0)
        {
            return 2;
        }
        return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (base.DynamicVars["Curse"].IntValue == 0)
        {
            base.DynamicVars["Curse"].UpgradeValueBy(1);
        }
        return base.AfterCombatVictory(room);
    }
}