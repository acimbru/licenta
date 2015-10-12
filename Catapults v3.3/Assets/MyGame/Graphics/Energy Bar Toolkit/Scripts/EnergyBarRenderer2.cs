/*
* Copyright (c) 2013 Mad Pixel Machine
* All Rights Reserved
*
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(EnergyBar))]
public class EnergyBarRenderer2 : EnergyBarBase {

    // ===========================================================
    // Constants
    // ===========================================================
    
    public static readonly Color Transparent = new Color(1, 1, 1, 0);
    
    // ===========================================================
    // Fields
    // ===========================================================
    public Vector2 screenPosition = new Vector2(10, 10);
    public bool screenPositionNormalized; // indicates that all coordinates are normalized to screen width and height
    
    public Vector2 size = new Vector2(100, 20);
    public bool sizeNormalized;
    public bool screenPositionCalculateSize = true;
    
    private Vector2 sizeReal;
    private Vector2 screenPositionReal;
    
    public Texture2D textureBackground; // deprecated
    public Color textureBackgroundColor = Color.white; // deprecated
    
    
    public Texture2D textureBar;
    private Texture2D _textureBar;  // here is stored last texture bar set
    
    public ColorType textureBarColorType = ColorType.Solid;
    public Color textureBarColor = Color.white;
    public Gradient textureBarGradient;
    
    public Texture2D textureForeground; // deprecated
    public Color textureForegroundColor = Color.white; // deprecated
    
    public GrowDirection growDirection = GrowDirection.LeftToRight;
    
    // radial parametters
    public RotatePoint radialRotatePoint = RotatePoint.VisibleAreaCenter;
    public Vector2 radialCustomCenter;
    public float radialOffset = 0.0f;
    public float radialLength = 1.0f;
    
    private Rect textureBarVisibleBounds;
    private Rect textureBarVisibleBoundsOrig;
    private float actualDisplayValue;
    
    //
    // effects
    //
    
    // burn effect
    public bool effectBurn = false;                 // bar draining will display 'burn' effect
    public Texture2D effectBurnTextureBar;
    public Color effectBurnTextureBarColor = Color.red;
    private float burnDisplayValue;
    
    // blink effect
    public bool effectBlink = false;
    public float effectBlinkValue = 0.2f;
    public float effectBlinkRatePerSecond = 1f;
    public Color effectBlinkColor = new Color(1, 1, 1, 0);
    
    private float effectBlinkAccum;
    private bool effectBlinkVisible = false;
    
    
//    private Material simpleFillMaterial; // material for simple fill
//    private Material radialMaterial; // material for radial bar
    
    // ===========================================================
    // Constructors (Including Static Constructors)
    // ===========================================================

    // ===========================================================
    // Getters / Setters
    // ===========================================================
    
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
    
    public override Rect TexturesRect {
        get {
//            sizeReal = Round(SizePixels);
//            screenPositionReal = RealPosition(Round(ScreenPositionPixels), SizePixels);
            
            var rect = new Rect(screenPositionReal.x, screenPositionReal.y, sizeReal.x, sizeReal.y);
            return rect;
        }
    }
    

    // ===========================================================
    // Methods
    // ===========================================================
    
    new void Start() {
        base.Start();
        
        // moving deprecated fields to new one
        if (texturesBackground.Length == 0 && textureBackground != null) {
            Array.Resize(ref texturesBackground, 1);
            var tex = new Tex();
            tex.texture = textureBackground;
            tex.color = textureBackgroundColor;
            texturesBackground[0] = tex;
            
            textureBackground = null;
        }
        
        if (texturesForeground.Length == 0 && textureForeground != null) {
            Array.Resize(ref texturesForeground, 1);
            var tex = new Tex();
            tex.texture = textureForeground;
            tex.color = textureForegroundColor;
            texturesForeground[0] = tex;
            
            textureForeground = null;
        }
    }
    
#region Start
    private Rect FindBounds(Texture2D texture) {
        
        int left = -1, top = -1, right = -1, bottom = -1;
        bool expanded = false;
        Color32[] pixels;
        try {
            pixels = texture.GetPixels32();
        } catch (UnityException) { // catch not readable
            return new Rect();
        }
            
        int w = texture.width;
        int h = texture.height;
        int x = 0, y = h - 1;
        for (int i = 0; i < pixels.Length; ++i) {
            var c = pixels[i];
            if (c.a != 0) {
                Expand(x, y, ref left, ref top, ref right, ref bottom);
                expanded = true;
            }
            
            if (++x == w) {
                y--;
                x = 0;
            }
        }
        
        if (!expanded) {
            throw new BadTextureException("bar texture has no visible pixels");
        }
        
        return new Rect(left, top, right - left + 1, bottom - top + 1);
    }
    
    private void Expand(int x, int y, ref int left, ref int top, ref int right, ref int bottom) {
        if (left == -1) {
            left = right = x;
            top = bottom = y;
        } else {
            if (left > x) {
                left = x;
            } else if (right < x) {
                right = x;
            }
            
            if (top > y) {
                top = y;
            } else if (bottom == -1 || bottom < y) {
                bottom = y;
            }    
        }
    }
#endregion

#region Update

    // updates sizing of textures. Need to be called in Update and OnGUI methods
    // in other way it will lead to bad scaling effect in Editor preview
    void UpdateSize() {
        sizeReal = Round(SizePixels);
        screenPositionReal = RealPosition(Round(ScreenPositionPixels), SizePixels);
    }

    void Update() {
        UpdateSize();
    
        if (!IsValid()) {
            return;
        }
    
        if (textureBar != _textureBar) { // texture bar dirty?
            textureBarVisibleBoundsOrig = FindBounds(textureBar);
            if (textureBarVisibleBoundsOrig.width == 0) {  // not readable (yet)
                return;
            }
            
            textureBarVisibleBounds = textureBarVisibleBoundsOrig;
            _textureBar = textureBar;
        }
        
//        FixRatio();
        
        if (screenPositionCalculateSize) {
            SizePixels = new Vector2(textureBar.width, textureBar.height);
        }
    
        if (effectBurn) {
            if (effectSmoothChange) {
                // in burn mode smooth primary bar only when it's increasing
                if (energyBar.ValueF > actualDisplayValue) {
                    SmoothDisplayValue(ref actualDisplayValue, energyBar.ValueF, effectSmoothChangeSpeed);
                } else {
                    actualDisplayValue = energyBar.ValueF;
                }
            } else {
                actualDisplayValue = energyBar.ValueF;
            }
            
            SmoothDisplayValue(ref burnDisplayValue, actualDisplayValue, effectSmoothChangeSpeed);
            burnDisplayValue = Mathf.Max(burnDisplayValue, actualDisplayValue);
            
        } else {
            if (effectSmoothChange) {
                SmoothDisplayValue(ref actualDisplayValue, energyBar.ValueF, effectSmoothChangeSpeed);
            } else {
                actualDisplayValue = energyBar.ValueF;
            }
            
            burnDisplayValue = actualDisplayValue;
        }
        
        // update blink effect
        if (effectBlink) {
            effectBlinkVisible = Blink(energyBar.ValueF, effectBlinkValue, effectBlinkRatePerSecond, ref effectBlinkAccum);
        }
    }
    
    public static bool Blink(float val, float blinkVal, float rate, ref float accum) {
        if (val <= blinkVal) {
    
            float rate2 = rate * 2;
        
            if (rate > 0) {
                accum += Time.deltaTime;
                int times = (int) (accum / (1 / rate));
                accum -= (1 / rate) * times;
            }
            
            return accum > (1 / rate2);
        } else {
            return false;
        }
    }
    
    private bool IsValid() {
        return textureBar != null;
    }
#endregion
    
#region OnGUI
    new public void OnGUI() {
        base.OnGUI();
    
        if (!RepaintPhase()) {
            return;
        }
    
        if (!IsValid()) {
            return;
        }
        
        UpdateSize();
        
        GUIDrawBackground();
    
        if (effectBurn && burnDisplayValue != 0) {
            var texture = effectBurnTextureBar != null ? effectBurnTextureBar : textureBar;
            DrawBar(burnDisplayValue, GetTextureBurnColor(), texture);
        }
        
        if (actualDisplayValue != 0) {
            DrawBar(actualDisplayValue, GetTextureBarColor(), textureBar);
        }
        
        GUIDrawForeground();
        
        GUIDrawLabel();
    }
    
    private Color GetTextureBarColor() {
        // if effect blink enabled, return blink color if blink is currently visible
        // or return regular color if blink is not visible
        if (effectBlink && effectBlinkVisible) {
            return effectBlinkColor;
        }
        
        if (growDirection == GrowDirection.ColorChange) {
            return textureBarGradient.Evaluate(energyBar.ValueF);
        }
    
        switch (textureBarColorType) {
            case ColorType.Solid:
                return textureBarColor;
            case ColorType.Gradient:
                return textureBarGradient.Evaluate(energyBar.ValueF);
            default:
                Debug.LogError("Unknown texture bar type! This is a bug! Please report this.");
                return Color.white;
        }
    }
    
    private Color GetTextureBurnColor() {
        // when blinking bisible do not display burn bar at all
        if (effectBlink && effectBlinkVisible) {
            return Transparent;
        }
        
        return effectBurnTextureBarColor;
    }
    
    private void DrawBar(float value, Color color, Texture2D texture) {
        var rect = new Rect(screenPositionReal.x, screenPositionReal.y, sizeReal.x, sizeReal.y);
        var bounds = textureBarVisibleBounds;
        var visibleRect = new Rect(
            bounds.xMin / textureBar.width,
            1 - bounds.yMin / textureBar.height,
            bounds.xMax / textureBar.width,
            1 - bounds.yMax / textureBar.height);
        
        switch (growDirection) {
            case GrowDirection.LeftToRight:
                DrawTextureHorizFill(rect, texture, visibleRect, color, false, value);
                break;
            case GrowDirection.RightToLeft:
                DrawTextureHorizFill(rect, texture, visibleRect, color, true, value);
                break;
            case GrowDirection.TopToBottom:
                DrawTextureVertFill(rect, texture, visibleRect, color, false, value);
                break;
            case GrowDirection.BottomToTop:
                DrawTextureVertFill(rect, texture, visibleRect, color, true, value);
                break;
            case GrowDirection.RadialCW:
                DrawTextureRadialFill(rect, texture, color, false, value, radialOffset, radialLength);
                break;
            case GrowDirection.RadialCCW:
                DrawTextureRadialFill(rect, texture, color, true, value, radialOffset, radialLength);
                break;
            case GrowDirection.ExpandHorizontal:
                DrawTextureExpandFill(rect, texture, visibleRect, color, false, value);
                break;
            case GrowDirection.ExpandVertical:
                DrawTextureExpandFill(rect, texture, visibleRect, color, true, value);
                break;
            case GrowDirection.ColorChange:
                DrawTexture(rect, texture, color);
                break;
            default:
                Debug.LogError("Unknown grow direction: " + growDirection);
                break;
        }
        
    }
    
    private Rect BoundsForEnergyBar(float value) {
        var s = Round(SizePixels);
    
        switch (growDirection) {
            case GrowDirection.LeftToRight: {
                var nWidth = textureBarVisibleBounds.width * value;
                return toAbsolute(new Rect(0, 0, textureBarVisibleBounds.x + nWidth, s.y));
            }
            case GrowDirection.RightToLeft: {
                var barWidth = textureBarVisibleBounds.width * value;
                var x = textureBarVisibleBounds.xMax - barWidth;
                var w = s.x - x;
                return toAbsolute(new Rect(x, 0, w, s.y));
            }
            case GrowDirection.BottomToTop: {
                var barHeight = textureBarVisibleBounds.height * value;
                float y = textureBarVisibleBounds.yMax - barHeight;
                float h = s.y - y;
                return toAbsolute(new Rect(0, y, s.x, h));
            }
            case GrowDirection.TopToBottom: {
                var nHeight = textureBarVisibleBounds.height * value;
                return toAbsolute(new Rect(0, 0, s.x, textureBarVisibleBounds.y + nHeight));
            }
            default:
                throw new Assertion("Unknown enum option");
        }
    }
    
    private Rect TexCoordsForEnergyBar(Rect energyBounds) {
        var pos = screenPositionReal;
        var s = sizeReal;
    
        switch (growDirection) {
            case GrowDirection.LeftToRight: {
                return new Rect(0, 0, (energyBounds.xMax - pos.x) / s.x, 1);
            }
            case GrowDirection.RightToLeft: {
                float x = (energyBounds.xMin - pos.x) / s.x;
                float w = 1 - x;
                return new Rect(x, 0, w, 1);
            }
            case GrowDirection.BottomToTop: {
                float y = (energyBounds.yMin - pos.y) / (s.y);
                float h = 1 - y;
                return new Rect(0, 0, 1, h);
            }
            case GrowDirection.TopToBottom: {
                float y = (energyBounds.yMax - pos.y) / s.y;
                return new Rect(0, 1 - y, 1, y);
            }
                
            default:
                throw new Assertion("Unknown enum option");
        }
    }
    
    private Rect toAbsolute(Rect rect) {
        return new Rect(rect.x + screenPositionReal.x, rect.y + screenPositionReal.y, rect.width, rect.height);
    }
#endregion

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
 
    public enum GrowDirection {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
        RadialCW,
        RadialCCW,
        ExpandHorizontal,
        ExpandVertical,
        ColorChange,
    }
          
    public enum ColorType {
        Solid,
        Gradient,
    }
    
    public enum RotatePoint {
        VisibleAreaCenter,
        TextureCenter,
        CustomPoint
    }
    
    public class BadTextureException : System.Exception {
       public BadTextureException() {
       }
    
       public BadTextureException(string message): base(message) {
       }
    }
}