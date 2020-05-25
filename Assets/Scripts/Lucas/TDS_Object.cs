using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TDS_Object : PunBehaviour
{
    #region Fields / Properties
    /// <summary>
    /// Get the view ID of this object photon view.
    /// </summary>
    public int PhotonID
    {
        get { return photonView.viewID; }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Destroys this object.
    /// </summary>
    public virtual void Destroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(photonView);
    }
    #endregion
}
