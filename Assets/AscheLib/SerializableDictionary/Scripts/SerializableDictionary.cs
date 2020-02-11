using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AscheLib.Collections {
	/// <summary>
	/// Base class for PropertyDrawer of SerializableDictionary
	/// </summary>
	[Serializable]
	public abstract class DrawableSerializableDictionaryBase { }

	/// <summary>
	/// Base class for generating a Dictionary that can be displayed in the Inspector
	/// </summary>
	[Serializable]
	public abstract class SerializableDictionaryBase<TKey, TValue, TPair> : DrawableSerializableDictionaryBase,
		ICollection<KeyValuePair<TKey, TValue>>,
		IEnumerable<KeyValuePair<TKey, TValue>>,
		IEnumerable,
		IDictionary<TKey, TValue>,
		ICollection,
		IDictionary,
		IDeserializationCallback,
		ISerializable
		where TPair : SerializableKeyValuePairBase<TKey, TValue>,
		new() {

		[SerializeField]
		private List<TPair> _kvArray = new List<TPair>();

		private object syncRoot = new object();

		public SerializableDictionaryBase() {

		}
		protected SerializableDictionaryBase(SerializationInfo info, StreamingContext context) {
			_kvArray = (List<TPair>)info.GetValue("_kvArray", typeof(List<TPair>));
		}

		public TValue this[TKey key] {
			set {
				if(ContainsKey(key)) {
                    var index = 0;
					foreach(var kv in _kvArray) {
						if(kv.Key.Equals(key))
							break;
						index++;
					}
					_kvArray[index] = SerializableKeyValuePairBase<TKey, TValue>.Create<TPair>(key, value);
				}
				else {
					Add(key, value);
				}
			}
			get {
				return GetValue(key);
			}
		}
		public bool HasDuplicationKey { get => _kvArray.GroupBy(pair => pair.Key).Where(group => group.Count() > 1).ToList().Count > 0; }

		public int Count => _kvArray.Count;
		public ICollection<TKey> Keys => _kvArray.Select(pair => pair.Key).ToList();
		public ICollection<TValue> Values => _kvArray.Select(pair => pair.Value).ToList();
		public bool IsReadOnly => false;

		bool IDictionary.IsFixedSize => false;
		bool ICollection.IsSynchronized => false;
		ICollection IDictionary.Keys => _kvArray.Select(pair => pair.Key).ToList();
		ICollection IDictionary.Values => _kvArray.Select(pair => pair.Value).ToList();
		object ICollection.SyncRoot => syncRoot;
		object IDictionary.this[object key] { get => this[(TKey)key]; set => this[(TKey)key] = (TValue)value; }

		public TValue GetValue(TKey key) {
			if(ContainsKey(key)) {
				return _kvArray.First(pair => pair.Key.Equals(key)).Value;
			}
			throw new KeyNotFoundException();
		}
		public bool TryGetValue(TKey key, out TValue value) {
			if(ContainsKey(key)) {
				value = this[key];
				return true;
			}
			value = default(TValue);
			return false;
		}
		public void Add(TKey key, TValue value) {
			if(!ContainsKey(key))
				_kvArray.Add(SerializableKeyValuePairBase<TKey, TValue>.Create<TPair>(key, value));
			else
				throw new ArgumentException();
		}
		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}
		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return _kvArray.FirstOrDefault(pair => pair.Key.Equals(item.Key) && pair.Value.Equals(item.Value)) != null;
		}
		public bool ContainsKey(TKey key) {
			return _kvArray.FirstOrDefault(pair => pair.Key.Equals(key)) != null;
		}
		public bool ContainsValue(TValue value) {
			return _kvArray.FirstOrDefault(pair => pair.Value.Equals(value)) != null;
		}
		public void Clear() {
			_kvArray.Clear();
		}

		private Dictionary<TKey, TValue> ToDictionary() {
			var result = new Dictionary<TKey, TValue>();
			foreach(var kv in _kvArray) {
				if(!result.ContainsKey(kv.Key)) result.Add(kv.Key, kv.Value);
			}
			return result;
		}

		public static implicit operator Dictionary<TKey, TValue>(SerializableDictionaryBase<TKey, TValue, TPair> serializableDictionary) {
			return serializableDictionary.ToDictionary();
		}
		protected static TSerializableDictionary ConvertCore<TSerializableDictionary>(Dictionary<TKey, TValue> dictionary) where TSerializableDictionary : SerializableDictionaryBase<TKey, TValue, TPair>, new() {
			var result = new TSerializableDictionary();
			foreach(var kv in dictionary) {
				result.Add(kv.Key, kv.Value);
			}
			return result;
		}

		public bool Remove(TKey key) {
			if(ContainsKey(key)) {
                var index = 0;
				foreach(var kv in _kvArray) {
					if( kv.Key.Equals(key))
						break;
					index++;
				}
				var removeTarget = _kvArray[index];
				var result = _kvArray.Remove(removeTarget);
				Remove(key);
				return result;
			}
			return false;
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			if(Contains(item)) {
                var index = 0;
				foreach(var kv in _kvArray) {
					if( kv.Key.Equals(item.Key) && kv.Value.Equals(item.Value))
						break;
					index++;
				}
				var removeTarget = _kvArray[index];
				return _kvArray.Remove(removeTarget);
			}
			return false;
		}
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return ToDictionary().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ToDictionary().GetEnumerator();
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			foreach (var kv in _kvArray) {
				array.SetValue(kv, arrayIndex);
				arrayIndex++;
			}
		}
		void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context) {
			try {
				info.AddValue("_kvArray", _kvArray, typeof(List<TPair>));
			}
			catch (Exception e) {
				throw new SerializationException(e.Message, e);
			}
		}
		void IDeserializationCallback.OnDeserialization (object sender) {
		}
		void IDictionary.Add (object key, object value) {
			Add((TKey)key, (TValue)value);
		}
		bool IDictionary.Contains (object key) {
			return ContainsKey((TKey)key);
		}
		IDictionaryEnumerator IDictionary.GetEnumerator () {
			return ToDictionary().GetEnumerator();
		}
		void IDictionary.Remove (object key) {
			Remove((TKey)key);
		}
		void ICollection.CopyTo (Array array, int index) {
			foreach (var kv in _kvArray) {
				array.SetValue(kv, index);
				index++;
			}
		}
	}

	/// <summary>
	/// Base class for generating KeyValuePair that can be displayed in Inspector
	/// </summary>
	[Serializable]
	public abstract class SerializableKeyValuePairBase<TKey, TValue> {
		[SerializeField]
		private TKey _key;
		public TKey Key { get { return _key; } }
		[SerializeField]
		private TValue _value;
		public TValue Value { get { return _value; } }
        [SerializeField]
        protected bool _isIgnore = false;
        public SerializableKeyValuePairBase() { }
		public SerializableKeyValuePairBase(TKey key, TValue value) {
			_key = key;
			_value = value;
		}
		public SerializableKeyValuePairBase(KeyValuePair<TKey, TValue> pair) {
			_key = pair.Key;
			_value = pair.Value;
		}
		public static TPair Create<TPair>(TKey key, TValue value) where TPair : SerializableKeyValuePairBase<TKey, TValue>, new() {
			var result = new TPair();
            result._key = key;
            result._value = value;
			return result;
		}
	}

#if UNITY_2020_1_OR_NEWER
	[Serializable] public class SerializableKeyValuePair<TKey, TValue> : SerializableKeyValuePairBase<TKey, TValue> { }
	[Serializable] public class SerializableDictionary<TKey, TValue> : SerializableDictionaryBase<TKey, TValue, SerializableKeyValuePair<TKey, TValue>> { }
#endif
}