using BaseLib.Abstracts;
using Deadcells.Scripts.character;
using Deadcells.Scripts.relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Deadcells.Scripts.events;

public sealed class DarkSoulEvent : CustomEventModel
{
    // 背景图位置
    public override string? CustomInitialPortraitPath => "res://Deadcells/images/events/deadcells-dark_souls_event.png";

    private bool seeEgg = false;

    private bool seeFire = false;

    private bool findGold = false;

    public override bool IsAllowed(IRunState runState) => runState.Players.Any(p => p.Character is Beheaded);

    // 设置一些数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(100),
        new GoldVar(100),
        new CardsVar(1)
    ];

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
        Option(Notice)
    ];

    // 失去生命
    private async Task Notice()
    {
        ChooseRewardTypePage1();
    }

    // 失去金币


    private async Task ChooseLeave()
    {
        SetEventFinished(PageDescription("LEAVE_CHOSEN1"));
    }

    // 进入事件第二阶段，两个选项：选择药水或者选择卡牌
    private void ChooseRewardTypePage1()
    {
        if (Owner!.GetRelic<RamRune>() == null)
        {
            SetEventState(PageDescription("CHOOSE_NO_RAMRUNE"), [
                new EventOption(this, null, "DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_NO_RAMRUNE.options.CHOOSE_RAMRUNE_LOCKED"), // 第二个参数代表该选项所在页面
                Option(ChooseLeave, "CHOOSE_NO_RAMRUNE")
            ]);
        }
        else
        {
            SetEventState(PageDescription("CHOOSE_WITH_RAMRUNE"), [
                Option(ChooseBreakDown, "CHOOSE_WITH_RAMRUNE"), // 第二个参数代表该选项所在页面
                Option(ChooseLeave, "CHOOSE_WITH_RAMRUNE")
            ]);
        }
    }

    private async Task ChooseBreakDown()
    {
        ChooseRewardTypePage2();
    }

    private void ChooseRewardTypePage2()
    {
        SetEventState(PageDescription("CHOOSE_BREAK_DOWN"), [
            Option(ChooseGoAhead, "CHOOSE_BREAK_DOWN"), // 第二个参数代表该选项所在页面
        ]);
    }

    private async Task ChooseGoAhead()
    {
        ChooseRewardTypePage3();
    }

    private void ChooseRewardTypePage3()
    {
        SetEventState(PageDescription("CHOOSE_GO_AHEAD_DES"), [
            new EventOption(this, this.seeEgg ? null : ChooseSeeEgg, this.seeEgg ? "DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_GO_AHEAD_DES.options.CHOOSE_SEE_EGG_LOCKED" : "DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_GO_AHEAD_DES.options.CHOOSE_SEE_EGG"),
            new EventOption(this, this.seeFire ? null : ChooseSeeFire, this.seeFire ? "DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_GO_AHEAD_DES.options.CHOOSE_SEE_FIRE_LOCKED" : "DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_GO_AHEAD_DES.options.CHOOSE_SEE_FIRE"),// 第二个参数代表该选项所在页面
            new EventOption(this, this.findGold ? null : ChooseFindGold, this.findGold ?"DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_GO_AHEAD_DES.options.CHOOSE_FIND_GOLD_LOCKED" : "DEADCELLS-DARK_SOUL_EVENT.pages.CHOOSE_GO_AHEAD_DES.options.CHOOSE_FIND_GOLD"),
            Option(ChooseLeave, "CHOOSE_GO_AHEAD_DES"),
        ]);
    }

    private async Task ChooseSeeEgg()
    {
        this.seeEgg = true;
        ChooseRewardTypePage4(Egg: this.seeEgg);
    }

    private async Task ChooseSeeFire()
    {
        this.seeFire = true;
        await CreatureCmd.Heal(Owner!.Creature, (Owner!.Creature.MaxHp * (DynamicVars.Heal.BaseValue / 100)), false);
        ChooseRewardTypePage4(Fire: this.seeFire);
    }

    private async Task ChooseFindGold()
    {
        this.findGold = true;
        int GoldGain = Owner!.RunState.Rng.CombatCardGeneration.NextInt(50, DynamicVars.Gold.IntValue);
        await PlayerCmd.GainGold(GoldGain, Owner!);
        await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool]), 3, Owner)]);
        ChooseRewardTypePage4(Gold: this.findGold);
    }

    private void ChooseRewardTypePage4(bool Egg = false, bool Fire = false, bool Gold = false)
    {
        try
        {
            if (seeEgg)
            {
                SetEventState(PageDescription("CHOOSE_SEE_EGG_DES"), [
                        Option(ChooseGoOn, "CHOOSE_SEE_EGG_DES"), // 第二个参数代表该选项所在页面
                ]);
            }
            if (seeFire)
            {
                SetEventState(PageDescription("CHOOSE_SEE_FIRE_DES"), [
                        Option(ChooseGoOn, "CHOOSE_SEE_FIRE_DES"), // 第二个参数代表该选项所在页面
                ]);
            }
            if (findGold)
            {
                SetEventState(PageDescription("CHOOSE_FIND_GOLD_DES"), [
                        Option(ChooseGoOn, "CHOOSE_FIND_GOLD_DES"), // 第二个参数代表该选项所在页面
                ]);
            }
        }
        catch (Exception)
        {
            SetEventState(PageDescription("CHOOSE_BUG_DES"), [
                Option(ChooseLeave, "CHOOSE_BUG_DES"), // 第二个参数代表该选项所在页面
            ]);
        }
    }

    private async Task ChooseGoOn()
    {
        ChooseRewardTypePage3();
    }
}
