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

    #region Fields and properties
    protected bool isFoldOut = true; 
    #endregion

    #region Methods
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!isFoldOut) return 25; 
        return 125 ;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.DrawRect(position, TDS_EditorUtility.BoxLightColor); 
        Rect _rect = new Rect(position.position.x, position.position.y, position.width, 25); 
        //Display the name of the enemy 
        isFoldOut = EditorGUI.Foldout(_rect, isFoldOut, property.FindPropertyRelative("enemyResourceName").stringValue, true, TDS_EditorUtility.HeaderStyle); 
        // Display the number of enemy to spawn
        if(property.FindPropertyRelative("enemyCount").arraySize == 0)
        {
            property.FindPropertyRelative("enemyCount").arraySize = 4; 
        }
        if(isFoldOut)
        {
            for (int i = 0; i < property.FindPropertyRelative("enemyCount").arraySize; i++)
            {
                _rect = new Rect(position.position.x + 50, position.position.y + (25 * (i + 1)), position.width - 75, 20);
                property.FindPropertyRelative("enemyCount").GetArrayElementAtIndex(i).intValue = EditorGUI.IntSlider(_rect, $"{i + 1} Players", property.FindPropertyRelative("enemyCount").GetArrayElementAtIndex(i).intValue, 0, 10);
            }
        }
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
        if(isFoldOut) TDS_EditorUtility.IntSlider("Spawn Chance", "", property.FindPropertyRelative("spawnChance"), 1, 100);

    }
    #endregion
}