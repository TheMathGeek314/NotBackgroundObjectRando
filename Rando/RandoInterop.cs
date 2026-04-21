using Modding;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using RandomizerCore.Logic;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace NotBackgroundObjectRando{
    public static class RandoInterop {
        public static void Hook() {
            RandoMenuPage.Hook();
            RequestModifier.Hook();
            LogicAdder.Hook();

            RandoController.OnExportCompleted += EditModules;
            SettingsLog.AfterLogSettings += LogRandoSettings;

            DefineLocations();
            DefineItems();

            if(ModHooks.GetMod("RandoSettingsManager") is Mod) {
                RSMInterop.Hook();
            }
        }

        private static void EditModules(RandoController controller) {
            if(!NotBackgroundObjectRando.globalSettings.Enabled || !NotBackgroundObjectRando.globalSettings.LockBehindItems)
                return;
            ItemChangerMod.Modules.GetOrAdd<NborModule>();
            ProgressionInitializer.OnCreateProgressionInitializer += DisablePogos;
        }

        private static void LogRandoSettings(LogArguments args, TextWriter w) {
            w.WriteLine("Logging NotBackgroundObjectRando settings:");
            w.WriteLine(JsonUtil.Serialize(NotBackgroundObjectRando.globalSettings));
        }

        private static void DefineLocations() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("BackgroundData.json"));
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            foreach(BackgroundObjectJson boj in new ParseJson(stream).parseFile<BackgroundObjectJson>()) {
                boj.store();
                AutoLocation bgLoc = boj.type switch {
                    nameof(BreakableInfectedVine) => new BreakableInfectedVineLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(Breakable) => new BreakableLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(BreakablePoleSimple) => new BreakablePoleSimpleLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(HealthManager) => new FlukeBabyLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(GrassBehaviour) => new GrassBehaviourLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(GrassSpriteBehaviour) => new GrassSpriteBehaviourLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(InfectedBurstLarge) => new InfectedBurstLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(JellyEgg) => new JellyEggLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(PlayMakerFSM) => new FsmPoleLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    nameof(TownGrass) => new TownGrassLocation() { name = GetPlacementName(boj), sceneName = boj.scene },
                    _ => null
                };
                InteropTag tag = AddTag(bgLoc);
                tag.Properties["PinSprite"] = new EmbeddedSprite("pin_barrel");
                tag.Properties["WorldMapLocation"] = (boj.scene.Replace("_boss_defeated",""), boj.x, boj.y);
                Finder.DefineCustomLocation(bgLoc);
            }
        }

        private static void DefineItems() {
            foreach(BackgroundObjectJson bgObj in BackgroundObjectJson.allObjects) {
                string icName = GetPlacementName(bgObj);
                BackgroundObjectItem bgItem = new(icName);
                Finder.DefineCustomItem(bgItem);
            }
        }

        public static string GetPlacementName(GameObject gameObject) {
            return $"{Consts.prefix}{gameObject.scene.name}_{Clean(gameObject.name)}_{gameObject.transform.position.x}_{gameObject.transform.position.y}";
        }

        public static string GetPlacementName(BackgroundObjectJson bgObj) {
            return $"{Consts.prefix}{bgObj.scene}_{Clean(bgObj.name)}_{bgObj.x}_{bgObj.y}";
        }

        public static string DisplayName(string name) {
            string trimmed = name.Substring(Consts.prefix.Length, name.LastIndexOf('_') - Consts.prefix.Length);
            return trimmed.Substring(0, trimmed.LastIndexOf('_'));
        }

        public static string Clean(string input) {
            return input.Replace(" ", "_").Replace("(", "[").Replace(")", "]");
        }

        public static InteropTag AddTag(TaggableObject obj) {
            InteropTag tag = obj.GetOrAddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = NotBackgroundObjectRando.instance.GetName();
            return tag;
        }

        private static void DisablePogos(LogicManager lm, GenerationSettings gs, ProgressionInitializer pi) {
            if(!NotBackgroundObjectRando.globalSettings.Enabled || !NotBackgroundObjectRando.globalSettings.LockBehindItems)
                return;
            pi.Setters.Add(new(lm.GetTerm("BACKGROUNDPOGOS"), 0));
            pi.Setters.Add(new(lm.GetTerm("INFECTIONSKIPS"), 0));
        }
    }
}
