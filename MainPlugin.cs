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
        public const string MOD_TITLE = "s649_WaterQualityImprovementProject";
        public const string MOD_VERSION = "0.2";
    }
    internal enum WishMode
    {
        NoMod, Modded, ForceWish
    }
    internal enum UseEPMode
    { 
        NoMod, NotDirty, AllDirty
    }
    internal enum PollutedWellMode
    { 
        NoMod, Modded, ForceEffect
    }
    

    [BepInPlugin(PluginDatas.GUID, PluginDatas.MOD_TITLE, PluginDatas.MOD_VERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        internal static ConfigEntry<UseEPMode> ce_UseEmptyPotionTweak;
        internal static ConfigEntry<bool> ce_DrinkWellTweak;
        internal static ConfigEntry<WishMode> ce_WellWishMode;
        internal static ConfigEntry<PollutedWellMode> ce_PollutedWellMode;
        internal static ConfigEntry<bool> ce_EnableWishForStolenFish;
        //internal ConfigItem<WishMode> configItem_WellWishMode;
        private void Start()
        {
            LoadConfig();
            new Harmony(this.GetType().Name).PatchAll();
        }
        private void LoadConfig() 
        {
            ce_UseEmptyPotionTweak = Config.BindItem(
                new ConfigItem<UseEPMode>
                {
                    Section = "TraitPotionEmptyTweak",
                    Key = "ChangePotionOnUse",
                    Value = UseEPMode.NotDirty,
                    Description = "Modify the procedure for using empty bottles near water sources or wells",
                    DescriptionJP = "空き瓶を水場や井戸に対して使う時の処理を改変する。"
                }
            );
            ce_DrinkWellTweak = Config.BindItem(
                new ConfigItem<bool>
                {
                    Section = "TraitWellTweak",
                    Key = "ModifyTraitWell",
                    Value = true,
                    Description = "Modify the procedure for drinking well water.",
                    DescriptionJP = "井戸水を飲んだ時の処理を改変する。"
                }
            );
            ce_WellWishMode = Config.BindItem(
                new ConfigItem<WishMode>
                {
                    Section = "TraitWellTweak",
                    Key = "WellWishMode",
                    Value = WishMode.Modded,
                    Description = "Control the probability of the event where a wish is made at the well.",
                    DescriptionJP = "井戸で願いを起こすイベントの確率を制御する。"
                }
            );
            ce_PollutedWellMode = Config.BindItem(
                new ConfigItem<PollutedWellMode>
                {
                    Section = "TraitWellTweak",
                    Key = "PollutedWellMode",
                    Value = PollutedWellMode.Modded,
                    Description = "Control the event that occurs when you drink water from a contaminated well.",
                    DescriptionJP = "汚染された井戸の水を飲んだ時のイベントを制御する。"
                }
            );
            ce_EnableWishForStolenFish = Config.BindItem(
                new ConfigItem<bool>
                {
                    Section = "TraitWellTweak",
                    Key = "EnableWishForStolenFish",
                    Value = true,
                    Description = "So that even players who don't have the Goddess's Fragrance can make a wish.",
                    DescriptionJP = "女神の残り香を持っていないプレイヤーでも願えるように。"
                }
            );
        }
        
    }
}
