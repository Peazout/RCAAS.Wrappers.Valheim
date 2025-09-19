using Newtonsoft.Json;
using RCAAS.Core.Helpers;
using RCAAS.Core.Interfaces;
using RCAAS.Wrappers.Steam;
using System.Diagnostics;
using System.Text;


namespace RCAAS.Wrappers.Valheim
{
    public sealed class ValheimWrapperExt : SteamWrapper
    {

        private ValheimArgsExt _args;

        public static string WorldFolder(int cmdappid) { return Path.Combine(FilesAndFoldersHelper.CmdAppRootFolder(cmdappid), "World"); }

        private List<string> SteamIds = new List<string>();

        public ValheimWrapperExt()
        {
            AnonymousLogin = true;
        }

        public override bool Initalize(IRCAASContext host)
        {
            if (!base.Initalize(host)) return false;
            AnonymousLogin = true;

            InputEncoding = Encoding.Unicode;

            if (Config != null && !string.IsNullOrWhiteSpace(Config.CmdArgs))
            {
                var args = JsonConvert.DeserializeObject<ValheimArgsExt>(Config.CmdArgs);
                // if (!string.IsNullOrWhiteSpace(args.Password)) args.Password = AppCoreHelper.DecodePasswordFromBase64(args.Password);
                Config.CmdArgs = args.ToString();
            }

            return true;

        }

        public ValheimArgsExt ValheimSettings
        {

            get
            {
                if (_args != null) return _args;
                if ((Config == null) || (Config.CmdArgs == null)) return new ValheimArgsExt();
                _args = JsonConvert.DeserializeObject<ValheimArgsExt>(Config.CmdArgs);
                // if (!string.IsNullOrWhiteSpace(args.Password)) args.Password = AppCoreHelper.DecodePasswordFromBase64(args.Password);
                return _args;

            }

        }
        public override async Task ChangeConfigAsync(IAppWrapperConfig value)
        {
            var orgargs = value.CmdArgs;
            if ((value != null) && (!string.IsNullOrWhiteSpace(value.CmdArgs)))
            {
                var args = JsonConvert.DeserializeObject<ValheimArgsExt>(value.CmdArgs);
                // if (!string.IsNullOrWhiteSpace(args.Password)) args.Password = AppCoreHelper.EncodePasswordToBase64(args.Password);
                value.CmdArgs = args.ToString();
            }

            await base.ChangeConfigAsync(value);
            // Config is set to value in base. Now we just need to return the text in clear to memory.
            Config.CmdArgs = orgargs;

        }

        protected override string CreateProcessArgs()
        {
            var str = new StringBuilder();
            str.Append(" -nographics -batchmode");
            str.Append($" -name \"{Config.Name}\"");
            str.Append($" -port {Config.Port}");
            str.Append($" -world \"{Config.Name}\"");
            str.Append($" -password \"{ValheimSettings.Password}\"");
            str.Append($" -savedir \"{WorldFolder(Config.Id)}\"");
            str.Append(" -public 1");
            // str.Append(" -logfile \"" + Path.Combine(FilesAndFoldersHelper.CmdAppRootFolder(Config.Id), "ValheimServer.log") + "\"");
            // Check modifiers
            if (!String.IsNullOrWhiteSpace(ValheimSettings.Combat)) str.Append($" -modifier combat {ValheimSettings.Combat}");
            if (!String.IsNullOrWhiteSpace(ValheimSettings.DeathPenalty)) str.Append($" -modifier deathpenalty {ValheimSettings.DeathPenalty}");
            if (!String.IsNullOrWhiteSpace(ValheimSettings.Resources)) str.Append($" -modifier resources {ValheimSettings.Resources}");
            if (!String.IsNullOrWhiteSpace(ValheimSettings.Raids)) str.Append($" -modifier raids {ValheimSettings.Raids}");
            if (!String.IsNullOrWhiteSpace(ValheimSettings.Portals)) str.Append($" -modifier portals {ValheimSettings.Portals}");

            return str.ToString();

        }

        protected override void OutputMessageHandler(object sender, DataReceivedEventArgs e)
        {
            var msg = new ValheimMessage(e);
            if (msg.IsNullMessage || msg.Received.Data.StartsWith("(Filename: ")) return;
            MyLog.Log(GetLogEvent(NLog.LogLevel.Debug, msg.MessageWithoutTimestamp, Id));

            try
            {
                if (msg.IsSteamHandShake)
                {
                    DoUserLoginAsync(username: "ValheimUser", externalid: msg.UserID).Wait();
                }
                else if (msg.IsSteamDisconnect) { DoUserLogoutAsync(username: msg.Username, externalid: msg.UserID).Wait(); }

            }
            catch (Exception ex)
            {
                MyLog.Error(ex.ToString());
                MyLog.Warn("Message resulted in exception: " + e.Data);
            }

        }


        protected override void BackupFilesToTempFolder(string folder)
        {
            var dirs = Directory.GetDirectories(FilesAndFoldersHelper.CmdAppRootFolder(Id), "world*");
            foreach (var dir in dirs)
            {
                var ignore = new List<string>();
                //Copy world to a temp Dir
                DirectoryInfo thisDir = new DirectoryInfo(dir);
                FilesAndFoldersHelper.Copy(dir, Path.Combine(folder, thisDir.Name), ignore);
            }

        }

        protected override void BackupCleanUp(string tempfolder)
        {
            base.BackupCleanUp(tempfolder);
            if (Users.Count == 0) HasChanged = false;

        }


        public override async Task<IAppWrapperConfig> InstallItemAsync(IAppWrapperConfig item)
        {
            item = await base.InstallItemAsync(item);

            await base.ApplyUpdateAsync();

            return await DBHelper.UpdateCmdAppsAsync(item);

        }
        /// <summary>
        /// Stop the server, if not force the use the /stop command.
        /// </summary>
        /// <param name="forcestop"></param>
        /// <returns></returns>
        public override async Task StopAsync(bool forcestop = false)
        {
            await base.StopAsync(true);

        }


        public void Save()
        {
            Send("save");
            //Generally this needs a long wait
            Thread.Sleep(WaitTime);

        }

    }

}
