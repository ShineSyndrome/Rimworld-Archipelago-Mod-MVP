using Verse;

namespace RimworldArchipelago.Client.Rimworld
{
    public class ComponentStateManager
    {

        private static ArchipelagoWorldComponent ArchiWorldComponent => Find.World?.GetComponent<ArchipelagoWorldComponent>();

        public string GetMultiworldMarker()
        {
            return ArchiWorldComponent.multiWorldPlayerStamp;
        }

        public void UpdateMultiworldMarker(string player)
        {
            ArchiWorldComponent.multiWorldPlayerStamp = player;
        }
        //public void ReceiveItem(long archipelagoItemId)
        //{
        //    bool readyToRecieve = Find.AnyPlayerHomeMap != null && archiWorldComponent != null;

        //    if (!readyToRecieve)
        //    {
        //        ItemsAwaitingReceipt.Add(archipelagoItemId);
        //        Log.Message($"Could not yet receive Archipelago item {archipelagoItemId}");
        //    }
        //    else
        //    {
        //        AddItemToWorldState(archipelagoItemId);
        //    }
        //}

        //public void AddItemToWorldState(long archipelagoItemId)
        //{
        //    if (ComponentStateManager.ItemsAwaitingReceipt.Contains(archipelagoItemId))
        //    {
        //        ComponentStateManager.ItemsAwaitingReceipt.Remove(archipelagoItemId);
        //    }

        //    archiWorldComponent.ReceivedItems.Add(archipelagoItemId);

        //    if (Main.Instance.ArchipeligoItemIdToRimWorldDef.ContainsKey(archipelagoItemId))
        //    {
        //        var defMapping = Main.Instance.ArchipeligoItemIdToRimWorldDef[archipelagoItemId];
        //        var defName = defMapping.DefName;
        //        var defType = defMapping.DefType;

        //        // TODO something other than ResearchProjectDef
        //        if (defType == "ResearchProjectDef")
        //        {
        //            var def = DefDatabase<ResearchProjectDef>.GetNamed(defName, true);
        //            Find.ResearchManager.FinishProject(def);
        //        }
        //        else
        //        {
        //            Log.Error($"Unrecognized RimWorld DefType {defType} associated with Archipelago item id {archipelagoItemId}");
        //        }
        //    }
        //    else
        //    {
        //        Log.Error($"Could not find RimWorld Def associated with Archipelago item id {archipelagoItemId}");
        //    }
        //}

        //public static void Reset()
        //{
        //    ItemsAwaitingReceipt.Clear();
        //    archiWorldComponent.ReceivedItems.Clear();
        //}
    }
}
