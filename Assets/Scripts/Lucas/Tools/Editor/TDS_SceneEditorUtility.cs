using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class TDS_SceneEditorUtility
{
    /* TDS_SceneEditorUtility# :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Class to centralize scene-related tools shortcuts.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_SceneEditorUtility class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #endregion

    #region Methods
    /// <summary>
    /// Launch the whole game from the first scene in build index.
    /// </summary>
    /*[MenuItem("Tools/Scenes/Launch Game from Start")]
    public static void StartGame()
    {
        //
    }*/
    #endregion
}

public class TDS_LoadSceneWindow : EditorWindow
{
    /* TDS_LoadSceneWindow# :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Window allowing to load any project scene.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_LoadSceneWindow.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// All project loaded scenes path.
    /// </summary>
    private string[] allScenesPath = new string[] { };
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Call this window from the Unity menu toolbar.
    /// </summary>
    [MenuItem("Tools/Scenes Loader")]
    public static void CallWindow()
    {
        GetWindow(typeof(TDS_LoadSceneWindow), true, "Load Scene Window").Show();
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        allScenesPath = Array.ConvertAll<string, string>(AssetDatabase.FindAssets("t:Scene"), AssetDatabase.GUIDToAssetPath);
        Array.Sort(allScenesPath);

        maxSize = new Vector2(275, (allScenesPath.Length * 20) + 5);
        minSize = maxSize;
    }

    // Implement your own editor GUI here
    private void OnGUI()
    {
        foreach (string _scene in allScenesPath)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(_scene));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load"))
            {
                EditorSceneManager.OpenScene(_scene);
                Close();
                return;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion

    #endregion
}
