using Godot;
using Godot.Collections;

namespace breakout;

public abstract partial class PriorityQueue<[MustBeVariant]T> : Resource
{
    [Export]
    public Array<T> Items { get => items; set 
        {
            items.Clear();
            foreach (var item in items)
                Insert(item);
        }
    }
    private Array<T> items = [];

    protected abstract int Prio(T item);

    // Returns index of parent
    int parent(int i) => (i - 1) / 2;

    // Returns index of left child
    int leftChild(int i) => 2 * i + 1;

    // Returns index of right child
    int rightChild(int i) => 2 * i + 2;

    // Shift up to maintain max-heap property
    void shiftUp(int i)
    {
        while (i > 0 && Prio(Items[parent(i)]) < Prio(Items[i]))
        {
            (Items[i], Items[parent(i)]) = (Items[parent(i)], Items[i]);
            i = parent(i);
        }
    }

    // Shift down to maintain max-heap property
    void shiftDown(int i)
    {
        var size = Items.Count;
        int maxIndex = i;
        int l = leftChild(i);
        if (l < size && Prio(Items[l]) > Prio(Items[maxIndex])) maxIndex = l;
        int r = rightChild(i);
        if (r < size && Prio(Items[r]) > Prio(Items[maxIndex])) maxIndex = r;

        if (i != maxIndex)
        {
            (Items[maxIndex], Items[i]) = (Items[i], Items[maxIndex]);
            shiftDown(maxIndex);
        }
    }

    // Insert a new element
    public void Insert(T p)
    {
        Items.Add(p);
        shiftUp(Items.Count - 1);
    }

    // Extract element with maximum priority
    public bool TryDequeue(out T first)
    {
        first = default;
        int size = Items.Count;
        if (size == 0) return false;
        first = Items[0];
        Items[0] = Items[size - 1];
        Items.RemoveAt(size - 1);
        shiftDown(0);
        return true;
    }

    // Get current maximum element
    public bool TryPeek(out T first)
    {
        first = default;
        if (Items.Count == 0) return false;
        first = Items[0];
        return true;
    }
}
