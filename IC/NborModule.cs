using System;
using UnityEngine;
using ItemChanger.Internal;
using ItemChanger.Modules;
using UnityEngine.SceneManagement;
using ItemChanger;

namespace NotBackgroundObjectRando {
    public class NborModule: Module {
        public override void Initialize() {
            On.GameManager.OnNextLevelReady += editScene;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += removeThatOneBubble;
        }

        public override void Unload() {
            On.GameManager.OnNextLevelReady -= editScene;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= removeThatOneBubble;
        }

        private void editScene(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            //this check is not needed as the Module is only conditionally added to itemchanger
            //if(NotBackgroundObjectRando.globalSettings.Enabled && NotBackgroundObjectRando.globalSettings.LockBehindItems) {
                foreach(Type t in Consts.types) {
                    foreach(Component c in GameObject.FindObjectsOfType(t)) {
                        string placementName = RandoInterop.GetPlacementName(c.gameObject);
                        if(Ref.Settings.Placements.ContainsKey(placementName) && RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(placementName) == 0) {
                            c.gameObject.SetActive(false);
                        }
                    }
                }
            //}
        }

        private void removeThatOneBubble(Scene arg0, Scene arg1) {
            if(arg1.name == SceneNames.Fungus3_26) {
                string placementName = "BgObj-Fungus3_26_Jelly_Egg_Empty_[13]_2.300436_8.709916";
                if(Ref.Settings.Placements.ContainsKey(placementName) && RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(placementName) == 0) {
                    GameObject.Find("Jelly Egg Empty (13)").SetActive(false);
                }
            }
        }
    }
}
