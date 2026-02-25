using breakout.components.scripts;
using breakout.moves;
using breakout.moves.components;
using Godot;
using Godot.Collections;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class MoveDefinition : Resource
{
    [Export]
    public string Name { get; set; } = "";
    [Export]
    public PackedScene? AnimationScene { get; set; }
    [Export]
    public int Power { get; set; } = 0;
    [Export]
    public int Accuracy { get; set; } = 100;
    [Export]
    public int MaxPP { get; set; } = 10;
    [Export]
    public bool IsSpecial { get; set; } = false;
    [Export]
    public uint Range { get; set; } = 100;
    [Export]
    public float Time { get; set; } = 0.1f;
    [Export]
    public PokeType Type { get; set; } = PokeType.Normal;
    [Export]
    public PokeType Type2 { get; set; } = PokeType.None;
    [Export]
    public string AnimationToPlay { get; set; } = "Attack";
    [Export]
    public float BaseCooldownSeconds { get; set; } = 1.0f;
    [Export]
    public bool IgnoreCaster { get; set; } = true;

    public override string ToString() => $"Move[name: {Name}, range: {Range}, type: {Type}]";
    
    public void SpawnMove(Node parent, Node2D caster, Node2D? existing)
    {
        if ((existing ?? AnimationScene?.Instantiate()) is BaseMove node)
        {
            node.Caster = caster;
            node.Configure<MoveRangeComponent>(m =>
            {
                m.Range = Range;
                m.Time = Time;
            });
            node.Configure<PokeTypeComponent>(t =>
            {
                t.Type1 = Type;
                t.Type2 = Type2;
            });

            if (node.TryGetComponent<CasterPosition>(out var pos))
                node.GlobalPosition = caster.GlobalPosition;
            if (node.TryGetComponent<CasterRotation>(out var rot))
                node.GlobalRotation = caster.TryGetComponent<PMDSprite>(out var sprite) ? sprite.Direction.Angle() : caster.GlobalRotation;
            if (node.TryGetComponent<CasterTeam>(out var team) && caster.TryGetComponent<TeamComponent>(out var cteam))
                node.Configure<TeamComponent>(m => m.SetTeam(cteam.Team));
            node.Configure<CircleComponent>(c =>
            {
                c.Color = Type.GetColor();
            });
            node.Configure<HitboxComponent>(hitbox =>
            {
                if (IgnoreCaster)
                    hitbox.Ignore.Add(caster);
                hitbox.DamageSourceCode = new()
                {
                    Amount = Power,
                    Source = caster,
                    // todo: get the source and type
                };
            });
            parent.AddChild(node);
        }
    }

    static Dictionary<string, MoveDefinition>? _allDefinitions;
    public static Dictionary<string, MoveDefinition> AllDefinitions => LoadDefs.LoadAll(ref _allDefinitions, "res://moves/", r => r.Name);
}
