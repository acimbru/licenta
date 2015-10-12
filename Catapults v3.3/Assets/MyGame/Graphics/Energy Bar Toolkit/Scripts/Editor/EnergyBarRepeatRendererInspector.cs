/**********************************************************
*          Copyright (c) 2012 Mad Pixel Machine           *
*                  All Rights Reserved                    *
*                                                         *
*             http://www.madpixelmachine.com              *
**********************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EnergyBarRepeatRenderer))]
public class EnergyBarRepeatRendererInspector : EnergyBarInspectorBase {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    private SerializedProperty icon;
    private SerializedProperty iconColor;
    private SerializedProperty iconSlot;
    private SerializedProperty iconSlotColor;
    private SerializedProperty iconSizeCalculate;
    private SerializedProperty iconSize;
    private SerializedProperty iconSizeNormalized;
    
    private SerializedProperty startPosition;
    private SerializedProperty startPositionNormalized;
    private SerializedProperty repeatCount;
    private SerializedProperty positionDelta;
    private SerializedProperty positionDeltaNormalized;
    
    private SerializedProperty effect;
    private SerializedProperty cutDirection;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    new void OnEnable() {
        base.OnEnable();
        
        icon = serializedObject.FindProperty("icon");
        iconColor = serializedObject.FindProperty("iconColor");
        iconSlot = serializedObject.FindProperty("iconSlot");
        iconSlotColor = serializedObject.FindProperty("iconSlotColor");
        iconSizeCalculate = serializedObject.FindProperty("iconSizeCalculate");
        iconSize = serializedObject.FindProperty("iconSize");
        iconSizeNormalized = serializedObject.FindProperty("iconSizeNormalized");
        
        startPosition = serializedObject.FindProperty("startPosition");
        startPositionNormalized = serializedObject.FindProperty("startPositionNormalized");
        repeatCount = serializedObject.FindProperty("repeatCount");
        positionDelta = serializedObject.FindProperty("positionDelta");
        positionDeltaNormalized = serializedObject.FindProperty("positionDeltaNormalized");
        
        effect = serializedObject.FindProperty("effect");
        cutDirection = serializedObject.FindProperty("cutDirection");
        
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        if (Foldout("Textures", true)) {
            BeginBox();
            PropertyField(repeatCount, "Repeat Count");
            PropertyField(icon, "Icon");
            CheckTextureIsGUI(icon.objectReferenceValue as Texture2D);
            CheckTextureFilterTypeNotPoint(icon.objectReferenceValue as Texture2D);
            Indent(() => { PropertyField(iconColor, "Icon Tint"); });
            
            PropertyField(iconSlot, "Slot Icon");
            CheckTextureIsGUI(iconSlot.objectReferenceValue as Texture2D);
            CheckTextureFilterTypeNotPoint(iconSlot.objectReferenceValue as Texture2D);
            Indent(() => { PropertyField(iconSlotColor, "Slot Icon Tint"); });
            FieldPremultipliedAlpha();
            EndBox();
        }
        
        if (Foldout("Position & Size", true)) {
            BeginBox();
            PropertyFieldVector2(startPosition, "Start Position");
            EditorGUI.indentLevel++;
            PropertySpecialNormalized(startPosition, startPositionNormalized);
            PropertyField(pivot, "Pivot");
            PropertyField(anchorObject, "Anchor");
            EditorGUI.indentLevel--;
            PropertyField(guiDepth, "GUI Depth");
            
            PropertyFieldToggleGroupInv2(iconSizeCalculate, "Manual Size", () => {
                Indent(() => {
                    PropertyFieldVector2(iconSize, "Icon Size");
                    PropertySpecialNormalized(iconSize, iconSizeNormalized);
                });
                
            });
            
            PropertyFieldVector2(positionDelta, "Icons Distance");
            EditorGUI.indentLevel++;
            PropertySpecialNormalized(positionDelta, positionDeltaNormalized);
            EditorGUI.indentLevel--;
            
            FieldRelativeToTransform();
            EndBox();
        }
        
        if (Foldout("Appearance", false)) {
            BeginBox();
            FieldLabel();
            EndBox();
        }
        
        if (Foldout("Effects", false)) {
            BeginBox();
            PropertyField(effect, "Grow Effect");
            if (effect.enumValueIndex == (int) EnergyBarRepeatRenderer.Effect.Cut) {
                PropertyField(cutDirection, "Cut Direction");
            }
            
            FieldSmoothEffect();
            EndBox();
        }
        
        serializedObject.ApplyModifiedProperties();
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