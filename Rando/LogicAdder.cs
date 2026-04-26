using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ItemChanger;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace NotBackgroundObjectRando {
    public static class LogicAdder {
        public static void Hook() {
            RCData.RuntimeLogicOverride.Subscribe(50, ApplyLogic);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            if(!NotBackgroundObjectRando.globalSettings.Enabled)
                return;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("ManualLogic.json"));
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            ManualLogicJson.AllClauses.Clear();
            foreach(ManualLogicJson manlog in new ParseJson(stream).parseFile<ManualLogicJson>()) {
                manlog.store();
            }
            JsonLogicFormat fmt = new();
            using Stream wp = assembly.GetManifestResourceStream(
                assembly.GetManifestResourceNames().Single(str => str.EndsWith("waypoints.json"))
            );
            lmb.DeserializeFile(LogicFileType.Waypoints, fmt, wp);
            foreach(BackgroundObjectJson bgObj in BackgroundObjectJson.allObjects) {
                string icName = RandoInterop.GetPlacementName(bgObj);
                Term term = lmb.GetOrAddTerm(icName, TermType.Int);
                lmb.AddItem(new SingleItem(icName, new TermValue(term, 1)));
                string clause = "";
                if(ManualLogicJson.AllClauses.TryGetValue(icName, out string logic)) {
                    clause = $"({logic})";
                }
                else {
                    string myScene = bgObj.scene == SceneNames.Crossroads_10_boss_defeated ? SceneNames.Crossroads_10 : bgObj.scene;
                    foreach(string t in lmb.Transitions.Where(transition => transition.StartsWith(myScene + "["))) {
                        clause += t + " | ";
                    }
                    clause = informUnusualSceneLogic(clause, myScene);
                    if(bgObj.requiresInfection)
                        clause += " + DREAMER";
                }
                if(NotBackgroundObjectRando.globalSettings.LockBehindItems)
                    clause += " + " + icName;
                lmb.AddLogicDef(new(icName, clause));
            }
        }

        private static string informUnusualSceneLogic(string clause, string scene) {
            switch(scene) {
                case SceneNames.Dream_01_False_Knight:
                    return "Defeated_False_Knight + DREAMNAIL";
                case SceneNames.Dream_02_Mage_Lord:
                    return "Defeated_Soul_Master + DREAMNAIL";
                case SceneNames.Dream_03_Infected_Knight:
                    return "Defeated_Broken_Vessel + DREAMNAIL";
                case SceneNames.Dream_Abyss:
                    return "*Void_Heart";
                case SceneNames.Dream_Backer_Shrine:
                case SceneNames.Dream_Room_Believer_Shrine:
                    return "*King's_Idol-Glade_of_Hope + DREAMNAIL";
                case SceneNames.Dream_Nailcollection:
                    return "*Dream_Nail";
                case SceneNames.GG_Blue_Room:
                    return "GG_Blue_Room[left1]?GG_Atrium";
                case SceneNames.GG_Broken_Vessel:
                    return "GG_Workshop + Defeated_Broken_Vessel";
                case SceneNames.GG_Crystal_Guardian:
                    return "GG_Workshop + Defeated_Crystal_Guardian";
                case SceneNames.GG_Ghost_Galien:
                    return "GG_Workshop + Defeated_Galien";
                case SceneNames.GG_Ghost_Gorb:
                    return "GG_Workshop + Defeated_Gorb";
                case SceneNames.GG_Ghost_Hu:
                    return "GG_Workshop + Defeated_Elder_Hu";
                case SceneNames.GG_Ghost_Marmu:
                case SceneNames.GG_Ghost_Marmu_V:
                    return "GG_Workshop + Defeated_Marmu";
                case SceneNames.GG_Ghost_Xero:
                    return "GG_Workshop + Defeated_Xero";
                case SceneNames.GG_Hive_Knight:
                    return "GG_Workshop + Defeated_Hive_Knight";
                case SceneNames.GG_Hornet_2:
                    return "GG_Workshop + Defeated_Hornet_2";
                case SceneNames.GG_Lost_Kin:
                    return "GG_Workshop + Defeated_Lost_Kin";
                case "GG_Mage_Knight":
                case SceneNames.GG_Mage_Knight_V:
                    return "GG_Workshop + (Defeated_Sanctum_Warrior | Defeated_Elegant_Warrior)";
                case SceneNames.GG_Mega_Moss_Charger:
                    return "GG_Workshop + *Boss_Geo-Massive_Moss_Charger";
                case "GG_Mighty_Zote":
                    return "GG_Workshop + RIGHTCLAW + WINGS + (LEFTSUPERDASH | LEFTDASH) + Wall-Godhome_Workshop?TRUE + Wall-Eternal_Ordeal?TRUE + UPWALLBREAK";
                case SceneNames.GG_Nosk:
                case SceneNames.GG_Nosk_V:
                    return "GG_Workshop + Defeated_Nosk";
                case SceneNames.GG_Sly:
                    return "GG_Atrium + ((Defeated_Hive_Knight + Defeated_Elder_Hu + Defeated_Collector + Defeated_Colosseum_2 + Defeated_Grimm + Defeated_Galien + Defeated_Uumuu + Defeated_Hornet_2) | (PANTHEON_KEY_3 ? FALSE) + COMBAT[Pantheon_3])";
                case SceneNames.GG_Soul_Master:
                    return "GG_Workshop + Defeated_Soul_Master";
                case SceneNames.GG_Soul_Tyrant:
                    return "GG_Workshop + Defeated_Soul_Tyrant";
                case SceneNames.GG_Traitor_Lord:
                    return "GG_Workshop + Defeated_Traitor_Lord";
                case SceneNames.Grimm_Nightmare:
                    return "Defeated_Grimm + Grimm_Main_Tent[left1] + Grimmchild + FLAMES>8 + DREAMNAIL";
                case SceneNames.Room_Colosseum_Bronze:
                    return "Room_Colosseum_01[left1] + Can_Replenish_Geo + (READ?TRUE) + (COLO_KEY_1?TRUE)";
                case SceneNames.Room_Colosseum_Gold:
                    return "Room_Colosseum_01[left1] + Can_Replenish_Geo + (READ?TRUE) + (COLO_KEY_3?TRUE)";
                case SceneNames.Room_Colosseum_Silver:
                    return "Room_Colosseum_01[left1] + Can_Replenish_Geo + (READ?TRUE) + (COLO_KEY_2?TRUE)";
                case SceneNames.Room_Final_Boss_Atrium:
                case SceneNames.Room_Final_Boss_Core:
                    return "Opened_Black_Egg_Temple";
                case SceneNames.Room_Jinn:
                    return "Room_Ouiji[left1]";
                case SceneNames.Room_Sly_Storeroom:
                    return "*Nailmaster's_Glory";
                case SceneNames.Room_Tram:
                    return "Lower_Tram";
                case SceneNames.Room_Tram_RG:
                    return "Upper_Tram";
                default:
                    try {
                        return $"({clause.Substring(0, clause.Length - 3)})";
                    }
                    catch(Exception) {
                        Modding.Logger.Log($"[NotBreakableObjectRando] - Found no transitions in {scene}");
                        return "TRUE";
                    }
            }
        }
    }
}
