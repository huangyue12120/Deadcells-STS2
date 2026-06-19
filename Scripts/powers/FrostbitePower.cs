using BaseLib.Abstracts;
using BaseLib.Extensions;
using Deadcells.Scripts.utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.powers;

public sealed class FrostbitePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("DamageDecrease", 10),
    };

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner)
        {
            return 1;
        }
        if (!props.IsPoweredAttack_())
        {
            return 1;
        }
        decimal num = 1 - (base.DynamicVars["DamageDecrease"].BaseValue / 100);
        return num;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == base.Owner && props.IsPoweredAttack_() && target != null && amount >= 0)
        {
            return (-this.Amount);
        }
        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            this.Flash();
            await PowerCmd.TickDownDuration(this);
            await PowerCmd.TickDownDuration(this);
        }
    }

    private Material _originalMaterial;

    private void ApplyShader()
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(base.Owner);
        var visuals = creatureNode?.Visuals as NCreatureVisuals;
        var spineBody = visuals?.SpineBody;
        if (spineBody == null)
        {
            Log.Warn("无法获取 SpineBody，无法应用燃烧材质");
            return;
        }

        _originalMaterial = spineBody.GetNormalMaterial();

        var baseMat = ResourceLoader.Load<ShaderMaterial>("res://Deadcells/images/vfx/ice.tres");
        if (baseMat == null)
            return;

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

    private void CleanupResources()
    {
        if (_burnMaterialInstance != null)
        {
            if (GodotObject.IsInstanceValid(_burnMaterialInstance))
            {
                _burnMaterialInstance.Dispose();
            }
        }
    }

    private void RemoveShader()
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(base.Owner);
        var visuals = creatureNode?.Visuals as NCreatureVisuals;
        if (visuals?.SpineBody != null)
        {
            MonsterMaterialStack.PopMaterial(base.Owner, _burnMaterialInstance, visuals);
            CleanupResources();
        }
    }

    private void RemoveAllAfterDeath()
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(base.Owner);
        var visuals = creatureNode?.Visuals as NCreatureVisuals;
        if (visuals?.SpineBody != null)
        {
            MonsterMaterialStack.ClearStack(base.Owner, visuals);
        }
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        ApplyShader();
        return base.AfterApplied(applier, cardSource);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature == base.Owner)
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

