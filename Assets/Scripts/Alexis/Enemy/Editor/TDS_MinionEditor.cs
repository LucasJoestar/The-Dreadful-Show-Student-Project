using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor; 

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

    /// <summary>SerializedProperty for <see cref="TDS_Minion.attacks"/> of type <see cref="bool"/>.</summary>
    SerializedProperty attacks = null;
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
    private void DrawSettings()
    {
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        if (TDS_EditorUtility.Button("Minion", "Wrap / unwrap Punk class settings", TDS_EditorUtility.HeaderStyle)) IsMinionUnfolded = !isMinionUnfolded;
        if (isMinionUnfolded)
        {
            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("HelpBox");

            //Draw a header for the enemy evolution settings
            EditorGUILayout.LabelField("Attack Settings", TDS_EditorUtility.HeaderStyle);
            TDS_EditorUtility.PropertyField("Minion Attacks", "", attacks);

            GUILayout.Space(3); 

            //Draw a header for the enemy evolution settings
            EditorGUILayout.LabelField("Evolve Settings", TDS_EditorUtility.HeaderStyle);
            TDS_EditorUtility.RadioToggle("Has evolved", "Is the enemy evolved", hasEvolved);

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties(); 

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
    }

    #region Unity Methods
    public override void OnInspectorGUI()
    {
        DrawSettings();

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
        attacks = serializedObject.FindProperty("attacks"); 

        //Get fold bool
        isMinionUnfolded = EditorPrefs.GetBool("isMinionUnfolded", isMinionUnfolded);
    }
    #endregion

    #endregion
}
