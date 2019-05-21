using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FireEater : TDS_Player 
{
    /* TDS_FireEater :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Gameplay class manipulating the Fire Eater player.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[21 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_FireEater class.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Variables
    /// <summary>Backing field for <see cref="IsDrunk"/>.</summary>
    [SerializeField] private bool isDrunk = false;

    /// <summary>
    /// Indicates if the Fire Eater is currently drunk.
    /// </summary>
    public bool IsDrunk
    {
        get { return isDrunk; }
        set
        {
            if (value) SetFireEaterAnim(FireEaterAnimState.Drunk);
            else SetFireEaterAnim(FireEaterAnimState.Sober);

            isDrunk = value;
        }
    }

    /// <summary>Backing field for <see cref="SoberUpTime"/>.</summary>
    [SerializeField] private float soberUpTime = 10;

    /// <summary>
    /// Time it takes to the Fire Eater to sober up.
    /// </summary>
    public float SoberUpTime
    {
        get { return soberUpTime; }
        set
        {
            if (value < 0) value = 0;
            soberUpTime = value;
        }
    }
    #endregion

    #region Memory
    /// <summary>
    /// Timer used to make the Fire Eater sober up, in the GetDrunkCoroutine method.
    /// </summary>
    [SerializeField] private float soberUpTimer = 0;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Drunk
    /// <summary>
    /// Starts the coroutine to make the Fire Eater get drunk.
    /// </summary>
    public void GetDrunk() => StartCoroutine(GetDrunkCotourine());

    /// <summary>
    /// Makes the Fire Eater get drunk, and sober him up after a certain time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDrunkCotourine()
    {
        IsDrunk = true;

        soberUpTimer = soberUpTime;

        while (soberUpTime > 0)
        {
            yield return null;
            soberUpTimer -= Time.deltaTime;
        }

        IsDrunk = false;
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set an animation state of the Fire Eater, used in the animator.
    /// </summary>
    /// <param name="_state">State to set in animation.</param>
    public void SetFireEaterAnim(FireEaterAnimState _state)
    {
        switch (_state)
        {
            case FireEaterAnimState.Sober:
                animator.SetBool("IsDrunk", false);
                break;

            case FireEaterAnimState.Drunk:
                animator.SetBool("IsDrunk", true);
                break;

            default:
                break;
        }
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

        // Set player type, just in case
        PlayerType = PlayerType.FireEater;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
