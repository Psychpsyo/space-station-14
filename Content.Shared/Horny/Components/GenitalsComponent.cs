using Content.Shared.Horny;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Chemistry.Reagent;

namespace Content.Shared.Horny.Components;

[NetworkedComponent, RegisterComponent, AutoGenerateComponentState(true)]
public sealed partial class GenitalsComponent : Component
{
    // penis, vagina or nothing
    [DataField, AutoNetworkedField]
    public Genitals Genitals = Genitals.Nothing;

    [DataField("cumAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string CumAction = "ActionCum";

    [DataField("cumActionEntity")]
    public EntityUid? CumActionEntity;

    /// <summary>
    /// The amount of units that this entity can cum in one go
    /// </summary>
    [DataField("cumVolume")]
    public int CumVolume = 10;

    /// <summary>
    /// The name of the reagent that will be ejaculated.
    /// </summary>
    [DataField("cumReagent")]
    public ProtoId<ReagentPrototype> CumReagent = "Cum";
}
