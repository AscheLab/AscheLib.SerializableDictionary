# AscheLib.SerializableDictionary
Created by Syunta Washidu (AscheLab)

## What's SerializableDictionary?
SerializableDictionary is an alternative class to System.Collections.Generic.Dictionary <TKey, TValue> class that can be displayed and edited in Inspector.

## Install
### Using UnityPackageManager
Find the manifest.json file in the Packages folder of your project and edit it to look like this.
```
"scopedRegistries": [
    {
      "name": "Unofficial Unity Package Manager Registry",
      "url": "https://upm-packages.dev",
      "scopes": [
        "com.aschelab"
      ]
    }
  ],
  "dependencies": {
    "com.aschelab.serializabledictionary": "1.4.1",
  ...
  }
```
### Using .unitypackage file
Please import this unitypackage.
https://github.com/AscheLab/AscheLib.SerializableDictionary/releases/latest/download/AscheLib.SerializableDictionary.unitypackage

## Using for SerializableDictionary
```csharp
using System;
using UnityEngine;
using AscheLib.Collections;
```
```csharp
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
```
If it is Unity 2020.1 or later, you can also write as follows
```csharp
// How to write Unity 2020.1 and above
public SerializableDictionary<string, int> _testDictionary;
private void Start() {
  if(!_testDictionary.ContainsKey("zzz")) {
    _testDictionary.Add("zzz", 2525);
  }
  int zzzValue = _testDictionary["zzz"];
  Debug.Log(zzzValue);
}
```

## Displayed on Inspector
![D1iYX-cU8AADGOD](https://user-images.githubusercontent.com/47095602/54342456-db90c680-467f-11e9-83b7-ae72eed0cbb0.png)

## License
This library is under the MIT License.