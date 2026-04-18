using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;

namespace Deadcells.Scripts.relics;

//生存卷轴
[Pool(typeof(DeadcellRelicPool))]
public sealed class Tactical : CustomRelicModel
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
    };

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await RewardsCmd.OfferCustom(base.Owner, [new CardRemovalReward(base.Owner)]);
    }
}