using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace s649_OsuiGet
{
    [HarmonyPatch]
    internal class TraitEmptyPotionPatch
    {
        internal static bool IsOnGlobalMap()
        {
            return EClass.pc.currentZone.id == "ntyris";
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitPotionEmpty), "OnUse")]
        internal static bool OnUsePrePatch(Chara c, Point p, TraitPotionEmpty __instance, ref bool __result) 
        {
            TraitWell well = __instance.GetWell(p);
            string biome = EClass.pc.currentZone.biome.id.ToString();
            Thing t;
            var owner = __instance.owner;
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
                SE.Play("water_farm");
                owner.ModNum(-1);
                c.Say("drawWater", owner.Duplicate(1), t);
                c.Pick(t);
                return false;
            }
                
            return true;
        }

        
    }
}
