using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using Mono.Unix.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimworldArchipelago.Client.Multiworld
{
    public static class MultiWorldSessionManager
    {
        public static ArchipelagoSession Session { get; private set; }

        public static LoginResult Connect(string address, string playerSlot, string password)
        {
            if (address.Contains(':'))
            {
                Session = ArchipelagoSessionFactory.CreateSession(new Uri($"ws://{address}"));
            }
            else
            {
                Session = ArchipelagoSessionFactory.CreateSession(address);
            }

            try
            {
                return Session.TryConnectAndLogin("RimWorld", playerSlot, ItemsHandlingFlags.AllItems, password: password);

            }
            catch (Exception e)
            {
                return new LoginFailure(e.GetBaseException().Message);
            }
        }

        // Note, unlike other state consistency concerns there's probably little point
        // in creating a system to handle if connection is dropped on win.
        // Best for them to reload and trigger complete again.
        public static void SendGoalCompletePacket()
        {
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            Session.Socket.SendPacket(statusUpdatePacket);
        }
    }
}
