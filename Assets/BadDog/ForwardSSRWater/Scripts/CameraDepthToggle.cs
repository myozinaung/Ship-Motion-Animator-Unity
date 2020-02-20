using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraDepthToggle : MonoBehaviour
{
    private void OnEnable()
    {
        Camera camera = GetComponent<Camera>();

        if(camera != null)
        {
            camera.depthTextureMode |= DepthTextureMode.Depth;
        }
    }
}
