using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//蛇牙
//造成 !D! 点伤害给予 !M! 层 中毒 . 如果目标至少有 !CN! 层 中毒 改为造成 !HD! 点伤害. NL 将这张牌放入你的抽牌堆.
[Pool(typeof(DeadcellsCardPool))]
public sealed class SnakeFangs() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PoisonPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5, ValueProp.Move),
        new ExtraDamageVar(7),
        new PowerVar<PoisonPower>(4)
        //new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, target) => target?.GetPowerAmount<PoisonPower>() >= 5 ? 1 : 0)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target.GetPowerAmount<PoisonPower>() >= 5)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue + base.DynamicVars.ExtraDamage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, base.DynamicVars["PoisonPower"].BaseValue, base.Owner.Creature, this);
        if (!base.Keywords.Contains(CardKeyword.Exhaust) && !base.ExhaustOnNextPlay)
        {
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(3);
    }

}
