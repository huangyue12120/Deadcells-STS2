using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadcells.Scripts.powers;

public sealed class UnyieldingWillPower : CustomPowerModel
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

    private bool LostHpInPreviousTurn => CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Any((DamageReceivedEntry e) => e.Receiver == base.Owner && !e.Result.WasFullyBlocked && e.RoundNumber + 1 == base.Owner.CombatState.RoundNumber);

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(!LostHpInPreviousTurn)
        {
            this.Flash();
            List<CardModel> cards = (from c in PileType.Discard.GetPile(base.Owner.Player).Cards
                                     orderby c.Rarity, c.Id
                                     select c).ToList();
            if(cards.Count > 0)
            {
                List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner.Player, new CardSelectorPrefs(base.SelectionScreenPrompt, 0, this.Amount))).ToList();
                if (list.Count > 0 && list.Count <= this.Amount)
                {
                    foreach (CardModel card in list)
                    {
                        await CardPileCmd.Add(card, PileType.Hand);
                    }
                }
            }
            await CardPileCmd.Draw(choiceContext, this.Amount, base.Owner.Player);
        }
        else
        {
            await CreatureCmd.GainBlock(base.Owner, 6*this.Amount, ValueProp.Move, null);
        }
    }
}