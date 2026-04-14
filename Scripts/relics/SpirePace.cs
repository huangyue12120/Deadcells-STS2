using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.relics;

//杀戮节奏
//你每打出一张牌，如果该牌的类型不同于你的上一张牌，抽 1 张牌
[Pool(typeof(DeadcellRelicPool))]
public sealed class SpirePace : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.CardPlay.Card != cardPlay.Card);
        if (cardPlayStartedEntry != null && cardPlay.Card.Type != cardPlayStartedEntry.CardPlay.Card.Type)
        {
            await CardPileCmd.Draw(context, base.Owner);
        }
    }
}
