using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class TDS_MrLoyal : TDS_Boss 
{
    /* TDS_MrLoyal :
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
    [SerializeField] private List<TDS_SpawnerArea> linkedArenas = new List<TDS_SpawnerArea>();
    [SerializeField] private float chargeCatsRate = 5f;
    // [SerializeField] private TDS_Cat[] cats = null; 
    // METTRE LE POUR DE TP DANS LE FX MANAGER

    #endregion

    #region Methods

    #region Original Methods               
    /// <summary>
    /// Make MrLoyal gets out of the battle, then make him call the cats at a rate
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetOutOfBattle()
    {
        /// Call the particle here
        /// Then wait some time and teleport Mr Loyal on his plateform
        /// Instantiate again particles on the teleportation spot
        TDS_SpawnerArea _currentArea = linkedArenas.Where(a => !a.IsDesactivated).FirstOrDefault(); 
        if(!_currentArea)
        {
            GetBackIntoBattle(); 
            yield break; 
        }
        _currentArea.StartSpawnArea();
        while (!_currentArea.IsDesactivated)
        {
            yield return new WaitForSeconds(chargeCatsRate);
            //Activate cats
        }
        GetBackIntoBattle(); 
    }

    /// <summary>
    /// Set MrLoyal's State to get him out of the battle
    /// </summary>
    private void RemoveFromBattle()
    {
        SetEnemyState(EnemyState.OutOfBattle);
        SetAnimationState((int)EnemyAnimationState.GetOutOfBattle); 
    }

    private void GetBackIntoBattle()
    {
        SetAnimationState((int)EnemyAnimationState.GetBackIntoBattle);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        onHalfHealth.AddListener(RemoveFromBattle); 
    }

	// Use this for initialization
    protected override void Start()
    {
        base.Start(); 
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
