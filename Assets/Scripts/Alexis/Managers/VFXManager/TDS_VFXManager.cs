using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq; 

public class TDS_VFXManager : MonoBehaviour 
{
    /* TDS_VFXManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
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
     *	    - Create the vfx database from a Asset Bundle
     *	    - Get the particle system by name
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    public static TDS_VFXManager Instance = null;

    private Dictionary<string, ParticleSystem> particleSystemsByName = new Dictionary<string, ParticleSystem>();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Load the VFX asset bundle of the particles systems
    /// Add the particles systems into the Dictionary particleSystemsByName
    /// </summary>
    private void LoadAssetBundle()
    {
        AssetBundle _vfxBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "VFXAssetsBundle"));
        if (!_vfxBundle)
        {
            Debug.Log("Asset Bundle not found");
            return;
        }
        ParticleSystem[] _particlesSystems = _vfxBundle.LoadAllAssets<ParticleSystem>();
        for (int i = 0; i < _particlesSystems.Length; i++)
        {
            particleSystemsByName.Add(_particlesSystems[i].name, _particlesSystems[i]);
        }
    }

    /// <summary>
    /// Get the Particle System with the name 
    /// </summary>
    /// <param name="_name">Name of the particle system</param>
    /// <returns></returns>
    public ParticleSystem GetParticleSystemByName(string _name)
    {
        if (!particleSystemsByName.ContainsKey(_name)) return null;
        return particleSystemsByName[_name]; 
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!Instance)
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
