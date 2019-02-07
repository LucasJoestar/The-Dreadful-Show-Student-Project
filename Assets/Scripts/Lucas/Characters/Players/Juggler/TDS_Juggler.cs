using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_Juggler : TDS_Player 
{
    /* TDS_Juggler :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Script of the Juggler controller. Now, you can play him. Yes you can.
     *	There should be only one juggler in by scene.
     *	    Just add this script to an object to make it behaviour as the Juggler,
     *	and be able to play with him.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[07 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the SetAnimHeavyAttack & SetAnimLightAttack methods.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[04 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Juggler class.
     *	
     *	    - Added the selectedThrowableIndex & Throwables fields ; added the SelectedThrowable property.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Throwables
    /// <summary>
    /// Index of the current selected throwable from <see cref="Throwables"/>.
    /// </summary>
    private int selectedThrowableIndex = 0;

    /// <summary>
    /// The actual selected throwable. It is the object to throw, when throwing... Yep.
    /// </summary>
    public TDS_Throwable SelectedThrowable
    {
        get { return Throwables[selectedThrowableIndex]; }
    }

    /// <summary>
    /// All throwables the juggler has in hands.
    /// </summary>
    public List<TDS_Throwable> Throwables = new List<TDS_Throwable>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Aim & Throwables
    /// <summary>
    /// Make the juggler aim for a throw. When releasing the throw button, throw the selected object.
    /// If the cancel throw button is pressed, cancel the throw, as it name indicate it.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Aim()
    {
        base.Aim();
        yield break;
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public override void Attack(bool _isLight)
    {
        // Triggers the right actions
        switch (comboCurrent.Count)
        {
            case 1:
                if (_isLight)
                {
                    currentAttack = attacks[0];
                    SetAnimLightAttack();
                }
                else
                {
                    currentAttack = attacks[1];
                    SetAnimHeavyAttack();
                }
                break;
            default:
                Debug.Log($"The Juggler was not intended to have more than one attack per combo, so... What's going on here ?");
                break;
        }
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set this player heavy attack animation.
    /// </summary>
    public void SetAnimHeavyAttack()
    {
        animator.SetTrigger("Heavy Attack");
    }

    /// <summary>
    /// Set this player light attack animation.
    /// </summary>
    public void SetAnimLightAttack()
    {
        animator.SetTrigger("Light Attack");
    }
    #endregion

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected override void FixedUpdate()
    {
        // If dead, return
        if (isDead) return;

        base.FixedUpdate();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // If dead, return
        if (isDead) return;

        base.Update();
	}
	#endregion

	#endregion
}
