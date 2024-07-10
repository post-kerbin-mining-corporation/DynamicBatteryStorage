using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DynamicBatteryStorage.UI
{
  /// <summary>
  /// Loads and holds references to Asset Bundled things
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class SystemsMonitorAssets : MonoBehaviour
  {
    public static GameObject OverlayPanelPrefab { get; private set; }
    public static GameObject ToolbarPanelPrefab { get; private set; }
    public static GameObject ToolbarPanelLoopPrefab { get; private set; }
    public static GameObject ReactorWidgetPrefab { get; private set; }
    public static GameObject ReactorToolbarPanelPrefab { get; private set; }
    public static Dictionary<string, Sprite> Sprites { get; private set; }


    internal static string ASSET_PATH = "GameData/DynamicBatteryStorage/UI/systemsmonitor.dat";
    internal static string SPRITE_ATLAS_NAME = "system-heat-sprites-1";
    private void Awake()
    {
      Utils.Log("[SystemsMonitorAssets]: Loading Assets");
      AssetBundle prefabs = AssetBundle.LoadFromFile(Path.Combine(KSPUtil.ApplicationRootPath, ASSET_PATH));

      /// Get the Prefabs
      OverlayPanelPrefab = prefabs.LoadAsset("SystemInfo") as GameObject;

      Utils.Log("[SystemsMonitorAssets]: Loaded UI Prefabs");
      /// Get the Sprite Atlas
      Sprite[] spriteSheet = prefabs.LoadAssetWithSubAssets<Sprite>(SPRITE_ATLAS_NAME);
      Sprites = new Dictionary<string, Sprite>();
      foreach (Sprite subSprite in spriteSheet)
      {
        Sprites.Add(subSprite.name, subSprite);
      }
      Utils.Log($"[SystemsMonitorAssets]: Loaded {Sprites.Count} sprites");
    }
  }
}
