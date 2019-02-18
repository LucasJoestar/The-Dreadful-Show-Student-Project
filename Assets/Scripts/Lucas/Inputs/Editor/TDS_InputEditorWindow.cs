using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TDS_InputEditorWindow : EditorWindow 
{
    /* TDS_InputEditorWindow :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Editor window used to edit inputs of the project from a scriptable object TDS_InputSettings reference.
     *	If no reference is find, create one.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[14 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_InputEditorWindow class.
     *	
     *	    - Added the scriptableObjectEditor field.field.
     *	    - Added the CallEditor, CreateInputAsset & LoadInputAsset static methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Editor
    /// <summary>
    /// Editor of the editing scriptable object to display.
    /// </summary>
    [SerializeField] private static Editor scriptableObjectEditor = null;

    /// <summary>Accessor for <see cref="scriptableObjectEditor"/>.</summary>
    public static Editor ScriptableObjectEditor { get { return scriptableObjectEditor; } }
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Menu Navigation & Asset Folder
    /// <summary>
    /// Calls the input editor window.
    /// </summary>
    [MenuItem("Window/Input Settings")]
    public static void CallEditor()
    {
        TDS_InputEditorWindow _window = GetWindow<TDS_InputEditorWindow>("Input Settings");
        _window.Show();
    }

    /// <summary>
    /// Creates the input settings scriptable object in the project.
    /// </summary>
    /// <param name="_ref">Object to create.</param>
    private static void CreateInputAsset(TDS_InputSettings _ref)
    {
        // Creates the directory of the asset if it does not exist
        string _path = Application.dataPath + "/Resources/" + Path.GetDirectoryName(TDS_InputSettings.INPUT_SO_DEFAULT_PATH);

        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }

        AssetDatabase.CreateAsset(_ref, "Assets/Resources/" + TDS_InputSettings.INPUT_SO_DEFAULT_PATH + ".asset");
    }

    /// <summary>
    /// Loads the input settings scriptable object of the project.
    /// </summary>
    [InitializeOnLoadMethod]
    private static void LoadInputAsset()
    {
        //Debug.Log("Editor ? => " + (scriptableObjectEditor != null));

        // If already having an editor, return
        if (scriptableObjectEditor) return;

        // Loads the scriptable object reference or create one if not found any
        TDS_InputSettings _so = Resources.Load<TDS_InputSettings>(TDS_InputSettings.INPUT_SO_DEFAULT_PATH);

        if (!_so)
        {
            _so = CreateInstance<TDS_InputSettings>();
            CreateInputAsset(_so);
        }

        // Creates the editor of the editing object
        scriptableObjectEditor = Editor.CreateEditor(_so);
    }
    #endregion

    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Loads the editor
        LoadInputAsset();
    }

    // Implement your own editor GUI here
    private void OnGUI()
    {
        // If the editing object is null, get or create one and refresh
        if (!scriptableObjectEditor.target || scriptableObjectEditor.serializedObject == null)
        {
            DestroyImmediate(scriptableObjectEditor);
            LoadInputAsset();
            return;
        }

        // Draws the header & the inspector of the scriptable object reference
        scriptableObjectEditor.DrawHeader();
        scriptableObjectEditor.OnInspectorGUI();
    }
    #endregion

    #endregion
}
