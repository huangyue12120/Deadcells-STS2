using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//多头弓
//随机丢弃 !CN! 张牌 NL 对随机敌人造成 !D! 点伤害
[Pool(typeof(DeadcellsCardPool))]
public sealed class MultipleNocksBow() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6, ValueProp.Move),
        new RepeatVar(2),
        new CardsVar(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        IEnumerable<CardModel> cardsToDiscard = PileType.Hand.GetPile(base.Owner).Cards.ToList().UnstableShuffle(base.Owner.RunState.Rng.CombatCardSelection).Take(base.DynamicVars.Cards.IntValue);
        await CardCmd.Discard(choiceContext, cardsToDiscard);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
    }

}