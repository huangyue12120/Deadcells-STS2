using BaseLib.Abstracts;
using Deadcells.Scripts.cards;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Deadcells.Scripts.events;

public sealed class HalfLifeEvent : CustomEventModel
{
    // 背景图位置
    public override string? CustomInitialPortraitPath => "res://Deadcells/images/events/deadcells-half_life_event.png";

    // 设置一些数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GoldVar(50),
        new CardsVar(1)
    ];

    // 什么时候会遇到。这里的条件是有任何一个玩家角色为枭首者。
    public override bool IsAllowed(IRunState runState) => runState.Players.Any(p => p.Character is Beheaded);

    // 事件开始前的逻辑。这里是禁止玩家移除药水
    protected override Task BeforeEventStarted(bool isPreFinished)
    {
        Owner!.CanRemovePotions = false;
        return Task.CompletedTask;
    }

    // 事件结束后的逻辑。这里是允许玩家移除药水
    protected override void OnEventFinished()
    {
        Owner!.CanRemovePotions = true;
    }

    // 生成事件初始选项。这里是两个选项：失去生命值或者失去金币，然后进入选择奖励阶段
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(Notice),
        Option(Leave)
    ];

    // 失去生命
    private async Task Notice()
    {
        ChooseRewardTypePage();
    }

    // 失去金币


    private async Task Leave()
    {
        SetEventFinished(PageDescription("LEAVE_CHOSEN1"));
    }

    // 进入事件第二阶段，两个选项：选择药水或者选择卡牌
    private void ChooseRewardTypePage()
    {
        SetEventState(PageDescription("CHOOSE_NOTICE"), [
            Option(ChooseQg, "CHOOSE_NOTICE", [HoverTipFactory.FromCard<CrowBar>()]), // 第二个参数代表该选项所在页面
            Option(ChooseSl, "CHOOSE_NOTICE", [HoverTipFactory.FromPower<GrenadePower>()]),
            Option(ChooseAll, "CHOOSE_NOTICE", [HoverTipFactory.FromCard<CrowBar>(), HoverTipFactory.FromPower<GrenadePower>()])
        ]);
    }

    // 选择药水奖励，然后结束事件
    private async Task ChooseQg()
    {
        List<CardModel> source = PileType.Deck.GetPile(Owner!).Cards.Where((CardModel c) => c != null && c.IsBasicStrikeOrDefend && c.Tags.Contains(CardTag.Strike) && c.IsRemovable).ToList();
        if (source == null || source.Count == 0)
        {
            SetEventFinished(PageDescription("LEAVE_CHOSEN1"));
        }
        else
        {
            int crowBarNumber = source.Count();
            await CardPileCmd.RemoveFromDeck(source);
            for (int i = 0; i < crowBarNumber; i++)
            {
                CardModel card = Owner!.RunState.CreateCard<CrowBar>(base.Owner);
                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
            }
            SetEventFinished(PageDescription("LEAVE_CHOSEN2")); // 结束事件时调用这个
        }
    }

    // 选择卡牌奖励，然后结束事件
    private async Task ChooseSl()
    {
        await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], (CardModel c) => c is DeadcellsCardModel dc && dc.DCCDTags.Contains(DeadcellsCardModel.DeadcellsCardTag.Grenade)), DynamicVars.Cards.IntValue, Owner)]);
        SetEventFinished(PageDescription("LEAVE_CHOSEN3"));
    }

    private async Task ChooseAll()
    {
        List<CardModel> source = PileType.Deck.GetPile(Owner!).Cards.Where((CardModel c) => c != null && c.IsBasicStrikeOrDefend && c.Tags.Contains(CardTag.Strike) && c.IsRemovable).ToList();
        if (source != null && source.Count != 0)
        {
            int crowBarNumber = source.Count();
            await CardPileCmd.RemoveFromDeck(source);
            for (int i = 0; i < crowBarNumber; i++)
            {
                CardModel card = Owner!.RunState.CreateCard<CrowBar>(base.Owner);
                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
            }
        }
        await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], (CardModel c) => c is DeadcellsCardModel dc && dc.DCCDTags.Contains(DeadcellsCardModel.DeadcellsCardTag.Grenade)), DynamicVars.Cards.IntValue, Owner)]);
        await PlayerCmd.LoseGold(DynamicVars.Gold.BaseValue, Owner!, GoldLossType.Stolen);
        SetEventFinished(PageDescription("LEAVE_CHOSEN4"));
    }
}