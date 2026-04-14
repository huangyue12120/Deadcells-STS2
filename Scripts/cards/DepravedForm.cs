using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.cards;

//堕落形态
//获得 6 点力量 . NL 你的 攻击 会造成 !M! 次你伤害25%的额外伤害. NL 获得1层 易伤 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class DepravedForm() : DeadcellsCardModel(3, CardType.Power, CardRarity.Rare, TargetType.Self)
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
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DepravedPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(6),
        new PowerVar<DepravedPower>(1),
        new PowerVar<VulnerablePower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["StrengthPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DepravedPower>(base.Owner.Creature, base.DynamicVars["DepravedPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(base.Owner.Creature, base.DynamicVars["VulnerablePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DepravedPower"].UpgradeValueBy(1);
    }

}
