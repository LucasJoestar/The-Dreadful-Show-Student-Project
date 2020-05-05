using UnityEngine;
using UnityEngine.Rendering;

public class CameraMirror : MonoBehaviour
{
    #region Fields / Properties
    public Camera Camera = null;
    private RenderTexture rtStatic, rtDynamic = null;

    public LayerMask staticLayerMask, dynamicLayerMask;
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Monobehaviour
    /*********************************
     *****     MONOBEHAVIOUR     *****
     ********************************/

    Plane[] planesFrustrum = new Plane[6];
    Renderer myRenderer;

    private void OnBecameVisible()
    {
        if (!rtStatic)
        {
            rtStatic = new RenderTexture(512, 512, 24);
            rtStatic.Create();

            rtDynamic = new RenderTexture(512, 512, 24);
            rtDynamic.Create();
        }

        Camera.targetTexture = rtStatic;
        Camera.cullingMask = staticLayerMask;
        Camera.Render();

        Camera.targetTexture = rtDynamic;
        Camera.cullingMask = dynamicLayerMask;
        Camera.clearFlags = CameraClearFlags.Nothing;


        CommandBuffer _buffer = new CommandBuffer();
        _buffer.Blit(rtStatic, rtDynamic);

        Camera.AddCommandBuffer(CameraEvent.BeforeSkybox, _buffer);

        // Object in bounds
        GeometryUtility.CalculateFrustumPlanes(Camera, planesFrustrum);
        GeometryUtility.TestPlanesAABB(planesFrustrum, myRenderer.bounds);
    }
    #endregion

    #endregion
}