using UnityEngine;

public class TDS_WaveBehaviour : MonoBehaviour 
{
    /* TDS_WaveBehaviour :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages the start animation moment and the speed of the waves.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[13 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_WaveBehaviour class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Animator of the wave.
    /// </summary>
    [SerializeField] private Animator animator = null;
	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

	// Use this for initialization
    private void Start()
    {
        if (animator)
        {
            animator.Play("Waves", 0, Random.Range(.001f, .999f));
            animator.SetFloat("Speed", Random.Range(.9f, 1.1f));
        }
    }
	#endregion
}
