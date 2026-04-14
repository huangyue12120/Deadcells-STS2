using BaseLib.Abstracts;
using Deadcells.Scripts.cards;
using Deadcells.Scripts.relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.character;

public sealed class Beheaded : PlaceholderCharacterModel
{
    public override Color NameColor => new Color("4C0099");

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 72;

    public override int StartingGold => 99;

    // 人物模型tscn路径。要自定义见下。!!!
    public override string CustomVisualPath => "res://Deadcells/animations/beheaded.tscn";
    // 卡牌拖尾路径。
    public override string CustomTrailPath => "res://Deadcells/scenes/beheaded/card_trail_beheaded.tscn";
    // 人物头像路径。
    public override string CustomIconTexturePath => "res://Deadcells/images/character/character_icon_beheaded.png";
    // 人物头像2号。
    public override string CustomIconPath => "res://Deadcells/scenes/beheaded/beheaded_icon.tscn";
    // 能量表盘tscn路径。要自定义见下。
    //public override CustomEnergyCounter? CustomEnergyCounter => new CustomEnergyCounter(EnergyCounterPaths, StsColors.darkBlue, StsColors.aqua);

    public override string CustomEnergyCounterPath => "res://Deadcells/scenes/beheaded/beheaded_energy_counter.tscn";
    // 篝火休息动画。
    //public override string CustomRestSiteAnimPath => "res://animation/joker_restsite.tscn";
    // 商店人物动画。
    public override string CustomMerchantAnimPath => "res://Deadcells/scenes/beheaded/beheaded_merchant.tscn";
    // 多人模式-手指。
    //public override string CustomArmPointingTexturePath => "res://images/character/hands/multiplayer_hand_joker_point.png";
    // 多人模式剪刀石头布-石头。
    //public override string CustomArmRockTexturePath => "res://images/character/hands/multiplayer_hand_joker_rock.png";
    // 多人模式剪刀石头布-布。
    //public override string CustomArmPaperTexturePath => "res://images/character/hands/multiplayer_hand_joker_paper.png";
    // 多人模式剪刀石头布-剪刀。
    //public override string CustomArmScissorsTexturePath => "res://images/character/hands/multiplayer_hand_joker_scissors.png";

    // 人物选择背景。
    public override string CustomCharacterSelectBg => "res://Deadcells/scenes/beheaded/char_select_bg_beheaded.tscn";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "res://Deadcells/images/character/char_select_beheaded.png";
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "res://Deadcells/images/character/char_select_beheaded_locked.png";
    // 人物选择过渡动画。
    public override string CustomCharacterSelectTransitionPath => "res://Deadcells/images/character/transitions/beheaded_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "res://Deadcells/images/character/map_marker_beheaded.png";
    // 攻击音效
    // public override string CustomAttackSfx => null;
    // 施法音效
    // public override string CustomCastSfx => null;
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    // public override string CharacterSelectSfx => null;
    // 过渡音效。这个不能删。 
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override CardPoolModel CardPool => ModelDb.CardPool<DeadcellsCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DeadcellRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DeadcellPotionPool>();

    public override IEnumerable<CardModel> StartingDeck => new CardModel[]
    {
        ModelDb.Card<StrikeBeheaded>(),
        ModelDb.Card<StrikeBeheaded>(),
        ModelDb.Card<StrikeBeheaded>(),
        ModelDb.Card<StrikeBeheaded>(),
        ModelDb.Card<DefendBeheaded>(),
        ModelDb.Card<DefendBeheaded>(),
        ModelDb.Card<DefendBeheaded>(),
        ModelDb.Card<DefendBeheaded>(),
        ModelDb.Card<Roll>(),
        ModelDb.Card<RoundingKnife>(),
        ModelDb.Card<BeginnerBow>(),
    };

    public override IReadOnlyList<RelicModel> StartingRelics => new RelicModel[]
    {
        ModelDb.Relic<EmeraldAmulet>()
    };

    private string EnergyCounterPaths(int i)
    {
        return "res://Deadcells/images/ui/combat/energy_counters/layer" + i + ".png";
    }

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    protected override CharacterModel? UnlocksAfterRunAs => null;

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}