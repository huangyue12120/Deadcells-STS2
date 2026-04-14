using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.relics;

//防御者
[Pool(typeof(DeadcellRelicPool))]
public sealed class Defender : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("DmgDeMu", 20),
        new IntVar("DmgDeAd", 1),
        new IntVar("Turn", 0)
    };

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner.Creature)
        {
            return 1;
        }
        if (base.DynamicVars["Turn"].IntValue == 0)
        {
            return 1 - (base.DynamicVars["DmgDeMu"].IntValue / 100);
        }
        return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner.Creature)
        {
            return amount;
        }
        if (base.DynamicVars["Turn"].IntValue == 1)
        {
            return (-base.DynamicVars["DmgDeAd"].BaseValue);
        }
        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            if (base.DynamicVars["Turn"].IntValue == 1)
            {
                base.DynamicVars["Turn"].UpgradeValueBy(1);
            }
            if (base.DynamicVars["Turn"].IntValue == 0)
            {
                base.DynamicVars["Turn"].UpgradeValueBy(1);
            }
        }
        return base.AfterTurnEnd(choiceContext, side);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        base.DynamicVars["Turn"].ResetToBase();
        return base.AfterCombatVictory(room);
    }
}