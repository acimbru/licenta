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
 
[CustomEditor(typeof(EnergyBarTransformRenderer))]   
public class EnergyBarTransformRendererInspector : EnergyBarInspectorBase {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty textureObjectTexture;
    SerializedProperty textureObjectColor;
    
    SerializedProperty screenPosition;
    SerializedProperty screenPositionNormalized;
    SerializedProperty screenPositionCalculateSize;
    SerializedProperty size;
    SerializedProperty sizeNormalized;
    
    SerializedProperty transformAnchor;
    
    SerializedProperty transformTranslate;
    SerializedProperty transformRotate;
    SerializedProperty transformScale;
    
    SerializedProperty translateFunctionStart;
    SerializedProperty translateFunctionEnd;
    SerializedProperty rotateFunctionStart;
    SerializedProperty rotateFunctionEnd;
    SerializedProperty scaleFunctionStart;
    SerializedProperty scaleFunctionEnd;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    new public void OnEnable() {
        base.OnEnable();
        
        var textureObject = serializedObject.FindProperty("textureObject");
        textureObjectTexture = textureObject.FindPropertyRelative("texture");
        textureObjectColor = textureObject.FindPropertyRelative("color");
        
        screenPosition = serializedObject.FindProperty("screenPosition");
        screenPositionNormalized = serializedObject.FindProperty("screenPositionNormalized");
        screenPositionCalculateSize = serializedObject.FindProperty("screenPositionCalculateSize");
        size = serializedObject.FindProperty("size");
        sizeNormalized = serializedObject.FindProperty("sizeNormalized");
        
        transformAnchor = serializedObject.FindProperty("transformAnchor");
        
        transformTranslate = serializedObject.FindProperty("transformTranslate");
        transformRotate = serializedObject.FindProperty("transformRotate");
        transformScale = serializedObject.FindProperty("transformScale");
        
        translateFunctionStart = serializedObject.FindProperty("translateFunction")
            .FindPropertyRelative("startPosition");
        translateFunctionEnd = serializedObject.FindProperty("translateFunction")
            .FindPropertyRelative("endPosition");
        rotateFunctionStart = serializedObject.FindProperty("rotateFunction")
            .FindPropertyRelative("startAngle");
        rotateFunctionEnd = serializedObject.FindProperty("rotateFunction")
            .FindPropertyRelative("endAngle");
        scaleFunctionStart = serializedObject.FindProperty("scaleFunction")
            .FindPropertyRelative("startScale");
        scaleFunctionEnd = serializedObject.FindProperty("scaleFunction")
            .FindPropertyRelative("endScale");
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        var t = target as EnergyBarTransformRenderer;
        
        if (Foldout("Textures", true)) {
            BeginBox();
            FieldBackgroundTextures();
            
            Indent(() => {
                EditorGUILayout.LabelField("Object Texture");
                GUITexture(textureObjectTexture, textureObjectColor);
                
                PropertyFieldVector2(transformAnchor, "Texture Anchor");
            });
            
            EditorGUILayout.Space();
            
            CheckTextureIsReadable(t.textureObject.texture);
            CheckTextureFilterTypeNotPoint(t.textureObject.texture);
            
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
        
        if (Foldout("Object Transform", true)) {
            BeginBox();
            PropertyFieldToggleGroup2(transformTranslate, "Translate", () => {
                Indent(() => {
                    PropertyFieldVector2(translateFunctionStart, "Start Point");
                    PropertyFieldVector2(translateFunctionEnd, "End Point");
                });
            });
            
            PropertyFieldToggleGroup2(transformRotate, "Rotate", () => {
                Indent(() => {
                    PropertyField(rotateFunctionStart, "Start Angle");
                    PropertyField(rotateFunctionEnd, "End Angle");
                });
            });
            
            PropertyFieldToggleGroup2(transformScale, "Scale", () => {
                Indent(() => {
                    PropertyFieldVector2(scaleFunctionStart, "Start Scale");
                    PropertyFieldVector2(scaleFunctionEnd, "End Scale");
                });
            });
            
            EndBox();
        }
        
        if (Foldout("Appearance", false)) {
            BeginBox();
            FieldLabel();
            EndBox();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace