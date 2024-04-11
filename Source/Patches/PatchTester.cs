using HarmonyLib;
using RimWorld;
using Verse;
using RimworldArchipelago.Client;
using System.Threading.Tasks;

namespace RimworldArchipelago.Patches
{
    //[HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    //public static class LoadGame
    //{
    //    public static async Task Postfix()
    //    {
    //        await Task.Delay(1000);
    //    }
    //}

    //[HarmonyPatch(typeof(Game), nameof(Game.InitNewGame))]
    //public static class InitNewGame
    //{
    //    private static ArchipelagoWorldData worldData => Find.World?.GetComponent<ArchipelagoWorldData>();

    //    public static void Postfix()
    //    {
    //        if (worldData == null)
    //        {
    //            Main.Instance.LogMessage("Worlddata still null here");
    //            return;
    //        }

    //        Main.Instance.LogMessage($"Value of int is {worldData.TestInt}");
    //        worldData.TestInt++;
    //    }
    //}
}
