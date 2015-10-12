/*
* Copyright (c) Mad Pixel Machine
* All Rights Reserved
*
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnergyBarToolkit {

public class MadTransform {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static T CreateChild<T>(Transform parent, string name) where T : Component {
        GameObject go = null;
    
        go = new GameObject(name);
        go.transform.parent = parent;
        
        var component = go.AddComponent<T>();
        return component;
    }
    
    public static T CreateChild<T>(Transform parent, string name, T template) where T : Component {
        var gameObject = CreateChild(parent, name, template.gameObject);
        return gameObject.AddComponent<T>();
    }
    
    public static GameObject CreateChild(Transform parent, string name, GameObject template) {
        GameObject go = null;
        go = GameObject.Instantiate(template) as GameObject;
        go.transform.parent = parent;
        go.name = name;
        
        return go;
    }
    
    public static T FindChild<T>(Transform parent) where T : Component {
        return FindChild(parent, (T t) => true);
    }
    
    public static T FindChild<T>(Transform parent, Predicate<T> predicate) where T : Component {
        int count = parent.childCount;
        for (int i = 0; i < count; ++i) {
            var child = parent.GetChild(i);
            T component = child.GetComponent<T>();
            if (component != null && predicate(component)) {
                return component;
            }
            
            var c = FindChild<T>(child, predicate);
            if (c != null) {
                return c;
            }
        }
        
        return null;
    }
    
    public static List<T> FindChildren<T>(Transform parent) where T : Component {
        return FindChildren(parent, (T t) => true);
    }
    
    public static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate) where T : Component {
        List<T> output = new List<T>();
        
        int count = parent.childCount;
        for (int i = 0; i < count; ++i) {
            var child = parent.GetChild(i);
            T component = child.GetComponent<T>();
            if (component != null && predicate(component)) {
                output.Add(component);
            }
            
            output.AddRange(FindChildren<T>(child, predicate));
        }
        
        return output;
    }
    
    public static T FindParent<T>(Transform t) where T : Component {
        var c = t.parent;
        while (c != null) {
            var comp = c.GetComponent<T>();
            if (comp != null) {
                return comp;
            }
            
            c = c.parent;
        }
        
        return null;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public delegate bool Predicate<T>(T t);

}

} // namespace