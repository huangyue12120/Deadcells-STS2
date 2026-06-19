using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//闪电光束
[Pool(typeof(DeadcellsCardPool))]
public sealed class LightningBolt() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool Purple => true;

    private int UsedTime {  get; set; }
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(7),
        new ExtraDamageVar(7),
        new CardsVar(4),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, target) =>
        {
            if (card is LightningBolt lb)
            {
                return lb.UsedTime;
            }
            return 1;
        })
    };

    private bool CanComeBack
    {
        get
        {
            var thisTurnEntries = CombatManager.Instance.History.CardPlaysStarted
                .Where(e => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState))
                .ToList();

            var previousEntry = thisTurnEntries
                .Where(e => e.CardPlay.Card != this)
                .LastOrDefault();

            int cardsPlayedThisTurnExcludingThis = thisTurnEntries.Count(e => e.CardPlay.Card != this);

            if (previousEntry != null && previousEntry.CardPlay.Card == this)
                return true;

            if (cardsPlayedThisTurnExcludingThis == 0)
                return true;

            return false;
        }
    }

    protected override bool ShouldGlowGoldInternal => CanComeBack && UsedTime >= 1;

    protected override bool ShouldGlowRedInternal => CanComeBack && UsedTime >= 3;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        if (CanComeBack)
        {
            await Task.Run(() => UsedTime += 1);
            await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
        }

        if (UsedTime >= base.DynamicVars.Cards.IntValue)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, UsedTime, ValueProp.Unpowered, base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationBase"].UpgradeValueBy(3);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        UsedTime = 0;
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        UsedTime = 0;
        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        UsedTime = 0;
        return base.AfterCombatVictory(room);
    }
}
