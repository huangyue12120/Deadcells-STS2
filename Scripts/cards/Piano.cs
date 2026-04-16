using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//节奏布祖基琴
//造成 !D! 点伤害. NL 将一张节奏布祖基琴II置入手牌. NL 消耗 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class Piano() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<PianoTwo>(base.IsUpgraded)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        if (CombatState == null) return;
        PianoTwo pianoTwo = CombatState.CreateCard<PianoTwo>(Owner);
        if (base.IsUpgraded)
        {
            CardCmd.Upgrade(pianoTwo);
        }

        await CardPileCmd.Add(pianoTwo, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}

[Pool(typeof(DeadcellsCardPool))]
public sealed class PianoTwo() : DeadcellsCardModel(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, false)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<PianoThree>(base.IsUpgraded)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(16, ValueProp.Move)
    };

    private bool WasLastCardPlayedPiano
    {
        get
        {
            CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card != this);
            if (cardPlayStartedEntry == null)
            {
                return false;
            }
            return cardPlayStartedEntry.CardPlay.Card.Id == ModelDb.Card<Piano>().Id;
        }
    }

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedPiano;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (WasLastCardPlayedPiano)
        {
            base.DynamicVars.Damage.UpgradeValueBy(16);
        }
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

        if (CombatState == null) return;
        PianoThree pianoThree = CombatState.CreateCard<PianoThree>(Owner);
        if (base.IsUpgraded)
        {
            CardCmd.Upgrade(pianoThree);
        }

        await CardPileCmd.Add(pianoThree, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

[Pool(typeof(DeadcellsCardPool))]
public sealed class PianoThree() : DeadcellsCardModel(2, CardType.Attack, CardRarity.Token, TargetType.AllEnemies, false)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<PianoTwo>(base.IsUpgraded),
        HoverTipFactory.FromCard<PianoThree>(base.IsUpgraded)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(32, ValueProp.Move)
    };

    private bool WasLastCardPlayedPiano
    {
        get
        {
            CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card != this);
            if (cardPlayStartedEntry == null)
            {
                return false;
            }
            return (cardPlayStartedEntry.CardPlay.Card.Id == ModelDb.Card<PianoTwo>().Id || cardPlayStartedEntry.CardPlay.Card.Id == this.Id);
        }
    }

    protected override bool ShouldGlowRedInternal => WasLastCardPlayedPiano;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (WasLastCardPlayedPiano && base.DynamicVars.Damage.IntValue == 32)
        {
            base.DynamicVars.Damage.UpgradeValueBy(32);
        }
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

        if (CombatState == null) return;
        PianoThree pianoThree = CombatState.CreateCard<PianoThree>(Owner);
        if (base.IsUpgraded)
        {
            CardCmd.Upgrade(pianoThree);
        }
        await CardPileCmd.Add(pianoThree, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
