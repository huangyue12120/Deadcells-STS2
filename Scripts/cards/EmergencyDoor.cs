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

//紧急逃生门
//获得 !B! 点 格挡 . NL 获得2层 缓冲 . NL 获得2层 虚弱 
[Pool(typeof(DeadcellsCardPool))]
public sealed class EmergencyDoor() : DeadcellsCardModel(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromPower<WeakPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(20, ValueProp.Move),
        new PowerVar<BufferPower>(2),
        new PowerVar<WeakPower>(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay, false);
        await PowerCmd.Apply<BufferPower>(base.Owner.Creature, base.DynamicVars["BufferPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(base.Owner.Creature, base.DynamicVars["WeakPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

}
