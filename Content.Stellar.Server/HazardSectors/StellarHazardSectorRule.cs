// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using System.Linq;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared._ES.Lighting.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Parallax;
using Content.Shared.Weather;
using Content.Stellar.Shared._ES.Core.Timer;
using Content.Stellar.Shared.HazardSectors;
using Content.Stellar.Shared.PostProcess;
using Content.Stellar.Shared.PostProcess.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Stellar.Server.HazardSectors;

public sealed class StellarHazardSectorRule : StellarGameRuleSystem<StellarHazardSectorRuleComponent>
{
    [Dependency] private readonly DockingSystem _dock = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedWeatherSystem _weather = default!;
    [Dependency] private readonly SharedPoweredLightSystem _lights = default!;
    [Dependency] private readonly ShuttleSystem _shuttleSystem = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly ESEntityTimerSystem _esTimer = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    private readonly HashSet<Entity<PoweredLightComponent, TransformComponent>> _lightSet = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarHazardSectorStationComponent, FTLCompletedEvent>(OnFTLComplete);
    }

    protected override void Started(EntityUid uid, StellarHazardSectorRuleComponent comp, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, comp, gameRule, args);

        if (!TryGetRandomStation(out var station))
            return;

        var gridUid = _station.GetLargestGrid(station.Value);
        if (!HasComp<MapGridComponent>(gridUid) || !TryComp<ShuttleComponent>(gridUid, out var shuttleComp))
            return;

        EnsureComp<StellarHazardSectorStationComponent>(gridUid.Value); // Marks the station for convenience.
        EnsureComp<ESTileBasedRoofComponent>(gridUid.Value); // Enables light passthrough for windows, ect.

        comp.SectorStation = gridUid.Value;
        comp.SectorMap = EnsureHazardSectorMap(comp.Parallax, comp.MapLight);
        if (comp.Weather is not null && _prototypeManager.TryIndex(comp.Weather, out var indexedWeather))
            _weather.SetWeather(Transform(comp.SectorMap).MapID, indexedWeather, null);

        _shuttleSystem.FTLToCoordinates(gridUid.Value, shuttleComp, Transform(comp.SectorMap).Coordinates, Angle.Zero, 0f, (float)comp.TravelTime.TotalSeconds);

        var streamEnt = _audio.PlayPvs(comp.TravelAmbience, comp.SectorStation);
        comp.AudioStream = streamEnt?.Entity;
        _audio.SetGridAudio(streamEnt);

        _esTimer.SpawnMethodTimer(comp.TravelTime, () => { comp.AudioStream = _audio.Stop(comp.AudioStream); }); // Set a timer to turn the audio off again
    }

    protected override void ActiveTick(EntityUid uid, StellarHazardSectorRuleComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);

        foreach (var phase in component.AmbiencePhases.AsEnumerable().Reverse())
        {
            if (phase.Completed)
                continue;

            var phaseStart = TimeSpan.Zero;
            if (phase.TimeBeforeEnd != null)
                phaseStart = ExpectedRoundEnd() - phase.TimeBeforeEnd.Value;
            if (phase.TimeAfterStart != null)
                phaseStart = Ticker.RoundStartTimeSpan + phase.TimeAfterStart.Value;

            if (_timing.CurTime < phaseStart)
                continue;

            DoPhase(component, phase);
        }
    }

    private void DoPhase(StellarHazardSectorRuleComponent comp, StellarHazardSectorAmbienceConfig phase)
    {
        // if (phase.AnnouncementNonsense != null && comp.ThreatActive)
        // {
        //  Code for sending announcement text using screen-announcing overlay goes here!
        // }

        if (phase.ApplyLut != null)
        {
            EnsureComp<StellarPostProcessComponent>(comp.SectorMap, out var postProcessComp);
            postProcessComp.UseLut = phase.ApplyLut;
            RaiseNetworkEvent(new StellarPostProcessUpdateEvent(GetNetEntity(comp.SectorMap), phase.ApplyLut));
            Dirty(comp.SectorMap, postProcessComp);
        }

        if (phase.TravelSetup != null)
        {
            _lightSet.Clear();
            _dock.SetDockBolts(comp.SectorStation, true);
            _lookup.GetChildEntities(comp.SectorStation, _lightSet);

            foreach (var ent in _lightSet)
            {
                _lights.SetState(ent, false); // Turn all the lights off
                _esTimer.SpawnMethodTimer(_random.Next(phase.MinLightToggleTime, phase.MaxLightToggleTime), () => { _lights.SetState(ent, true); });
            }
        }

        _audio.PlayGlobal(phase.StageMusic, Filter.Broadcast(), false, AudioParams.Default);
        phase.Completed = true;
    }

    private void OnFTLComplete(Entity<StellarHazardSectorStationComponent> ent, ref FTLCompletedEvent args)
    {
        _shuttleSystem.Disable(ent); // Stations don't need to move, dummy. This permanently anchors it and eliminates the need for Station Anchors.
    }

    private EntityUid EnsureHazardSectorMap(string parallax, Color lightColor)
    {
        var query = AllEntityQuery<StellarHazardSectorMapComponent>();

        while (query.MoveNext(out var uid, out _))
        {
            return uid;
        }

        var mapUid = _mapSystem.CreateMap();
        var parallaxComp = EnsureComp<ParallaxComponent>(mapUid);
        var mapLight = EnsureComp<MapLightComponent>(mapUid);
        EnsureComp<StellarHazardSectorMapComponent>(mapUid);
        _metaData.SetEntityName(mapUid, "Hazard Sector");
        mapLight.AmbientLightColor = lightColor;
        parallaxComp.Parallax = parallax;

        return mapUid;
    }
}
