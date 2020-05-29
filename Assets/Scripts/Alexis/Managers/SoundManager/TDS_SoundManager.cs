using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TDS_SoundManager : MonoBehaviour
{
    /* TDS_SoundManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Holds all sounds of the game and play them]
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
	 *	Date :			[05/07/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the class]
     *	    - Create the audioclip database from a Asset Bundle
     *	    - Get the audioclip by name
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Audio Sources & Mixer
    /// <summary>
    /// Audio mixer to play sounds.
    /// </summary>
    [SerializeField] private AudioMixer audioMixer = null;

    /// <summary>
    /// Audio source used to play narrator quotes.
    /// </summary>
    [SerializeField] private AudioSource narratorSource = null;
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_SoundManager Instance = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Narrator
    /// <summary>
    /// Plays a narrator quote audio track.
    /// </summary>
    /// <param name="_audioTrack">Audio track to play.</param>
    public void PlayNarratorQuote(AudioClip _audioTrack)
    {
        narratorSource.clip = _audioTrack;
        narratorSource.Play();
    }
    #endregion

    #region UI
    /// <summary>
    /// (Un)pause the game.
    /// </summary>
    /// <param name="_doPause">Should the game be pause or not.</param>
    public void Pause(bool _doPause)
    {
        AudioListener.pause = _doPause;

        // PAUSE
    }

    /// <summary>
    /// Plays a sound for confirm in UI.
    /// </summary>
    public void PlayUIConfirm() => AkSoundEngine.PostEvent("Play_HUD_CONFIRM", TDS_GameManager.MainAudio);

    /// <summary>
    /// Plays a sound for a big confirm in UI.
    /// </summary>
    public void PlayUIBigConfirm() => AkSoundEngine.PostEvent("Play_HUD_BIGCONFIRM", TDS_GameManager.MainAudio);

    /// <summary>
    /// Plays a sound for "over" effect in UI.
    /// </summary>
    public void PlayUIOver() => AkSoundEngine.PostEvent("Play_HUD_OVER", TDS_GameManager.MainAudio);

    /// <summary>
    /// Plays a sound when player is ready.
    /// </summary>
    public void PlayUIReady() => AkSoundEngine.PostEvent("Play_HUD_READY", TDS_GameManager.MainAudio);
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this; 
        }
        else
        {
            Destroy(this);
            return; 
        }
    }
    #endregion

    #endregion
}
