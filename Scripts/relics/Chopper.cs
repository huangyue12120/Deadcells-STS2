using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.relics;

//斩杀者
//回合结束斩杀所有生命值低于15%的敌人
[Pool(typeof(DeadcellRelicPool))]
public sealed class Chopper : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // 小图标
    public override string PackedIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("DeadLine", 15)
    };

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            foreach (Creature enemy in base.Owner.Creature.CombatState.HittableEnemies)
            {
                if (enemy.CurrentHp <= (enemy.MaxHp * (base.DynamicVars["DeadLine"].IntValue / 100)))
                {
                    await Task.Run(() => CreatureCmd.Kill(enemy));
                }
            }
        }
    }
}