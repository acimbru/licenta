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

[CustomEditor(typeof(EnergyBarSequenceRenderer))]
public class EnergyBarSequenceRendererInspector : EnergyBarInspectorBase {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    private SerializedProperty sizeCalculate;
    private SerializedProperty method;
    private SerializedProperty position;
    private SerializedProperty positionNormalized;
    private SerializedProperty size;
    private SerializedProperty sizeNormalized;
    
    private SerializedProperty gridTexture;
    private SerializedProperty gridWidth;
    private SerializedProperty gridHeight;
    
    private SerializedProperty color;
    
    private SerializedProperty frameCountManual;
    private SerializedProperty frameCount;
    
//    private SerializedProperty sequence;

    // ===========================================================
    // Constructors (Including Static Constructors)
    // ===========================================================

    // ===========================================================
    // Getters / Setters
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    new void OnEnable() {
        base.OnEnable();
    
        sizeCalculate = serializedObject.FindProperty("sizeCalculate");
        position = serializedObject.FindProperty("position");
        positionNormalized = serializedObject.FindProperty("positionNormalized");
        method = serializedObject.FindProperty("method");
        size = serializedObject.FindProperty("size");
        sizeNormalized = serializedObject.FindProperty("sizeNormalized");
        
        gridTexture = serializedObject.FindProperty("gridTexture");
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");
        
        frameCountManual = serializedObject.FindProperty("frameCountManual");
        frameCount = serializedObject.FindProperty("frameCount");
        
        color = serializedObject.FindProperty("color");
    }

    void Update() {
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        var t = target as EnergyBarSequenceRenderer;
        
        if (Foldout("Textures", true)) {
        BeginBox();
        
            EditorGUILayout.PropertyField(method, new GUIContent("Render Method"));
            
            switch ((EnergyBarSequenceRenderer.Method) method.enumValueIndex) {
                case EnergyBarSequenceRenderer.Method.Grid:
                    OnGUIGrid();
                    
                    break;
                case EnergyBarSequenceRenderer.Method.Sequence:
                    PropertyFieldWithChildren("sequence");
                    break;
            }
            
            PropertyField(color, "Color Tint");
            
            FieldBackgroundTextures();
            FieldForegroundTextures();
            FieldPremultipliedAlpha();
            EndBox();
        }
    
        if (Foldout("Position & Size", true)) {
            BeginBox();
            PropertyFieldVector2(position, "Position");
            EditorGUI.indentLevel++;
            PropertySpecialNormalized(position, positionNormalized);
            PropertyField(pivot, "Pivot");
            PropertyField(anchorObject, "Anchor");
            EditorGUI.indentLevel--;
            
            PropertyField(guiDepth, "GUI Depth");
            
            PropertyFieldToggleGroupInv2(sizeCalculate, "Manual Size", () => {
                Indent(() => {
                    PropertyFieldVector2(size, "Size");
                    PropertySpecialNormalized(size, sizeNormalized);
                });
            });
            
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
            FieldSmoothEffect();
            EndBox();
        }
        
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.PrefixLabel("Preview:");
        BeginBox(); {
            var rect = EditorGUILayout.BeginVertical();
            GUILayout.Space(100);
            
            if (t.IsValid()) {
                Rect texCoords;
                var texture = t.GetTexture(out texCoords);
                if (texture != null) {
                    GUI.DrawTextureWithTexCoords(PreserveAspect(rect), texture, texCoords);
                }
            }
            EditorGUILayout.EndVertical();
        } EndBox();
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    private void OnGUIGrid() {
        EditorGUILayout.PropertyField(gridTexture, new GUIContent("Grid Texture"));
        CheckTextureIsGUI(gridTexture.objectReferenceValue as Texture2D);
        
        EditorGUILayout.PropertyField(gridWidth, new GUIContent("Grid Width"));
        EditorGUILayout.PropertyField(gridHeight, new GUIContent("Grid Height"));
        
        
        PropertyFieldToggleGroup2(frameCountManual, "Manual Frame Count", () => {
            PropertyField(frameCount, "Frame Count");
        });
//        frameCountManual.boolValue = EditorGUILayout.BeginToggleGroup("Manual Frame Count", frameCountManual.boolValue); {
            
//        EditorGUILayout.EndToggleGroup();
    }
    
    // returns new rect that preserves aspect of bar size
    private Rect PreserveAspect(Rect rect) {
        if (rect.height == 0) {
            return rect;
        }
    
        var t = target as EnergyBarSequenceRenderer;
        Vector2 size = t.size;
        
        float sourceAspect = size.x / size.y;
        float targetAspect = rect.width / rect.height;
        
        float width, height;
        
        if (sourceAspect < targetAspect) {
            height = rect.height;
            width = rect.width * sourceAspect / targetAspect;
        } else {
            width = rect.width;
            height = rect.height * targetAspect / sourceAspect;
        }
        
        return new Rect(rect.x + (rect.width - width) / 2, rect.y + (rect.height - height) / 2, width, height);
        
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}