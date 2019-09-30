using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_EnemyDisappearSystem : MonoBehaviour
{
    /* TDS_EnemyDisappearSystem :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	#####################
 *	### MODIFICATIONS ###
 *	#####################
 *	
*/

    #region Fields / Properties
    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// All enemies linked to this disappear system.
    /// </summary>
    [SerializeField] private TDS_Enemy[] enemies = new TDS_Enemy[] { };

    /// <summary>
    /// Counter for disappeared enemies.
    /// </summary>
    private int disappearCounter = 0;

    /// <summary>
    /// Spawner Area associated to this system linked enemies.
    /// </summary>
    [SerializeField] private TDS_SpawnerArea spawnerArea = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activates this disappear system.
    /// </summary>
    public void Activate()
    {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (enemies.Length == 0) enemies = spawnerArea.SpawnedEnemies.ToArray();

        // Set the destination of every linked enemy into this collider.
        foreach (TDS_Enemy _enemy in enemies)
        {
            if (_enemy.IsDead)
            {
                disappearCounter++;
                continue;
            }
            _enemy.StopAll();
            _enemy.PlayerTarget = null; 
            _enemy.Agent.SetDestination(new Vector3(collider.bounds.center.x, 0, _enemy.transform.position.z));
            _enemy.SetEnemyState(EnemyState.GettingInRange);
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!collider.isTrigger) collider.isTrigger = true;
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.HasTag("Enemy")) return;

        // If collider is among of linked enemies one, make him disappear
        TDS_Enemy _enemy = null;
        if ((_enemy = enemies.FirstOrDefault(e => e.Collider == other)) != null)
        {
            if (spawnerArea != null) spawnerArea.RemoveEnemy(_enemy);
            PhotonNetwork.Destroy(other.gameObject);
            disappearCounter++;

            // Destroy this GameObject is every enemies has been destroyed
            if (disappearCounter == enemies.Length) Destroy(gameObject);
        }
    }
    #endregion

    #endregion
}
