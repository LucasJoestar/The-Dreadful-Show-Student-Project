using UnityEngine;
using UnityEngine.Rendering;

public class TDS_CameraMirror : MonoBehaviour
{
    #region Fields / Properties
    [SerializeField] private new Camera camera = null;
    [SerializeField] private new MeshRenderer renderer = null;
    [SerializeField] LayerMask staticLayerMask, dynamicLayerMask;

    private RenderTexture rtStatic, rtDynamic = null;

    #endregion

    #region Methods
    /*********************************
     *****     MONOBEHAVIOUR     *****
     ********************************/

    private void Awake()
    {
        camera.gameObject.SetActive(false);
    }

    private void OnBecameVisible()
    {
        camera.gameObject.SetActive(true);
        
        if (!rtStatic)
        {
            rtStatic = new RenderTexture(512, 512, 24);
            rtStatic.Create();

            rtDynamic = new RenderTexture(512, 512, 24);
            rtDynamic.Create();

            camera.targetTexture = rtStatic;
            camera.cullingMask = staticLayerMask;
            camera.Render();

            camera.targetTexture = rtDynamic;
            camera.cullingMask = dynamicLayerMask;
            camera.clearFlags = CameraClearFlags.Nothing;

            CommandBuffer _buffer = new CommandBuffer();
            _buffer.Blit(rtStatic, rtDynamic);

            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _buffer);

            Material _material = new Material(renderer.sharedMaterial);
            _material.mainTexture = rtDynamic;

            renderer.material = _material;
        }
    }

    private void OnBecameInvisible()
    {
        camera.gameObject.SetActive(false);
    }
    #endregion
}
