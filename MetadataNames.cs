namespace breakout;

public static class MetadataNames
{
    public static class SquadStrategy
    {
        public const string GROUP = $"{nameof(MetadataNames)}_{nameof(SquadStrategy)}_{nameof(GROUP)}";
        public const string COOLDOWN = $"{nameof(MetadataNames)}_{nameof(SquadStrategy)}_{nameof(COOLDOWN)}";
    }

    public static class DirectPursuitStrategy
    {
        public const string STOPPED = $"{nameof(MetadataNames)}_{nameof(DirectPursuitStrategy)}_{nameof(STOPPED)}";
    }
}
