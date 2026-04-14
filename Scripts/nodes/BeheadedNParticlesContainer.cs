using Godot;
using Godot.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;



namespace Deadcells.Scripts.nodes;

[GlobalClass]
public partial class BeheadedNParticlesContainer : NParticlesContainer
{
    public override void _Ready()
    {
        base._Ready();
        Traverse.Create(this).Field("_particles").SetValue(new Array<GpuParticles2D>());
    }
}
