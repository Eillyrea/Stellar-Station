// SPDX-FileCopyrightText: 2025 EmoGarbage404
// SPDX-FileCopyrightText: 2025 mirrorcult
//
// SPDX-License-Identifier: MIT

namespace Content.Stellar.Client._ES.Sprite.Components;

/// <summary>
/// Handles a sprite that fades out while synced to a <see cref="ESTimedDespawnComponent"/>
/// </summary>
[RegisterComponent]
[Access(typeof(ESTimedDespawnSpriteFadeSystem))]
public sealed partial class ESTimedDespawnSpriteFadeComponent : Component
{
    [DataField]
    public TimeSpan FadeTime = TimeSpan.FromSeconds(1);
}
