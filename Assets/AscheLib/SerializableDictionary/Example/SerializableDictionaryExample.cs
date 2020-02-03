using System;
using UnityEngine;
using AscheLib.Collections;

public class SerializableDictionaryExample : MonoBehaviour {
	// Define KeyValuePair class that can be displayed in Inspector (key: string, value: int)
	[Serializable] public class SerializableKeyValuePair : SerializableKeyValuePairBase<string, int> { }
	// Define Dictionary class that can be displayed in Inspector (key: string, value: int)
	[Serializable] public class SerializableDictionary : SerializableDictionaryBase<string, int, SerializableKeyValuePair> { }

	// Use the Dictionary class that can be displayed in the Inspector
	public SerializableDictionary _testDictionary;

	// It can be used like System.Collections.Generic.Dictionary<TKey, TValue> class
	private void Start() {
		if(!_testDictionary.ContainsKey("zzz")) {
			_testDictionary.Add("zzz", 2525);
		}
		int zzzValue = _testDictionary["zzz"];
		Debug.Log(zzzValue);
	}
}