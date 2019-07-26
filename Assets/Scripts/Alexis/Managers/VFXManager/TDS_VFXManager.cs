﻿using Photon;
using UnityEngine;

public class TDS_VFXManager : PunBehaviour
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

    #region FXs
    // FXs used during fights and other sweet moments
    [Header("Fight FXs")]
    [SerializeField] private ParticleSystem[] hitFXs = new ParticleSystem[] { };
    /// <summary>Fight FXs, used randomly when dealing damages to something or someone.</summary>
    public ParticleSystem[] HitFXs { get { return hitFXs; } }

    [SerializeField] private ParticleSystem kaboomFX = null;
    /// <summary>Fight FXs, used for explosions !.</summary>
    public ParticleSystem KaboomFX { get { return kaboomFX; } }

    // Magic FXs used for all kind of magic effects
    [Header("Magic FXs")]
    [SerializeField] private ParticleSystem poofFX = null;
    /// <summary>Magic FX, with the "poof" word appearing in a smoke style.</summary>
    public ParticleSystem PoofFX { get { return poofFX; } }

    [SerializeField] private ParticleSystem magicPoofFX = null;
    /// <summary>Magic FX, mainly used for making things appear.</summary>
    public ParticleSystem MagicPoofFX { get { return magicPoofFX; } }

    [SerializeField] private ParticleSystem disappearMagicPoofFX = null;
    /// <summary>Magic FX, mainly used for making things disappear.</summary>
    public ParticleSystem DisappearMagicPoofFX { get { return disappearMagicPoofFX; } }

    [SerializeField] private ParticleSystem rabbitMagicPoofFX = null;
    /// <summary>Magic FX, usde to make the rabbit disappaer.</summary>
    public ParticleSystem RabbitMagicPoofFX { get { return rabbitMagicPoofFX; } }

    // Beard Lady related FXs
    [Header("Beard Lady FXs")]
    [SerializeField] private ParticleSystem beardGrowFX = null;
    /// <summary>FX used when the Beard Lady's beard grows up.</summary>
    public ParticleSystem BeardGrowFX { get { return beardGrowFX; } }

    [SerializeField] private ParticleSystem beardDamagedFX = null;
    /// <summary>FX used when the Beard lady's beard gets damaged.</summary>
    public ParticleSystem BeardDamagedFX { get { return beardDamagedFX; } }
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_VFXManager Instance = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Get the related FX.
    /// </summary>
    /// <param name="_fxType">Type of FX to get object from.</param>
    /// <returns>Returns the Particle system associated with the gieven type.</returns>
    public ParticleSystem GetFX(FXType _fxType)
    {
        switch (_fxType)
        {
            case FXType.Kaboom:
                return kaboomFX;

            case FXType.Poof:
                return poofFX;

            case FXType.MagicAppear:
                return magicPoofFX;

            case FXType.MagicDisappear:
                return disappearMagicPoofFX;

            case FXType.RabbitPoof:
                return rabbitMagicPoofFX;

            case FXType.BeardGrowsUp:
                return beardGrowFX;

            case FXType.BeardDamaged:
                return beardDamagedFX;

            default:
                // Nothing to see here...
                return null;
        }
    }

    /// <summary>
    /// Spawns a specific FX at a given position.
    /// </summary>
    /// <param name="_fxtype">Type of FX to instantiate.</param>
    /// <param name="_position">Position where to spawn the FX.</param>
    public void SpawnEffect(FXType _fxtype, Vector3 _position)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, GetType(), "SpawnEffect"), new object[] { (int)_fxtype, _position.x, _position.y, _position.z });
    }

    /// <summary>
    /// Spawns a specific FX at a given position.
    /// </summary>
    /// <param name="_fxtype">Type of FX to instantiate.</param>
    /// <param name="_transformPhoton">ID of the transform used as FX parent.</param>
    public void SpawnEffect(FXType _fxtype, int _transformPhotonID)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, GetType(), "SpawnEffect"), new object[] { (int)_fxtype, _transformPhotonID });
    }

    /// <summary>
    /// Spawns a specific FX at a given position.
    /// </summary>
    /// <param name="_fxtype">Type of FX to instantiate.</param>
    /// <param name="_x">X coordinate where to spawn the effect.</param>
    /// <param name="_y">Y coordinate where to spawn the effect.</param>
    /// <param name="_z">Z coordinate where to spawn the effect.</param>
    private void SpawnEffect(int _fxType, float _x, float _y, float _z)
    {
        ParticleSystem _fx = GetFX((FXType)_fxType);

        if (_fx != null) Instantiate(_fx, new Vector3(_x, _y, _z), Quaternion.identity);
        else
        {
            Debug.Log("The FX for type \"" + (FXType)_fxType + "\" is not referenced !");
        }
    }

    /// <summary>
    /// Spawns a specific FX at a given position.
    /// </summary>
    /// <param name="_fxtype">Type of FX to instantiate.</param>
    /// <param name="_photonViewID">ID of the photon view of the transform to parent the FX to.</param>
    private void SpawnEffect(int _fxType, int _photonViewID)
    {
        ParticleSystem _fx = GetFX((FXType)_fxType);

        if (_fx != null)
        {
            PhotonView _photonView = PhotonView.Find(_photonViewID);
            if (_photonView) Instantiate(_fx, _photonView.transform, false);
        }
        else
        {
            Debug.Log("The FX for type \"" + (FXType)_fxType + "\" is not referenced !");
        }
    }

    /// <summary>
    /// Spawns a random hit effect at a given position.
    /// </summary>
    /// <param name="_position">Position where to spawn the FX.</param>
    public void SpawnHitEffect(Vector3 _position)
    {
        if (hitFXs.Length > 0)
        {
            Instantiate(hitFXs[Random.Range(0, hitFXs.Length)], _position, Quaternion.identity);
        }
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
