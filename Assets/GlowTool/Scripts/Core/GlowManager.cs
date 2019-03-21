using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowManager : MonoBehaviour
{

    #region F/P
    public List<GameObject> AllObjectsToMakeGlowy = new List<GameObject>();
    #endregion

    #region Methods
    public void AddObject(GameObject _go)
    {
        if (!_go) return;
        AllObjectsToMakeGlowy.Add(_go);
    }

    public void RemoveAllObjects() => AllObjectsToMakeGlowy.Clear();

    public void RemoveObjectAt(int _o) => AllObjectsToMakeGlowy.RemoveAt(_o);
    #endregion
}
