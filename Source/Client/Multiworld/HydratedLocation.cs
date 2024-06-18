using Archipelago.MultiClient.Net.Models;
using RimworldArchipelago.Client.Data;
using System;

namespace RimworldArchipelago.Client.Multiworld
{
    public class HydratedLocation
    {
        public NetworkItem NetworkItem { get; set; }
        public bool SelfItem { get; set; }
        public string ItemName { get; set; }
        public string PlayerAlias { get; set; }
        public bool IsResearchLocation => NetworkItem.Location >= 11_000 && NetworkItem.Location < 12_000;
        public string ItemLabel => SelfItem ? $"Your {ItemName}" : $"{PlayerAlias}'s {ItemName}";
        public string LocationName { get { return GetLocationName(); } }

        private string GetLocationName()
        {
            if (IsResearchLocation)
            {
                return ResearchLocations.AllResearchLocations[NetworkItem.Location];
            }

            throw new InvalidOperationException("Item number not handled.");
        }
    }
}
