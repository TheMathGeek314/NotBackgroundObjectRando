using ItemChanger;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace NotBackgroundObjectRando {
    public class RequestModifier {
        public static void Hook() {
            RequestBuilder.OnUpdate.Subscribe(0, ApplyBackgroundObjectDefs);
            RequestBuilder.OnUpdate.Subscribe(-499, SetupItems);
            RequestBuilder.OnUpdate.Subscribe(-499.5f, DefinePools);
        }

        private static void ApplyBackgroundObjectDefs(RequestBuilder rb) {
            if(!NotBackgroundObjectRando.globalSettings.Enabled)
                return;
            foreach(BackgroundObjectJson bgObj in BackgroundObjectJson.allObjects) {
                string icName = RandoInterop.GetPlacementName(bgObj);
                rb.AddLocationByName(icName);
                rb.EditLocationRequest(icName, info => {
                    info.customPlacementFetch = (factory, placement) => {
                        if(factory.TryFetchPlacement(icName, out AbstractPlacement ap1))
                            return ap1;
                        AbstractLocation absLoc = Finder.GetLocation(icName);
                        absLoc.flingType = FlingType.Everywhere;
                        AbstractPlacement ap = absLoc.Wrap();
                        factory.AddPlacement(ap);
                        return ap;
                    };
                    info.getLocationDef = () => new() {
                        Name = icName,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false,
                        SceneName = bgObj.scene
                    };
                });
            }
        }

        private static void SetupItems(RequestBuilder rb) {
            if(!NotBackgroundObjectRando.globalSettings.Enabled || !NotBackgroundObjectRando.globalSettings.LockBehindItems)
                return;
            foreach(BackgroundObjectJson bgObj in BackgroundObjectJson.allObjects) {
                string icName = RandoInterop.GetPlacementName(bgObj);
                rb.EditItemRequest(icName, info => {
                    info.getItemDef = () => new ItemDef() {
                        Name = icName,
                        Pool = "BackgroundObjects",
                        MajorItem = false,
                        PriceCap = 1
                    };
                });
                rb.AddItemByName(icName);
            }
        }

        private static void DefinePools(RequestBuilder rb) {
            GlobalSettings gs = NotBackgroundObjectRando.globalSettings;
            if(!gs.Enabled)
                return;
            ItemGroupBuilder group = null;
            string label = RBConsts.SplitGroupPrefix + "BackgroundObjects";
            foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups()) {
                if(igb.label == label) {
                    group = igb;
                    break;
                }
            }
            group ??= rb.MainItemStage.AddItemGroup(label);
            rb.OnGetGroupFor.Subscribe(0.01f, ResolveBgObjGroup);
            bool ResolveBgObjGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb) {
                gb = default;
                return false;
            }
        }
    }
}
