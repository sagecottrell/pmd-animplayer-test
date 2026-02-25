using Godot;
using System;

namespace breakout;

public sealed class RefGetSetMeta<[MustBeVariant] T>(Node node, string name, T @default) : IDisposable
{
    public Node Node { get; } = node;
    public string Name { get; } = name;
    public T Value { get; set; } = node.GetMeta(name, Variant.From(@default)).As<T>();

    public void Dispose()
    {
        Node.SetMeta(Name, Variant.From(Value));
    }
}

public static class RefGetSetMeta
{
    public static RefGetSetMeta<T> Create<[MustBeVariant] T>(Node node, string name, T @default) => new(node, name, @default);
}