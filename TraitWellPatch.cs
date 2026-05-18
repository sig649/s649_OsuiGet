using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace s649_OsuiGet
{
    [HarmonyPatch]
    internal class TraitWellPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitWell), "TrySetAct")]
        internal static bool TrySetActPrePatch(TraitWell __instance, ActPlan p)
        {
            p.TrySetAct("actDrink", delegate
            {
                var well = __instance;
                if (well.Charges <= 0)
                {
                    EClass.pc.Say("drinkWell_empty", EClass.pc, well.owner);
                    return false;
                }
                EClass.pc.Say("drinkWell", EClass.pc, well.owner);
                EClass.pc.PlaySound("drink");
                EClass.pc.PlayAnime(AnimeID.Shiver);
                if(well.IsHoly) //if (__instance.IsHoly || EClass.rnd(5) == 0)
                {
                    //if (!well.polluted)
                    if (EClass.rnd(2) == 0)
                        ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, (!well.polluted) ? 500 : (-500));
                    else
                        ActEffect.Proc(EffectId.Mutation, 100, !well.polluted ? BlessedState.Blessed : BlessedState.Cursed, EClass.pc);
                }
                else if (well.polluted && EClass.rnd(3) == 0)
                {
                    TraitWell.BadEffect(EClass.pc);
                    ActEffect.Proc(EffectId.Mutation, EClass.pc);
                }
                else if (EClass.rnd(2) == 0 && !well.polluted && !EClass.player.wellWished)
                {
                    if (EClass.player.CountKeyItem("well_wish") > 0)
                    {
                        EClass.player.ModKeyItem("well_wish", -1);
                        //ActEffect.Proc(EffectId.Wish, EClass.pc, null, 50 + EClass.player.CountKeyItem("well_enhance") * 50);
                        ActEffect.Proc(EffectId.Wish, EClass.pc, null, 50 + EClass.player.CountKeyItem("well_enhance") * 50 + EClass.player.flags.fishStolen * 50);
                        EClass.player.wellWished = true;
                    }
                    else
                    {
                        Msg.SayNothingHappen();
                    }
                }
                else if (well.polluted)
                {
                    EClass.pc.Say("drinkWater_dirty");
                    TraitWell.BadEffect(EClass.pc);
                }
                else
                {
                    EClass.pc.Say("drinkWater_clear");
                }
                well.ModCharges(-1);
                return true;
            }, __instance.owner);
            return false;
        }
    }
}
