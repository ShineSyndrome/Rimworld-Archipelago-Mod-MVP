using RimWorld;
using System;
using Verse;

namespace RimworldArchipelago.Client.Rimworld
{
    public class ArchipelagoGameComponent : GameComponent
    {
        private Main Main { get; } = Main.Instance;
        private Game Game { get; }

        public ArchipelagoGameComponent(Game game) : base()
        {
            Game = game;
        }

        public override void StartedNewGame()
        {
            if (Main.Connected)
            {
                Main.UpdatePlayerStamp();
                Main.LoadMultiworld();
            }
        }

        public override void LoadedGame()
        {
            if (!string.IsNullOrWhiteSpace(Main.GetPlayerStamp))
            {
                if (!Main.Connected)
                {
                    throw new InvalidOperationException("Loaded MW game before connecting! Do not save. Return to the main menu, and connect before loading the game again.");
                }

                var LoadedGameSeed = Main.GetMWSeedStamp;

                if (LoadedGameSeed != Main.Session.RoomState.Seed)
                {
                    throw new InvalidOperationException("Loaded multiworld save belonging to a different MW seed than current connection! Quit without saving, and try again.");
                }

                if (Main.GetPlayerStamp != Main.PlayerSlot)
                {
                    throw new InvalidOperationException("Loaded multiworld save belonging to a different MW slot than current connection! Quit without saving, and try again.");
                }

                Main.LoadMultiworld();
            }

            base.LoadedGame();
        }
    }
}
