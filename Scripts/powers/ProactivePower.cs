using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using Deadcells.Scripts.cards;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadcells.Scripts.powers;

public sealed class ProactivePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("ExtraDmg", 0)
    };

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner || dealer != base.Owner)
        {
            return 1;
        }
        if (!props.IsPoweredAttack_())
        {
            return 1;
        }
        if(CanGive)
        {
            return 1;
        }
        decimal num = 1 + (base.DynamicVars["ExtraDmg"].BaseValue / 100);

        return num;
    }

    private bool CanGive { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack)
        {
            this.Flash();
            this.CanGive = true;
            await PowerCmd.Apply<ProactivePower>(context, base.Owner, (decimal)(1 - this.Amount), base.Owner, null);
            await Task.Run(() => base.DynamicVars["ExtraDmg"].UpgradeValueBy(-base.DynamicVars["ExtraDmg"].BaseValue));
        }

        if (!this.CanGive && cardPlay.Card.Type == CardType.Skill)
        {
            this.Flash();
            await PowerCmd.Apply<ProactivePower>(context, base.Owner, 1, base.Owner, null);
            await Task.Run(() => base.DynamicVars["ExtraDmg"].UpgradeValueBy(33));
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        this.CanGive = false;
        if(base.DynamicVars["ExtraDmg"].IntValue > 0)
        {
            base.DynamicVars["ExtraDmg"].UpgradeValueBy(-base.DynamicVars["ExtraDmg"].BaseValue);
        }
        await PowerCmd.Apply<ProactivePower>(choiceContext, base.Owner, (decimal)(1 - this.Amount), base.Owner, null);
    }
}