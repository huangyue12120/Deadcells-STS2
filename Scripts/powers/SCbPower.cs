using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.powers;

// 音速
// 发射你抽到的下 1 张攻击牌
public sealed class SCbPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    private HashSet<CardModel>? _autoplayingCards;

    private HashSet<CardModel> AutoplayingCards
    {
        get
        {
            AssertMutable();
            if (_autoplayingCards == null)
            {
                _autoplayingCards = new HashSet<CardModel>();
            }
            return _autoplayingCards;
        }
    }

    public override async Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == base.Owner && card.Type == CardType.Attack && !base.Owner.CombatState.HittableEnemies.All((Creature c) => c.ShowsInfiniteHp))
        {
            if (this.Amount > 0)
            {
                AutoplayingCards.Add(card);
                await CardCmd.AutoPlay(choiceContext, card, null);
                AutoplayingCards.Remove(card);
                await PowerCmd.TickDownDuration(this);
            }
        }
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (!AutoplayingCards.Contains(command.ModelSource))
        {
            return Task.CompletedTask;
        }
        command.WithHitFx("vfx/hellraiser_attack_vfx", command.HitSfx, command.TmpHitSfx).WithAttackerAnim("Cast", command.Attacker.Player.Character.CastAnimDelay).SpawningHitVfxOnEachCreature()
            .WithHitVfxSpawnedAtBase();
        return Task.CompletedTask;
    }
}
