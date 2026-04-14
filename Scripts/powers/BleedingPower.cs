using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

//流血
//每使用一张牌，受到amount点伤害，层数减少1
public sealed class BleedingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    private int cardPlayedNum = 0;

    private float Sign = 0.5f;

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        this.cardPlayedNum = 0;
        return base.BeforeApplied(target, amount, applier, cardSource);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        cardPlayedNum++;
        if (cardPlayedNum > 1)
        {
            this.Flash();
            int damage = this.Amount == 1 ? 1 : Mathf.FloorToInt(this.Amount * this.Sign);
            await CreatureCmd.Damage(context, base.Owner, (decimal)damage, ValueProp.Unpowered, cardPlay.Card);
            await PowerCmd.Apply<BleedingPower>(base.Owner, (decimal)-damage, base.Owner, null);
        }
    }
}
