using BaseLib.Config;
using BaseLib.Config.UI;
using Deadcells.Scripts.utils;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;


[ModInitializer("Init")]
public static class Initialize
{
    public const string ModId = "Deadcells";
    private const string LogPrefix = "[Deadcells]";

    public static void Init()
    {
        Log.Info($"{LogPrefix} Init called", 2);
        Harmony _harmony = new Harmony(ModId);

        //ModConfigRegistry.Register(ModId, new JokerModConfig());
        _harmony.PatchAll(typeof(Initialize).Assembly);

        Log.Info($"{LogPrefix} - Harmony PatchAll completed", 2);
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Initialize).Assembly);

        ModConfigRegistry.Register(ModId, new DeadcellsModConfig());
    }
}
