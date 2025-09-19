using RCAAS.Core.Helpers;


namespace RCAAS.Wrappers.Valheim
{

	public class ValheimArgsExt : BaseArgs
	{

        public string Password { get; set; }
        public string Combat { get; set; }
        public string DeathPenalty { get; set; }
        public string Resources { get; set; }
        public string Raids { get; set; }
        public string Portals { get; set; }

        public ValheimArgsExt()
		{
		}

	}

}
