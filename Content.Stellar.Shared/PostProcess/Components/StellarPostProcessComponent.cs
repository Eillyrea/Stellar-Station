// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.GameStates;

namespace Content.Stellar.Shared.PostProcess.Components;

/// <summary>
/// Component for use in manipulating the Postprocessing Overlay.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StellarPostProcessComponent : Component
{
    /// <summary>
    /// A provided LUT for entities to use in Post Processing.
    /// </summary>
    [DataField, AutoNetworkedField] public string? UseLut;
}
