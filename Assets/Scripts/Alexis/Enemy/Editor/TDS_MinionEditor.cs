using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor;

#pragma warning disable 0414

[CustomEditor(typeof(TDS_Minion))]
public class TDS_MinionEditor : TDS_EnemyEditor 
{
    /* TDS_MinionEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
     *	Editor class of the TDS_Punk
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[13/02/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the editor class]
     *	    - Add serialized properties for the properties hasEvolved and attacks
	 *	-----------------------------------
	*/

    #region Fields and Properties

    #region Serialized Properties
    /// <summary>SerializedProperty for <see cref="TDS_Minion.hasEvolved"/> of type <see cref="bool"/>.</summary>
    SerializedProperty hasEvolved = null;

    SerializedProperty ragingThreshold = null;
    SerializedProperty resetRageDelay = null; 

    #endregion

    #region FoldOut
    /// <summary>Backing field for <see cref="IsMinionUnfolded"/></summary>
    private bool isMinionUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Character class is unfolded or not.
    /// </summary>
    public bool IsMinionUnfolded
    {
        get { return isMinionUnfolded; }
        set
        {
            isMinionUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isMinionUnfolded", value);
        }
    }
    #endregion

    #region Target Script Info
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isMinionMultiEditing = false;

    /// <summary>
    /// All editing instances of the Enemy class.
    /// </summary>
    private List<TDS_Minion> minions = new List<TDS_Minion>();
    #endregion 

    #endregion

    #region Methods
    protected override void DrawSettings()
    {
        base.DrawSettings();

        EditorGUILayout.LabelField("Evolve", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.RadioToggle("Has Evolved", "Is the minion evolved", hasEvolved);
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Rage", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.FloatField("Reset Rage Delay", "Delay to reset the rage", resetRageDelay);
        TDS_EditorUtility.IntSlider("Reset Rage Threshold", "Count of attacks to ignore the next damages", ragingThreshold, 1, 5); 
    }

    #region Unity Methods
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => minions.Add((TDS_Minion)t));
        if (targets.Length == 1) isMinionMultiEditing = false;
        else isMinionMultiEditing = true;

        // Get the property
        hasEvolved = serializedObject.FindProperty("hasEvolved");
        resetRageDelay = serializedObject.FindProperty("resetRageDelay");
        ragingThreshold = serializedObject.FindProperty("ragingThreshold"); 

        //Get fold bool
        isMinionUnfolded = EditorPrefs.GetBool("isMinionUnfolded", isMinionUnfolded);
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
    }
    #endregion

    #endregion
}
