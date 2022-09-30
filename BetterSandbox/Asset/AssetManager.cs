using UnityEngine;

namespace TemplateProject.Asset
{
    public static class AssetManager
    {
        private static readonly AssetBundle TimerAssetsBundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("picktimer_assets", typeof(BetterSandbox.BetterSandbox).Assembly);
    }
}
