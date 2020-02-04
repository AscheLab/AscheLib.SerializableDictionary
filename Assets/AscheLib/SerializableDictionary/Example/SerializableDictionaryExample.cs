using System;
using UnityEngine;
using AscheLib.Collections;

public class SerializableDictionaryExample : MonoBehaviour {
#if UNITY_2020_1_OR_NEWER
	// How to write Unity 2020.1 and above
	public SerializableDictionary<string, int> _testDictionary;
#else
	// Define KeyValuePair class that can be displayed in Inspector (key: string, value: int)
	[Serializable] public class SerializableKeyValuePair : SerializableKeyValuePairBase<string, int> { }
	// Define Dictionary class that can be displayed in Inspector (key: string, value: int)
	[Serializable] public class SerializableDictionary : SerializableDictionaryBase<string, int, SerializableKeyValuePair> { }

	// Use the Dictionary class that can be displayed in the Inspector
	public SerializableDictionary _testDictionary;
#endif

	// It can be used like System.Collections.Generic.Dictionary<TKey, TValue> class
	private void Start() {
		if(!_testDictionary.ContainsKey("zzz")) {
			_testDictionary.Add("zzz", 2525);
		}
		int zzzValue = _testDictionary["zzz"];
		Debug.Log(zzzValue);
	}
}