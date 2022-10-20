using UnityEngine;

namespace BetterSandbox.Asset
{
    public static class AssetManager
    {
        private static readonly AssetBundle BetterSandboxAssetsBundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("bettersandbox_assets", typeof(BetterSandbox).Assembly);

        public static GameObject SandboxCanvas = BetterSandboxAssetsBundle.LoadAsset<GameObject>("SandboxCanvas");

        public static GameObject ManageSandbox = BetterSandboxAssetsBundle.LoadAsset<GameObject>("ManageSandbox");
        public static GameObject ManageSandboxMenu = BetterSandboxAssetsBundle.LoadAsset<GameObject>("ManageSandboxMenu");
        public static GameObject CardSearch = BetterSandboxAssetsBundle.LoadAsset<GameObject>("CardSearch");
    }
}