using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 

public class TDS_Mime : TDS_Minion 
{
    /* TDS_Mime :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
     *	Class that holds the behaviour of the Mime.
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

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Cast the wandering method 
    /// When too much enemies attack the same player, the enemy has to wander 
    /// Move until reaching a position then wait between 0 and 1 seconds before searching a new target
    /// Before moving again, taunt
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Wander()
    {
        yield return base.Wander();
        agent.StopAgent();
        
    }


    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake(); 
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
