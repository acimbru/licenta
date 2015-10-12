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
using EnergyBarToolkit;

public abstract class EnergyBarBase : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Tex[] texturesBackground = new Tex[0];
    public Tex[] texturesForeground = new Tex[0];
    
    public Pivot pivot = Pivot.TopLeft;
    public int guiDepth = 1;
    public GameObject anchorObject;
    public Camera anchorCamera; // camera on which anchor should be visible. if null then Camera.main
    
    public bool positionSizeFromTransform = false;
    public bool positionSizeFromTransformNormalized = false;
    
    // tells if textures has premultiplied alpha
    public bool premultipliedAlpha;
    
    // Label
    public bool labelEnabled;
    public GUISkin labelSkin;
    public Vector2 labelPosition;
    public bool labelPositionNormalized;
    public string labelFormat = "{cur}/{max}";
    public Color labelColor = Color.white;
    public bool labelOutlineEnabled = true;
    public Color labelOutlineColor = Color.black;
    
    // materials for shaders
    private Material[] materials;
    private bool materialsLoaded;
    
    // smooth effect
    public bool effectSmoothChange = false;          // smooth change value display over time
    public float effectSmoothChangeSpeed = 0.5f;    // value bar width percentage per second
    
    // Storing screen width & height in public variables to make pixel <-> normalized coordinates conversion possible from the inspector.
    // Screen.width and Screen.height accessed from the inspector code will return size of inspector window and that's not what I want.
    [HideInInspector]
    public int storedScreenWidth = 800;
    [HideInInspector]
    public int storedScreenHeight = 600;
    
    protected EnergyBar energyBar;
    
    // ===========================================================
    // Getters / Setters
    // ===========================================================
    
    public abstract Rect TexturesRect { get; }
    
    protected float ValueF {
        get {
            return energyBar.ValueF;
        }
    }
    
    Vector2 LabelPositionPixels {
        get {
            var rect = TexturesRect;
            Vector2 v;
            if (labelPositionNormalized) {
                v = new Vector2(rect.x + labelPosition.x * rect.width, rect.y + labelPosition.y * rect.height);
            } else {
                v = new Vector2(rect.x + labelPosition.x, rect.y + labelPosition.y);
            }
            
            return v;
        }
    }

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    protected void OnEnable() {
        CreateMaterial();
    
        // I want all bars to go through layout phase because I need to set GUI.depth
        // on this phase. If I won't, the ordering will be broken.
        // Also I must remember to put this on the beginning of each OnGUI() method
        // for all derived classes:
        // if (!RepaintPhase()) {
        //     return;
        // }
        useGUILayout = true;
        
        energyBar = GetComponent<EnergyBar>();
        Assert(energyBar != null, "Cannot access energy bar?!");
    }
    
    protected void OnDisable() {
        DestroyMaterial();
    }
    
    protected void Start() {
        // do nothing
    }
    
    protected void OnGUI() {
        GUI.depth = guiDepth;
        
        if (!Application.isPlaying) {
            // I cannot get Screen.width and Screen.height from inspector so I store it here
            storedScreenWidth = Screen.width;
            storedScreenHeight = Screen.height;
        }
    }
    
    protected bool RepaintPhase() {
        return Event.current.type == EventType.Repaint;
    }
    
    // GUI helper methods
    
    protected void GUIDrawBackground() {
        if (texturesBackground != null) {
            var rect = TexturesRect;
        
            foreach (var bg in texturesBackground) {
                var t = bg.texture;
                if (t != null) {
                    DrawTexture(rect, t, bg.color);
                }
            }
        }
    }
    
    protected void GUIDrawForeground() {
        if (texturesForeground != null) {
            var rect = TexturesRect;
        
            foreach (var fg in texturesForeground) {
                var t = fg.texture;
                if (t != null) {
                    DrawTexture(rect, t, fg.color);
                }
            }
        }
    }
    
    private void CreateMaterial() {
        MadDebug.Assert(!materialsLoaded, "materials already loaded");
    
        int materialsCount = Enum.GetNames(typeof(MaterialType)).Length;
        materials = new Material[materialsCount];
        
        materials[(int) MaterialType.StandardTransparent] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Unlit"));
        materials[(int) MaterialType.StandardTransparentPre] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Unlit Pre"));
        materials[(int) MaterialType.HorizontalFill] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Horizontal Fill"));
        materials[(int) MaterialType.HorizontalFillPre] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Horizontal Fill Pre"));
        materials[(int) MaterialType.VerticalFill] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Vertical Fill"));
        materials[(int) MaterialType.VerticalFillPre] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Vertical Fill Pre"));
        materials[(int) MaterialType.ExpandFill] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Expand Fill"));
        materials[(int) MaterialType.ExpandFillPre] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Expand Fill Pre"));
        materials[(int) MaterialType.RadialFill] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Radial Fill"));
        materials[(int) MaterialType.RadialFillPre] = new Material(Shader.Find("Custom/Energy Bar Toolkit/Radial Fill Pre"));
        
        foreach (var mat in materials) {
            mat.hideFlags = HideFlags.DontSave;
        }
        
        materialsLoaded = true;
    }
    
    private void DestroyMaterial() {
        MadDebug.Assert(materialsLoaded, "materials not loaded");
        
        foreach (var mat in materials) {
            if (Application.isPlaying) {
                Destroy(mat);
            } else {
                DestroyImmediate(mat);
            }
        }
        
        materialsLoaded = false;
        materials = null;
    }
    
    protected void DrawTexture(Rect rect, Texture2D texture, Color tint) {
        var mat = MaterialStandard();
        mat.SetColor("_Color", PremultiplyAlpha(tint));
        Graphics.DrawTexture(rect, texture, mat);
    }
    
    protected void DrawTexture(Rect rect, Texture2D texture, Rect coords, Color tint) {
        var mat = MaterialStandard();
        mat.SetColor("_Color", PremultiplyAlpha(tint));
        Graphics.DrawTexture(rect, texture, coords, 0, 0, 0, 0, mat);
    }
    
    protected Color PremultiplyAlpha(Color c) {
        return new Color(c.r * c.a, c.g * c.a, c.b * c.a, c.a);
    }
    
    protected void DrawTextureHorizFill(
        Rect rect, Texture2D texture, Rect visibleRect, Color color, bool invert, float progress) {
        
        var mat = MaterialHorizFill();
        mat.SetColor("_Color", PremultiplyAlpha(color));
        mat.SetFloat("_Invert", invert ? 1 : 0);
        mat.SetFloat("_Progress", progress);
        mat.SetVector("_Rect", ToVector4(visibleRect));
        
        Graphics.DrawTexture(rect, texture, mat);
    }
    
    protected void DrawTextureVertFill(
        Rect rect, Texture2D texture, Rect visibleRect, Color color, bool invert, float progress) {
        
        var mat = MaterialVertFill();
        mat.SetColor("_Color", PremultiplyAlpha(color));
        mat.SetFloat("_Invert", invert ? 1 : 0);
        mat.SetFloat("_Progress", progress);
        mat.SetVector("_Rect", ToVector4(visibleRect));
        
        Graphics.DrawTexture(rect, texture, mat);
    }
    
    protected void DrawTextureExpandFill(
        Rect rect, Texture2D texture, Rect visibleRect, Color color, bool invert, float progress) {
        
        var mat = MaterialExpandFill();
        mat.SetColor("_Color", PremultiplyAlpha(color));
        mat.SetFloat("_Invert", invert ? 1 : 0);
        mat.SetFloat("_Progress", progress);
        mat.SetVector("_Rect", ToVector4(visibleRect));
        
        Graphics.DrawTexture(rect, texture, mat);
    }
    
    protected void DrawTextureRadialFill(
        Rect rect, Texture2D texture, Color color, bool invert, float progress, float offset, float length) {
    
        var mat = MaterialRadialFill();
        mat.SetColor("_Color", color);
        mat.SetFloat("_Invert", invert ? 1 : 0);
        mat.SetFloat("_Progress", progress);
        mat.SetFloat("_Offset", offset);
        mat.SetFloat("_Length", length);
        
        Graphics.DrawTexture(rect, texture, mat);
    }
    
    protected void GUIDrawLabel() {
        if (!labelEnabled) {
            return;
        }
        
        var skin = labelSkin;
        if (skin == null) {
            skin = GUI.skin;
        }
    
        float outlineSize = 1; // only size "1" looks good
    
        var text = LabelFormatResolve(labelFormat);
        var size = skin.label.CalcSize(new GUIContent(text));
        if (labelOutlineEnabled) {
            float outlineSize2 = outlineSize * 2;
            size.x += outlineSize2;
            size.y += outlineSize2;
        }
        
        var pos = LabelPositionPixels;
        Rect rect = new Rect(pos.x, pos.y, size.x, size.y);
        
        GUI.color = labelColor;
        if (labelOutlineEnabled) {
            LabelWithOutline(rect, text, skin.label, labelOutlineColor, outlineSize);
        } else {
            GUI.Label(rect, text, skin.label);
        }
        GUI.color = Color.white;
    }
    
    private string LabelFormatResolve(string format) {
        format = format.Replace("{cur}", "" + energyBar.valueCurrent);
        format = format.Replace("{min}", "" + energyBar.valueMin);
        format = format.Replace("{max}", "" + energyBar.valueMax);
        format = format.Replace("{cur%}", string.Format("{0:00}", energyBar.ValueF * 100));
        format = format.Replace("{cur2%}", string.Format("{0:00.0}", energyBar.ValueF * 100));
        
        return format;
    }
    
    void LabelWithOutline(Rect rect, string text, GUIStyle style, Color color, float size) {
        Color prevColor = GUI.color;
        GUI.color = color;
        
        foreach (Vector2 trans in new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)} ) {
            var nRect = new Rect(rect.x + trans.x * size, rect.y + trans.y * size, rect.width, rect.height);
            GUI.Label(nRect, text, style);
        }
        
        GUI.color = prevColor;
        GUI.Label(rect, text, style);
    }
    
    Vector4 ToVector4(Rect r) {
        return new Vector4(r.xMin, r.yMin, r.width, r.height);
    }
    
    protected Material MaterialStandard() {
        Assert(materialsLoaded, "Materials not loaded");
    
        if (premultipliedAlpha) {
            return materials[(int) MaterialType.StandardTransparentPre];
        } else {
            return materials[(int) MaterialType.StandardTransparent];
        }
    }
    
    protected Material MaterialHorizFill() {
        Assert(materialsLoaded, "Materials not loaded");
    
        if (premultipliedAlpha) {
            return materials[(int) MaterialType.HorizontalFillPre];
        } else {
            return materials[(int) MaterialType.HorizontalFill];
        }
    }
    
    protected Material MaterialVertFill() {
        Assert(materialsLoaded, "Materials not loaded");
    
        if (premultipliedAlpha) {
            return materials[(int) MaterialType.VerticalFillPre];
        } else {
            return materials[(int) MaterialType.VerticalFill];
        }
    }
    
    protected Material MaterialExpandFill() {
        Assert(materialsLoaded, "Materials not loaded");
    
        if (premultipliedAlpha) {
            return materials[(int) MaterialType.ExpandFillPre];
        } else {
            return materials[(int) MaterialType.ExpandFill];
        }
    }
    
    protected Material MaterialRadialFill() {
        Assert(materialsLoaded, "Materials not loaded");
    
        if (premultipliedAlpha) {
            return materials[(int) MaterialType.RadialFillPre];
        } else {
            return materials[(int) MaterialType.RadialFill];
        }
    }
    
    public void Assert(bool condition, string message) {
        
        if (!condition) {
            throw new Assertion(message);
        }
    }
    
    protected Vector2 Round(Vector2 v) {
        return new Vector2(Mathf.Round(v.x), Mathf.Round (v.y));
    }
    
    protected Vector2 TransformScale() {
        if (positionSizeFromTransform) {
            var s = transform.lossyScale;
            return new Vector2(s.x, s.y);
        } else {
            return Vector2.one;
        }
    }
    
    protected Vector2 RealPosition(Vector2 pos, Vector2 bounds) {
        Vector2 transformAnchor = Vector2.zero;
        if (positionSizeFromTransform) {
            if (positionSizeFromTransformNormalized) {
                transformAnchor = new Vector2(transform.position.x * Screen.width, -transform.position.y * Screen.height);
            } else {
                transformAnchor = new Vector2(transform.position.x, -transform.position.y);
            }
            
            pos.Scale(transform.lossyScale);
        }
        
        Vector2 screenAnchor = Vector2.zero;
        Vector2 boundsAnchor = Vector2.zero;
    
        switch (pivot) {
            case Pivot.TopLeft:
                screenAnchor = Vector2.zero;
                boundsAnchor = Vector2.zero;
                break;
            case Pivot.Top:
                screenAnchor = new Vector2(Screen.width / 2, 0);
                boundsAnchor = new Vector2(bounds.x / 2, 0);
                break;
            case Pivot.TopRight:
                screenAnchor = new Vector2(Screen.width, 0);
                boundsAnchor = new Vector2(bounds.x, 0);
                break;
            case Pivot.Right:
                screenAnchor = new Vector2(Screen.width, Screen.height / 2);
                boundsAnchor = new Vector2(bounds.x, bounds.y / 2);
                break;
            case Pivot.BottomRight:
                screenAnchor = new Vector2(Screen.width, Screen.height);
                boundsAnchor = new Vector2(bounds.x, bounds.y);
                break;
            case Pivot.Bottom:
                screenAnchor = new Vector2(Screen.width / 2, Screen.height);
                boundsAnchor = new Vector2(bounds.x / 2, bounds.y);
                break;
            case Pivot.BottomLeft:
                screenAnchor = new Vector2(0, Screen.height);
                boundsAnchor = new Vector2(0, bounds.y);
                break;
            case Pivot.Left:
                screenAnchor = new Vector2(0, Screen.height / 2);
                boundsAnchor = new Vector2(0, bounds.y / 2);
                break;
            case Pivot.Center:
                screenAnchor = new Vector2(Screen.width / 2, Screen.height / 2);
                boundsAnchor = new Vector2(bounds.x / 2, bounds.y / 2);
                break;
        }
        
        if (anchorObject != null) {
            Camera cam;
            if (anchorCamera != null) {
                cam = anchorCamera;
            } else {
                cam = Camera.main;
            }
        
            screenAnchor = cam.WorldToScreenPoint(anchorObject.transform.position);
            screenAnchor.y = Screen.height - screenAnchor.y;
        }
        
        var o = transformAnchor + screenAnchor - boundsAnchor + pos;
        return o;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void SmoothDisplayValue(ref float displayValue, float target, float speed) {
        if (!Application.isPlaying) {
            // do not smooth if in edit mode
            displayValue = target;
            return;
        }
        
        float deltaTotal = target - displayValue;
        if (deltaTotal == 0) {
            return;
        }
        
        float delta;
        
        if (deltaTotal < 0) {
            delta = -speed;
        } else {
            delta = speed;
        }
        
        delta *= Time.deltaTime;
        
        if (Mathf.Abs(delta) > Mathf.Abs(deltaTotal)) {
            displayValue = target;
        } else {
            displayValue += delta;
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Pivot {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Center,
    }
    
    protected enum MaterialType {
        StandardTransparent     = 0,
        StandardTransparentPre  = 1,
        HorizontalFill          = 2,
        HorizontalFillPre       = 3,
        VerticalFill            = 4,
        VerticalFillPre         = 5,
        ExpandFill              = 6,
        ExpandFillPre           = 7,
        RadialFill              = 8,
        RadialFillPre           = 9,
    }
    
    public class Assertion : System.Exception {
       public Assertion() {
       }
    
       public Assertion(string message): base(message) {
       }
    }
    
    [System.Serializable]
    public class Tex {
        public int width { get { return texture.width; } }
        public int height { get { return texture.height; } }
        
        public bool Valid {
            get {
                return texture != null;
            }
        }
    
        public Texture2D texture;
        public Color color = Color.black;
    }

}
