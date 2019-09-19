using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(TDS_Mime))]
public class TDS_MimeEditor : TDS_MinionEditor 
{
    /* TDS_MimeEditor :
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
    /// <summary>SerializedProperty for <see cref="TDS_Mime.fishngRodClip"/> of type <see cref="AudioClip"/>. </summary>
    private SerializedProperty fishingRodClip = null;
    #endregion

    #region Methods

    #region Original Methods
    protected override void DrawEnemyEditor()
    {
        base.DrawEnemyEditor();
        TDS_EditorUtility.PropertyField("Fishing rod clip", "Clip played during the fishing rod animation", fishingRodClip);

        serializedObject.ApplyModifiedProperties(); 
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        fishingRodClip = serializedObject.FindProperty("fishingRodClip"); 
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
