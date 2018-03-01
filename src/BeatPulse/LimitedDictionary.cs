using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public class LimitedDictionary<TK, TV> : IDictionary<TK, TV>
    {
        private readonly IDictionary<TK, TV> _values;
        private readonly int _maxitems;
        private Queue<TK> _keys;
        public LimitedDictionary(int maxitems)
        {
            _keys = new Queue<TK>();
            _values = new Dictionary<TK, TV>();
            _maxitems = maxitems;
        }
        public TV this[TK key] { get => _values[key]; set => _values[key] = value; }

        public ICollection<TK> Keys => _values.Keys;

        public ICollection<TV> Values => _values.Values;

        public int Count => _values.Count;

        public bool IsReadOnly => false;

        public void Add(TK key, TV value)
        {
            if (_values.Count == _maxitems)
            {
                var lru = _keys.Dequeue();
                _values.Remove(lru);
            }

            _keys.Enqueue(key);
            _values.Add(key, value);
        }

        public void Add(KeyValuePair<TK, TV> item) => Add(item.Key, item.Value);

        public void AddOrUpdate(TK key, TV value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }

        public void Clear()
        {
            _values.Clear();
            _keys.Clear();
        }

        public bool Contains(KeyValuePair<TK, TV> item) => _values.Contains(item);

        public bool ContainsKey(TK key) => _values.ContainsKey(key);

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator() => _values.GetEnumerator();

        public bool Remove(TK key)
        {
            if (_values.ContainsKey(key))
            {
                _values.Remove(key);
                _keys = new Queue<TK>(_values.Keys);
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            if (_values.ContainsKey(item.Key))
            {
                _values.Remove(item);
                _keys = new Queue<TK>(_values.Keys);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TK key, out TV value) => _values.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();
    }
}
