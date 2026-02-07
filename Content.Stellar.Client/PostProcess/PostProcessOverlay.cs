// SPDX-FileCopyrightText: 2026 TheShuEd
// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening
/*
 * This file is ALL RIGHTS RESERVED, Copyright (c) 2025 CrystallEdge, with usage rights granted to Stellar Station by the Copyright Holder.
 * https://github.com/crystallpunk-14/crystall-punk-14/blob/master/LICENSE_CE.TXT
 */

using Content.Client.Resources;
using Content.Stellar.Shared.CCVars;
using Content.Stellar.Shared.PostProcess;
using Content.Stellar.Shared.PostProcess.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Stellar.Client.PostProcess;

// This overlay serves as the foundational post processing overlay.
// Ideally, for performance reasons, post processing designed to be present at all times, such as additive light blending or tonemapping, should be done as part of a single shader pass.
public sealed class PostProcessOverlay : Overlay
{
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override bool RequestScreenTexture => true;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    private readonly ShaderInstance _basePostProcessShader;

    private readonly ProtoId<ShaderPrototype> _shaderProto = "StellarPostProcess";
    private readonly string _lutBasePath = "/Textures/_ST/Shaders/lut-base.png";

    public PostProcessOverlay()
    {
        IoCManager.InjectDependencies(this);
        _basePostProcessShader = _proto.Index(_shaderProto).InstanceUnique();
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        var playerEntity = _player.LocalSession?.AttachedEntity;

        if (!_entity.TryGetComponent<EyeComponent>(playerEntity, out var eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;

        if (!_lightManager.Enabled || !eyeComp.Eye.DrawLight || !eyeComp.Eye.DrawFov)
            return false;

        return true;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        if (args.Viewport.Eye == null)
            return;

        var lutPath = _lutBasePath;
        if (_entity.TryGetComponent<StellarPostProcessComponent>(args.MapUid, out var mapComp) && mapComp.UseLut != null)
        {
            lutPath = mapComp.UseLut;
        }

        var lutTexture = _cache.GetTexture(lutPath);
        var worldHandle = args.WorldHandle;
        var viewport = args.WorldBounds;

        _basePostProcessShader.SetParameter("LUT_TEXTURE", lutTexture);
        _basePostProcessShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _basePostProcessShader.SetParameter("LIGHT_TEXTURE", args.Viewport.LightRenderTarget.Texture);
        _basePostProcessShader.SetParameter("Zoom", args.Viewport.Eye.Zoom.X);

        worldHandle.UseShader(_basePostProcessShader);
        worldHandle.DrawRect(viewport, Color.White);
        worldHandle.UseShader(null);
    }
}

public sealed class PostProcessSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEntityManager _entity = default!;

    public override void Initialize()
    {
        base.Initialize();

        if (_cfg.GetCVar(STCCVars.PostProcess) && !_overlay.HasOverlay<PostProcessOverlay>())
        {
            _overlay.AddOverlay(new PostProcessOverlay());
        }

        SubscribeNetworkEvent<StellarPostProcessUpdateEvent>(OnPostProcessUpdate);
        Subs.CVar(_cfg, STCCVars.PostProcess, OnCVarUpdate, true);
    }

    private void OnPostProcessUpdate(StellarPostProcessUpdateEvent args)
    {
        var map = GetEntity(args.Target);
        if (!_entity.TryGetComponent<StellarPostProcessComponent>(map, out var postProcessComp))
            return;
        postProcessComp.UseLut = args.Lut;
        Dirty(map, postProcessComp);
    }

    private void OnCVarUpdate(bool enabled)
    {
        if (enabled && !_overlay.HasOverlay<PostProcessOverlay>())
        {
            _overlay.AddOverlay(new PostProcessOverlay());
        }
        else if (!enabled && _overlay.HasOverlay<PostProcessOverlay>())
        {
            _overlay.RemoveOverlay<PostProcessOverlay>();
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<PostProcessOverlay>();
    }
}
