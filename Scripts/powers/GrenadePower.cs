using BaseLib.Abstracts;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Text;

namespace Deadcells.Scripts.powers;

public sealed class GrenadePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    // 记录处于冷却中的卡牌及其剩余冷却回合数
    private readonly List<CooldownEntry> cooldownEntries = new List<CooldownEntry>();

    // 内部冷却条目
    private class CooldownEntry
    {
        public required DeadcellsCardModel Card { get; set; }
        public required int RemainingTurns { get; set; }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (cooldownEntries.Count == 0)
                yield break;

            // 尝试获取本地化条目
            const string table = "powers";
            const string titleKey = "DEADCELLS-GRENADE_COOLDOWN_TITLE";
            const string descKey = "DEADCELLS-GRENADE_COOLDOWN_DESC";
            const string lineKey = "DEADCELLS-GRENADE_COOLDOWN_LINE";

            // 检查本地化条目是否存在
            if (!LocString.Exists(table, titleKey) ||
                !LocString.Exists(table, descKey) ||
                !LocString.Exists(table, lineKey))
            {
                // 回退：显示第一张冷却卡牌的悬停提示
                yield return HoverTipFactory.FromCard(cooldownEntries.First().Card);
                yield break;
            }

            // 使用全新的 LocString 实例，避免缓存问题
            var titleLoc = new LocString(table, titleKey);
            var descLoc = new LocString(table, descKey);

            // 构建冷却列表文本
            var sb = new StringBuilder();
            foreach (var entry in cooldownEntries)
            {
                string cardName = entry.Card.TitleLocString?.GetFormattedText()
                                  ?? entry.Card.Title;
                int remaining = entry.RemainingTurns;

                var lineLoc = new LocString(table, lineKey);
                lineLoc.Add("card_name", cardName);
                lineLoc.Add("remaining", (decimal)remaining);
                sb.AppendLine(lineLoc.GetFormattedText());
            }

            // 将列表注入描述模板
            descLoc.Add("cooldown_list", sb.ToString().TrimEnd());
            yield return new HoverTip(titleLoc, descLoc);
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    public Task AddGrenadeToCooldown(DeadcellsCardModel card, int baseCooldown)
    {
        cooldownEntries.Add(new CooldownEntry
        {
            Card = card,
            RemainingTurns = baseCooldown
        });
        return Task.CompletedTask;
    }

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        cooldownEntries.Clear();
        return base.BeforeApplied(target, amount, applier, cardSource);
    }

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        this.Flash();
        for (int i = cooldownEntries.Count - 1; i >= 0; i--)
        {
            var entry = cooldownEntries[i];
            entry.RemainingTurns--;

            if (entry.RemainingTurns <= 0)
            {
                await CardPileCmd.Add(entry.Card, PileType.Draw, CardPilePosition.Random);
                cooldownEntries.RemoveAt(i);
            }

            if (cooldownEntries.Count == 0)
            {
                await PowerCmd.Remove(this);
            }
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side && combatState.RoundNumber <= 1)
        {
            cooldownEntries.Clear();
        }
        return base.AfterSideTurnStart(side, combatState);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        cooldownEntries.Clear();
        return base.AfterCombatVictory(room);
    }
}
