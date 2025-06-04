// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Server._NF.Bank;
using Content.Server.Chat.Managers; // Lua
using Content.Server.Popups; // Lua
using Content.Server.Station.Systems; // Lua
using Content.Shared._NF.Bank.Components;
using Content.Shared._NF.Roles.Components;
using Content.Shared.Chat; // Lua
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Lua.CLVar;
using Content.Shared.Popups; // Lua
using Content.Shared.Roles;
using Robust.Shared.Configuration; // Lua
using Robust.Shared.Player;
using Robust.Shared.Prototypes; // Lua

namespace Content.Server._Lua.AutoSalarySystem;

public sealed class AutoSalarySystem : EntitySystem
{
    [Dependency] private readonly BankSystem _bank = default!;
    [Dependency] private readonly PopupSystem _popup = default!; // Lua
    [Dependency] private readonly IChatManager _chatManager = default!; // Lua
    [Dependency] private readonly StationJobsSystem _stationJobs = default!; // Lua
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!; // Lua
    [Dependency] private readonly IConfigurationManager _cfg = default!; // Lua

    private float _interval;
    private float _currentTime;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
        _cfg.OnValueChanged(CLVars.AutoSalaryInterval, v => _interval = v, true);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _currentTime -= frameTime;

        if (_currentTime <= 0)
        {
            _currentTime = _interval;
            ProcessSalary();
        }
    }

    private void OnRoundRestart(RoundRestartCleanupEvent args)
    {
        _currentTime = _interval;
    }

    // Lua start
    private void ProcessSalary()
    {
        var query = EntityQueryEnumerator<HumanoidAppearanceComponent, BankAccountComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out _, out _, out var actor))
        {
            var station = GetOwningStation(uid);
            if (station == null)
                continue;

            if (TryComp<JobTrackingComponent>(uid, out var jobTracking) &&
                jobTracking.Active &&
                _stationJobs.TryGetOriginalJob(station.Value, actor.PlayerSession.UserId, out var jobId))
            {
                Logger.Info($"DEBUG: {ToPrettyString(uid)} оригинальный JobID: {jobId}");
                var salary = GetSalary(jobId);
                if (_bank.TryBankDeposit(uid, salary))
                {
                    NotifySalaryReceived(uid, salary);
                }
            }
        }
    }
    // Lua end

    private EntityUid? GetOwningStation(EntityUid uid)
    {
        var stationSystem = Get<StationSystem>();
        return stationSystem.GetOwningStation(uid);
    }

    // Lua start
    private void NotifySalaryReceived(EntityUid uid, int salary)
    {
        if (!TryComp(uid, out BankAccountComponent? bank))
            return;

        if (!TryComp(uid, out ActorComponent? actor))
            return;

        var changeAmount = $"+{salary}";
        var message = Loc.GetString(
            "bank-program-change-balance-notification",
            ("balance", bank.Balance),
            ("change", changeAmount),
            ("currencySymbol", "$")
        );

        _popup.PopupEntity(message, uid, Filter.Entities(uid), true, PopupType.Small);

        _chatManager.ChatMessageToOne(
            ChatChannel.Notifications,
            message,
            message,
            EntityUid.Invalid,
            false,
            actor.PlayerSession.Channel
        );
    }
    // Lua end

    private int GetSalary(ProtoId<JobPrototype> jobId)
    {
        if (!_prototypeManager.TryIndex(jobId, out var jobPrototype))
            throw new KeyNotFoundException($"Неизвестный ID работы: {jobId}");

        return jobPrototype.Salary;
    }
}
