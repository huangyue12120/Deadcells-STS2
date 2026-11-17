using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//冥河提灯
//在深渊汲取与灵魂冲击之间选择一张加入手牌。
[Pool(typeof(DeadcellsCardPool))]
public sealed class FerrymanSLantern() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    private int _ammunition;

    [SavedProperty]
    public int Ammunition
    {
        get
        {
            return _ammunition;
        }
        set
        {
            AssertMutable();
            _ammunition = value;
            base.DynamicVars["Ammunition"].BaseValue = value;
        }
    }

    internal void SetAmmunition(int value)
    {
        Ammunition = value;
        if (DeckVersion is FerrymanSLantern deckVersion)
        {
            deckVersion.Ammunition = value;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<FerrymanSLanternTwo>(base.IsUpgraded),
        HoverTipFactory.FromCard<FerrymanSLanternThree>(base.IsUpgraded)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1),
        new IntVar("Ammunition", Ammunition)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        
        var cardsToChoose = new List<CardModel>
        {
            ModelDb.Card<FerrymanSLanternTwo>(),
            ModelDb.Card<FerrymanSLanternThree>()
        }.Select(e => (CardModel)e.MutableClone()).ToList();

        foreach (var c in cardsToChoose)
        {
            CombatState.AddCard(c, Owner);
        }

        CardModel cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cardsToChoose, base.Owner, canSkip: false);
        if (cardModel != null)
        {
            if (this.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            if (cardModel is FerrymanSLanternTwo)
            {
                ((FerrymanSLanternTwo)cardModel).SetParent(this);
            }
            if (cardModel is FerrymanSLanternThree)
            {
                ((FerrymanSLanternThree)cardModel).SetParent(this);
            }
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, cardPlay.Card.Owner);
        }
    }

    protected override void OnUpgrade()
    {

    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars["Ammunition"].BaseValue = Ammunition;
    }

}

//深渊汲取
//造成 !D! 点伤害, NL 获得 !AM! 枚弹药.
[Pool(typeof(DeadcellsCardPool))]
public sealed class FerrymanSLanternTwo : DeadcellsCardModel
{
    private FerrymanSLantern? _parent;

    public FerrymanSLanternTwo()
        : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, false)
    {
    }

    internal void SetParent(FerrymanSLantern parent) => _parent = parent;

    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8, ValueProp.Move),
        new IntVar("AmmunitionIncrease", 2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this._parent != null)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, null)
                .Execute(choiceContext);
            this._parent.SetAmmunition(this._parent.Ammunition + base.DynamicVars["AmmunitionIncrease"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3);
    }
}

//灵魂冲击
//消耗 !AB! 弹药 , NL 每枚 弹药 造成 !D! 点伤害.
[Pool(typeof(DeadcellsCardPool))]
public sealed class FerrymanSLanternThree : DeadcellsCardModel
{
    private FerrymanSLantern? _parent;

    public FerrymanSLanternThree()
        : base(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, false)
    {
    }

    internal void SetParent(FerrymanSLantern parent) => _parent = parent;

    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(21, ValueProp.Move)
    };

    protected override bool IsPlayable => (this._parent != null && this._parent.Ammunition != 0);

    protected override bool ShouldGlowGoldInternal => (this._parent != null && this._parent.Ammunition >= 5);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this._parent != null)
        {
            for (var i = 0; i < this._parent.Ammunition; i++)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                    .Targeting(cardPlay.Target)
                    .WithHitFx("vfx/vfx_attack_slash", null, null)
                    .Execute(choiceContext);
            }
            this._parent.SetAmmunition(0);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5);
    }
}
