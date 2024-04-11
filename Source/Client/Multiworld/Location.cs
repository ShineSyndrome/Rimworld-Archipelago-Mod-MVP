using Archipelago.MultiClient.Net.Models;

namespace RimworldArchipelago.Client.Multiworld
{
    public class Location
    {
        public NetworkItem NetworkItem { get; set; }
        public bool SelfItem;
        public bool IsResearchLocation => NetworkItem.Location >= 11_000 && NetworkItem.Location < 12_000;
    }
}
