using UnityEngine;

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
    [SerializeField] private AudioClip fishingRodClip = null; 
    #endregion

    #region Methods

    #region Original Methods
    public void PlayFishingRodSound()
    {
        if (!audioSource || !fishingRodClip) return;
        TDS_SoundManager.Instance.PlaySoundAtPosition(audioSource, fishingRodClip, transform.position, TDS_SoundManager.FX_GROUP_NAME, true, 1); 
    }

    public void StopFishingRodSound()
    {
        if (!audioSource || !audioSource.isPlaying) return;
        audioSource.Stop();
        audioSource.clip = null; 
    }
    #endregion

    #region Unity Methods
    #endregion

    #endregion
}
