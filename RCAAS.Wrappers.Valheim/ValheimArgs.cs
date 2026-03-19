using RCAAS.Core.Wrappers;


namespace RCAAS.Wrappers.Valheim;

/// <summary>
/// Configuration arguments for Valheim server.
/// </summary>
public class ValheimArgsExt : BaseArgs
{
    /// <summary>
    /// Gets or sets the server password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the combat modifier setting.
    /// </summary>
    public string? Combat { get; set; }

    /// <summary>
    /// Gets or sets the death penalty modifier setting.
    /// </summary>
    public string? DeathPenalty { get; set; }

    /// <summary>
    /// Gets or sets the resources modifier setting.
    /// </summary>
    public string? Resources { get; set; }

    /// <summary>
    /// Gets or sets the raids modifier setting.
    /// </summary>
    public string? Raids { get; set; }

    /// <summary>
    /// Gets or sets the portals modifier setting.
    /// </summary>
    public string? Portals { get; set; }
}
