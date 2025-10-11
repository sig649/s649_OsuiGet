using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
namespace s649_OsuiGet
{
    public static class PluginDatas
    {
        public const string GUID = "s649_OsuiGet";
        public const string MOD_TITLE = "Osui Get";
        public const string MOD_VERSION = "0.1.1.0";
    }

    [BepInPlugin(PluginDatas.GUID, PluginDatas.MOD_TITLE, PluginDatas.MOD_VERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            new Harmony(this.GetType().Name).PatchAll();
        }
    }
}
