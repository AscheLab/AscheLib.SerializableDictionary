using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
		IDictionary<TKey, TValue>
		where TPair : SerializableKeyValuePairBase<TKey, TValue>,
		new() {
		[SerializeField]
		private List<TPair> _kvArray = new List<TPair>();

		public TValue this[TKey key] {
			set {
				if(ContainsKey(key)) {
					int index = 0;
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

		public int Count { get { return _kvArray.Count; } }
		public ICollection<TKey> Keys { get { return _kvArray.Select(pair => pair.Key).ToList(); } }
		public ICollection<TValue> Values { get { return _kvArray.Select(pair => pair.Value).ToList(); } }
		public bool IsReadOnly { get { return false; } }
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
				result.Add(kv.Key, kv.Value);
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
				int index = 0;
				foreach(var kv in _kvArray) {
					if( kv.Key.Equals(key))
						break;
					index++;
				}
				var removeTarget = _kvArray[index];
				return _kvArray.Remove(removeTarget);
			}
			return false;
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			if(Contains(item)) {
				int index = 0;
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
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Base class for PropertyDrawer of SerializableKeyValuePair
	/// </summary>
	[Serializable]
	public abstract class DrawableSerializableKeyValuePairBase { }

	/// <summary>
	/// Base class for generating KeyValuePair that can be displayed in Inspector
	/// </summary>
	[Serializable]
	public abstract class SerializableKeyValuePairBase<TKey, TValue> : DrawableSerializableKeyValuePairBase {
		[SerializeField]
		private TKey _key;
		public TKey Key { get { return _key; } }
		[SerializeField]
		private TValue _value;
		public TValue Value { get { return _value; } }
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
			TPair newPair = new TPair();
			newPair._key = key;
			newPair._value = value;
			return newPair;
		}
	}
}