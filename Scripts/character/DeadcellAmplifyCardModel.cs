using BaseLib.Abstracts;
using Deadcells.Scripts.powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadcells.Scripts.character;

public abstract class DeadcellAmplifyCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true,
    bool autoAdd = true)
    : DeadcellsCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary, autoAdd)
{
    //卡图
    //public override string PortraitPath => $"res://images/cards/{Id.Entry.ToLowerInvariant()}.png";

    private bool _costModifiedForAmplify;

    protected virtual bool IsAmplified { get; private set; }

    protected virtual int extraCost => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
    }

    private void ValidateAmplify()
    {
        bool inHand = Owner.PlayerCombatState?.Hand.Cards.Contains(this) ?? false;

        bool shouldAmplify = false;
        bool costFree = false;

        if (inHand)
        {
            int totalEnergyNeeded = base.CanonicalEnergyCost + extraCost;
            int currentEnergy = base.Owner.PlayerCombatState.Energy;

            if (currentEnergy >= totalEnergyNeeded)
            {
                shouldAmplify = true;
                costFree = false;
            }
        }

        SetAmplifyState(shouldAmplify, costFree);
    }

    private void SetAmplifyState(bool shouldAmplify, bool costFree)
    {
        bool wasAmplified = IsAmplified;
        IsAmplified = shouldAmplify;

        if (shouldAmplify && !costFree && !_costModifiedForAmplify)
        {
            EnergyCost.AddThisCombat(1, false);
            _costModifiedForAmplify = true;
        }

        else if ((!shouldAmplify || costFree) && _costModifiedForAmplify)
        {
            EnergyCost.AddThisCombat(-1, false);
            _costModifiedForAmplify = false;
        }
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this) ValidateAmplify();
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 任何牌打出后，手牌中本卡的费用相关状态可能变化（能量被消耗）
        ValidateAmplify();
        return base.AfterCardPlayed(context, cardPlay);
    }

    public override Task AfterEnergyReset(Player player)
    {
        ValidateAmplify();
        return base.AfterEnergyReset(player);
    }

    public override Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        ValidateAmplify();
        return base.AfterPotionUsed(potion, target);
    }

    public override Task AfterModifyingEnergyGain()
    {
        ValidateAmplify();
        return base.AfterModifyingEnergyGain();
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this) ValidateAmplify();
        return base.AfterCardEnteredCombat(card);
    }
}
