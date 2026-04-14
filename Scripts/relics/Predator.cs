using BaseLib.Abstracts;
using BaseLib.Utils;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.relics;

//猎杀者
//斩杀一名敌人时获得2隐匿
[Pool(typeof(DeadcellRelicPool))]
public sealed class Predator : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<SmokeBombPower>(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<SmokeBombPower>()
    };

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (target.Side != base.Owner.Creature.Side)
        {
            Flash();
            await PowerCmd.Apply<SmokeBombPower>(base.Owner.Creature, base.DynamicVars["SmokeBombPower"].BaseValue, base.Owner.Creature, null);
        }
    }
}