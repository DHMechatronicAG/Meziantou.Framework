using System.Collections;

namespace Meziantou.Framework.Collections.Concurrent;

public sealed class SynchronizedList<T> : IList<T>, IReadOnlyList<T>
{
    private readonly List<T> _list = [];

    public int Count
    {
        get
        {
            lock (_list)
            {
                return _list.Count;
            }
        }
    }

    bool ICollection<T>.IsReadOnly
    {
        get
        {
            lock (_list)
            {
                return ((ICollection<T>)_list).IsReadOnly;
            }
        }
    }

    public T this[int index]
    {
        get
        {
            lock (_list)
            {
                return _list[index];
            }
        }
        set
        {
            lock (_list)
            {
                _list[index] = value;
            }
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        lock (_list)
        {
            return ((IReadOnlyCollection<T>)[.. _list]).GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int EnsureCapacity(int capacity)
    {
        lock (_list)
        {
            return _list.EnsureCapacity(capacity);
        }
    }

    public void Add(T item)
    {
        lock (_list)
        {
            _list.Add(item);
        }
    }

    public void Clear()
    {
        lock (_list)
        {
            _list.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_list)
        {
            return _list.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_list)
        {
            _list.CopyTo(array, arrayIndex);
        }
    }

    public bool Remove(T item)
    {
        lock (_list)
        {
            return _list.Remove(item);
        }
    }

    public int IndexOf(T item)
    {
        lock (_list)
        {
            return _list.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        lock (_list)
        {
            _list.Insert(index, item);
        }
    }

    public void RemoveAt(int index)
    {
        lock (_list)
        {
            _list.RemoveAt(index);
        }
    }

    public void CopyTo(Array array, int index)
    {
        lock (_list)
        {
            ((ICollection)_list).CopyTo(array, index);
        }
    }
}
