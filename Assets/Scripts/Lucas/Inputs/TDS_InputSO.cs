using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TDS_InputSO : ScriptableObject
{
    /* TDS_InputSO :
     * 
     *  #####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Stock all game inputs into a scriptable object
     *	
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[05 / 07 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TDS_InputSO class.
    */

    #region Fields / Properties
    /// <summary>
    /// Path from a resources folder where to find the input asset.
    /// </summary>
    public const string INPUT_ASSET_PATH = "Input/InputAsset";


    /// <summary>
    /// All controllers available for the game.
    /// </summary>
    public TDS_Controller[] Controllers = new TDS_Controller[] { };

    /// <summary>
    /// All axis from this project.
    /// </summary>
    public TDS_AxisToInput[] Axis = new TDS_AxisToInput[] { };

    /// <summary>
    /// All custom buttons from this project.
    /// </summary>
    public TDS_Button[] Buttons = new TDS_Button[] { };
    #endregion

    #if UNITY_EDITOR
    /// <summary>
    /// Creates a new controller in the input asset.
    /// </summary>
    [MenuItem("Tools/Inputs/Create Controller")]
    public static void CreateController()
    {
        TDS_InputSO _inputs = Resources.Load<TDS_InputSO>(INPUT_ASSET_PATH);
        if (!_inputs)
        {
            CreateInputAsset();
            _inputs = Resources.Load<TDS_InputSO>(INPUT_ASSET_PATH);
        }

        TDS_Controller[] _newControllers = new TDS_Controller[_inputs.Controllers.Length + 1];

        for (int _i = 0; _i < _newControllers.Length - 1; _i++)
        {
            _newControllers[_i] = _inputs.Controllers[_i];
        }

        _newControllers[_newControllers.Length - 1] = new TDS_Controller();
        _inputs.Controllers = _newControllers;
    }

    /// <summary>
    /// Creates a input asset if none exist
    /// </summary>
    [MenuItem("Tools/Inputs/Create Input Asset")]
    public static void CreateInputAsset()
    {
        // If an input asset already exist, just return
        if (Resources.Load(INPUT_ASSET_PATH) != null) return;

        TDS_InputSO _inputAsset = CreateInstance<TDS_InputSO>();

        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "Resources", System.IO.Path.GetDirectoryName(INPUT_ASSET_PATH)));

        AssetDatabase.CreateAsset(_inputAsset, System.IO.Path.Combine("Assets/Resources", INPUT_ASSET_PATH) + ".asset");
        AssetDatabase.SaveAssets();
    }
#endif
}
