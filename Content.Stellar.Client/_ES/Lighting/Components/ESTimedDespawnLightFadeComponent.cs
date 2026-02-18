// SPDX-FileCopyrightText: 2025 EmoGarbage404
// SPDX-FileCopyrightText: 2025 mirrorcult
//
// SPDX-License-Identifier: MIT

namespace Content.Stellar.Client._ES.Lighting.Components;

/// <summary>
/// Handles a point light that fades out while synced to a <see cref="ESTimedDespawnComponent"/>
/// </summary>
[RegisterComponent]
[Access(typeof(ESTimedDespawnLightFadeComponent))]
public sealed partial class ESTimedDespawnLightFadeComponent : Component
{
    [DataField]
    public TimeSpan FadeTime = TimeSpan.FromSeconds(1);
}
