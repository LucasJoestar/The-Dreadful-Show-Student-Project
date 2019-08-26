using UnityEngine;
using UnityEngine.Audio;
using System.Linq; 

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

    #region Events

    #endregion

    #region Fields / Properties

    #region Constants 
    public const string MUSIC_GROUP_NAME = "Music";
    public const string VOICES_GROUP_NAME = "Voices";
    public const string FX_GROUP_NAME = "FX";

    #endregion
    public static TDS_SoundManager Instance = null;
    [SerializeField] private AudioMixer audioMixer = null;
    #region AudioClip

    #endregion
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Get the mixer group of the groupName
    /// USE THIS METHOD WITH THE CONST MUSIC_GROUP_NAME. VOICES_GROUP_NAME AND FX_GROUP_NAME
    /// </summary>
    /// <param name="_groupName"></param>
    /// <returns></returns>
    public AudioMixerGroup GetMixerGroupOfName(string _groupName)
    {
        if (!audioMixer) return null;
        return audioMixer.FindMatchingGroups(_groupName).FirstOrDefault(); 
    }

    /// <summary>
    /// Play a sound at a position
    /// </summary>
    /// <param name="_source">Source that will play the sound</param>
    /// <param name="_playedClip">Clip taht will be played</param>
    /// <param name="_position">Position where the clip will be played</param>
    /// <param name="_groupName">Group of the audioMixer /!\ USE THE CONST IN THE SOUNDMANAGER TO GET THE RIGHT NAME OF THE GROUP /!\ </param>
    /// <param name="_isLooping">Does the sound has to loop</param>
    public void PlaySoundAtPosition(AudioSource _source, AudioClip _playedClip, Vector3 _position, string _groupName = "Master", bool _isLooping = false, float _volume = 1f)
    {
        _source.transform.position = _position;
        _source.outputAudioMixerGroup = GetMixerGroupOfName(_groupName);
        if(_isLooping)
        {
            _source.loop = true;
            _source.volume = _volume;
            _source.clip = _playedClip;
            _source.Play();
            return; 
        }
        _source.loop = false;
        _source.PlayOneShot(_playedClip, _volume); 
    }
    #endregion

    #region Unity Methods
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
