using UnityEngine;
using UnityEditor;

public class XboxControllerTool : EditorWindow
{
    #region F/P
    Texture XboxTexture = null;
    #endregion

    #region Meths
    [MenuItem("Tools/XBox Controller/Windows Mapping")]
    public static void ShowWindow()
    {
        XboxControllerTool _window = (XboxControllerTool)GetWindow(typeof(XboxControllerTool));
        _window.InitMenu("Windows");
        _window.Show();
    }

    [MenuItem("Tools/XBox Controller/Mac Mapping")]
    public static void ShowMac()
    {
        XboxControllerTool _window = (XboxControllerTool)GetWindow(typeof(XboxControllerTool));
        _window.InitMenu("Mac");
        _window.Show();
    }

    [MenuItem("Tools/XBox Controller/Linux Mapping")]
    public static void ShowLinux()
    {      
        XboxControllerTool _window = (XboxControllerTool)GetWindow(typeof(XboxControllerTool));
        _window.InitMenu("Linux");
        _window.Show();
    }

    void InitMenu(string _OS)
    {
       XboxTexture = Resources.Load(_OS) as Texture;
    }  

    void OnGUI()
    {        
        GUI.color = Color.red;
        if (GUILayout.Button("X"))
        {
            Close();
        }
        GUI.color = Color.white;

        EditorGUILayout.Space();

        if (XboxTexture)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Rect _textureRect = GUILayoutUtility.GetRect(XboxTexture.width/1.5f, XboxTexture.height/1.5f);
            EditorGUI.DrawPreviewTexture(_textureRect, XboxTexture);

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("No Texture founded!!!!", MessageType.Error);
        }
    }
    #endregion
}