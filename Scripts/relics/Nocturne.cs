using BaseLib.Abstracts;
using BaseLib.Utils;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.relics;

//夜歌
//每打出5张技能牌随即对1名敌人造成5点伤害并给予1层夜歌标记
[Pool(typeof(DeadcellRelicPool))]
public sealed class Nocturne : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private int _cardsPlayedThisTurn;

    // 小图标
    public override string PackedIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(5),
        new DamageVar(5, ValueProp.Unpowered),
        new PowerVar<NocturneAttackedPower>(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<NocturneAttackedPower>()
    };

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount
    {
        get
        {
            if (!base.IsCanonical)
            {
                return _cardsPlayedThisTurn;
            }
            return 0;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Skill)
        {
            _cardsPlayedThisTurn++;
            if (_cardsPlayedThisTurn % base.DynamicVars.Cards.IntValue == 0)
            {
                Flash();
                Creature randomEnemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
                await CreatureCmd.Damage(context, randomEnemy, base.DynamicVars.Damage, base.Owner.Creature);
                if (randomEnemy.IsDead)
                {
                    randomEnemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
                }
                await PowerCmd.Apply<NocturneAttackedPower>(context, randomEnemy, base.DynamicVars["NocturneAttackedPower"].BaseValue, null, null);
                _cardsPlayedThisTurn = 0;
            }

        }
        InvokeDisplayAmountChanged();
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
        {
            return Task.CompletedTask;
        }
        _cardsPlayedThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _cardsPlayedThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}