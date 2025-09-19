using RCAAS.Core.Helpers;
using RCAAS.Core.Interfaces;
using RCAAS.Data;
using System;
using System.Threading.Tasks;


namespace RCAAS.Wrappers.Valheim
{
    public class ValheimPluginHelperExt : BasePluginHelper
    {

        public override BaseArgs GetDefaultArgs()
        {
            return new ValheimArgsExt();
        }

        public async override Task<IAppWrapperConfig> GetDefaultCmdAppItemAsync()
        {
            var item = await base.GetDefaultCmdAppItemAsync();

            item.Name = "RCAAS Valheim server anno " + DateTime.Now.ToString("yyyy");
            item.WrapperName = "Valheim";
            item.Filename = "valheim_server.exe";
            item.ExternalId = 896660;
            item.Port = 2456;
            // if (string.IsNullOrWhiteSpace(item.Password)) item.Password = "AntiqueWhite";
            item.Port = await EthernetHelper.FindNextFreePortAsync(item.Port);

            return item;

        }

    }
}
