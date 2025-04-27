using Robust.Shared.Utility;

namespace Content.Server._Lua.FTLPoints.Tick.GenerateNewSector;

/// <summary>
/// Generates a new sector starting from here when there are more than the required grids in this map
/// </summary>
[RegisterComponent]
public sealed partial class GenerateNewSectorTickComponent : Component
{
    public bool Generated = false;
}
