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
        AssetBundle _vfxBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "AssetBundles", "vfxassetsbundle"));
        if (!_vfxBundle)
        {
            Debug.Log("Asset Bundle not found");
            return;
        }
        GameObject[] _particlesSystems = _vfxBundle.LoadAllAssets<GameObject>();
        for (int i = 0; i < _particlesSystems.Length; i++)
        {
            particleSystemsByName.Add(_particlesSystems[i].name, _particlesSystems[i].GetComponent<ParticleSystem>());
        }
    }

    /// <summary>
    /// Get the Particle System with the name 
    /// </summary>
    /// <param name="_name">Name of the particle system</param>
    /// <returns></returns>
    public ParticleSystem GetParticleSystemByName(string _name)
    {
        if (!particleSystemsByName.ContainsKey(_name))
        {
            Debug.Log("This gameobject does not exist");
            return null;
        }
        return particleSystemsByName[_name]; 
    }

    public void InstanciateRandomHitEffect(Vector3 _position)
    {
        string _randomName = particleSystemsByName.Keys.ToList()[(int)UnityEngine.Random.Range(0, particleSystemsByName.Count)];
        ParticleSystem _system = particleSystemsByName[_randomName];
        Vector3 _offset = new Vector3(UnityEngine.Random.Range(-.5f, .5f), UnityEngine.Random.Range(.1f, 1.8f), 0);
        Instantiate(_system.gameObject, _position + _offset, Quaternion.identity); 
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
        LoadAssetBundle(); 
    }
	#endregion

	#endregion
}
