using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TDS_Object : PunBehaviour
{
    #region Fields / Properties
    /// <summary>
    /// Audio source linked to this object.
    /// </summary>
    [SerializeField] protected AudioSource audioSource = null;

    /// <summary>Public accessor for <see cref="audioSource"/>.</summary>
    public AudioSource AudioSource
    {
        get { return audioSource; }
    }

    /// <summary>
    /// Get the view ID of this object photon view.
    /// </summary>
    public int PhotonID
    {
        get { return photonView.viewID; }
    }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Destroys this object.
    /// </summary>
    public virtual void Destroy()
    {
        if (!PhotonNetwork.isMasterClient) return;
        PhotonNetwork.Destroy(photonView);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }
    #endregion

    #endregion
}
