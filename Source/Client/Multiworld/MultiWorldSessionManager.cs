using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
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
        public List<HydratedLocation> AllHydratedLocations { get; private set; }
        public Dictionary<long, string> Players { get; private set; }
        public string Seed { get; private set; }

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

            LoginResult loginResult;

            try
            {
                loginResult = Session.TryConnectAndLogin("RimWorld", playerSlot, ItemsHandlingFlags.AllItems, password: password);
            }
            catch (Exception e)
            {
                return new LoginFailure(e.GetBaseException().Message);
            }

            return loginResult;
        }

        public void InitialiseFromConnection(string playerSlot)
        {
            CurrentPlayerId = Session.Players.AllPlayers.Single(x => x.Name == playerSlot).Slot;
            SlotData = Session.DataStorage.GetSlotData(CurrentPlayerId);

            var allLocationIds = Session.Locations.AllLocations.ToArray();
            Task.Run(async () => await InitialLocationsScout(allLocationIds));

            Seed = Session.RoomState.Seed;
        }

        public int GetSlotDataInteger(string key)
        {
            if (SlotData.TryGetValue(key, out var slotValue))
            {
                return Convert.ToInt32(slotValue);
            }

            throw new ArgumentException($"Key not found in slot data {key}");
        }

        private async Task InitialLocationsScout(long[] allLocationIds)
        {
            AllHydratedLocations = (await Session.Locations.ScoutLocationsAsync(false, allLocationIds)).Locations
                     .Select(l => new HydratedLocation
                     {
                         NetworkItem = l,
                         SelfItem = l.Player == CurrentPlayerId,
                         ItemName = Session.Items.GetItemName(l.Item),
                         PlayerAlias = Session.Players.GetPlayerAlias(l.Player)
                     }).ToList();
        }

        public void SendGoalCompletePacket()
        {
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            Session.Socket.SendPacket(statusUpdatePacket);
        }
    }
}
