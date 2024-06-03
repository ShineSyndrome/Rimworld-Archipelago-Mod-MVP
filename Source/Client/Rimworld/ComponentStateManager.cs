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

        public string GetMWSeed()
        {
            return ArchiWorldComponent.mwSeed;
        }

        public void UpdateMultiworldMarker(string player, string seed)
        {
            ArchiWorldComponent.multiWorldPlayerStamp = player;
            ArchiWorldComponent.mwSeed = seed;
        }
    }
}
