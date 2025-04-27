using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Company;

/// <summary>
/// Prototype for a company that can be assigned to players.
/// </summary>
[Prototype("company")]
public sealed class CompanyPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The name of the company.
    /// </summary>
    [DataField("name", required: true)]
    public string Name { get; private set; } = default!;

    /// <summary>
    /// The name of the company.
    /// </summary>
    [DataField("description", required: false)]
    public string Description { get; private set; } = default!;

    /// <summary>
    /// The color used to display the company name in examine text.
    /// </summary>
    [DataField("color")]
    public Color Color { get; private set; } = Color.Yellow;

    /// <summary>
    /// Whether this company should be disabled from selection in the UI.
    /// Companies with this set to true will still be assigned automatically through the job system,
    /// but players won't be able to select them manually.
    /// </summary>
    [DataField("disabled")]
    public bool Disabled { get; private set; } = false;

    /// <summary>
    /// Закрытые компании с поддержкой по логину
    /// </summary>
    [DataField("logins", required: false)]
    public List<string> Logins { get; private set; } = new();
}
