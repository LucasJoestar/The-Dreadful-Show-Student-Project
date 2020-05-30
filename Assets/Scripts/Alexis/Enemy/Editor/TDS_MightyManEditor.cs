using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(TDS_MightyMan))]
public class TDS_MightyManEditor : TDS_MinionEditor 
{
    /* TDS_MightyManEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>SerializedProperty for <see cref="TDS_Minion.playFirstTaunt"/> of type <see cref="bool"/>.</summary>
    SerializedProperty playFirstTaunt = null;
    #endregion

    #region Methods

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        playFirstTaunt = serializedObject.FindProperty("PlayFirstTaunt");
    }

    protected override void DrawSettings()
    {
        TDS_EditorUtility.Toggle("Play First Taunt", string.Empty, playFirstTaunt);
        base.DrawSettings();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
    }
    #endregion

    #endregion
}
