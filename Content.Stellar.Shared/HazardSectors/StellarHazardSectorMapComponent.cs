// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.GameStates;

namespace Content.Stellar.Shared.HazardSectors;

/// <summary>
/// Marker component for use in tagging Hazard Sector maps.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class StellarHazardSectorMapComponent : Component;
