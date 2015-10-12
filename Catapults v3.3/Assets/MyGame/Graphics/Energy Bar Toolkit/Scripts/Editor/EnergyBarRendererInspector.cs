/*
* Copyright (c) 2013 Mad Pixel Machine
* All Rights Reserved
*
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EnergyBarRenderer))]
public class EnergyBarRendererInspector : EnergyBarInspectorBase {

    // ===========================================================
    // Constants
    // ===========================================================
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    private SerializedProperty textureBar;
    
    private SerializedProperty textureBarColorType;
    private SerializedProperty textureBarColor;
    private SerializedProperty textureBarGradient;
    
    private SerializedProperty screenPosition;
    private SerializedProperty screenPositionNormalized;
    private SerializedProperty screenPositionCalculateSize;
    private SerializedProperty size;
    private SerializedProperty sizeNormalized;
    
    private SerializedProperty growDirection;
    
    private SerializedProperty radialOffset;
    private SerializedProperty radialLength;
    
    private SerializedProperty effectBurn;
    private SerializedProperty effectBurnTextureBar;
    private SerializedProperty effectBurnTextureBarColor;
    
    private SerializedProperty effectBlink;
    private SerializedProperty effectBlinkValue;
    private SerializedProperty effectBlinkRatePerSecond;
    private SerializedProperty effectBlinkColor;

    // ===========================================================
    // Constructors (Including Static Constructors)
    // ===========================================================

    // ===========================================================
    // Getters / Setters
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    new public void OnEnable() {
        base.OnEnable();
        
        textureBar = serializedObject.FindProperty("textureBar");
        
        textureBarColorType = serializedObject.FindProperty("textureBarColorType");
        textureBarColor = serializedObject.FindProperty("textureBarColor");
        textureBarGradient = serializedObject.FindProperty("textureBarGradient");
        
        screenPosition = serializedObject.FindProperty("screenPosition");
        screenPositionNormalized = serializedObject.FindProperty("screenPositionNormalized");
        screenPositionCalculateSize = serializedObject.FindProperty("screenPositionCalculateSize");
        size = serializedObject.FindProperty("size");
        sizeNormalized = serializedObject.FindProperty("sizeNormalized");
        
        growDirection = serializedObject.FindProperty("growDirection");
        
        radialOffset = serializedObject.FindProperty("radialOffset");
        radialLength = serializedObject.FindProperty("radialLength");
        
        effectBurn = serializedObject.FindProperty("effectBurn");
        effectBurnTextureBar = serializedObject.FindProperty("effectBurnTextureBar");
        effectBurnTextureBarColor = serializedObject.FindProperty("effectBurnTextureBarColor");
        
        effectBlink = serializedObject.FindProperty("effectBlink");
        effectBlinkValue = serializedObject.FindProperty("effectBlinkValue");
        effectBlinkRatePerSecond = serializedObject.FindProperty("effectBlinkRatePerSecond");
        effectBlinkColor = serializedObject.FindProperty("effectBlinkColor");
        
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        var t = target as EnergyBarRenderer;
        
        if (Foldout("Textures", true)) {
            BeginBox();
            FieldBackgroundTextures();
            
            EditorGUILayout.PropertyField(textureBar, new GUIContent("Bar Texture"));         
            CheckTextureIsReadable(t.textureBar);
            CheckTextureFilterTypeNotPoint(t.textureBar);
            
            FieldForegroundTextures();
            
            FieldPremultipliedAlpha();
            EndBox();
        }
        
        if (Foldout("Position & Size", true)) {
            BeginBox();
            
            PropertyFieldVector2(screenPosition, "Position");
            
            EditorGUI.indentLevel++;
            PropertySpecialNormalized(screenPosition, screenPositionNormalized);
            PropertyField(pivot, "Pivot");
            PropertyField(anchorObject, "Anchor");
            PropertyField(anchorCamera, "Anchor Camera", "Camera on which world coordinates will be translated to "
                + "screen coordinates.");
            EditorGUI.indentLevel--;
            
            PropertyField(guiDepth, "GUI Depth");
            
//            screenPositionCalculateSize.boolValue = !EditorGUILayout.BeginToggleGroup("Manual Size", !screenPositionCalculateSize.boolValue);
            PropertyFieldToggleGroupInv2(screenPositionCalculateSize, "Manual Size", () => {
                Indent(() => {
                    PropertyFieldVector2(size, "Size");
                    EditorGUI.indentLevel++;
                    PropertySpecialNormalized(size, sizeNormalized);
                    EditorGUI.indentLevel--;
                });
            });
            
            FieldRelativeToTransform();
            EndBox();
        }
        
        if (Foldout("Appearance", false)) {
            BeginBox();
            
            var dir = (EnergyBarRenderer.GrowDirection) growDirection.enumValueIndex;
        
            if (dir == EnergyBarRenderer.GrowDirection.ColorChange) {
                GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(textureBarColorType, new GUIContent("Bar Color Type"));
            EditorGUI.indentLevel++;
                switch ((EnergyBarRenderer.ColorType)textureBarColorType.enumValueIndex) {
                    case EnergyBarRenderer.ColorType.Solid:
                        EditorGUILayout.PropertyField(textureBarColor, new GUIContent("Bar Color"));
                        break;
                        
                    case EnergyBarRenderer.ColorType.Gradient:
                        EditorGUILayout.PropertyField(textureBarGradient, new GUIContent("Bar Gradient"));
                        break;
                }
                
            EditorGUI.indentLevel--;
            
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(growDirection, new GUIContent("Grow Direction"));
            
            if (dir == EnergyBarRenderer.GrowDirection.RadialCW || dir == EnergyBarRenderer.GrowDirection.RadialCCW) {
                Indent(() => {
                    EditorGUILayout.Slider(radialOffset, -1, 1, "Offset");
                    EditorGUILayout.Slider(radialLength, 0, 1, "Length");
                });
            } else if (dir == EnergyBarRenderer.GrowDirection.ColorChange) {
                EditorGUILayout.PropertyField(textureBarGradient, new GUIContent("Bar Gradient"));
            }
            
            FieldLabel();
            
            EndBox();
        }
        
        if (Foldout("Effects", false)) {
            BeginBox();
            
            FieldSmoothEffect();
            
            PropertyFieldToggleGroup2(effectBurn, "Burn Out Effect", () => {
                Indent(() => {
                    PropertyField(effectBurnTextureBar, "Burn Texture Bar");
                    PropertyField(effectBurnTextureBarColor, "Burn Color");
                });
            });
            
            PropertyFieldToggleGroup2(effectBlink, "Blink Effect", () => {
                Indent(() => {
                    PropertyField(effectBlinkValue, "Value");
                    PropertyField(effectBlinkRatePerSecond, "Rate (per second)");
                    PropertyField(effectBlinkColor, "Color");
                });
            });
            
            EndBox();
        }
        
        EditorGUILayout.Space();
        
        serializedObject.ApplyModifiedProperties();
    
//        base.OnInspectorGUI();
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}