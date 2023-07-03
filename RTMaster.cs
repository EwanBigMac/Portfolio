using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Experimental.Rendering;

public class RTMaster : MonoBehaviour
{
    public ComputeShader RTShader;
    private RenderTexture RTTarget;
    private Camera cam;
    public Texture SkyboxTexture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        cam=GetComponent<Camera>();
    }

    private void SetShaderParameters()
    {
        RTShader.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        RTShader.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
        RTShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
/*
        Vector4 origin = cam.cameraToWorldMatrix * new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

        Debug.Log($"Origin: {origin.x}, {origin.y}, {origin.z}");

        Vector4 dir = cam.projectionMatrix.inverse * new Vector4(0, 0, 0, 1);
        dir = cam.cameraToWorldMatrix * new Vector4(dir.x, dir.y, dir.z, 0.0f);
        dir = dir.normalized;

        Debug.Log($"Dir: {dir.x}, {dir.y}, {dir.z}");
*/


        Render(destination);
    }



    private void Render(RenderTexture destination)
    {
        // Make sure we have a current render target
        InitRenderTexture();
        // Set the target and dispatch the compute shader
        RTShader.SetTexture(0, "Result", RTTarget);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RTShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        // Blit the result texture to the screen
        Graphics.Blit(RTTarget, destination);
    }

    private void InitRenderTexture()
    {
        if (RTTarget == null || RTTarget.width != Screen.width || RTTarget.height != Screen.height)
        {
            // Release render texture if we already have one
            if (RTTarget != null)
                RTTarget.Release();
            // Get a render target for Ray Tracing
            RTTarget = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            RTTarget.enableRandomWrite = true;
            RTTarget.Create();
        }
    }
}
