using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

[CustomEditor(typeof(GlowManager))]
public class GlowCustomEditor : Editor
{
    #region F/P
    GlowManager p_target;
    #endregion

    #region Meths
    void ChangeCamera()
    {
        if(Camera.main)
        {
            DestroyImmediate(Camera.main.gameObject);
        }
        Instantiate(Resources.Load("MainCameraPostPro"));
        Debug.Log("Camera post process is now on the scene.");
    }

    void DropAeraGUI()
    {        
        Event _e = Event.current;
        Rect _dropZone = GUILayoutUtility.GetRect(0, 25, GUILayout.ExpandWidth(true));
        GUI.Box(_dropZone, "Drop objects here");
        switch (_e.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!_dropZone.Contains(_e.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (_e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    if (DragAndDrop.objectReferences == null) return;
                    foreach (UnityEngine.Object o in DragAndDrop.objectReferences)
                    {
                        GameObject _go = (GameObject)o;
                        if (_go)
                            p_target.AddObject(_go);
                    }
                }
                break;
            default:
                break;
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    void EditGlowWindow()
    {
        GlowEditWindow _window = (GlowEditWindow)EditorWindow.GetWindow(typeof(GlowEditWindow));
        _window.Show();
    }

    void EditLightsWindow()
    {
        GlowLightEditWindow _window = (GlowLightEditWindow)EditorWindow.GetWindow(typeof(GlowLightEditWindow));
        _window.Show();
    }

    void ManageObjects()
    {
        for (int o = 0; o < p_target.AllObjectsToMakeGlowy.Count; o++)
        {            
            p_target.AllObjectsToMakeGlowy[o].AddComponent<GlowEffect>();            
        }
    }

    void ShowNHideListWindow(List<GameObject> _objectsToMakeGlowy)
    {
        GlowListWindow _window = (GlowListWindow)EditorWindow.GetWindow(typeof(GlowListWindow));
        _window.Show();        
    }
    #endregion 

    #region UniMeths
    public void OnEnable ()
    {
        p_target = (GlowManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUITools.ActionButton("Change camera",ChangeCamera, Color.green, Color.black);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DropAeraGUI();
        ManageObjects();
        GUITools.ActionButton("Show list", ShowNHideListWindow,p_target.AllObjectsToMakeGlowy , Color.cyan, Color.black);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUITools.ActionButton("Edit glows", EditGlowWindow, Color.magenta, Color.black);
        EditorGUILayout.Space();
        GUITools.ActionButton("Edit lights", EditLightsWindow, Color.yellow, Color.black);
    }
    #endregion
}
