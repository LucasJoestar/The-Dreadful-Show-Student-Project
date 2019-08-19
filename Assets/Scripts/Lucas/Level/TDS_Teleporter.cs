using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_Teleporter : MonoBehaviour
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

    #region Fields / Properties
    /// <summary>
    /// Detected tags by the teleporter.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags();
    #endregion

    #region Original Methods
    /// <summary>
    /// Teleports the given object into actual game zone.
    /// </summary>
    /// <param name="_object"></param>
    public void Teleport(Collider _object)
    {
        TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;
        Vector3 _actualPosition = _object.transform.position;

        Vector3 _newPosition = new Vector3(_object.bounds.min.x < _bounds.XMin ? (_bounds.XMin + _object.bounds.size.x + 1) : _object.bounds.max.x > _bounds.XMax ?                                       (_bounds.XMax - _object.bounds.size.x - 1) : _actualPosition.x,
                                           _object.bounds.min.y < 0 ? 2.5f : _actualPosition.y,
                                           _object.bounds.min.z < _bounds.ZMin ? (_bounds.ZMin + _object.bounds.size.z + 1) : _object.bounds.max.z > _bounds.ZMax ? (_bounds.ZMax - _object.bounds.size.z - 1) : _actualPosition.z);

        _object.transform.position = _newPosition;
    }
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTag(detectedTags.ObjectTags)) Teleport(other);
    }
    #endregion
}
