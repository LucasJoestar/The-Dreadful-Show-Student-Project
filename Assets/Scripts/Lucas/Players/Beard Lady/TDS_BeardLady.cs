using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_BeardLady : TDS_Player 
{
    /* TDS_BeardLady :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[19 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_BeardLady class.
     *	
     *	    Implemented methods to regulate the state of the beard.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="CurrentBeardState"/>.</summary>
    [SerializeField] private BeardState currentBeardState = BeardState.Normal;

    /// <summary>
    /// Current state of the Beard Lady's beard.
    /// The more long it is, the more long the character range is.
    /// </summary>
    public BeardState CurrentBeardState
    {
        get { return currentBeardState; }
        private set
        {
            currentBeardState = value;

            SetAnimBeard(value);
        }
    }

    /// <summary>Backing field for <see cref=""/>.</summary>
    [SerializeField] private float beardGrowInterval = 5;

    /// <summary>
    /// Interval at which the Beard Lady's beard grow if everything is okay.
    /// </summary>
    public float BeardGrowInterval
    {
        get { return beardGrowInterval; }
        set
        {
            if (value < 0) value = 0;
            beardGrowInterval = value;

            ResetBeardGrow();
        }
    }
    #endregion

    #region Methods

    #region Original Methods

    #region Beard
    /// <summary>
    /// Let's degrade that kind woman's beard !
    /// </summary>
    private void DegradeBeard()
    {
        if (currentBeardState > 0)
        {
            if (currentBeardState == BeardState.VeryVeryLongDude)
            {
                InvokeBeard();
            }

            CurrentBeardState--;
        }
    }

    /// <summary>
    /// Let's grow the beard of this sweet lady !
    /// </summary>
    private void GrowBeard()
    {
        CurrentBeardState++;

        if (currentBeardState == BeardState.VeryVeryLongDude)
        {
            CancelInvokeBeard();
        }
    }

    /// <summary>
    /// Reset variables used to grow the beard.
    /// </summary>
    private void ResetBeardGrow()
    {
        CancelInvokeBeard();
        InvokeBeard();
    }

    /// <summary>
    /// Invoke repeatedly the method to grow the beard.
    /// </summary>
    private void InvokeBeard() => InvokeRepeating("GrowBeard", beardGrowInterval, beardGrowInterval);

    /// <summary>
    /// Cancel invoke of the method to grow the beard.
    /// </summary>
    private void CancelInvokeBeard() => CancelInvoke("GrowBeard");
    #endregion

    #region Health
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected override void Die()
    {
        base.Die();

        // Stop beard from growing
        CancelInvoke("GrowBeard");
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        // Executes base method
        if (!base.TakeDamage(_damage)) return false;

        // Reset beard grow on hit
        ResetBeardGrow();

        return true;
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <param name="_position">Position in world space from where the hit come from.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage, Vector3 _position)
    {
        // Executes base method
        if (!base.TakeDamage(_damage, _position)) return false;

        // Reset beard grow on hit
        ResetBeardGrow();

        return true;
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set the Beard Lady's beard animator state.
    /// </summary>
    /// <param name="_state">State of the beard.</param>
    public void SetAnimBeard(BeardState _state)
    {
        animator.SetInteger("Beard", (int)_state);
    }
    #endregion

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

        // Let's make this beard grow repeatedly until it reach its maximum value
        if (currentBeardState != BeardState.VeryVeryLongDude) InvokeBeard();
        SetAnimBeard(currentBeardState);
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
	}
	#endregion

	#endregion
}
