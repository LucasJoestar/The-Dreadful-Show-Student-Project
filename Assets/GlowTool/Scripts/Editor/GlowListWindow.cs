using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

public class GlowListWindow : EditorWindow
{
    GlowManager targetGlowManager;

    void ManageObjects (List<GameObject> _objectsToMakeGlowy)
    {
        for (int o = 0; o < _objectsToMakeGlowy.Count; o++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_objectsToMakeGlowy[o], typeof(GameObject), true);
            GUITools.ActionButton("X", targetGlowManager.RemoveObjectAt, o, Color.red, Color.black);
            EditorGUILayout.EndHorizontal();
        }
    }

    public void OnGUI()
    {
        GUITools.ActionButton("X", Close, Color.red, Color.black);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}