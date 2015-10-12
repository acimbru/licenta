/*
* Copyright (c) Mad Pixel Machine
* All Rights Reserved
*
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnergyBarToolkit;

namespace EnergyBarToolkit {

/// <summary>
/// Energy bar renderer that indicates progress change by transforming
/// (translating, resizing, rotating) its texture.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(EnergyBar))]
public class EnergyBarTransformRenderer : EnergyBarBase {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Vector2 screenPosition = new Vector2(10, 10);
    public bool screenPositionNormalized; // indicates that all coordinates are normalized to screen width and height
    
    public Vector2 size = new Vector2(100, 20);
    public bool sizeNormalized;
    public bool screenPositionCalculateSize = true;
    
    private Vector2 sizeReal;
    private Vector2 sizeOrig; // keeps the original size of this energy bar
    private Vector2 screenPositionReal;
    
    public Tex textureObject;
    
    public bool transformTranslate;
    public bool transformRotate;
    public bool transformScale;
    public Vector2 transformAnchor = new Vector2(0.5f, 0.5f);
    
    public TranslateFunction translateFunction;
    public RotateFunction rotateFunction;
    public ScaleFunction scaleFunction;
    
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    public override Rect TexturesRect {
        get {
            var rect = new Rect(screenPositionReal.x, screenPositionReal.y, sizeReal.x, sizeReal.y);
            return rect;
        }
    }
    
    private Vector2 ScreenPositionPixels {
        get {
            if (screenPositionNormalized) {
                return new Vector2(screenPosition.x * Screen.width, screenPosition.y * Screen.height);
            } else {
                return screenPosition;
            }
        }
    }
    
    private Vector2 SizePixels {
        get {
            Vector2 o;
            if (sizeNormalized) {
                o = new Vector2(size.x * Screen.width, size.y * Screen.height);
                
            } else {
                o = size;
            }
            
            o.Scale(TransformScale());
            return o;
        }
        
        set {
            if (sizeNormalized) {
                size = new Vector2(value.x / Screen.width, value.y / Screen.height);
            } else {
                size = value;
            }
        }
    }

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

//    void Start() {
//    }

    void Update() {
        var anyTexture = AnyBackgroundOrForegroundTexture();
        if (anyTexture != null) {
            sizeOrig = new Vector2(anyTexture.width, anyTexture.height);
        } else if (textureObject.Valid) {
            sizeOrig = new Vector2(textureObject.texture.width, textureObject.texture.height);
        }
        
        if (screenPositionCalculateSize) {    
            SizePixels = sizeOrig;
        }
        
        UpdateSize();
    }
    
    // updates sizing of textures. Need to be called in Update and OnGUI methods
    // in other way it will lead to bad scaling effect in Editor preview
    void UpdateSize() {
        sizeReal = Round(SizePixels);
        screenPositionReal = RealPosition(Round(ScreenPositionPixels), SizePixels);
    }
    
    Texture2D AnyBackgroundOrForegroundTexture() {
        return AnyBackgroundTexture() ?? AnyForegroundTexture();
    }
    
    Texture2D AnyBackgroundTexture() {
        if (texturesBackground != null) {
            foreach (var tex in texturesBackground) {
                if (tex.texture != null) {
                    return tex.texture;
                }
            }
        }
        return null;
    }
    
    Texture2D AnyForegroundTexture() {
        if (texturesForeground != null) {
            foreach (var tex in texturesForeground) {
                if (tex.texture != null) {
                    return tex.texture;
                }
            }
        }
        
        return null;
    }
    
    new public void OnGUI() {
        base.OnGUI();
        
        if (!RepaintPhase()) {
            return;
        }
    
        UpdateSize();
        
        
        GUIDrawBackground();
        
        if (textureObject.Valid) {
            DrawObject();
        }
        
        GUIDrawForeground();
        GUIDrawLabel();
    }
    
    void DrawObject() {
        Vector3 translate = Vector3.zero;
        if (transformTranslate) {
            translate = translateFunction.Value(ValueF);
            translate = new Vector3(translate.x * sizeReal.x, translate.y * sizeReal.y, 0);
        }
        
        Quaternion rotation = Quaternion.identity;
        if (transformRotate) {
            rotation = rotateFunction.Value(ValueF);
        }
        
        Vector3 scale = Vector3.one;
        if (transformScale) {
            scale = scaleFunction.Value(ValueF);
        }
        
        float tx = textureObject.width * transformAnchor.x;
        float ty = textureObject.height * transformAnchor.y;
        


        var matrix = Matrix4x4.identity;
        
        MadMatrix.Translate(ref matrix, -tx, -ty, 0);
        MadMatrix.Scale(ref matrix, scale);
        MadMatrix.Rotate(ref matrix, rotation);
        
        MadMatrix.Scale(ref matrix, sizeReal.x / sizeOrig.x, sizeReal.y / sizeOrig.y, 1);
//        if (positionSizeFromTransform) {
//            MadMatrix.Scale(ref matrix, transform.lossyScale);
//        }
        
        MadMatrix.Translate(ref matrix, screenPositionReal);
        MadMatrix.Translate(ref matrix, sizeReal.x / 2, sizeReal.y / 2, 0);
        MadMatrix.Translate(ref matrix, translate);
        
        GUI.matrix = matrix;
            
        DrawTexture(
            new Rect(
                0, 0,
                textureObject.width, textureObject.height),
            textureObject.texture,
            textureObject.color);
            
        GUI.matrix = Matrix4x4.identity;
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum TransformType {
        Translate,
        Scale,
        Rotate,
    }
    
    public abstract class TransformFunction {
    }
    
    [System.Serializable]
    public class TranslateFunction : TransformFunction {
        public Vector2 startPosition;
        public Vector2 endPosition;
        
        public Vector2 Value(float progress) {
            progress = Mathf.Clamp01(progress);
            
            var result = Vector2.Lerp(startPosition, endPosition, progress);
            return result;
        }
    }
    
    [System.Serializable]
    public class ScaleFunction : TransformFunction {
        public Vector2 startScale = Vector3.one;
        public Vector2 endScale = Vector3.one;
        
        public Vector3 Value(float progress) {
            progress = Mathf.Clamp01(progress);
            
            var result = Vector2.Lerp(startScale, endScale, progress);
            return new Vector3(result.x, result.y, 1);
        }
    }
    
    [System.Serializable]
    public class RotateFunction : TransformFunction {
        public float startAngle;
        public float endAngle;
        
        public Quaternion Value(float progress) {
            progress = Mathf.Clamp01(progress);
            
            float angle = Mathf.Lerp(startAngle, endAngle, progress);
            
            var result = Quaternion.Euler(0, 0, angle);
            return result;
        }
    }
    
}

} // namespace