using EverySceneUtil;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NotBackgroundObjectRando {
    public class NotBackgroundObjectRando: Mod, IGlobalSettings<GlobalSettings> {
        new public string GetName() => "NotBackgroundObjectRando";
        public override string GetVersion() => "1.0.0.0";

        public static GlobalSettings globalSettings { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        internal static NotBackgroundObjectRando instance;

        public NotBackgroundObjectRando(): base(null) {
            instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            RandoInterop.Hook();
            ModHooks.HeroUpdateHook += heroUpdate;
        }

        private void heroUpdate() {
            if(Input.GetKeyDown(KeyCode.O)) 
                PerSceneTask(true);
            if(Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.Q)) {
                doEverySceneStuff();
            }
        }

        private async void doEverySceneStuff() {
            ESU_Params p = new() {
                AdditionalScenes = AdditionalSceneOptions.AlwaysLoadWithExtras,
                Infection = InfectionOptions.Both,
                LogSceneName = false,
                OnLoad = PerSceneTask1
            };
            backgroundObjectInformation.Clear();
            infectedBgObjInformation.Clear();
            await EverySceneUtil.EverySceneUtil.ForEveryScene(p, KeyCode.O);
            foreach(var data in backgroundObjectInformation) {
                Log("{" + $"\t\"scene\": \"{data.Item1}\",\"type\": \"{data.Item2}\",\"name\": \"{data.Item3}\",\"x\":{data.Item4},\"y\": {data.Item5},\"requiresInfection\": false" + "},");
            }
            Log("hi");
            foreach(var data in infectedBgObjInformation) {
                if(!backgroundObjectInformation.Contains(data)) {
                    Log("{" + $"\t\"scene\": \"{data.Item1}\",\"type\": \"{data.Item2}\",\"name\": \"{data.Item3}\",\"x\":{data.Item4},\"y\": {data.Item5},\"requiresInfection\": true" + "},");
                }
            }
        }

        FieldInfo gsbInteraction = typeof(GrassSpriteBehaviour).GetField("interaction", BindingFlags.Instance | BindingFlags.NonPublic);
        List<(string, string, string, float, float)> backgroundObjectInformation = new();
        List<(string, string, string, float, float)> infectedBgObjInformation = new();
        List<(string, string, string, float, float)> oobLocations = [
            //out of bounds
            ("Mines_32", "Breakable", "brk_Crystal1", 49.43232f, -12.88081f),
            ("Mines_04", "Breakable", "brk_Crystal1", 49.43232f, -12.88081f),
            ("Mines_16", "Breakable", "brk_Crystal1", 49.43232f, -12.88081f),
            ("Crossroads_16", "PlayMakerFSM", "Breakable Pole", 31.86311f, 3.178232f),
            ("Crossroads_16", "PlayMakerFSM", "Breakable Pole 1", 36.25311f, 3.088232f),
            ("Crossroads_16", "PlayMakerFSM", "Breakable Pole 2", 38.1431f, 3.268232f)
        ];
        private void PerSceneTask1() => PerSceneTask(false);
        private void PerSceneTask(bool isManual) {
            //bool verifyCollider(GameObject g) => g.TryGetComponent<Collider2D>(out var c) && c.enabled;
            foreach(Type t in Consts.types) {
            /*foreach(Type t in new Type[] { typeof(Breakable), typeof(BreakablePoleSimple), typeof(GrassSpriteBehaviour),
                                            typeof(GrassBehaviour), typeof(InfectedBurstLarge), typeof(BreakableInfectedVine),
                                            typeof(JellyEgg), typeof(PlayMakerFSM), typeof(TownGrass)}) {*/
            /*foreach((Type, Func<Component, bool>) tuple in new (Type, Func<Component, bool>)[] {
                (typeof(Breakable), b => verifyCollider(b.gameObject)),
                (typeof(BreakablePoleSimple), bps => verifyCollider(bps.gameObject)),
                (typeof(GrassSpriteBehaviour), gsp => verifyCollider(gsp.gameObject) && (bool)gsbInteraction.GetValue(gsp)),
                (typeof(GrassBehaviour), gb => verifyCollider(gb.gameObject)),
                (typeof(InfectedBurstLarge), ibl => verifyCollider(ibl.gameObject)),
                (typeof(BreakableInfectedVine), biv => verifyCollider(biv.gameObject)),
                (typeof(JellyEgg), je => !((JellyEgg)je).bomb),
                (typeof(PlayMakerFSM), fsm => ((PlayMakerFSM)fsm).FsmName == "Break Pole"),
                (typeof(TownGrass), tg => verifyCollider(tg.gameObject))
            }) {*/
                foreach(Component c in GameObject.FindObjectsOfType(t)) {
                    if(c.gameObject.GetComponent<PersistentBoolItem>() != null)
                        continue;
                    if(!c.gameObject.activeInHierarchy)
                        continue;
                    if(!c.gameObject.TryGetComponent<Collider2D>(out var col) || !col.enabled)
                        continue;
                    if(c is HealthManager && !c.gameObject.name.StartsWith("fluke_baby"))
                        continue;
                    if(c is GrassSpriteBehaviour gsb && !(bool)gsbInteraction.GetValue(gsb))
                        continue;
                    if(c is JellyEgg egg && egg.bomb)
                        continue;
                    if(c is PlayMakerFSM fsm && fsm.FsmName != "Break Pole")
                        continue;
                    if(c.gameObject.scene.name == "Abyss_18" && c.transform.parent != null && c.transform.parent.name.Contains("Windy_Set"))
                        continue;
                    if(c.gameObject.scene.name == "Abyss_19" && c.transform.parent != null) {
                        if(c.transform.parent.name == "Pre_Double_Jump")
                            continue;
                        if(c.transform.parent.parent != null && c.transform.parent.parent.name == "infected_door")
                            continue;
                    }
                    if(c.gameObject.scene.name == "Abyss_20" && c.transform.parent != null && c.transform.parent.name == "vines static")
                        continue;
                    if(c.gameObject.scene.name == "Crossroads_22" && c.transform.parent != null && c.transform.parent.parent != null) {
                        if(c.transform.parent.parent.name == "infected_door")
                            continue;
                    }
                    if(c.gameObject.scene.name == "Dream_Abyss" && c.transform.position.y > 230)
                        continue;

                    (string, string, string, float, float) data = (c.gameObject.scene.name, t.Name, c.gameObject.name, c.gameObject.transform.position.x, c.gameObject.transform.position.y);
                    if(oobLocations.Any(tup => tup.Item1 == data.Item1 && tup.Item2 == data.Item2 && tup.Item3 == data.Item3 && Mathf.Abs(tup.Item4 - data.Item4) < 0.01f && Mathf.Abs(tup.Item5 - data.Item5) < 0.01f))
                        continue;
                    if(isManual) {
                        //Log($"Found {data.Item2}:{data.Item3} in {data.Item1} at {data.Item4}/{data.Item5}");
                        Log($"\t{data.Item2}\t{recursiveParent(data.Item3, c.gameObject)}\t{data.Item1}\t{data.Item4}\t{data.Item5}\t{PlayerData.instance.hasDoubleJump}");
                        string recursiveParent(string name, GameObject gameObject) {
                            if(gameObject.transform.parent == null)
                                return name;
                            return recursiveParent($"{gameObject.transform.parent.name}/{name}", gameObject.transform.parent.gameObject);
                        }
                        //testing
                    }
                    else {
                        if(PlayerData.instance.crossroadsInfected) {
                            if(!infectedBgObjInformation.Contains(data))
                                infectedBgObjInformation.Add(data);
                        }
                        else {
                            if(!backgroundObjectInformation.Contains(data))
                                backgroundObjectInformation.Add(data);
                        }
                    }
                }
            }
        }
    }
}

//technically breakable objects that don't fit into a convenient formula
//fsm("Crossroads Sign Control") in c01
//Crossroads_27/Direction Pole Tram/"FSM"
// --- checked king's pass, crossroads, greenpath, canyon+archive, fungal,
//      deepnest but only from mlords to super secret