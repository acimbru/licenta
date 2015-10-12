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

[CustomEditor(typeof(SimpleEvent))]
public class SimpleEventInspector : EditorBase {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty energyBar;
    SerializedProperty targetType;
    SerializedProperty targetGameObjects;
    SerializedProperty targetTags;
    SerializedProperty onTriggerEnterAction;
    SerializedProperty onTriggerStayAction;
    SerializedProperty onTriggerLeaveAction;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        energyBar = serializedObject.FindProperty("energyBar");
        targetType = serializedObject.FindProperty("targetType");
        targetGameObjects = serializedObject.FindProperty("targetGameObjects");
        targetTags = serializedObject.FindProperty("targetTags");
        onTriggerEnterAction = serializedObject.FindProperty("onTriggerEnterAction");
        onTriggerStayAction = serializedObject.FindProperty("onTriggerStayAction");
        onTriggerLeaveAction = serializedObject.FindProperty("onTriggerLeaveAction");
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        var t = target as SimpleEvent;
        var collider = t.GetComponent<Collider>();
        if (collider != null) {
            if (!collider.isTrigger) {
                if (ErrorFix("This game object collider must be marked as 'Trigger'. Change it?")) {
                    collider.isTrigger = true;
                }
            }
        } else {
            if (ErrorFix("This game object doesn't have a collider. Attach it now?")) {
                collider = t.gameObject.AddComponent<MeshCollider>();
                collider.isTrigger = true;
            }
        }
    
        PropertyField(energyBar, "Energy Bar");
        PropertyField(targetType, "Trigger With");
        
        Indent(() => {
            switch ((SimpleEvent.Target) targetType.enumValueIndex) {
                case SimpleEvent.Target.GameObjects:
                    GUITargetGameObjects();
                    break;
                case SimpleEvent.Target.Tags:
                    GUITargetTags();
                    break;
                default:
                    Debug.LogError("Unknown option: " + targetType.enumValueIndex);
                    break;
            }
        });
        
        if (Foldout("On Trigger Enter", false)) {
            Indent(() => {
                GUIAction(onTriggerEnterAction, false);
            });
        }
        
        if (Foldout("On Trigger Stay", false)) {
            Indent(() => {
                GUIAction(onTriggerStayAction, true);
            });
        }
        
        if (Foldout("On Trigger Leave", false)) {
            Indent(() => {
                GUIAction(onTriggerLeaveAction, false);
            });
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    void GUITargetGameObjects() {
        ArrayList(targetGameObjects, "Game Object List");
    }
    
    void GUITargetTags() {
        ArrayList(targetTags, "Tag List", (prop) => {
            prop.stringValue = EditorGUILayout.TagField(prop.stringValue);
        });
    }
    
    void GUIAction(SerializedProperty prop, bool withInterval) {
        var changeBar = prop.FindPropertyRelative("changeBar");
        var changeBarType = prop.FindPropertyRelative("changeBarType");
        var changeBarValue = prop.FindPropertyRelative("changeBarValue");
        var sendMessage = prop.FindPropertyRelative("sendMessage");
        var intervaled = prop.FindPropertyRelative("intervaled");
        var timeInterval = prop.FindPropertyRelative("timeInterval");
        var signals = prop.FindPropertyRelative("signals");
        
        PropertyFieldToggleGroup(changeBar, "Change Bar", () => {
            Indent(() => {
                PropertyField(changeBarType, "Change Method");
                Indent(() => {
                    string example;
                    switch ((SimpleEvent.Action.Type) changeBarType.enumValueIndex) {
                        case SimpleEvent.Action.Type.DecreaseByPercent:
                        case SimpleEvent.Action.Type.IncreaseByPercent:
                        case SimpleEvent.Action.Type.SetToPercent:
                            example = " (0.0 - 1.0)";
                            break;
                        default:
                            example = "";
                            break;
                    }
                    PropertyField(changeBarValue, "Value" + example);
                    
                    intervaled.boolValue = withInterval;
                    if (withInterval) {
                        PropertyField(timeInterval, "Time Interval");
                    }
                });
            });
        });
        
        PropertyFieldToggleGroup(sendMessage, "Send Message", () => {
            Indent(() => {
                ArrayList(signals, "Signals", (signalProp) => {
                    GUISignal(signalProp);
                });
            });
        });
    }
    
    void GUISignal(SerializedProperty prop) {
        var receiverType = prop.FindPropertyRelative("receiverType");
        var receiver = prop.FindPropertyRelative("receiver");
        var methodName = prop.FindPropertyRelative("methodName");
        var argument = prop.FindPropertyRelative("argument");
        
        PropertyField(receiverType, "Receiver Type");
        if (receiverType.enumValueIndex == (int) SimpleEvent.Signal.ReceiverType.FixedGameObject) {
            Indent(() => {
                PropertyField(receiver, "Receiver Object");
            });
        }
        PropertyField(methodName, "Method Name");
        PropertyField(argument, "Method Argument");
        
        string argumentStr;
        switch ((SimpleEvent.Signal.MessageArgument) argument.intValue) {
            case SimpleEvent.Signal.MessageArgument.BarValue:
                argumentStr = "int value";
                break;
            case SimpleEvent.Signal.MessageArgument.BarValuePercent:
                argumentStr = "float valuePercent";
                break;
            case SimpleEvent.Signal.MessageArgument.Caller:
                argumentStr = "GameObject caller";
                break;
            default:
                Debug.LogError("Unknown option: " + argument.intValue);
                argumentStr = "-error-";
                break;
        }
        
        Info("Receiver: OnEvent(" + argumentStr + ")");
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace