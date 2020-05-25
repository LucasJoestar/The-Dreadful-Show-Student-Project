using Photon;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TDS_RPCManager : PunBehaviour
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
     *	    - Création d'une methode GetInfo qui retourne le string formé de l'ID du photon view qui appelle la methode, le type du script dans lequel la methode se trouve et le nom de la méthode
     *	    utilisé par la Method CallMethodOnline
     *	    - Création d'une methode CallMethodOnline qui va chercher une methode dans un script pour l'appeler avec ses arguments
     *	    - Création d'une methode GetComponentWithID qui retourne un component de type T sur l'objet possédant le photon view avec un ID particulier
	 *
	 *	-----------------------------------
	*/

    private const char INFO_SEPARATOR = '#';
    private const char RPC_SEPARATOR = '|';

    public static TDS_RPCManager Instance;

    List<RPCBuffer> rpcBuffers =    new List<RPCBuffer>()
                                        {
                                            new RPCBuffer(PhotonTargets.All),
                                            new RPCBuffer(PhotonTargets.AllBuffered),
                                            new RPCBuffer(PhotonTargets.MasterClient),
                                            new RPCBuffer(PhotonTargets.Others),
                                            new RPCBuffer(PhotonTargets.OthersBuffered)
                                        };

    #region Methods

    #region Original Methods   
    [PunRPC]
    private void CallMethodOnline(string _infos, object[] _args)
    {
        string[] _rpcs = _infos.Split(RPC_SEPARATOR);
        int _paramCounter = 0;

        for (int _i = 0; _i < _rpcs.Length; _i++)
        {
            string _rpc = _rpcs[_i];

            if (string.IsNullOrEmpty(_rpc))
                continue;

            //Split the information
            string[] _infoArray = _rpc.Split(INFO_SEPARATOR);

            // Get the id, if it can't be gotten return
            int _id = int.Parse(_infoArray[0]);

            //Get the type, if it can't be gotten return
            Type _t = Type.GetType(_infoArray[1]);

            int _paramsCount = int.Parse(_infoArray[3]);
            object[] _params = new object[_paramsCount];
            for (int _j = 0; _j < _params.Length; _j++)
            {
                _params[_j] = _args[_paramCounter + _j];
            }

            _paramCounter += _paramsCount;

            PhotonView _photonView = PhotonView.Find(_id);
            if (!_photonView)
                continue;

            Component _comp = _photonView.GetComponent(_t);
            if (!_comp)
                continue;

            // Get the arguments if they exist
            //Try to call the method with the name selected on the object target with the arguments
            try
            {
                _t.InvokeMember(_infoArray[2], BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, _comp, _params);
            }
            catch (TargetException _targetException)
            {
                // If the method can't be found, catch the exception
                Debug.LogError($"Couldn't find the target. The {_infoArray[1]} with the ID {_infoArray[0]} does not exists\n{_targetException.Message}");
                continue;
            }
            catch (MissingMethodException _e)
            {
                // If the method can't be found, catch the exception
                Debug.LogError($"Couldn't find the {_infoArray[2]} Method in the selected script.\n{_e.Message}");
                continue;
            }
        }
    }

    // ----------------

    public void CallRPC(PhotonTargets _target, PhotonView _photonView, Type _type, string _methodName, object[] _params)
    {
        for (int _i = 0; _i < rpcBuffers.Count; _i++)
        {
            if (rpcBuffers[_i].Target == _target)
            {
                rpcBuffers[_i].RPC +=   $"{_photonView.viewID}{INFO_SEPARATOR}" +
                                        $"{_type.ToString()}{INFO_SEPARATOR}" +
                                        $"{_methodName}{INFO_SEPARATOR}" +
                                        $"{_params.Length}{INFO_SEPARATOR}{RPC_SEPARATOR}";
                rpcBuffers[_i].RPCParams.AddRange(_params);

                break;
            }
        }
    }

    public void CallRPC(PhotonPlayer _target, PhotonView _photonView, Type _type, string _methodName, object[] _params)
    {
        for (int _i = 0; _i < rpcBuffers.Count; _i++)
        {
            if (rpcBuffers[_i].PlayerTarget == _target)
            {
                rpcBuffers[_i].RPC += $"{_photonView.viewID}{INFO_SEPARATOR}" +
                                        $"{_type.ToString()}{INFO_SEPARATOR}" +
                                        $"{_methodName}{INFO_SEPARATOR}" +
                                        $"{_params.Length}{INFO_SEPARATOR}{RPC_SEPARATOR}";
                rpcBuffers[_i].RPCParams.AddRange(_params);

                break;
            }
        }
    }
    #endregion

    #region Photon
    public override void OnPhotonPlayerConnected(PhotonPlayer _newPlayer)
    {
        rpcBuffers.Add(new RPCBuffer(_newPlayer));
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer _otherPlayer)
    {
        for (int _i = 0; _i < rpcBuffers.Count; _i++)
        {
            if (rpcBuffers[_i].PlayerTarget == _otherPlayer)
            {
                rpcBuffers.RemoveAt(_i);
                break;
            }
        }
    }
    #endregion

    #region Unity Methods
    private void LateUpdate()
    {
        // Send out RPC
        for (int _i = 0; _i < rpcBuffers.Count; _i++)
        {
            RPCBuffer _rpc = rpcBuffers[_i];

            if (!string.IsNullOrEmpty(_rpc.RPC))
            {
                photonView.RPC("CallMethodOnline", _rpc.Target, _rpc.RPC, _rpc.RPCParams.ToArray());

                _rpc.RPC = string.Empty;
                _rpc.RPCParams.Clear();
            }
        }

        PhotonNetwork.SendOutgoingCommands();
    }

    private void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }
    #endregion

    #endregion
}

[Serializable]
public class RPCBuffer
{
    public PhotonTargets Target;
    public PhotonPlayer PlayerTarget;

    public string RPC = string.Empty;
    public List<object> RPCParams = new List<object>();

    public RPCBuffer(PhotonTargets _target)
    {
        Target = _target;
    }

    public RPCBuffer(PhotonPlayer _playerTarget)
    {
        PlayerTarget = _playerTarget;
    }
}
