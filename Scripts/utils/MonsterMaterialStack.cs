using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Collections.Generic;

namespace Deadcells.Scripts.utils;

public static class MonsterMaterialStack
{
    // 怪物实例ID -> 材质列表（索引0为默认材质，后续按添加顺序存储）
    private static Dictionary<ModelId, List<Material>> _materialLists = new();

    // 怪物实例ID -> 默认材质副本（复用，避免重复复制）
    private static Dictionary<ModelId, Material> _defaultMaterials = new();

    /// <summary>
    /// 为怪物添加一个材质效果（优先级最高，显示在顶部）。
    /// </summary>
    /// <returns>返回材质实例，用于后续精确移除。</returns>
    public static Material PushMaterial(Creature owner, Material newMaterial, NCreatureVisuals visuals)
    {
        ModelId id = owner.ModelId;
        var spineBody = visuals?.SpineBody;
        if (spineBody == null) return null;

        // 初始化默认材质（仅一次）
        if (!_defaultMaterials.ContainsKey(id))
        {
            var currentMat = spineBody.GetNormalMaterial();
            _defaultMaterials[id] = currentMat != null
                ? (Material)currentMat.Duplicate()
                : new ShaderMaterial();
        }

        // 初始化列表
        if (!_materialLists.ContainsKey(id))
        {
            _materialLists[id] = new List<Material> { _defaultMaterials[id] };
        }

        var list = _materialLists[id];
        // 新材质追加到末尾（优先级最高）
        list.Add(newMaterial);
        // 应用最新材质
        spineBody.SetNormalMaterial(newMaterial);

        return newMaterial; // 返回材质引用，供 Pop 时使用
    }

    /// <summary>
    /// 移除指定的材质效果。
    /// </summary>
    /// <param name="materialToRemove">必须是之前 Push 返回的材质实例。</param>
    public static void PopMaterial(Creature owner, Material materialToRemove, NCreatureVisuals visuals)
    {
        ModelId id = owner.ModelId;
        if (!_materialLists.ContainsKey(id)) return;

        var list = _materialLists[id];
        // 不允许移除默认材质（索引0）
        if (list.Count <= 1 || materialToRemove == list[0]) return;

        int index = list.IndexOf(materialToRemove);
        if (index == -1) return;

        // 移除指定材质
        list.RemoveAt(index);

        var spineBody = visuals?.SpineBody;
        if (spineBody == null) return;

        // 恢复栈顶（列表最后一项）
        Material topMaterial = list[list.Count - 1];
        spineBody.SetNormalMaterial(topMaterial);

        // 如果只剩默认材质，可以清理字典（可选）
        if (list.Count == 1)
        {
            _materialLists.Remove(id);
            // 保留 _defaultMaterials，以防怪物再次被施加效果
        }
    }

    /// <summary>
    /// 清除怪物所有自定义材质（恢复默认）。
    /// </summary>
    public static void ClearStack(Creature owner, NCreatureVisuals visuals)
    {
        ModelId id = owner.ModelId;
        if (_materialLists.ContainsKey(id))
        {
            _materialLists.Remove(id);
        }
        if (_defaultMaterials.TryGetValue(id, out var defaultMat) && visuals?.SpineBody != null)
        {
            visuals.SpineBody.SetNormalMaterial(defaultMat);
        }
    }
}