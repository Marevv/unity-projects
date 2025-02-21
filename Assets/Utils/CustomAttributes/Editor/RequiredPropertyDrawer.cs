using UnityEditor;
using UnityEngine;

namespace CustomAttributes
{
    [CustomPropertyDrawer(typeof(RequiredFieldAttribute))]
    public class RequiredPropertyDrawer : PropertyDrawer
    {
        Texture2D requiredIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Utils/CustomAttributes/Icons/ErrorIcon.png");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            Rect fieldRect = new Rect(position.x, position.y, position.width - 20, position.height);
            EditorGUI.PropertyField(fieldRect, property, label);

            if(IsFieldUnassignes(property))
            {
                Rect iconRect =  new(position.xMin - 16f, fieldRect.y + 1 , 16, 16);
                GUI.Label(iconRect, new GUIContent(requiredIcon, "This field is required and is either missing or empty!"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                EditorApplication.RepaintHierarchyWindow();
            }

            EditorGUI.EndProperty();
        }

        bool IsFieldUnassignes(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference when property.objectReferenceValue:
                case SerializedPropertyType.ExposedReference when property.exposedReferenceValue:
                case SerializedPropertyType.AnimationCurve when property.animationCurveValue is { length: > 0 }:
                case SerializedPropertyType.String when !string.IsNullOrEmpty(property.stringValue):
                    return false;
                default:
                    return true;
            }
        }
    }
}
