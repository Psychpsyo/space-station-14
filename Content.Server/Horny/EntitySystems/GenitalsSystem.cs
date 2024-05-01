using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Shared.Horny.Components;
using Content.Shared.Horny;
using Content.Shared.Chemistry.Components;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;

namespace Content.Server.Horny.EntitySystems;

public sealed class GenitalsSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;
    [Dependency] private readonly MarkingManager _markingManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GenitalsComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<GenitalsComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<GenitalsComponent, GenitalsChangedEvent>(OnGenitalsChanged);
        SubscribeLocalEvent<GenitalsComponent, EmoteEvent>(OnEmote);
        SubscribeLocalEvent<GenitalsComponent, CumActionEvent>(OnCumAction);
    }

    private void OnMapInit(EntityUid uid, GenitalsComponent component, MapInitEvent args)
    {
        // try to add cum action when genitals component is added
        if (component.Genitals == Genitals.Penis)
            _actions.AddAction(uid, ref component.CumActionEntity, component.CumAction);
    }

    private void OnShutdown(EntityUid uid, GenitalsComponent component, ComponentShutdown args)
    {
        // remove cum action when genitals component is removed
        if (component.Genitals == Genitals.Penis)
            _actions.RemoveAction(uid, component.CumActionEntity);
    }

    private void OnGenitalsChanged(EntityUid uid, GenitalsComponent component, GenitalsChangedEvent args)
    {
        if (args.OldGenitals != Genitals.Penis && args.NewGenitals == Genitals.Penis)
            _actions.AddAction(uid, ref component.CumActionEntity, component.CumAction);

        if (args.OldGenitals == Genitals.Penis && args.NewGenitals != Genitals.Penis)
            _actions.RemoveAction(uid, component.CumActionEntity);
    }

    private void OnEmote(EntityUid uid, GenitalsComponent component, ref EmoteEvent args)
    {
        if (args.Handled || args.Emote.ID != "Cum")
            return;

        args.Handled = TryCum(uid, component);
    }

    private void OnCumAction(EntityUid uid, GenitalsComponent component, CumActionEvent args)
    {
        if (args.Handled)
            return;

        if (TryCum(uid, component))
        {
            _chat.TryEmoteWithChat(uid, "Cum");
            args.Handled = true;
        }
    }

    private bool TryCum(EntityUid uid, GenitalsComponent component)
    {
        if (component.Genitals != Genitals.Penis)
            return false;

        var cumSolution = new Solution(component.CumReagent, component.CumVolume);
        var xform = Transform(uid);
        _puddle.TrySpillAt(xform.Coordinates, cumSolution, out _);
        return true;
    }
    public void SetGenitals(EntityUid uid, Genitals genitals, bool sync = true, GenitalsComponent? genitalsComp = null)
    {
        if (!Resolve(uid, ref genitalsComp) || genitalsComp.Genitals == genitals)
            return;

        var oldGenitals = genitalsComp.Genitals;
        genitalsComp.Genitals = genitals;
        if (TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
            humanoid.MarkingSet.EnsureGenitals(genitals, _markingManager);
        RaiseLocalEvent(uid, new GenitalsChangedEvent(oldGenitals, genitals));

        if (sync)
        {
            Dirty(uid, genitalsComp);
        }
    }
}
