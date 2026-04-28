using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//飞刀
//造成 !D! 点伤害. NL 给予 !M! 层 流血 . NL 消耗 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class ThrowingKnife() : DeadcellsCardModel(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, false)
{
    protected override bool Red => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.SingleBlood];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BleedingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(1, ValueProp.Move),
        new PowerVar<BleedingPower>(4)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        await PowerCmd.Apply<BleedingPower>(cardPlay.Target, base.DynamicVars["BleedingPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {

    }

}

