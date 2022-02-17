using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

namespace AscheLib.Collections {
	/// <summary>
	/// PropertyDrawer used by SerializableDictionary
	/// </summary>
	[CustomPropertyDrawer(typeof(DrawableSerializableDictionaryBase), true)]
	public class SerializableDictionaryInspectorDisplayDrawer : PropertyDrawer {
		private const string WarningMessage = "Duplicate key exists\r\nThis value will be ignored.";
		ReorderableList _reorderableList;
        Dictionary<int, bool> _ignoreDictionary = new Dictionary<int, bool>();

        public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label) {
            label = EditorGUI.BeginProperty(position, label, serializedProperty);
            var list = GetList(serializedProperty, label);
            var listProperty = serializedProperty.FindPropertyRelative("_kvArray");

            var height = 0f;
			for(var i = 0; i < listProperty.arraySize; i++) {
				height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
			}
			list.elementHeight = height;
			list.DoList(position);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label) {
			var listHeight = GetList(serializedProperty, label).GetHeight();
			return listHeight;
		}

		private ReorderableList GetList(SerializedProperty serializedProperty, GUIContent label) {
            UpdateIgnoreDictionary(serializedProperty);
			if (_reorderableList == null) {
                var listProperty = serializedProperty.FindPropertyRelative("_kvArray");
                var labelText = label.text;
				_reorderableList = new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true);
				_reorderableList.drawElementCallback += (rect, index, selected, focused) => {
                    var property = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    DrawKeyValue(rect, property, GUIContent.none, _ignoreDictionary[index]);
                };
				_reorderableList.drawHeaderCallback += rect => {
					EditorGUI.LabelField(rect, labelText);
				};
				_reorderableList.elementHeightCallback += (index) => {
                    var property = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    return GetKeyValueHeight(property, label, _ignoreDictionary[index]);
				};
				_reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, serializedProperty.displayName);
			}

			return _reorderableList;
		}

        private void UpdateIgnoreDictionary(SerializedProperty serializedProperty) {
			BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			var parentInfo = GetFieldInfoFromSerializedProperty(serializedProperty, bindingAttr);
			var parentValue = GetValueFromSerializedProperty(serializedProperty, bindingAttr);
            var dictionaryType = parentValue.GetType();
            var kvArrayInfo = GetSuperClassGetField(dictionaryType, "_kvArray", bindingAttr);
            var kvArray = (IList)kvArrayInfo.GetValue(parentValue);

            var keyList = new List<object>();
            var count = 0;
            foreach (var kv in kvArray) {
                var keyInfo = GetSuperClassGetField(kv.GetType(), "_key", bindingAttr);
                var key = keyInfo.GetValue(kv);
                _ignoreDictionary[count] = keyList.Contains(key);
                keyList.Add(key);
                count++;
            }
        }

        private FieldInfo GetSuperClassGetField(Type type, string name, BindingFlags bindingAttr) {
            var info = type.GetField(name, bindingAttr);
            if (info != null)
                return info;
            else if (type.BaseType != null)
                return GetSuperClassGetField(type.BaseType, name, bindingAttr);
            return null;
        }

		private FieldInfo GetFieldInfoFromSerializedProperty(SerializedProperty property, BindingFlags bindingAttr) {
			object obj = property.serializedObject.targetObject;
			foreach (var path in property.propertyPath.Split('.')) {
				var type = obj.GetType();
				var field = type.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (field != null)
					return field;
			}
			return null;
		}

		private object GetValueFromSerializedProperty(SerializedProperty property, BindingFlags bindingAttr) {
			object obj = property.serializedObject.targetObject;
			foreach (var path in property.propertyPath.Split('.')) {
				var type = obj.GetType();
				var field = type.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				obj = field.GetValue(obj);
			}
			return obj;
		}

		private void DrawKeyValue(Rect position, SerializedProperty serializedProperty, GUIContent label, bool isIgnore) {
            label = EditorGUI.BeginProperty(position, label, serializedProperty);
            var keyProperty = serializedProperty.FindPropertyRelative("_key");
            var valueProperty = serializedProperty.FindPropertyRelative("_value");

            var keyHeight = EditorGUI.GetPropertyHeight(keyProperty, label);
            var valueHeight = EditorGUI.GetPropertyHeight(valueProperty, label);
            var helpBoxHeight = isIgnore ? GetHelpBoxHeight() : 0;

            var backgroundRect = new Rect(position.x, position.y, position.width, keyHeight + valueHeight + helpBoxHeight);
			GUI.Box(backgroundRect, GUIContent.none, new GUIStyle("CN Box"));

            var currentPosition = position.y;
			EditorGUI.PropertyField(new Rect(position.x + 13, currentPosition, position.width -13, keyHeight), keyProperty, true);
			currentPosition += keyHeight;

			EditorGUI.PropertyField(new Rect(position.x + 13, currentPosition, position.width -13, valueHeight), valueProperty, true);
            currentPosition += valueHeight;

            if (isIgnore) {
                EditorGUI.HelpBox(new Rect(position.x + 13, currentPosition, position.width - 13, helpBoxHeight), WarningMessage, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private float GetKeyValueHeight(SerializedProperty serializedProperty, GUIContent label, bool isIgnore) {
            var keyProperty = serializedProperty.FindPropertyRelative("_key");
            var valueProperty = serializedProperty.FindPropertyRelative("_value");
            var isIgnoreProperty = serializedProperty.FindPropertyRelative("_isIgnore");

            var keyHeight = EditorGUI.GetPropertyHeight(keyProperty, label);
            var valueHeight = EditorGUI.GetPropertyHeight(valueProperty, label);
            var helpBoxHeight = isIgnore ? GetHelpBoxHeight() : 0;

            return keyHeight + valueHeight + helpBoxHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private float GetHelpBoxHeight() {
            var style = new GUIStyle("HelpBox");
            var content = new GUIContent(WarningMessage);
            return Mathf.Max(style.CalcHeight(content, Screen.width - 53), 40);
        }
    }
}