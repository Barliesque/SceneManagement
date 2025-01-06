using UnityEditor;
using UnityEngine;

namespace Barliesque.SceneManagement.Editor
{

    [CustomPropertyDrawer(typeof(SceneGroupIDAttribute))]
    public class SceneGroupIDPropertyDrawer : PropertyDrawer
    {
        static private SerializedProperty _field;
        
        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width -= 20f;
            EditorGUI.BeginProperty(position, label, property);
            property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
            EditorGUI.EndProperty();
            
            position.x += position.width;
            position.width = 20f;

            if (EditorGUI.DropdownButton(position, new GUIContent(), FocusType.Keyboard))
            {
                ShowDropdown(property);
            }
        }
        
        
        static public void ShowDropdown(SerializedProperty property)
        {
            _field = property;
            var selected = property.stringValue;
            var menu = new GenericMenu();
            
            var sceneLoader = Object.FindObjectOfType<SceneLoader>();
            if (!sceneLoader)
            {
                menu.AddDisabledItem(new GUIContent("No SceneLoader found!"));
            }
            else
            {
                var groups = SceneLoader.GroupNames;
                foreach (var group in groups)
                {
                    menu.AddItem(new GUIContent(group), group == selected, OnItemSelected, group);
                }
            }
            menu.ShowAsContext();
        }
        

        static private void OnItemSelected(object userdata)
        {
            _field.serializedObject.Update();
            _field.stringValue = (string)userdata;
            _field.serializedObject.ApplyModifiedProperties();
        }
         
    }

}