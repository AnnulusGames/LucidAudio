using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AnnulusGames.LucidTools.Audio
{
    internal static class AudioEditorUtil
    {
        public static float headerHeight => EditorGUIUtility.singleLineHeight * 1.4f;
        public static float fieldHeight => EditorGUIUtility.singleLineHeight * 1.13f;

        public static void DrawBox(Rect rect)
        {
            EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox);
        }

        public static void Field(ref Rect position, SerializedProperty property)
        {
            Field(ref position, property, fieldHeight);
        }

        public static void Field(ref Rect position, SerializedProperty property, float height)
        {
            EditorGUI.PropertyField(position, property);
            position.y += height;
        }

        public static void Field(ref Rect position, SerializedProperty property, string label)
        {
            Field(ref position, property, label, fieldHeight);
        }

        public static void Field(ref Rect position, SerializedProperty property, string label, float height)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(label));
            position.y += height;
        }

        public static bool FoldoutGroup(Rect position, bool foldout, SerializedProperty property, SerializedProperty fieldProperty, GUIContent label, float height)
        {
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            
            Rect boxRect = position;
            boxRect.xMin -= 2f;
            boxRect.height = height;
            AudioEditorUtil.DrawBox(boxRect);

            boxRect.height = headerHeight;
            AudioEditorUtil.DrawBox(boxRect);

            position.xMin += 10f;
            EditorGUIUtility.labelWidth -= 10f;

            Rect foldoutRect = position;
            foldoutRect.height = headerHeight;
            foldoutRect.xMin += 4f;
            if (!foldout && fieldProperty != null) foldoutRect.xMax -= foldoutRect.width * 0.6f;

            bool value = EditorGUI.Foldout(
                foldoutRect,
                foldout,
                label,
                true,
                EditorStyles.foldout
            );

            if (fieldProperty != null)
            {
                Rect fieldRect = position;
                fieldRect.height = EditorGUIUtility.singleLineHeight;
                fieldRect.y += 3.5f;
                fieldRect.xMax -= 2.5f;
                AudioEditorUtil.Field(ref fieldRect, fieldProperty, " ");
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            return value;
        }
    }
}
