using Godot;
using System;
using System.Collections.Generic;

namespace breakout;


public class For<T, TContainer>
    where TContainer : Node
{
    private Dictionary<string, (Node result, Action<T> update)> _nodes { get; set; } = [];
    private Dictionary<string, T> _values { get; set; } = [];

    public Func<T, string>? Key { get; set; }

    public Func<T, (Node result, Action<T> update)>? Map { get; set; }

    public Func<IEnumerable<T>>? Items { get; set; }

    public TContainer? Container { get; set; }

    public void Update()
    {
        if (Items is null || Key is null || Map is null || Container is null)
            return;
        var newNodes = new Dictionary<string, (Node result, Action<T> update)>();
        var newValues = new Dictionary<string, T>();
        var indexes = new Dictionary<int, Node>();
        var index = 0;
        foreach (var item in Items())
        {
            var key = Key(item);
            if (_nodes.TryGetValue(key, out var existingNode_update))
            {
                var (existingNode, update) = existingNode_update;
                newNodes[key] = existingNode_update;
                newValues[key] = item;
                if (item?.Equals(_values[key]) == false)
                    update(item);
                _nodes.Remove(key);
                _values.Remove(key);
                indexes[index++] = existingNode;
            }
            else
            {
                var (newNode, update) = Map(item);
                newNodes[key] = (newNode, update);
                newValues[key] = item;
                Container.AddChild(newNode);
                indexes[index++] = newNode;
            }
        }
        // Remove old nodes
        foreach (var (oldNode, _) in _nodes.Values)
        {
            if (oldNode.GetParent() is Node parent)
            {
                parent.RemoveChild(oldNode);
                oldNode.QueueFree();
            }
        }
        _nodes = newNodes;
        _values = newValues;
        if (Container.IsInsideTree() == false)
            return;
        foreach (var (idx, child) in indexes)
        {
            Container.MoveChild(child, idx);
        }
    }

    public void Dispose()
    {
        _nodes.Clear();
        _values.Clear();
        Container = null;
        Items = null;
        Key = null;
        Map = null;
    }
}
