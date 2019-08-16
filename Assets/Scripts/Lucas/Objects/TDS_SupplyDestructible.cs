using System.Collections.Generic;
using UnityEngine;

public class TDS_SupplyDestructible : TDS_Destructible
{
    #region Fields / Properties

    #endregion

    #region Methods
    /// <summary>
    /// Loots a random object from a given list.
    /// </summary>
    /// <param name="_availableLoot">List of available objects to loot.</param>
    /// <returns>Returns the looted object.</returns>
    protected override GameObject Loot(ref List<GameObject> _availableLoot)
    {
        GameObject _loot = base.Loot(ref _availableLoot);
        TDS_Throwable _throwable = _loot.GetComponent<TDS_Throwable>();
        
        // Link the looted throwable to the first active spawn area found
        if (_throwable && (TDS_SpawnerArea.ActivatedAreas.Count > 0))
        {
            TDS_SpawnerArea.ActivatedAreas[0].LinkThrowable(_throwable);
        }

        return _loot;
    }
    #endregion
}
