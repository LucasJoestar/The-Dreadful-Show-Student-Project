using System;
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
     *	    - Création d'une methode GetInfo qui retourne le string formé de l'ID du photon view qui appelle la methode, le type du script dans lequel la methode se trouve et le nom de la méthode
     *	    utilisé par la Method CallMethodOnline
     *	    - Création d'une methode CallMethodOnline qui va chercher une methode dans un script pour l'appeler avec ses arguments
     *	    - Création d'une methode GetComponentWithID qui retourne un component de type T sur l'objet possédant le photon view avec un ID particulier
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
    public static T GetComponentWithID<T>(int _photonViewID)
    {
        PhotonView _photonView = PhotonView.Find(_photonViewID);
        if (!_photonView) return default(T);
        T _component = _photonView.GetComponent<T>();
        return _component;
    }

    /// <summary>
    /// Call a method with a selected Name on a targeted Script 
    /// /// </summary>
    /// <param name="_info"> Info: PhotonViewID#Type#MethodName</param>
    [PunRPC]
    public void CallMethodOnline(string _info, params object[] args)
    {
        //Split the information
        string[] _infoArray = _info.Split('#');
        // Get the id, if it can't be gotten return
        int _id;
        if (!int.TryParse(_infoArray[0], out _id))
        {
            return;
        }
        //Get the type, if it can't be gotten return
        Type _t = Type.GetType(_infoArray[1]);
        if (_t == null)
        {
            return;
        }
        //Get the method GetTypeWithId with reflection to call it with the gotten type
        MethodInfo _methodInfo = typeof(TDS_RPCManager).GetMethod("GetComponentWithID");
        MethodInfo _methodConstructed = _methodInfo.MakeGenericMethod(_t);
        object[] _arg = { _id }; 
        // Get the object calling the method
        var _target = _methodConstructed.Invoke(null, _arg);
        // Get the arguments if they exist
        //Try to call the method with the name selected on the object target with the arguments
        try
        {
            _t.InvokeMember(_infoArray[2].ToString(), BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy , null, _target, args);
        }
        catch (TargetException _targetException)
        {
            // If the method can't be found, catch the exception
            Debug.LogError($"Couldn't find the target. The {_infoArray[1]} with the ID {_infoArray[0]} does not exists\n{_targetException.Message}");
            return;
        }
        catch (MissingMethodException _e)
        {
            // If the method can't be found, catch the exception
            Debug.LogError($"Couldn't find the {_infoArray[2]} Method in the selected script.\n{_e.Message}"); 
            return;
        }
    }


    /// <summary>
    /// Get informations used by the called method Online
    /// Concat in a string the informations
    /// Info: ID#typeName#MethodName#Arguments
    /// </summary>
    /// <param name="_photonViewID">ID from the PhotonView of the caller</param>
    /// <param name="_type">Type of the caller</param>
    /// <param name="_methodName">Name of the called method</param>
    /// <param name="args">arguments used in the method</param>
    /// <returns></returns>
    public static string GetInfo(int _photonViewID, Type _type, string _methodName)
    {
        string _info = $"{_photonViewID}#{_type.ToString()}#{_methodName}";
        return _info;
    }

    /// <summary>
    /// Get informations used by the called method Online
    /// Concat in a string the informations
    /// Info: ID#typeName#MethodName#Arguments
    /// </summary>
    /// <param name="_photonView">PhotonView of the caller</param>
    /// <param name="_type">Type of the caller</param>
    /// <param name="_methodName">Name of the called method</param>
    /// <param name="args">arguments used in the method</param>
    /// <returns></returns>
    public static string GetInfo(PhotonView _photonView, Type _type, string _methodName)
    {
        return GetInfo(_photonView.viewID, _type, _methodName);
    }
    #endregion

    #region Unity Methods
    void Awake()
    {      
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return; 
        }
    }
    void Start()
    {
        if (!RPCPhotonView)
            RPCPhotonView = GetComponent<PhotonView>();
    }
    #endregion
    #endregion
}