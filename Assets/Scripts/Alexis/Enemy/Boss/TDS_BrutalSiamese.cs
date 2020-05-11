using System.Collections;
using UnityEngine;

public class TDS_BrutalSiamese : TDS_Enemy 
{
    /* TDS_BrutalSiamese :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	[Behaviour of the brutal siamese]
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
     *	Date :			[24/05/2019]
     *	Author :		[THIEBAUT Alexis]
     *
     *	Changes :
     *
     *	[Initialisation of the class]
     *  
     *	-----------------------------------
    */

    #region Events

    #endregion

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods
    private IEnumerator WaitSecondsBeforeAttacking(float _waitingSeconds = 2f)
    {
        IsPacific = true;
        yield return new WaitForSeconds(_waitingSeconds);
        IsPacific = false;
    }
    #endregion

    #region Overriden Methods
    protected override Vector3 GetAttackingPosition(out bool _hasToWander)
    {
        _hasToWander = false; 
        return base.GetAttackingPosition();
    }

    protected override void Die()
    {
        base.Die();
        if (Area) Area.RemoveEnemy(this);
    }
    #endregion

    #region Unity Methods
	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        canThrow = false;

        StartCoroutine(WaitSecondsBeforeAttacking());
    }
	#endregion

	#endregion
}
