using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
namespace s649_OsuiGet
{
    [HarmonyPatch]
    internal class TraitWellPatch
    {
        static bool WellPatchEnable => MainPlugin.ce_DrinkWellTweak.Value;
        static WishMode WishMode => MainPlugin.ce_WellWishMode.Value;
        static bool ForceWish => WishMode == WishMode.ForceWish && !EClass.player.wellWished;
        static PollutedWellMode PollutedWellMode => MainPlugin.ce_PollutedWellMode.Value;
        static bool EnableWishForPlayerStolenFish => MainPlugin.ce_EnableWishForStolenFish.Value;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitWell), "TrySetAct")]
        internal static bool TrySetActPrePatch(TraitWell __instance, ActPlan p)
        {
            if (!WellPatchEnable) return true;
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

                var effectHappen = 0;
                //var effectPotential = false;
                //var effectMutation = false;
                //var effectEvent = false;
                if (well.IsHoly)
                {
                    /*
                     if (EClass.rnd(2) == 0)
                        ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, (!well.polluted) ? 500 : (-500));
                    else
                        ActEffect.Proc(EffectId.Mutation, 100, !well.polluted ? BlessedState.Blessed : BlessedState.Cursed, EClass.pc);
                     */
                    Debug.Log($"WQIP:HolyWell");
                    ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, (!well.polluted) ? 100 : (-100));
                    ActEffect.Proc(EffectId.Mutation, 100, !well.polluted ? BlessedState.Blessed : BlessedState.Cursed, EClass.pc);


                }
                else if (well.polluted)//osen ido
                {
                    Debug.Log($"WQIP:Well_Polluted/Mode[{PollutedWellMode}]");
                    switch (PollutedWellMode) 
                    {
                        case PollutedWellMode.Modded:
                            ModdedPoluttedWell();
                            break;
                        case PollutedWellMode.ForceEffect:
                            ModdedPoluttedWell(true);
                            break;
                        default:
                            VanillaPollutedWell();
                            break;
                    }
                    
                }
                else //normal ido
                {
                    Debug.Log($"WQIP:mode[{WishMode}]_force[{ForceWish}]_wished[{EClass.player.wellWished}]_wishhelp[{EnableWishForPlayerStolenFish}]");
                    if (ForceWish)
                    {
                        WellWishEvent();
                    } 
                    else 
                    {
                        if (EClass.rnd(5) == 0)//元々の潜在能力上昇判定
                        {
                            effectHappen++;
                        }
                        if (EClass.rnd(25) < 4)//基本のBadEffect判定
                        {
                            effectHappen++;
                        }
                        if (EClass.rnd(100) < 16)//元々のMutation判定
                        {
                            effectHappen++;
                        }
                        //luckycoin event
                        
                        if (effectHappen > 0)
                        {
                            RewardOren(effectHappen);

                        }
                        else
                        {
                            WellWishEvent();
                        }
                    }
                }
            /*
            else if (well.polluted && EClass.rnd(3) == 0)
            {
                TraitWell.BadEffect(EClass.pc);
                ActEffect.Proc(EffectId.Mutation, EClass.pc);
            }
            
            WellEventProceed:
                if (well.polluted)
                {
                    EClass.pc.Say("drinkWater_dirty");
                    TraitWell.BadEffect(EClass.pc);
                }
                else
                {
                    EClass.pc.Say("drinkWater_clear");
                }
            WellEventDone:
            */
                well.ModCharges(-1);
                return true;
            }, __instance.owner);
            return false;
        }
        static string RewardGatya()
        {
            string result = "";
            if (EClass.rnd(2) == 0)
            {
                result = "money";
                return result;
            }
            if (EClass.rnd(5) == 0)
            {
                result = "casino_coin";
                return result;
            }
            if (EClass.rnd(5) == 0)
            {
                result = "scratchcard";
                return result;
            }
            if (EClass.rnd(5) == 0)
            {
                result = "plat";
                return result;
            }
            if (EClass.rnd(5) == 0)
            {
                result = "money2";
                return result;
            }
            if (EClass.rnd(10) == 0 || EClass.debug.enable)
            {
                result = "medal";
                return result;
            }
            return result;
        }
        static int RewardValue(int baseV, int num) 
        {
            int sum = 0;
            for (int i = 0; i < num; i++) 
            {
                sum += EClass.rnd(baseV) + 1;
            }
            return sum;
        }
        static void WellWishEvent() 
        {
            bool wishhelper = EnableWishForPlayerStolenFish
                && !EClass.player.wellWished
                && EClass.player.CountKeyItem("well_wish") == 0
                && EClass.player.flags.fishStolen > 0;
            Debug.Log($"WQIP:WW/Stl[{EClass.player.flags.fishStolen}]_KeyItem[{EClass.player.CountKeyItem("well_wish")}]_flag[{EClass.player.wellWished}]_Deb[{EClass.debug.enable}]");
            if (EClass.rnd(EClass.debug.enable || WishMode == WishMode.Modded ? 2 : 10) == 0 && !EClass.player.wellWished)
            {
                if (EClass.player.CountKeyItem("well_wish") > 0)
                {
                    Debug.Log($"WQIP:WW/VanillaWish");
                    EClass.player.ModKeyItem("well_wish", -1);
                    ActEffect.Proc(EffectId.Wish, EClass.pc, null, 50 + EClass.player.CountKeyItem("well_enhance") * 50 + EClass.player.flags.fishStolen * 50);
                    EClass.player.wellWished = true;
                }
                else if (wishhelper)
                {
                    Debug.Log($"WQIP:WW/helpWish");
                    //EClass.player.ModKeyItem("well_wish", -1);
                    ActEffect.Proc(EffectId.Wish, EClass.pc, null, 50 + EClass.player.CountKeyItem("well_enhance") * 50 + EClass.player.flags.fishStolen * 50);
                    EClass.player.wellWished = true;
                    EClass.player.flags.fishStolen--;
                } 
                else
                {
                    Msg.SayNothingHappen();
                }
            }
            else
            {
                EClass.pc.Say("drinkWater_clear");
            }
        }
        static void RewardOren(int happen)
        {
            string reward = "";
            int rewardNum = 1;
            int pLV = EClass.pc.LV;
            int num = happen;// EClass.rnd(happen) + 1;
            for (int i = 0; i < num; i++)
            {
                reward = RewardGatya();
            }
            switch (reward)
            {
                case "casino_coin":
                    rewardNum = RewardValue(1 * (pLV / 5 + 1), num);
                    break;
                case "scratchcard":
                    rewardNum = num;
                    break;
                case "money":
                    rewardNum = RewardValue(1 * (pLV + 1), num);
                    break;
                case "plat":
                    rewardNum = RewardValue(1 * (pLV / 5 + 1), num);
                    break;
                case "money2":
                    rewardNum = RewardValue(1 * (pLV / 10 + 1), num);
                    break;
                case "medal":
                    //rewardNum = RewardValue(10, effectHappen);
                    break;
                default:
                    break;
            }
            if (reward != "")
            {
                //Msg.SetColor(Color.);
                Card thing = ThingGen.Create(reward, -1, -1).SetNum(rewardNum).SetHidden(false);
                string text = Lang.isJP
                    ? $"{EClass.pc.NameSimple}は井戸の中から{thing.Name}を見つけた。"
                    : $"{EClass.pc.NameSimple} found {thing.Name} in the well.";
                Msg.SayRaw(text);
                EClass.pc.PlaySound("medal", 1f, true);
                EClass._zone.AddCard(thing, EClass.pc.pos);
            }
        }
        static void VanillaPollutedWell() 
        {
            if (EClass.rnd(5) == 0)
            {
                ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, EClass.rnd(2) == 0 ? 100 : -100);
            }
            else if (EClass.rnd(5) == 0)
            {
                TraitWell.BadEffect(EClass.pc);
            }
            else if (EClass.rnd(4) == 0)
            {
                ActEffect.Proc(EffectId.Mutation, EClass.pc);
            }
            else
            {
                EClass.pc.Say("drinkWater_dirty");
                TraitWell.BadEffect(EClass.pc);
            }
            
        }
        static void ModdedPoluttedWell(bool force = false)
        {
            
            int effectHappen = 0;
            if (EClass.rnd(2) == 0 || force)
            {
                effectHappen++;
                ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, EClass.rnd(2) == 0 ? 100 : -100);
            }
            
            if (EClass.rnd(2) == 0 || force)//
            {
                //effectHappen++;//effectEvent = true;
                TraitWell.BadEffect(EClass.pc);
            }
            
            if (EClass.rnd(2) == 0 || force)//
            {
                effectHappen++;
                ActEffect.Proc(EffectId.Mutation, 100, BlessedState.Normal, EClass.pc);
            }
            if (effectHappen == 0)
            {
                EClass.pc.Say("drinkWater_dirty");
                //TraitWell.BadEffect(EClass.pc);
            }
        }
    }
    
}
