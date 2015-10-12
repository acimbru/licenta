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

public class MadDebug {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public static string internalPostfix =
        "\nThis is an internal error. Please report this to support@madpixelmachine.com";

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    public static void Assert(bool condition, string text) {
        System.Diagnostics.Debug.Assert(condition, text);
    }

    public static void Internal(string message) {
        Debug.LogError(message + internalPostfix);
    }
    
    public static void Internal(string message, Object context) {
        Debug.LogError(message + internalPostfix, context);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace