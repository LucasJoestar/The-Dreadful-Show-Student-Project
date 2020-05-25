using Photon;
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
    [SerializeField] private ParticleSystem[] opponentHitFXs = new ParticleSystem[] { };
    /// <summary>Fight FXs, used randomly when dealing damages to an opponent.</summary>
    public ParticleSystem[] OpponentHitFXs { get { return opponentHitFXs; } }

    [SerializeField] private ParticleSystem[] playerHitFXs = new ParticleSystem[] { };
    /// <summary>Fight FXs, used randomly when dealing damages to the local player.</summary>
    public ParticleSystem[] PlayerHitFXs { get { return playerHitFXs; } }

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

    // Players related FXs
    [Header("Player FXs")]
    [SerializeField] private ParticleSystem beardGrowFX = null;
    /// <summary>FX used when the Beard Lady's beard grows up.</summary>
    public ParticleSystem BeardGrowFX { get { return beardGrowFX; } }

    [SerializeField] private ParticleSystem beardDamagedFX = null;
    /// <summary>FX used when the Beard lady's beard gets damaged.</summary>
    public ParticleSystem BeardDamagedFX { get { return beardDamagedFX; } }

    [SerializeField] private GameObject healFX = null;
    /// <summary>FX used when the player gets healed.</summary>
    public GameObject HealFX { get { return healFX; } }

    [Header("Enemies FX")]
    [SerializeField] private GameObject loyalTeleportationFX = null; 
    public GameObject LoyalTeleportationFX { get { return loyalTeleportationFX; } }
    [SerializeField] private GameObject loyalEndTeleportationFX = null;
    public GameObject LoyalEndTeleportationFX { get { return loyalEndTeleportationFX; } }
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
    public GameObject GetFX(FXType _fxType)
    {
        switch (_fxType)
        {
            case FXType.Kaboom:
                return kaboomFX.gameObject;

            case FXType.Poof:
                return poofFX.gameObject;

            case FXType.MagicAppear:
                return magicPoofFX.gameObject;

            case FXType.MagicDisappear:
                return disappearMagicPoofFX.gameObject;

            case FXType.RabbitPoof:
                return rabbitMagicPoofFX.gameObject;

            case FXType.BeardGrowsUp:
                return beardGrowFX.gameObject;

            case FXType.BeardDamaged:
                return beardDamagedFX.gameObject;

            case FXType.Heal:
                return healFX;

            case FXType.MrLoyalTeleportation:
                return loyalTeleportationFX;

            case FXType.MrLoyalEndTeleportation:
                return loyalEndTeleportationFX; 
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
        TDS_RPCManager.Instance.CallRPC(PhotonTargets.All, photonView, GetType(), "SpawnEffect", new object[] { (int)_fxtype, _position.x, _position.y, _position.z });
    }

    /// <summary>
    /// Spawns a specific FX at a given position.
    /// </summary>
    /// <param name="_fxRype">Type of FX to instantiate.</param>
    /// <param name="_transformPhoton">PhotonView of the transform to use as FX parent.</param>
    public void SpawnEffect(FXType _fxRype, PhotonView _transformPhotonView)
    {
        if (PhotonNetwork.offlineMode)
        {
            Instantiate(GetFX(_fxRype), _transformPhotonView.transform, false);
            return;
        }

        TDS_RPCManager.Instance.CallRPC(PhotonTargets.All, photonView, GetType(), "SpawnEffect", new object[] { (int)_fxRype, _transformPhotonView.viewID });
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
        GameObject _fx = GetFX((FXType)_fxType);

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
        GameObject _fx = GetFX((FXType)_fxType);

        if (_fx != null)
        {
            PhotonView _photonView = PhotonView.Find(_photonViewID);
            if (_photonView)
            {
                Instantiate(_fx, _photonView.transform, false);
            }
        }
        else
        {
            Debug.Log("The FX for type \"" + (FXType)_fxType + "\" is not referenced !");
        }
    }

    /// <summary>
    /// Spawns a random opponent hit effect at a given position.
    /// </summary>
    /// <param name="_position">Position where to spawn the FX.</param>
    public void SpawnOpponentHitEffect(Vector3 _position)
    {
        if (opponentHitFXs.Length > 0)
        {
            Instantiate(opponentHitFXs[Random.Range(0, opponentHitFXs.Length)], _position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Spawns a random opponent hit effect at a given position.
    /// </summary>
    /// <param name="_position">Position where to spawn the FX.</param>
    public void SpawnOpponentHitEffect(float _x, float _y, float _z)
    {
        SpawnOpponentHitEffect(new Vector3(_x, _y, _z));
    }

    /// <summary>
    /// Spawns a random player hit effect at a given position.
    /// </summary>
    /// <param name="_position">Position where to spawn the FX.</param>
    public void SpawnPlayerHitEffect(Vector3 _position)
    {
        if (playerHitFXs.Length > 0)
        {
            Instantiate(playerHitFXs[Random.Range(0, playerHitFXs.Length)], _position, Quaternion.identity);
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
