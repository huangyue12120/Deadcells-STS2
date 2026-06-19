using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//牙签
//轻锤\蓄意重锤\修补中选择加入手牌 . 
[Pool(typeof(DeadcellsCardPool))]
public sealed class Toothpick() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Green => true;
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
        HoverTipFactory.FromCard<ToothpickTwo>(base.IsUpgraded),
        HoverTipFactory.FromCard<ToothpickThree>(base.IsUpgraded),
        HoverTipFactory.FromCard<ToothpickFour>(base.IsUpgraded),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("Ammunition").WithMultiplier((card, _) =>
            {
                // 安全转换，因为只有本卡实例才会调用此委托
                if (card is Toothpick tp)
                    return tp._ammunition;
                return 0;
            })
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var cardsToChoose = new List<CardModel>
            {
                ModelDb.Card<ToothpickTwo>(),
                ModelDb.Card<ToothpickThree>(),
                ModelDb.Card<ToothpickFour>(),
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
            if (cardModel is ToothpickTwo)
            {
                ((ToothpickTwo)cardModel).SetParent(this);
            }
            if (cardModel is ToothpickThree)
            {
                ((ToothpickThree)cardModel).SetParent(this);
            }
            if (cardModel is ToothpickFour)
            {
                ((ToothpickFour)cardModel).SetParent(this);
            }

            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, cardPlay.Card.Owner);
        }
    }

    protected override void OnUpgrade()
    {

    }

}

//轻锤
//造成 !D! 点伤害.
[Pool(typeof(DeadcellsCardPool))]
public sealed class ToothpickTwo() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, false)
{
    private Toothpick? _parent;
    internal void SetParent(Toothpick parent) => _parent = parent;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Toothpick>(base.IsUpgraded),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3);
    }

}

//蓄意重锤
//需要消耗 !AM! 枚弹药 造成 !D! 点伤害.
[Pool(typeof(DeadcellsCardPool))]
public sealed class ToothpickThree() : DeadcellsCardModel(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, false)
{
    private Toothpick? _parent;
    internal void SetParent(Toothpick parent) => _parent = parent;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Toothpick>(base.IsUpgraded),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(34, ValueProp.Move),
        new IntVar("AmmunitionDecrease", 1)
    };

    protected override bool IsPlayable => (this._parent != null && this._parent.Ammunition != 0);
    protected override bool ShouldGlowGoldInternal => (this._parent != null && this._parent.Ammunition == 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this._parent != null && this._parent.Ammunition == 1)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, null)
                .Execute(choiceContext);
            this._parent.Ammunition -= base.DynamicVars["AmmunitionDecrease"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(8);
    }

}

//修补
//将 弹药 填充至最大值. NL 获得 !B! 点格挡
[Pool(typeof(DeadcellsCardPool))]
public sealed class ToothpickFour() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self, false)
{
    private Toothpick? _parent;
    internal void SetParent(Toothpick parent) => _parent = parent;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Toothpick>(base.IsUpgraded),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4, ValueProp.Move),
        new IntVar("AmmunitionIncrease", 1)
    };

    protected override bool IsPlayable => (this._parent != null && this._parent.Ammunition == 0);

    protected override bool ShouldGlowGoldInternal => (this._parent != null && this._parent.Ammunition == 0);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (this._parent != null && this._parent.Ammunition < 1)
        {
            this._parent.Ammunition += base.DynamicVars["AmmunitionIncrease"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2);
    }

}
