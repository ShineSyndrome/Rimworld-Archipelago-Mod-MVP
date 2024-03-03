using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
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
    }
}
