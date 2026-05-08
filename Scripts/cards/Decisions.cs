using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadcells.Scripts.cards;

[Pool(typeof(DeadcellsCardPool))]
public sealed class Decisions() : DeadcellsCardModel(1, CardType.Power, CardRarity.Ancient, TargetType.AllEnemies)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<TmpDecRed>(),
        HoverTipFactory.FromCard<TmpDecPurple>(),
        HoverTipFactory.FromCard<TmpDecGreen>(),
        HoverTipFactory.FromCard<TmpDecGray>(),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var cardsToChoose = new List<CardModel>
            {
                ModelDb.Card<TmpDecRed>(),
                ModelDb.Card<TmpDecPurple>(),
                ModelDb.Card<TmpDecGreen>(),
                ModelDb.Card<TmpDecGray>()
            }.Select(e => (CardModel)e.MutableClone()).ToList();

        foreach (var c in cardsToChoose)
        {
            c.Owner = Owner;
        }

        foreach (CardModel cardModel in await CardSelectCmd.FromSimpleGrid(choiceContext, cardsToChoose, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 1)))
        {
            if (cardModel != null)
            {
                if (cardModel is TmpDecRed)
                {
                    await PowerCmd.Apply<ProactivePower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
                if (cardModel is TmpDecPurple)
                {
                    await PowerCmd.Apply<StrategicRetreatPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
                if (cardModel is TmpDecGreen)
                {
                    await PowerCmd.Apply<UnyieldingWillPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
                if (cardModel is TmpDecGray)
                {
                    await CardPileCmd.Draw(choiceContext, 4, base.Owner);
                    await PowerCmd.Apply<GraySpeedPower>(base.Owner.Creature, 3, base.Owner.Creature, this);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.AddKeyword(CardKeyword.Innate);
    }
}

[Pool(typeof(DeadcellsCardPool))]
public sealed class TmpDecRed() : DeadcellsCardModel(-1, CardType.Power, CardRarity.Token, TargetType.None, false)
{
    protected override bool Red => true;
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
    }

    protected override void OnUpgrade()
    {
    }
}

[Pool(typeof(DeadcellsCardPool))]
public sealed class TmpDecGreen() : DeadcellsCardModel(-1, CardType.Power, CardRarity.Token, TargetType.None, false)
{
    protected override bool Green => true;
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
    }

    protected override void OnUpgrade()
    {
    }
}

[Pool(typeof(DeadcellsCardPool))]
public sealed class TmpDecPurple() : DeadcellsCardModel(-1, CardType.Power, CardRarity.Token, TargetType.None, false)
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

    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
    }

    protected override void OnUpgrade()
    {
    }
}

[Pool(typeof(DeadcellsCardPool))]
public sealed class TmpDecGray() : DeadcellsCardModel(-1, CardType.Power, CardRarity.Token, TargetType.None, false)
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
    }

    protected override void OnUpgrade()
    {
    }
}