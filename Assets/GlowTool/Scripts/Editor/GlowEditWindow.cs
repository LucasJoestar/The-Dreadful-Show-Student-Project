using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

public class GlowEditWindow : EditorWindow
{
    public void OnGUI()
    {
        GUITools.ActionButton("X", Close, Color.red, Color.black);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}
