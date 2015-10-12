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

public class MadGUI {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    static Color Darker(Color color) {
        return new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f);
    }
    
    static Color Brighter(Color color) {
        return new Color(color.r * 1.1f, color.g * 1.1f, color.b * 1.1f);
    }
    
    static Color ToggleColor(Color color, ref bool last) {
        if (last) {
            last = false;
            return Darker(color);
        } else {
            last = true;
//            return Brighter(color);
            return color;
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
#region ScrollableList
    public class ScrollableList<T> where T : ScrollableListItem {
        public string label = "Scrollable List";
        public int height = 200;
        
        public bool selectionEnabled = false;
        public string emptyListMessage = "No elements!";
        
        RunnableVoid1<T> _selectionCallback = (arg1) => {};
        public RunnableVoid1<T> selectionCallback {
            get { return _selectionCallback; }
            set { _selectionCallback += value; }
        }
        
        T _selectedItem;
        public T selectedItem {
            get { return _selectedItem; }
            set { _selectedItem = value; selectionCallback(value); }
        }
        
        Vector2 position;
        GUISkin skin;
        
        public ScrollableList() {
            skin = Resources.Load("GUISkins/editorSkin", typeof(GUISkin)) as GUISkin;
        }
        
        public void Draw(List<T> items) {
            bool toggleColor = false;
            Color baseColor = GUI.color;
        
            GUILayout.Label(label);
            position = EditorGUILayout.BeginScrollView(
                position, false, true, GUILayout.Height(height));
                
            foreach (var item in items) {
                var rect = EditorGUILayout.BeginHorizontal();
                if (selectionEnabled) {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                        if (rect.Contains(Event.current.mousePosition)) {
                            DeselectAll(items);
                            item.selected = true;
                            selectedItem = item;
                        }
                    }
                }
                
                GUI.color = toggleColor ? Color.clear : new Color(1, 1, 1, 0.2f);
                toggleColor = !toggleColor;
                if (item.selected) {
                    GUI.color = toggleColor ? new Color(1, 1, 1, 0.4f) : new Color(1, 1, 1, 0.6f);
                }
                
                GUI.Box(rect, "", skin.box);
                GUI.color = baseColor;
                
                EditorGUILayout.BeginVertical();
                item.OnGUI();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            
            if (items.Count == 0) {
                GUILayout.Label(emptyListMessage);
            }
                
            EditorGUILayout.EndScrollView();
            
            GUI.color = baseColor;
        }
        
        void DeselectAll(List<T> items) {
            foreach (var i in items) {
                i.selected = false;
            }
        }
    };
    
    public abstract class ScrollableListItem {
        public bool selected;
        
        public abstract void OnGUI();
    }
    
    public class ScrollableListItemLabel : ScrollableListItem {
        public string label;
        
        public ScrollableListItemLabel(string label) {
            this.label = label;
        }
        
        public override void OnGUI() {
            EditorGUILayout.LabelField(label);
        }
    }
    
#endregion
#region Wrappers
    public static void Box(RunnableVoid0 runnable) {
        Box("", runnable);
    }
    
    public static void Box(string label, RunnableVoid0 runnable) {
        BeginBox(label);
        runnable();
        EndBox();
    }
          
    public static void BeginBox() {
        BeginBox("");
    }
    
    public static void BeginBox(string label) {
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < EditorGUI.indentLevel; ++i) {
            EditorGUILayout.Space();
        }
        var rect = EditorGUILayout.BeginVertical();
        
        GUI.Box(rect, GUIContent.none);
        if (!string.IsNullOrEmpty(label)) {
            GUILayout.Label(label, "BoldLabel");
        }
        
        EditorGUILayout.Space();
    }
    
    public static void EndBox() {
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    
    public static void Indent(RunnableVoid0 runnable) {
        EditorGUI.indentLevel++;
        runnable();
        EditorGUI.indentLevel--;
    }
    
    public static void IndentBox(RunnableVoid0 runnable) {
        IndentBox("", runnable);
    }
    
    public static void IndentBox(string label, RunnableVoid0 runnable) {
        Box(label, () => {
            Indent(() => {
                runnable();
            });
        });
    }
    
    public static void Disabled(RunnableVoid0 runnable) {
        ConditionallyEnabled(false, runnable);
    }
    
    public static void ConditionallyEnabled(bool enabled, RunnableVoid0 runnable) {
        bool prevState = GUI.enabled;
        GUI.enabled = enabled;
        runnable();
        GUI.enabled = prevState;
    }
#endregion
#region Messages
    public static bool InfoFix(string message) {
        return MessageFix(message, MessageType.Info);
    }
    
    public static bool InfoFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Info);
    }
          
    public static bool WarningFix(string message) {
        return MessageFix(message, MessageType.Warning);
    }
    
    public static bool ErrorFix(string message) {
        return MessageFix(message, MessageType.Error);
    }
    
    public static bool ErrorFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Error);
    }
    
    public static bool MessageFix(string message, MessageType messageType) {
        return MessageWithButton(message, "Fix it", messageType);
    }
    
    public static bool MessageFix(string message, string fixMessage, MessageType messageType) {
        return MessageWithButton(message, fixMessage, messageType);
    }
    
    public static bool MessageWithButton(string message, string buttonLabel, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        bool result = GUILayout.Button(buttonLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        return result;
    }
    
    public static void Warning(string message) {
        Message(message, MessageType.Warning);
    }
    
    public static void Info(string message) {
        Message(message, MessageType.Info);
    }
    
    public static void Error(string message) {
        Message(message, MessageType.Error);
    }
    
    public static void Message(string message, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
    }
#endregion
    
    public delegate void RunnableVoid0();
    public delegate void RunnableVoid1<T>(T arg1);
}

} // namespace