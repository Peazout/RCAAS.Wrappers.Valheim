using RCAAS.Core.Wrappers;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace RCAAS.Wrappers.Valheim
{
    public class ValheimMessage : BaseRCAASConsoleMessage
    {
        private readonly Regex regPlayerhandshake = new Regex(@"^(: Got handshake from client )([0-9]+)");
        private readonly Regex regSteamDisconnect = new Regex(@"^(: Closing socket )([0-9]+)");
        private readonly Regex regPlayerLoggedIn = new Regex(@"^(: Got character ZDOID from )([\w ]+)( : )([\w\-]+)");
        private readonly Regex regPlayerLoggedOut = new Regex(@"^(Destroying abandoned non persistent zdo )([0-9]+)(:[0-9]+ )([\w\-]+)");

        public string Username { get; set; }
        public string UserID { get; set; }

        public ValheimMessage(DataReceivedEventArgs args) : base(args)
        {
            regRemoveDateStamp = new Regex(@"^([0-9]+\/[0-9]+\/[0-9]+ )");
            regRemoveTimeStamp = new Regex(@"([0-9]+:[0-9]+:[0-9]+)");
        }

        public bool IsSteamHandShake
        {
            get
            {
                // Got handshake from client 76561197974511380
                var result = regPlayerhandshake.Match(MessageWithoutTimestamp);
                if (!result.Success) return false;

                UserID = result.Groups[2].Value;
                return true;

            }

        }

        public bool IsSteamDisconnect
        {
            get
            {
                // Closing socket 76561197974511380
                var result = regSteamDisconnect.Match(MessageWithoutTimestamp);
                if (!result.Success) return false;

                UserID = result.Groups[2].Value;
                return true;

            }

        }

        public bool IsLoggedIn
        {
            get
            {
                // Got character ZDOID from Ratz : 1650277545:1                
                var result = regPlayerLoggedIn.Match(MessageWithoutTimestamp);
                if (!result.Success) return false;

                Username = result.Groups[2].Value;
                UserID = result.Groups[4].Value;

                return true;
            }
        }

        public bool IsLoggedOut
        {
            get
            {
                // Destroying abandoned non persistent zdo 1650277545:1 owner 1650277545
                var result = regPlayerLoggedOut.Match(MessageWithoutTimestamp);
                if (!result.Success) return false;

                UserID = result.Groups[2].Value;

                return true;
            }
        }

    }

}
