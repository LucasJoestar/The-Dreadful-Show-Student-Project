using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 
using TMPro;

public class TDS_FloatingText : PunBehaviour
{
    #region Fields / Properties
    [SerializeField]
    float destroyTime = 1.5f;
    [SerializeField]
    float offset = 2;
    [SerializeField]
    Color textColor;
    [SerializeField]
    TextMeshPro text;
    [SerializeField]
    float randomizedOffset = .5f;
    // Vector3 randomizePosition  {get {return new Vector3(randomizedOffset, 0, 0);}}
    #endregion

    #region Methodes
    #region Original Methodes
    public void Init(int _damage)
    {
        if(PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Init"), new object[] { _damage });
            StartCoroutine(DestoryAfterTime()); 
        }
        if (text)
        {
            text.faceColor = textColor; 
            //text.color = textColor;
            text.text = _damage.ToString();
        }


        transform.localPosition += Vector3.up * offset;
        transform.localPosition += new Vector3(Random.Range(-randomizedOffset, randomizedOffset),
                                               Random.Range(-randomizedOffset, randomizedOffset),
                                               0);
    }

    private IEnumerator DestoryAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        PhotonNetwork.Destroy(gameObject);
    }
    #endregion

    #region Unity Methodes

    #endregion
    #endregion
}
