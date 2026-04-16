using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.cards;

//适者生存
//获得 !M! 点 力量 和 敏捷 . (upg: *选择 :获得 !CN! 点 力量 或 !CN! 点 敏捷 .)
[Pool(typeof(DeadcellsCardPool))]
public sealed class Sotf() : DeadcellsCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromCard<TmpStrCard>(),
        HoverTipFactory.FromCard<TmpDexCard>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(1),
        new PowerVar<DexterityPower>(1),
        new IntVar("ExtraStrOrDex", 1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, this);
        if (this.IsUpgraded)
        {
            if (CombatState == null) return;
            var cardsToChoose = new List<CardModel>
            {
                ModelDb.Card<TmpStrCard>(),
                ModelDb.Card<TmpDexCard>()
            }.Select(e => (CardModel)e.MutableClone()).ToList();

            foreach (var c in cardsToChoose)
            {
                c.Owner = Owner;
            }

            CardModel cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cardsToChoose, base.Owner, canSkip: false);
            
            if (cardModel != null)
            {
                if (cardModel is TmpStrCard)
                {
                    await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["ExtraStrOrDex"].BaseValue, base.Owner.Creature, this);
                }
                if (cardModel is TmpDexCard)
                {
                    await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars["ExtraStrOrDex"].BaseValue, base.Owner.Creature, this);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {

    }

}

[Pool(typeof(DeadcellsCardPool))]
public sealed class TmpStrCard() : DeadcellsCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.None, false)
{
    protected override bool Gray => true;
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

    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
    }

    protected override void OnUpgrade()
    {

    }

}

[Pool(typeof(DeadcellsCardPool))]
public sealed class TmpDexCard() : DeadcellsCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.None, false)
{
    protected override bool Gray => true;
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

    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
    }

    protected override void OnUpgrade()
    {

    }

}
