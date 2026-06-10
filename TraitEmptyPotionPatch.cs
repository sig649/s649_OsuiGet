using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
namespace s649_OsuiGet
{
    [HarmonyPatch]
    internal class TraitEmptyPotionPatch
    {
        //static bool PatchEnable => MainPlugin.ce_UseEmptyPotionTweak.Value;
        /*
        enum WaterResource 
        {
            Well,
            FreshWater,
            SeaWater,
            Unknown
        }
        */
        /*
        internal static bool IsOnGlobalMap()
        {
            return EClass.pc.currentZone.id == "ntyris";
        }
        */
        const string idDirtyWater = "water_dirty";
        const string idNotDirtyWater = "water_not_dirty";
        const string idSeaWater = "1142";

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitPotionEmpty), "OnUse")]
        internal static bool OnUsePrePatch(Chara c, Point p, TraitPotionEmpty __instance) 
        {
            UseEPMode epMode = MainPlugin.ce_UseEmptyPotionTweak.Value;
            if (epMode == UseEPMode.NoMod) return true;

            TraitWell well = __instance.GetWell(p);
            string biome = EClass.pc.currentZone.biome.id.ToString();
            bool seawater = biome == "Sand" || biome == "Water"; // 海水フラグ
            //WaterResource wr = WaterResource.Unknown;
            bool finiteResource = well != null;
            //Thing t;
            string liquid = epMode == UseEPMode.AllDirty ? idDirtyWater : idNotDirtyWater;
            
            //所得源情報
            if (finiteResource)//有限(well)
            {
                //wr = WaterResource.Well;
                if (well.Charges <= 0) return true; //枯れているなら弾く
                if (well.IsHoly) return true; //聖なる井戸は弾く

                /*
                if (well.Charges <= 0)//枯れている
                {
                    c.Say("drinkWell_empty", c, well.owner);
                    return false;
                }
                */
                if (well.polluted)
                {
                    if (EClass.rnd(20) == 0 || epMode == UseEPMode.AllDirty)
                        liquid = idDirtyWater;
                    else
                        liquid = "potion";
                }

                well?.ModCharges(-1);
            }
            else
            {
                if (seawater) 
                    liquid = idSeaWater;
                //else 
                //    liquid = epMode == UseEPMode.AllDirty ? idDirtyWater : idNotDirtyWater;
            }
            Thing t = ThingGen.Create(liquid);
            /*
            //Traitの使用先が井戸ではない or Traitの使用先が聖なる井戸でも汚染されている井戸でもない
            if (well == null || (well != null && !well.IsHoly && !well.polluted))
            {
                
                
                if (well == null)
                {
                    if (biome == "Sand" || biome == "Water")
                    {//sea
                        t = ThingGen.Create("1142");//siomizu
                    }
                    else
                    {
                        t = ThingGen.Create("water_dirty");
                    }
                }
                else
                {
                    if (well.Charges <= 0)
                    {
                        c.Say("drinkWell_empty", c, well.owner);
                        return false;
                    }
                    t = ThingGen.Create("water_dirty");
                    well?.ModCharges(-1);
                }
                
                
            }
            */
            var owner = __instance.owner;
            SE.Play("water_farm");
            owner.ModNum(-1);
            c.Say("drawWater", owner.Duplicate(1), t);
            c.Pick(t);
            UnityEngine.Debug.Log($"WQIP:Use/C[{c.NameSimple}]_t[{t.NameSimple}]_owner[{owner.NameSimple}]_mode[{epMode}]_biome[{biome}]");
            return false;
        }

        
    }
}
