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

//冰霜爆破
//获得 !B! 点格挡 NL 给予所有敌人 !M! 层 冻伤 x次
[Pool(typeof(DeadcellsCardPool))]
public sealed class FrostBlast() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool Green => true;
    protected override bool Purple => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    protected override bool HasEnergyCostX => true;

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
        new BlockVar(4, ValueProp.Move),
        new PowerVar<FrostbitePower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = this.IsUpgraded ? (ResolveEnergyXValue() + 1) : ResolveEnergyXValue();
        for (int i = 0; i < xValue; i++)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
            foreach (Creature enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<FrostbitePower>(choiceContext, enemy, base.DynamicVars["FrostbitePower"].BaseValue, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {

    }

}
