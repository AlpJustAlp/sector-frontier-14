// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Server._Lua.Cloning.Components;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server._Lua.Cloning.Events;

public sealed class CloningAppearanceEvent : EntityEventArgs
{
    public ICommonSession Player = default!;
    public CloningAppearanceComponent Component = default!;
    public EntityCoordinates Coords { get; set; }
    public EntityUid? StationUid { get; set; }
}
