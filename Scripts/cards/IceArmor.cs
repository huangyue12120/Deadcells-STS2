using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//冰川护甲
//获得 !B! 点 格挡 . NL 如果你没有 格挡 改为获得 !CN! 点 格挡 . NL 给予所有敌人 !M! 层 冻伤 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class IceArmor() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<FrostbitePower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5, ValueProp.Move),
        new BlockVar("FixedBlock", 9, ValueProp.Move),
        new PowerVar<FrostbitePower>(1)
    };

    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.Block == 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.Owner.Creature.Block == 0)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["FixedBlock"].BaseValue, ValueProp.Move, cardPlay);
        }
        else
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        }
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            if (enemy == null) continue;
            await PowerCmd.Apply<FrostbitePower>(choiceContext,enemy, base.DynamicVars["FrostbitePower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3);
        base.DynamicVars["FixedBlock"].UpgradeValueBy(3);
    }

}
