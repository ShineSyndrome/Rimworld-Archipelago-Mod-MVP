namespace RimworldArchipelago.Client.Extensions
{
    public static class LongExtensions
    {
        public static bool IsResearchLocation(this long locationId)
        { 
            return locationId >= 11_000 && locationId < 12_000;
        }

        public static bool IsCraftLocation(this long locationId)
        {
            return locationId >= 12_000 && locationId < 13_000;
        }

        public static bool IsPurchaseLocation(this long locationId)
        {
            return locationId >= 13_000 && locationId < 14_000;
        }
    }
}
