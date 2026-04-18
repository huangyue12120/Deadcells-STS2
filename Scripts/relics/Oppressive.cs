using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Deadcells.Scripts.relics;

//暴虐卷轴
[Pool(typeof(DeadcellRelicPool))]
public sealed class Oppressive : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // 小图标
    public override string PackedIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1),
        new PowerVar<StrengthPower>(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>()
    };

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c.IsUpgradable).ToList().StableShuffle(base.Owner.RunState.Rng.Niche)
            .Take(base.DynamicVars.Cards.IntValue);
        foreach (CardModel item in enumerable)
        {
            CardCmd.Upgrade(item);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber <= 1)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, null);
        }
    }
}
