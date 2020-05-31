using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_OffBoundDetector : MonoBehaviour
{
    /* TDS_SpawnerArea :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *	
     *  Date :			
     *	Author :		
     *
     *	Changes :
     *
    */

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        TDS_Player _player = other.GetComponent<TDS_Player>();
        if (_player && _player.photonView.isMine && !_player.IsDead)
        {
            TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;
            Vector3 _actualPosition = other.transform.position;

            Vector3 _newPosition = new Vector3(other.bounds.min.x < _bounds.XMin ? (_bounds.XMin + other.bounds.size.x + 1) : other.bounds.max.x > _bounds.XMax ?                                           (_bounds.XMax - other.bounds.size.x - 1) : _actualPosition.x,
                                                other.bounds.min.y < 0 ? 2.5f : _actualPosition.y,
                                                other.bounds.min.z < _bounds.ZMin ? (_bounds.ZMin + other.bounds.size.z + 1) : other.bounds.max.z > _bounds.ZMax ?              (_bounds.ZMax - other.bounds.size.z - 1) : _actualPosition.z);

            other.transform.position = _newPosition;

            Debug.LogError("TELEPORT : " + other.gameObject.name);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            TDS_Throwable _object = other.GetComponent<TDS_Throwable>();

            if (_object && _object.transform.position.x < TDS_Camera.Instance.CurrentBounds.XMin)
                _object.Destroy();
        }
    }
    #endregion
}
