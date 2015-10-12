/*
* Copyright (c) Mad Pixel Machine
* All Rights Reserved
*
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace EnergyBarToolkit {

public class EditorBase : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected void Indent(Runnable0 runnable) {
        EditorGUI.indentLevel++;
        runnable();
        EditorGUI.indentLevel--;
    }
    
    protected void Separator() {
        var rect = EditorGUILayout.BeginHorizontal();
        int indentPixels = (EditorGUI.indentLevel + 1) * 10 - 5;
        GUI.Box(new Rect(indentPixels, rect.yMin, rect.width - indentPixels, rect.height), "");
        EditorGUILayout.EndHorizontal();
    }

    protected void PropertyFieldWithChildren(string name) {
        var property = serializedObject.FindProperty(name);
        EditorGUILayout.BeginVertical();
        do {
            if (property.propertyPath != name && !property.propertyPath.StartsWith(name + ".") ) {
                break;
            }  
            EditorGUILayout.PropertyField(property);
        } while (property.NextVisible(true));
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
 
    protected bool InfoFix(string message) {
        return MessageFix(message, MessageType.Info);
    }
          
    protected bool WarningFix(string message) {
        return MessageFix(message, MessageType.Warning);
    }
    
    protected bool ErrorFix(string message) {
        return MessageFix(message, MessageType.Error);
    }
    
    protected bool MessageFix(string message, MessageType messageType) {
        return MessageWithButton(message, "Fix it", messageType);
    }
    
    protected bool MessageWithButton(string message, string buttonLabel, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        bool result = GUILayout.Button(buttonLabel);
        EditorGUILayout.EndHorizontal();
        
        return result;
    }
    
    protected void Warning(string message) {
        Message(message, MessageType.Warning);
    }
    
    protected void Info(string message) {
        Message(message, MessageType.Info);
    }
    
    protected void Message(string message, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
    }
    
    protected void BeginBox() {
        BeginBox("");
    }
    
    protected void BeginBox(string label) {
        if (!string.IsNullOrEmpty(label)) {
            GUILayout.Label(label);
        }
    
        var rect = EditorGUILayout.BeginVertical();
        
        int identPixels = (EditorGUI.indentLevel + 1) * 15 - 5;
        var rect2 = new Rect(identPixels, rect.yMin, rect.width - identPixels, rect.height);
        GUI.Box(rect2, GUIContent.none);
        
        EditorGUI.indentLevel++;
        EditorGUILayout.Space();
    }
    
    protected void EndBox() {
        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
    
//    private bool ex = false;
    
    protected void PropertyField(SerializedProperty obj, string label) {
        EditorGUILayout.PropertyField(obj, new GUIContent(label));
    }
    
    protected void PropertyField(SerializedProperty obj, string label, string tooltip) {
        EditorGUILayout.PropertyField(obj, new GUIContent(label, tooltip));
    }
    
    protected void PropertyFieldVector2(SerializedProperty obj, string label) {
        obj.vector2Value = EditorGUILayout.Vector2Field(label, obj.vector2Value);
    }
    
    protected void PropertyFieldToggleGroup(SerializedProperty obj, string label, Runnable0 runnable) {
        obj.boolValue = EditorGUILayout.BeginToggleGroup(label, obj.boolValue);
        
        runnable();
        EditorGUILayout.EndToggleGroup();
    }
    
    protected void PropertyFieldToggleGroup2(SerializedProperty obj, string label, Runnable0 runnable) {
        obj.boolValue = EditorGUILayout.Toggle(label, obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    protected void PropertyFieldToggleGroupInv2(SerializedProperty obj, string label, Runnable0 runnable) {
        obj.boolValue = !EditorGUILayout.Toggle(label, !obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = !obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    // allow to change coordinates from normalized back and forth
    // and try to convert it to match previous position
    protected bool PropertySpecialNormalized(SerializedProperty coords, SerializedProperty normalized) {
        var t = target as EnergyBarBase;
        return PropertySpecialNormalized(coords, normalized,
            new Vector2(t.storedScreenWidth, t.storedScreenHeight));
    }
    
    protected bool PropertySpecialNormalized(SerializedProperty coords, SerializedProperty normalized, Vector2 res) {
        bool normalizedBefore = normalized.boolValue;
        PropertyField(normalized, "Normalized");
        
        if (!normalizedBefore && normalized.boolValue) {
            coords.vector2Value = new Vector2(coords.vector2Value.x / res.x, coords.vector2Value.y / res.y);
        } else if (normalizedBefore && !normalized.boolValue) {
            coords.vector2Value = new Vector2(coords.vector2Value.x * res.x, coords.vector2Value.y * res.y);
        }
        
        return normalized.boolValue;
    }
    
    protected bool Foldout(string name, bool defaultState) {
        bool state = EditorPrefs.GetBool(name, defaultState);
        
        bool newState = EditorGUILayout.Foldout(state, name);
        if (newState != state) {
            EditorPrefs.SetBool(name, newState);
        }
        
        return newState;
    }
    
    protected Vector2 RountToInt(Vector2 v) {
        var x = Mathf.RoundToInt(v.x);
        var y = Mathf.RoundToInt(v.y);
        return new Vector2(x, y);
    }
    
    protected Vector3 RountToInt(Vector3 v) {
        var x = Mathf.RoundToInt(v.x);
        var y = Mathf.RoundToInt(v.y);
        var z = Mathf.RoundToInt(v.z);
        return new Vector3(x, y, z);
    }
    
    protected void ArrayList(SerializedProperty property, string title) {
        ArrayList(property, title, (prop) => { PropertyField(prop, ""); });
    }
    
    protected void ArrayList(SerializedProperty property, string title, Runnable1<SerializedProperty> renderer) {
        if (Foldout(title, false)) {
            Indent(() => {
                if (property.arraySize == 0) {
                    GUILayout.Label("   Use 'Add' button to add items");
                } else {
                    int arrSize = property.arraySize;
                    Separator();
                    for (int i = 0; i < arrSize; ++i) {
                        var go = property.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.BeginVertical();
                        renderer(go);
                        EditorGUILayout.EndVertical();
                        
                        GUI.color = Color.red;
                        if (GUILayout.Button("X", GUILayout.ExpandWidth(false))) {
                            property.DeleteArrayElementAtIndex(i);
                            arrSize--;
                        }
                        GUI.color = Color.white;
                        EditorGUILayout.EndHorizontal();
                        
                        if (i + 1 < arrSize) {
                            EditorGUILayout.Space();
                        }
                        Separator();
                    }
                }
                
                GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add", GUILayout.ExpandWidth(false))) {
                    property.InsertArrayElementAtIndex(property.arraySize);
      
                    // when creating new array element like this, the color will be initialized with
                    // (0, 0, 0, 0) - zero aplha. This may be confusing for end user so this workaround looks
                    // for color fields and sets them to proper values                  
                    var element = property.GetArrayElementAtIndex(property.arraySize - 1);                  
                    var enumerator = element.GetEnumerator();
                    while (enumerator.MoveNext()) {
                        var el = enumerator.Current as SerializedProperty;
                        if (el.type == "ColorRGBA") {
                            el.colorValue = Color.white;
                        }
                    }
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            });
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    protected delegate void Runnable0();
    protected delegate void Runnable1<T>(T t);
    protected delegate T Runnable<T>();
    protected delegate void Updater<T1, T2>(ref T1 o, T2 after);

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace