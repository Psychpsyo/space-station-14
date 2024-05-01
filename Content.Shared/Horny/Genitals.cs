namespace Content.Shared.Horny
{
    public enum Genitals : byte
    {
        Penis,
        Vagina,
        Nothing,
    }

    /// <summary>
    ///     Raised when entity has changed their genitals.
    ///     Not to be confused with sex or gender changes.
    /// </summary>
    public record struct GenitalsChangedEvent(Genitals OldGenitals, Genitals NewGenitals);
}
