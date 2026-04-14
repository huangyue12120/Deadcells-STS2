using BaseLib.Abstracts;
using BaseLib.Utils;
using Deadcells.Scripts.cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.relics;

//绿宝石护符
[Pool(typeof(DeadcellRelicPool))]
public sealed class EmeraldAmulet : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Roll>()
    };

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber <= 1)
        {
            Flash();
            Roll roll = base.Owner.Creature.CombatState.CreateCard<Roll>(base.Owner);
            roll.AddKeyword(CardKeyword.Retain);
            roll.AddKeyword(CardKeyword.Exhaust);
            await CardPileCmd.Add(roll, PileType.Hand);
        }
    }

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<EmeraldAmuletPlus>();
    }
}

//绿宝石护符+
[Pool(typeof(DeadcellRelicPool))]
public sealed class EmeraldAmuletPlus : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Roll>(true)
    };

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber <= 1)
        {
            Flash();
            for (int i = 0; i < 2; i++)
            {
                Roll roll = base.Owner.Creature.CombatState.CreateCard<Roll>(base.Owner);
                CardCmd.Upgrade(roll);
                roll.AddKeyword(CardKeyword.Retain);
                roll.AddKeyword(CardKeyword.Exhaust);
                await CardPileCmd.Add(roll, PileType.Hand);
            }
        }
    }
}
