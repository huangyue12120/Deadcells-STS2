using BaseLib.Abstracts;
using Deadcells.Scripts.utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

//流血
//每使用一张牌，受到amount点伤害，层数减少1
public sealed class BleedingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    private int cardPlayedNum = 0;

    private float Sign = 0.5f;

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        this.cardPlayedNum = 0;
        return base.BeforeApplied(target, amount, applier, cardSource);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        cardPlayedNum++;
        if (cardPlayedNum > 1)
        {
            this.Flash();
            int damage = this.Amount == 1 ? 1 : Mathf.FloorToInt(this.Amount * this.Sign);
            await CreatureCmd.Damage(context, base.Owner, (decimal)damage, ValueProp.Unpowered, cardPlay.Card);
            await PowerCmd.Apply<BleedingPower>(base.Owner, (decimal)-damage, base.Owner, null);
        }
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        ApplyShader();
        return base.AfterApplied(applier, cardSource);
    }

    private void ApplyShader()
    {
        // 1. 获取视觉节点
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(base.Owner);
        var visuals = creatureNode?.Visuals as NCreatureVisuals;
        if (visuals?.SpineBody == null)
        {
            Log.Warn("Mod: 无法获取 SpineBody");
            return;
        }

        // 2. 创建燃烧材质副本
        var baseMat = ResourceLoader.Load<ShaderMaterial>("res://Deadcells/images/vfx/blood.tres");
        if (baseMat == null) return;
        _burnMaterialInstance = (ShaderMaterial)baseMat.Duplicate();
        var noiseTex = new NoiseTexture2D();
        noiseTex.Width = 256;
        noiseTex.Height = 256;
        noiseTex.Seamless = true;
        noiseTex.GenerateMipmaps = true;
        noiseTex.Normalize = true;

        var fastNoise = new FastNoiseLite();
        fastNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        fastNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
        fastNoise.FractalOctaves = 3;
        noiseTex.Noise = fastNoise;

        _burnMaterialInstance.SetShaderParameter("noise_tex", noiseTex);

        // 4. 推入材质栈（自动处理叠加）
        MonsterMaterialStack.PushMaterial(base.Owner, _burnMaterialInstance, visuals);
    }

    private void RemoveShader()
    {
        // 获取视觉节点
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(base.Owner);
        var visuals = creatureNode?.Visuals as NCreatureVisuals;
        if (visuals?.SpineBody != null)
        {
            // 弹出当前效果，恢复上一个材质
            MonsterMaterialStack.PopMaterial(base.Owner, _burnMaterialInstance, visuals);
        }
    }

    private void RemoveAllAfterDeath()
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(base.Owner);
        var visuals = creatureNode?.Visuals as NCreatureVisuals;
        if (visuals?.SpineBody != null)
        {
            // 弹出当前效果，恢复上一个材质
            MonsterMaterialStack.ClearStack(base.Owner, visuals);
        }
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if(creature == base.Owner)
        {
            RemoveAllAfterDeath();
        }
        return base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        RemoveShader();
        return base.AfterRemoved(oldOwner);
    }

    private ShaderMaterial _burnMaterialInstance;
}
