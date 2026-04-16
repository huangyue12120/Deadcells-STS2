using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
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
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("Ammunition").WithMultiplier((card, _) =>
            {
                // 安全转换，因为只有本卡实例才会调用此委托
                if (card is FerrymanSLantern fs)
                    return fs._ammunition;
                return 0;
            })
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
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, true);
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber <= 1)
        {
            Ammunition = 0;
        }
        return base.AfterSideTurnStart(side, combatState);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        Ammunition = 0;
        return base.AfterCombatVictory(room);
    }

    protected override void OnUpgrade()
    {

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
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, null)
                .Execute(choiceContext);
            this._parent.Ammunition += base.DynamicVars["AmmunitionIncrease"].IntValue;
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
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .WithHitFx("vfx/vfx_attack_slash", null, null)
                    .Execute(choiceContext);
            }
            this._parent.Ammunition = 0;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5);
    }
}