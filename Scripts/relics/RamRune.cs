using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Deadcells.Scripts.relics;


//绿宝石护符
[Pool(typeof(DeadcellRelicPool))]
public sealed class RamRune : CustomRelicModel
{
    private int atkAmt = 0;
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2)
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack)
        {
            this.atkAmt++;
            if (this.atkAmt >= base.DynamicVars.Cards.IntValue)
            {
                foreach (Creature enemy in base.Owner.Creature.CombatState.HittableEnemies)
                {
                    if (enemy.HasPower<MinionPower>())
                    {
                        await Task.Run(() => CreatureCmd.Kill(enemy));
                        break;
                    }
                    else if (enemy.Block > 0)
                    {
                        await CreatureCmd.LoseBlock(enemy, enemy.Block);
                        break;
                    }
                }
            }
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        this.atkAmt = 0;
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            this.atkAmt = 0;
        }
        return base.AfterTurnEnd(choiceContext, side);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        this.atkAmt = 0;
        return base.AfterCombatVictory(room);
    }
}
