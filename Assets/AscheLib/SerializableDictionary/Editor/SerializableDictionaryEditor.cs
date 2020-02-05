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
		private const string WarningMessage = "The same key is registered.\r\nIf the same key is registered, only the data registered earlier can be obtained.";
		ReorderableList _reorderableList;

		public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label) {
			label = EditorGUI.BeginProperty(position, label, serializedProperty);
			SerializedProperty listProperty = serializedProperty.FindPropertyRelative("_kvArray");
			ReorderableList list = GetList(listProperty, label);

			float height = 0f;
			for(var i = 0; i < listProperty.arraySize; i++) {
				height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
			}
			list.elementHeight = height;
			list.DoList(position);
			EditorGUI.EndProperty();

			if(HasDuplicationKey(serializedProperty)) {
				position.y += list.GetHeight();
				position.height = GetHelpBoxHeight();
				EditorGUI.HelpBox(position, WarningMessage, MessageType.Warning);
			}
		}

		public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label) {
			SerializedProperty listProperty = serializedProperty.FindPropertyRelative("_kvArray");
			var listHeight = GetList(listProperty, label).GetHeight();
			return HasDuplicationKey(serializedProperty) ? listHeight + GetHelpBoxHeight() : listHeight;
		}

		private ReorderableList GetList(SerializedProperty serializedProperty, GUIContent label) {
			if (_reorderableList == null) {
				string labelText = label.text;
				_reorderableList = new ReorderableList(serializedProperty.serializedObject, serializedProperty, true, true, true, true);
				_reorderableList.drawElementCallback += (Rect rect, int index, bool selected, bool focused) => {
					SerializedProperty property = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
					EditorGUI.PropertyField(rect, property, GUIContent.none);
				};
				_reorderableList.drawHeaderCallback += rect => {
					EditorGUI.LabelField(rect, labelText);
				};
				_reorderableList.elementHeightCallback += (int index) => {
					SerializedProperty property = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
					return EditorGUI.GetPropertyHeight(property, label);
				};
			}

			return _reorderableList;
		}

		private bool HasDuplicationKey(SerializedProperty serializedProperty) {
			Type parentType = serializedProperty.serializedObject.targetObject.GetType();
			FieldInfo fieldInfo = parentType.GetField(serializedProperty.propertyPath);
			var value = (DrawableSerializableDictionaryBase)fieldInfo.GetValue(serializedProperty.serializedObject.targetObject);
			return value.HasDuplicationKey;
		}
		private float GetHelpBoxHeight () {
			var style = new GUIStyle("HelpBox");
			var content = new GUIContent(WarningMessage);
			return Mathf.Max(style.CalcHeight(content, Screen.width - 53), 40);
		}
	}

	/// <summary>
	/// PropertyDrawer used by SerializableKeyValuePair
	/// </summary>
	[CustomPropertyDrawer(typeof(DrawableSerializableKeyValuePairBase), true)]
	public class SerializableKeyValuePairInspectorDisplayDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label) {
			label = EditorGUI.BeginProperty(position, label, serializedProperty);
			SerializedProperty keyProperty = serializedProperty.FindPropertyRelative("_key");
			SerializedProperty valueProperty = serializedProperty.FindPropertyRelative("_value");

			float keyHeight = EditorGUI.GetPropertyHeight(keyProperty, label);
			float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, label);

			Rect backgroundRect = new Rect(position.x, position.y, position.width, keyHeight + valueHeight);
			GUI.Box(backgroundRect, GUIContent.none, new GUIStyle("CN Box"));

			float currentPosition = position.y;
			EditorGUI.PropertyField(new Rect(position.x + 13, currentPosition, position.width -13, keyHeight), keyProperty, true);
			currentPosition += keyHeight;

			EditorGUI.PropertyField(new Rect(position.x + 13, currentPosition, position.width -13, valueHeight), valueProperty, true);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label) {
			SerializedProperty keyProperty = serializedProperty.FindPropertyRelative("_key");
			SerializedProperty valueProperty = serializedProperty.FindPropertyRelative("_value");

			float keyHeight = EditorGUI.GetPropertyHeight(keyProperty, label);
			float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, label);

			return keyHeight + valueHeight + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}