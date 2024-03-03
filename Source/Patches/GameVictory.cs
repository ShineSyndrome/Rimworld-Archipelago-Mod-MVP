using HarmonyLib;
using RimWorld;
using RimworldArchipelago.Client;


namespace RimworldArchipelago.Patches
{
    [HarmonyPatch(typeof(GameVictoryUtility), nameof(GameVictoryUtility.MakeEndCredits))]
    public static class MakeEndCredits
    {
        public static void Postfix()
        {
            Main.Instance.TriggerGoalComplete();
        }
    }
}
