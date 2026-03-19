using Newtonsoft.Json;
using RCAAS.Core.Helpers;
using RCAAS.Core.Interfaces;
using RCAAS.Core.Wrappers.Steam;
using System.Diagnostics;
using System.Text;

namespace RCAAS.Wrappers.Valheim;

public class ValheimWrapperExt : SteamWrapper
{
    private ValheimArgsExt? _args;

    public static string WorldFolder(int cmdappid) =>
        Path.Combine(FilesAndFoldersHelper.CmdAppRootFolder(cmdappid), "World");

    private List<string> SteamIds = [];

    public ValheimWrapperExt()
    {
        AnonymousLogin = true;
    }

    public override bool Initalize(IRCAASContext host)
    {
        if (!base.Initalize(host))
        {
            return false;
        }

        AnonymousLogin = true;
        InputEncoding = Encoding.Unicode;

        if (Config is not null && !string.IsNullOrWhiteSpace(Config.CmdArgs))
        {
            var args = JsonConvert.DeserializeObject<ValheimArgsExt>(Config.CmdArgs);
            if (args is not null)
            {
                Config.CmdArgs = args.ToString() ?? string.Empty;
            }
        }

        return true;
    }

    public ValheimArgsExt ValheimSettings
    {
        get
        {
            if (_args is not null)
            {
                return _args;
            }

            if (Config?.CmdArgs is null)
            {
                return new ValheimArgsExt();
            }

            _args = JsonConvert.DeserializeObject<ValheimArgsExt>(Config.CmdArgs);
            return _args ?? new ValheimArgsExt();
        }
    }
    public override async Task ChangeConfigAsync(IAppWrapperConfig value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var orgArgs = value.CmdArgs;
        if (!string.IsNullOrWhiteSpace(value.CmdArgs))
        {
            var args = JsonConvert.DeserializeObject<ValheimArgsExt>(value.CmdArgs);
            if (args is not null)
            {
                value.CmdArgs = args.ToString() ?? string.Empty;
            }
        }

        await base.ChangeConfigAsync(value).ConfigureAwait(false);

        if (Config is not null)
        {
            Config.CmdArgs = orgArgs;
        }
    }

    protected override string CreateProcessArgs()
    {
        ArgumentNullException.ThrowIfNull(Config);

        var str = new StringBuilder();
        str.Append(" -nographics -batchmode");
        str.Append($" -name \"{Config.Name}\"");
        str.Append($" -port {Config.Port}");
        str.Append($" -world \"{Config.Name}\"");

        var password = ValheimSettings.Password;
        if (!string.IsNullOrWhiteSpace(password))
        {
            str.Append($" -password \"{password}\"");
        }

        str.Append($" -savedir \"{WorldFolder(Config.Id)}\"");
        str.Append(" -public 1");

        AppendModifierIfSet(str, "combat", ValheimSettings.Combat);
        AppendModifierIfSet(str, "deathpenalty", ValheimSettings.DeathPenalty);
        AppendModifierIfSet(str, "resources", ValheimSettings.Resources);
        AppendModifierIfSet(str, "raids", ValheimSettings.Raids);
        AppendModifierIfSet(str, "portals", ValheimSettings.Portals);

        return str.ToString();
    }

    private static void AppendModifierIfSet(StringBuilder builder, string modifierName, string? modifierValue)
    {
        if (!string.IsNullOrWhiteSpace(modifierValue))
        {
            builder.Append($" -modifier {modifierName} {modifierValue}");
        }
    }

    protected override void OutputMessageHandler(object sender, DataReceivedEventArgs e)
    {
        var msg = new ValheimMessage(e);
        if (msg.IsNullMessage || msg.Received.Data?.StartsWith("(Filename: ", StringComparison.Ordinal) == true)
        {
            return;
        }

        MyLog.Log(GetLogEvent(NLog.LogLevel.Debug, msg.MessageWithoutTimestamp, Id));

        try
        {
            if (msg.IsSteamHandShake)
            {
                _ = HandleUserLoginAsync(msg.UserID);
            }
            else if (msg.IsSteamDisconnect)
            {
                _ = HandleUserLogoutAsync(msg.Username, msg.UserID);
            }
        }
        catch (Exception ex)
        {
            MyLog.Error(ex, "Message processing failed");
            MyLog.Warn($"Message resulted in exception: {e.Data}");
        }
    }

    private async Task HandleUserLoginAsync(string? userId)
    {
        try
        {
            await DoUserLoginAsync(username: "ValheimUser", externalid: userId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            MyLog.Error(ex, "User login failed");
        }
    }

    private async Task HandleUserLogoutAsync(string? username, string? userId)
    {
        try
        {
            await DoUserLogoutAsync(username: username, externalid: userId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            MyLog.Error(ex, "User logout failed");
        }
    }


    protected Task BackupFilesToTempFolder(string tempfolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tempfolder);

        var dirs = Directory.GetDirectories(FilesAndFoldersHelper.CmdAppRootFolder(Id), "world*");
        foreach (var dir in dirs)
        {
            var ignore = new List<string>();
            var thisDir = new DirectoryInfo(dir);
            FilesAndFoldersHelper.Copy(dir, Path.Combine(tempfolder, thisDir.Name), ignore);
        }

        return Task.CompletedTask;
    }

    protected async Task BackupCleanUpAsync(string tempfolder)
    {
        await base.BackupCleanUpAsync(tempfolder).ConfigureAwait(false);

        if (Users.Count == 0)
        {
            HasChanged = false;
        }
    }


    public override async Task<IAppWrapperConfig> InstallItemAsync(IAppWrapperConfig item)
    {
        ArgumentNullException.ThrowIfNull(item);

        item = await base.InstallItemAsync(item).ConfigureAwait(false);
        await base.ApplyUpdateAsync().ConfigureAwait(false);

        return await DBHelper.UpdateCmdAppsAsync(item).ConfigureAwait(false);
    }
    /// <summary>
    /// Stops the Valheim server. Always forces immediate stop.
    /// </summary>
    /// <param name="forcestop">This parameter is ignored; stop is always forced.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StopAsync(bool forcestop = false)
    {
        await base.StopAsync(true).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously sends a save command to the server and waits for completion.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the wait operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        Send("save");
        await Task.Delay(WaitTime, cancellationToken).ConfigureAwait(false);
    }

}
