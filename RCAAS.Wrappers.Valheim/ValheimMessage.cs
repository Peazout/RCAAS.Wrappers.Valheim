using RCAAS.Core.Wrappers;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace RCAAS.Wrappers.Valheim;

/// <summary>
/// Represents a console message from the Valheim server with parsed user information.
/// </summary>
public partial class ValheimMessage : BaseRCAASConsoleMessage
{
    [GeneratedRegex(@"^(: Got handshake from client )([0-9]+)")]
    private static partial Regex PlayerHandshakeRegex();

    [GeneratedRegex(@"^(: Closing socket )([0-9]+)")]
    private static partial Regex SteamDisconnectRegex();

    [GeneratedRegex(@"^(: Got character ZDOID from )([\w ]+)( : )([\w\-]+)")]
    private static partial Regex PlayerLoggedInRegex();

    [GeneratedRegex(@"^(Destroying abandoned non persistent zdo )([0-9]+)(:[0-9]+ )([\w\-]+)")]
    private static partial Regex PlayerLoggedOutRegex();

    /// <summary>
    /// Gets or sets the username associated with the message.
    /// </summary>
    public string? Username { get; private set; }

    /// <summary>
    /// Gets or sets the Steam user ID associated with the message.
    /// </summary>
    public string? UserID { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValheimMessage"/> class.
    /// </summary>
    /// <param name="args">The data received event arguments from the server output.</param>
    public ValheimMessage(DataReceivedEventArgs args) : base(args)
    {
        regRemoveDateStamp = new Regex(@"^([0-9]+\/[0-9]+\/[0-9]+ )");
        regRemoveTimeStamp = new Regex(@"([0-9]+:[0-9]+:[0-9]+)");

        ParseMessage();
    }

    private void ParseMessage()
    {
        if (IsSteamHandShake || IsSteamDisconnect || IsLoggedIn || IsLoggedOut)
        {
            // Properties are set in the getter methods
        }
    }

    /// <summary>
    /// Gets a value indicating whether the message represents a Steam handshake event.
    /// </summary>
    public bool IsSteamHandShake
    {
        get
        {
            var result = PlayerHandshakeRegex().Match(MessageWithoutTimestamp);
            if (!result.Success)
            {
                return false;
            }

            UserID = result.Groups[2].Value;
            return true;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the message represents a Steam disconnect event.
    /// </summary>
    public bool IsSteamDisconnect
    {
        get
        {
            var result = SteamDisconnectRegex().Match(MessageWithoutTimestamp);
            if (!result.Success)
            {
                return false;
            }

            UserID = result.Groups[2].Value;
            return true;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the message represents a player logged in event.
    /// </summary>
    public bool IsLoggedIn
    {
        get
        {
            var result = PlayerLoggedInRegex().Match(MessageWithoutTimestamp);
            if (!result.Success)
            {
                return false;
            }

            Username = result.Groups[2].Value;
            UserID = result.Groups[4].Value;

            return true;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the message represents a player logged out event.
    /// </summary>
    public bool IsLoggedOut
    {
        get
        {
            var result = PlayerLoggedOutRegex().Match(MessageWithoutTimestamp);
            if (!result.Success)
            {
                return false;
            }

            UserID = result.Groups[2].Value;

            return true;
        }
    }
}
