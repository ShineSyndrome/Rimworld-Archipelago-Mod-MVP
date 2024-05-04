using Archipelago.MultiClient.Net.Models;

namespace RimworldArchipelago.Client.Multiworld
{
    public class HydratedLocation
    {
        public NetworkItem NetworkItem { get; set; }
        public bool SelfItem { get; set; }
        public string ItemName { get; set; }
        public string PlayerAlias { get; set; }
        public bool IsResearchLocation => NetworkItem.Location >= 11_000 && NetworkItem.Location < 12_000;
        public string ItemLabel => $"{PlayerAlias}'s {ItemName}";
    }
}
