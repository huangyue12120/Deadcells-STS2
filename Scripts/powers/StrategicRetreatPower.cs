using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadcells.Scripts.powers;

public sealed class StrategicRetreatPower : CustomPowerModel
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
        
    };


    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == this.Owner && dealer != null && !dealer.IsPlayer && props.IsPoweredAttack_() && result.UnblockedDamage > 0)
        {
            this.Flash();
            foreach (Creature c in CombatState.Enemies)
            {
                string NextMoveID = c.Monster.NextMove.Id;
                await CreatureCmd.Stun(c, NextMoveID);
            }
            await PowerCmd.TickDownDuration(this);
        }
    }
}
