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
    private bool _triggeredThisTurn;
    public override RelicRarity Rarity => RelicRarity.Rare;

    // 小图标
    public override string PackedIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2)
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner && cardPlay.Card.Type == CardType.Attack && !_triggeredThisTurn)
        {
            this.atkAmt++;
            if (this.atkAmt >= base.DynamicVars.Cards.IntValue)
            {
                _triggeredThisTurn = true;
                var enemies = base.Owner.Creature.CombatState.HittableEnemies.ToList();
                Creature? minion = enemies.FirstOrDefault(enemy => enemy.HasPower<MinionPower>());
                if (minion != null)
                {
                    await CreatureCmd.Kill(minion);
                }
                else
                {
                    Creature? enemy = enemies.FirstOrDefault(enemy => enemy.Block > 0);
                    if (enemy != null)
                    {
                        await CreatureCmd.LoseBlock(context, enemy, enemy.Block, base.Owner.Creature);
                    }
                }
            }
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner)
        {
            this.atkAmt = 0;
            _triggeredThisTurn = false;
        }
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            this.atkAmt = 0;
            _triggeredThisTurn = false;
        }
        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        this.atkAmt = 0;
        _triggeredThisTurn = false;
        return base.AfterCombatVictory(room);
    }
}
