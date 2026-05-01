using BepInEx;

namespace Coop;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private static Plugin instance;

    private void Awake()
    {
        instance = this;
        new CreatePlayers().Init();
    }

    public static void Log(object data)
    {
        instance.Logger.LogInfo(data);
    }
}
