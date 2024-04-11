using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RimworldArchipelago.Client.Multiworld
{
    public class MultiWorldSessionManager
    {
        public ArchipelagoSession Session { get; private set; }
        public int CurrentPlayerId { get; private set; }
        public Dictionary<string, object> SlotData { get; private set; }
        public IEnumerable<Location> AllLocations { get; private set; }

        public LoginResult Connect(string address, string playerSlot, string password)
        {
            if (address.Contains(':'))
            {
                Session = ArchipelagoSessionFactory.CreateSession(new Uri($"ws://{address}"));
            }
            else
            {
                Session = ArchipelagoSessionFactory.CreateSession(address);
            }

            LoginResult loginResult = null;

            try
            {
                loginResult = Session.TryConnectAndLogin("RimWorld", playerSlot, ItemsHandlingFlags.AllItems, password: password);
            }
            catch (Exception e)
            {
                return new LoginFailure(e.GetBaseException().Message);
            }

            CurrentPlayerId = Session.Players.AllPlayers.Single(x => x.Name == playerSlot).Slot;
            SlotData = Session.DataStorage.GetSlotData(CurrentPlayerId);

            var allLocationIds = Session.Locations.AllLocations.ToArray();
            Task.Run(async () => await InitialLocationsScout(allLocationIds));

            return loginResult;
        }

        private async Task InitialLocationsScout(long[] allLocationIds)
        {
            AllLocations = (await Session.Locations.ScoutLocationsAsync(false, allLocationIds)).Locations
                     .Select(l => new Location { NetworkItem = l, SelfItem = l.Player == CurrentPlayerId });
        }

        public void SendGoalCompletePacket()
        {
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            Session.Socket.SendPacket(statusUpdatePacket);
        }
    }
}
