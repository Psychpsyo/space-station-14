using Content.Server.Horny.EntitySystems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Chemistry.Reagent;


namespace Content.Server.Horny.Components;

[RegisterComponent]
[Access(typeof(GenitalsSystem))]
public sealed partial class GenitalsComponent : Component
{
    [DataField("cumAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string CumAction = "ActionCum";

    [DataField("cumActionEntity")]
    public EntityUid? CumActionEntity;

    /// <summary>
    /// The amount of units that this entity can cum in one go
    /// </summary>
    [DataField("amount")]
    public int Amount = 10;

    /// <summary>
    /// The name of the reagent that will be ejaculated.
    /// </summary>
    [DataField("cumReagent")]
    public ProtoId<ReagentPrototype> CumReagent = "Cum";
}
