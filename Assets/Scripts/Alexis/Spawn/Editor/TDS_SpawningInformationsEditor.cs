using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TDS_SpawningInformations))]
public class TDS_SpawningInformationsEditor : PropertyDrawer
{
    /* TDS_SpawningInformationsEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Editor of the Spawning Information class
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[12/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation de la class]
     *   - Implémentation de la methode OnGUI pour afficher les settings de la Spawning Information 

	 *	-----------------------------------
	*/

    #region Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Display the name of the enemy 
        GUILayout.Label(property.FindPropertyRelative("enemyResourceName").stringValue, TDS_EditorUtility.HeaderStyle); 
        // Display the number of enemy to spawn
        TDS_EditorUtility.IntSlider("Number of enemies", "", property.FindPropertyRelative("enemyCount"), 1, 10);
    }
    #endregion
}

[CustomPropertyDrawer(typeof(TDS_RandomSpawningInformations))]
public class TDS_RandomSpawningInformationsEditor : TDS_SpawningInformationsEditor
{
    /* TDS_SpawningInformationsEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Editor of the Random Spawning Information class
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[12/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation de la class]
	 *      - Implémentation de la methode OnGUI pour afficher les settings de la Random Spawning Information 
	 *	-----------------------------------
	*/

    #region Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
        // Display the chance of spawn for the random enemies
        TDS_EditorUtility.IntSlider("Spawn Chance", "", property.FindPropertyRelative("spawnChance"), 1, 100);

    }
    #endregion
}