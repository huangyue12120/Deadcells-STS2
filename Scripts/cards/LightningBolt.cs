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
    private int _damageIncreases;
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
        new DamageVar(7, ValueProp.Move),
        new ExtraDamageVar(7),
        new CardsVar(4)
    };

    private bool CanComeBack(CardPlay? currentPlay = null)
    {
        if (base.CombatState == null || !CombatManager.Instance.IsInProgress)
        {
            return false;
        }
        var previousEntry = CombatManager.Instance.History.CardPlaysStarted
            .LastOrDefault(e => e.CardPlay.Player == base.Owner && e.HappenedThisTurn(base.CombatState)
                && e.CardPlay != currentPlay);
        return previousEntry == null || previousEntry.CardPlay.Card == this;
    }

    protected override bool ShouldGlowGoldInternal => CanComeBack() && UsedTime >= 1;

    protected override bool ShouldGlowRedInternal => UsedTime >= base.DynamicVars.Cards.IntValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        bool canComeBack = CanComeBack(cardPlay);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        UsedTime += 1;
        if (canComeBack)
        {
            _damageIncreases += 1;
            UpdateDamage();
            await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
        }

        if (UsedTime > base.DynamicVars.Cards.IntValue)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, UsedTime, ValueProp.Unpowered, base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        base.DynamicVars.Damage.BaseValue = (base.IsUpgraded ? 10 : 7)
            + _damageIncreases * base.DynamicVars.ExtraDamage.BaseValue;
    }

    private void ResetTurn()
    {
        UsedTime = 0;
        _damageIncreases = 0;
        UpdateDamage();
    }

    public override Task BeforeCombatStart()
    {
        ResetTurn();
        return base.BeforeCombatStart();
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner)
        {
            ResetTurn();
        }
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            ResetTurn();
        }
        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        ResetTurn();
        return base.AfterCombatVictory(room);
    }
}
