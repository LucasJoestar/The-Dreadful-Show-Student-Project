using System;
using System.Collections;
using System.Collections.Generic;
using System.IO; 
using UnityEngine;

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
    /// <summary>
    /// Database of all audioclips stored by names
    /// </summary>
    private Dictionary<string, AudioClip> audioClipByNames = new Dictionary<string, AudioClip>(); 
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Load the sound asset bundle of the audioclips
    /// Add the audioclips into the Dictionary audioClipByNames
    /// </summary>
    private void LoadAssetBundle()
    {
        AssetBundle _soundBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "SoundAssetsBundle")); 
        if(!_soundBundle)
        {
            Debug.Log("Asset Bundle not found");
            return; 
        }
        AudioClip[] _audioclips = _soundBundle.LoadAllAssets<AudioClip>();
        for (int i = 0; i < _audioclips.Length; i++)
        {
            audioClipByNames.Add(_audioclips[i].name, _audioclips[i]); 
        }
    }

    /// <summary>
    /// Get the audioclip with the specified name
    /// </summary>
    /// <param name="_name">Name of the audioclip</param>
    /// <returns></returns>
    public AudioClip GetAudioClipByName(string _name)
    {
        if (!audioClipByNames.ContainsKey(_name)) return null;
        return audioClipByNames[_name]; 
    }

    /// <summary>
    /// Get the audio clip with the selected name and play it
    /// </summary>
    /// <param name="_audioclipName">Name of the audioclip to play</param>
    public void PlayAudioClipByName(string _audioclipName)
    {
        AudioClip _clip = GetAudioClipByName(_audioclipName);
        if (!_clip) return;
        /// PLAY THE AUDIO CLIP HERE /// 
        /// Where do we have to play the clip? 
        /// Does the clip has to follow something or does it follow the object? The Camera? Anything else?
        AudioSource.PlayClipAtPoint(_clip, transform.position); 
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        LoadAssetBundle(); 
    }
    #endregion

    #endregion
}
