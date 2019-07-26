using System;
using UnityEngine;
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

    #region Fields / Properties


    [SerializeField] private TDS_VFXElement[] particleSystems = new TDS_VFXElement[] {};
    private TDS_VFXElement[] hitParticleSystems = null;

    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static TDS_VFXManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Get the Particle System with the name 
    /// </summary>
    /// <param name="_name">Name of the particle system</param>
    /// <returns></returns>
    public ParticleSystem GetParticleSystemByName(string _name)
    {
        TDS_VFXElement _element = particleSystems.Where(p => p.Name == _name).FirstOrDefault();
        if (_element == null) return null;
        return _element.ParticleSystem; 
    }

    /// <summary>
    /// Instantiate a raondom hit effect when a character is hit.
    /// </summary>
    /// <param name="_position"></param>
    public void InstanciateRandomHitEffect(Vector3 _position)
    {
        if(hitParticleSystems == null)
        {
            hitParticleSystems = particleSystems.Where(e => e.IsHitEffect).ToArray();
        }
        int _randomIndex = (int)UnityEngine.Random.Range((int)0, (int)hitParticleSystems.Length);
        ParticleSystem _system = hitParticleSystems[_randomIndex].ParticleSystem; 
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
    }
    
	#endregion

	#endregion
}

[Serializable]
public class TDS_VFXElement
{
    [SerializeField] private ParticleSystem particleSystem;
    public ParticleSystem ParticleSystem
    {
        get { return particleSystem;  }
    }

    [SerializeField] bool isHitEffect = false; 
    public bool IsHitEffect
    {
        get { return isHitEffect;  }
    }

    public string Name
    {
        get { return particleSystem.name;  }
    }

    public TDS_VFXElement(ParticleSystem _system)
    {
        particleSystem = _system; 
    }
}