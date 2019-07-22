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
    private List<ParticleSystem> hitParticleSystems = new List<ParticleSystem>();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Load the VFX asset bundle of the particles systems
    /// Add the particles systems into the Dictionary particleSystemsByName
    /// </summary>
    private void LoadAssetBundle()
    {
        AssetBundle _vfxBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AssetBundles", "vfxassetsbundle"));
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
        _vfxBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AssetBundles", "hitvfxassetsbundle"));
        if (!_vfxBundle)
        {
            Debug.Log("Asset Bundle not found");
            return;
        }
        hitParticleSystems = _vfxBundle.LoadAllAssets<GameObject>().ToList().Select(o => o.GetComponent<ParticleSystem>()).ToList();
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
        int _randomIndex = (int)UnityEngine.Random.Range((int)0, (int)hitParticleSystems.Count);
        ParticleSystem _system = hitParticleSystems[_randomIndex]; 
        Vector3 _offset = new Vector3(UnityEngine.Random.Range(-.5f, .5f), UnityEngine.Random.Range(.1f, 1.8f), 0);
        Instantiate(_system.gameObject, _position + _offset, Quaternion.identity); 
    }

    public void InstanciateParticleSystemByName(string _name, Vector3 _position)
    {
        ParticleSystem _system = GetParticleSystemByName(_name);
        if (_system == null) return;
        if (!PhotonNetwork.isMasterClient) return;
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(GetComponent<PhotonView>(), this.GetType(), "InstanciateParticleSystemByName"), new object[] { _name, _position.x, _position.y, _position.z });
        Instantiate(_system.gameObject, _position, Quaternion.identity);
    }

    public void InstanciateParticleSystemByName(string _name, float _positionX, float _positionY, float _positionZ)
    {
        if (PhotonNetwork.isMasterClient) return; 
        ParticleSystem _system = GetParticleSystemByName(_name);
        if (_system == null) return;
        Instantiate(_system.gameObject, new Vector3(_positionX, _positionY, _positionZ), Quaternion.identity);
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
