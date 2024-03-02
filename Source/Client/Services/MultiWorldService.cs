using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static RimworldArchipelago.Client.Main;

namespace RimworldArchipelago.Client.Services
{
    public class MultiWorldService
    {
        public ArchipelagoSession Session { get; private set; }
        public IDictionary<string, long> DefNameToArchipelagoId { get; } = new ConcurrentDictionary<string, long>();
        public IDictionary<long, RimWorldDef> ArchipeligoItemIdToRimWorldDef { get; } = new ConcurrentDictionary<long, RimWorldDef>();


        private static readonly Lazy<MultiWorldService> lazy =
            new Lazy<MultiWorldService>(() => new MultiWorldService());

        public static MultiWorldService Instance { get { return lazy.Value; } }

        private MultiWorldService()
        {
        }

        public async Task<bool> Connect(string address, string playerSlot, string password = null)
        {
            ArchipelagoSession newSession;

            if (address.Contains(':'))
            {
                newSession = ArchipelagoSessionFactory.CreateSession(new Uri($"ws://{address}"));
            }
            else
            {
                newSession = ArchipelagoSessionFactory.CreateSession(address);
            }

            LoginResult loginResult;

            try
            {
                loginResult = newSession.TryConnectAndLogin("RimWorld", playerSlot, ItemsHandlingFlags.AllItems, password: password);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }

            if (!loginResult.Successful)
            {
                LoginFailure failure = (LoginFailure)loginResult;
                string errorMessage = $"Failed to Connect to {address} as {playerSlot}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }
                Log.Error(errorMessage);
                return false;
            }

            Session = newSession;
            Log.Message("Successfully Connected.");
            var loginSuccess = (LoginSuccessful)loginResult;
            Log.Message($"{loginSuccess}");
            Log.Message(JsonConvert.SerializeObject(loginSuccess));

            var ArchipelagoLoader = new ArchipelagoLoader();
            await ArchipelagoLoader.Load();

            return true;
        }

        public async Task SendLocationCheck(string defName)
        {
            await Session.Locations.CompleteLocationChecksAsync(DefNameToArchipelagoId[defName]);
        }
    }
}
