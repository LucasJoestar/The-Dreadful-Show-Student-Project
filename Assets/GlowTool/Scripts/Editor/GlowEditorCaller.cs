using UnityEditor;
using UnityEngine;

public class GlowEditorCaller : MonoBehaviour
{
    [MenuItem("Tools/GlowManager/Call GlowEditorManager")]
    public static void CallManager()
    {
        if (FindObjectOfType<GlowManager>()) return;
        GameObject _manager = new GameObject("GlowManager", typeof(GlowManager));
        Selection.activeGameObject = _manager;
    }
}
