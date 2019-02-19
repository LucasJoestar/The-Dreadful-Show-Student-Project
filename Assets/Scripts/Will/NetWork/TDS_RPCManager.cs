using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection; 
using UnityEngine;

public class TDS_RPCManager : MonoBehaviour 
{
    /* TDS_RPCManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[19/02/2019]
	 *	Author :		[COMMINGES William & THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation de la classe TDS_RPCManager]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    public static TDS_RPCManager Instance;
    public PhotonView RPCPhotonView;
    #endregion

    #region Methods
    #region Original Methods   
    /// <summary>
    /// Get component with its photon view ID
    /// </summary>
    /// <typeparam name="T">Desired Component</typeparam>
    /// <param name="_photonViewID">Photon view ID</param>
    /// <returns></returns>
    public static T GetTypeWithID<T>(int _photonViewID)
    {
        Debug.Log(typeof(T)); 
        PhotonView _photonView = PhotonView.Find(_photonViewID);
        if (!_photonView) return default(T);
        T _component = _photonView.GetComponent<T>();
        return _component;
    }


    /// <summary>
    /// Call a method on a targeted Script 
    ///
    /// </summary>
    /// <param name="_info"> Info: PhtonViewID#Type#MethodName#arguments</param>
    [PunRPC]
    public void CallMethodOnline(string _info)
    {
        string[] _infoArray = _info.Split('#');
        int _id;
        if (!int.TryParse(_infoArray[0], out _id)) return;
        Type _t = Type.GetType(_infoArray[1]);
        MethodInfo _methodInfo = typeof(TDS_RPCManager).GetMethod("GetTypeWithID");
        MethodInfo _methodConstructed = _methodInfo.MakeGenericMethod(_t);
        object[] _args = { _id }; 
        var _target = _methodConstructed.Invoke(null, _args);
        // Complete with arguments of the method
        // Type.InvokeMember(_infoArray[2], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, _target, ); 
    }

    public static string GetInfo(PhotonView _photonView, Type _type, string _methodName, params object[] args)
    {
        string _info = $"{_photonView.viewID}#{_type.ToString()}#{_methodName}";
        for (int i = 0; i < args.Length; i++)
        {
            _info += $"#{args[i]}"; 
        }
        return _info; 
    }
    #endregion

    #region Unity Methods
    void Awake()
    {      
        if (!Instance) Instance = this;        
    }
    void Start()
    {
        if (!RPCPhotonView)
            RPCPhotonView = GetComponent<PhotonView>();
    }
    #endregion
    #endregion
}
