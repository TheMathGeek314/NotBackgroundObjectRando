using System.Collections.Generic;

namespace NotBackgroundObjectRando {
    public class ManualLogicJson {
        public static Dictionary<string, string> AllClauses = new();

        public string location;
        public string logic;

        public void store() {
            AllClauses.Add(location, logic);
        }
    }
}
