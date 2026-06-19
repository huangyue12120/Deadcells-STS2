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
//掠夺
//获得 !CN! 点 敏捷 . NL 战斗结束时额外获得 !M! 枚 *金币 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class Rob() : DeadcellsCardModel(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool Gold => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<RobPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<DexterityPower>(2),
        new PowerVar<RobPower>(20)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, this.Owner.Creature, this);
        await PowerCmd.Apply<RobPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RobPower"].BaseValue, this.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Dexterity.UpgradeValueBy(2);
        base.DynamicVars["RobPower"].UpgradeValueBy(5);
    }

}
