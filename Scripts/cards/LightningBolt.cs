using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//闪电光束
//造成 !D! 点伤害，如果上一张牌是 *闪电光束 ，打出后回到手中并使伤害+7。如果本回合你打出了4张以上 *闪电光束 ，则回合结束时受到 !M! 点伤害。
[Pool(typeof(DeadcellsCardPool))]
public sealed class LightningBolt() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool Purple => true;
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
        new CardsVar(4),
        new IntVar("UsedTime", 0),
        new IntVar("TotalDamage", 7)
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

    protected override bool ShouldGlowGoldInternal => CanComeBack && base.DynamicVars["UsedTime"].IntValue >= 1;

    protected override bool ShouldGlowRedInternal => CanComeBack && base.DynamicVars["UsedTime"].IntValue >= 3;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await DamageCmd.Attack(base.DynamicVars["TotalDamage"].BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        if (CanComeBack)
        {
            await Task.Run(() => base.DynamicVars["UsedTime"].UpgradeValueBy(1));
            await Task.Run(() => base.DynamicVars["TotalDamage"].UpgradeValueBy(base.DynamicVars.ExtraDamage.BaseValue));
            await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
        }

        if (base.DynamicVars["UsedTime"].IntValue >= base.DynamicVars.Cards.IntValue)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars["UsedTime"].BaseValue, ValueProp.Unblockable, base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        base.DynamicVars["UsedTime"].ResetToBase();
        base.DynamicVars["TotalDamage"].ResetToBase();
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        base.DynamicVars["UsedTime"].ResetToBase();
        base.DynamicVars["TotalDamage"].ResetToBase();
        return base.AfterTurnEnd(choiceContext, side);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        base.DynamicVars["UsedTime"].ResetToBase();
        base.DynamicVars["TotalDamage"].ResetToBase();
        return base.AfterCombatVictory(room);
    }
}
