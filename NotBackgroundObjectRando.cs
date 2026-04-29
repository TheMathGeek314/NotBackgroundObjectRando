using Modding;
using System.Collections.Generic;
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
        }
    }
}

//technically breakable objects that don't fit into a convenient formula
//fsm("Crossroads Sign Control") in c01
//Crossroads_27/Direction Pole Tram/"FSM"

//Incomplete list of locations that dissapear with bench or stag rando
//BgObj-Fungus1_36_green_grass_1[1]_22,14_2,85 with "stone sactuarry" bench. 
//Something and a zote bench in city bridge,left of storerooms. 
//quirrel peak bench/stag.
