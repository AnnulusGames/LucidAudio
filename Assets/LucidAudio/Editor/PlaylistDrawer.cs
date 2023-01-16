using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AnnulusGames.LucidTools.Audio
{
    [CustomPropertyDrawer(typeof(Playlist))]
    class PlaylistDrawer : PropertyDrawer
    {
        private ReorderableList reorderableList;

        private void Init(SerializedProperty property)
        {
            if (reorderableList == null)
            {
                SerializedProperty listProperty = property.FindPropertyRelative("list");
                reorderableList = new ReorderableList(property.serializedObject, listProperty);
                reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "List");

                reorderableList.elementHeightCallback = index => EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(index));

                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(index);
                    rect.height = EditorGUI.GetPropertyHeight(elementProperty);
                    EditorGUI.PropertyField(rect, elementProperty);
                };
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            SerializedProperty displayNameProperty = property.FindPropertyRelative("displayName");

            property.isExpanded = AudioEditorUtil.FoldoutGroup(
                position,
                property.isExpanded,
                property,
                property.FindPropertyRelative("clip"),
                string.IsNullOrEmpty(displayNameProperty.stringValue) ? label : new GUIContent(displayNameProperty.stringValue),
                GetPropertyHeight(property, label)
            );

            if (property.isExpanded)
            {
                position.xMin += 10f;
                EditorGUIUtility.labelWidth -= 10f;
                position.y += AudioEditorUtil.headerHeight + 3f;
                position.xMax -= 2.5f;
                position.height = EditorGUIUtility.singleLineHeight;

                AudioEditorUtil.Field(ref position, displayNameProperty);
                position.y += 3f;

                AudioEditorUtil.Field(ref position, property.FindPropertyRelative("audioType"));

                position.y += 6f;
                position.xMin -= 5f;
                EditorGUIUtility.labelWidth += 5f;
                reorderableList.DoList(position);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);
            if (property.isExpanded)
            {
                return AudioEditorUtil.headerHeight + reorderableList.GetHeight() + 17f + AudioEditorUtil.fieldHeight * 2;
            }
            else
            {
                return AudioEditorUtil.headerHeight;
            }
        }
    }

}