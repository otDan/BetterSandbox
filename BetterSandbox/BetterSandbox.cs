using BepInEx;
using BetterSandbox.Mode;
using HarmonyLib;
using RWF;
using UnboundLib;
using UnboundLib.GameModes;
using SandboxHandler = BetterSandbox.Mode.SandboxHandler;

namespace BetterSandbox
{
    [BepInDependency("com.willis.rounds.unbound")]
    [BepInDependency("pykess.rounds.plugins.moddingutils")]
    [BepInDependency("pykess.rounds.plugins.pickncards")]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class BetterSandbox : BaseUnityPlugin
    {
        private const string ModId = "ot.dan.rounds.bettersandbox";
        private const string ModName = "Better Sandbox";
        public const string Version = "1.0.0";
        public const string ModInitials = "";
        private const string CompatibilityModName = "BetterSandbox";
        public static BetterSandbox Instance { get; private set; }
        private const bool DEBUG = true;

        private void Awake()
        {
            Instance = this;
            
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        private void Start()
        {
            GameModeManager.RemoveHandler(GameModeManager.SandBoxID);
            GameModeManager.AddHandler<Sandbox>(SandboxHandler.GameModeID, new SandboxHandler());
            GameModeManager.AddHandler<Sandbox>(TeamSandboxHandler.GameModeID, new TeamSandboxHandler());
        }

        public void Log(string debug)
        {
            if (DEBUG) UnityEngine.Debug.Log(debug);
        }
    }
}