using UnityEngine;

public class TDS_WavesBehaviour : MonoBehaviour 
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

	#region Unity Methods
	// Use this for initialization
    private void Start()
    {
        Animator[] _animators = GetComponentsInChildren<Animator>();

        foreach (Animator _animator in _animators)
        {
            _animator.Play("Waves", 0, Random.Range(.001f, .999f));
            _animator.SetFloat("Speed", Random.Range(.9f, 1.1f));
        }

        Destroy(this);
    }
	#endregion
}
