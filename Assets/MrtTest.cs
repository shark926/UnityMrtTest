﻿using UnityEngine;

[ExecuteInEditMode]
public class MrtTest : MonoBehaviour
{
    [SerializeField] Shader _shader;

    Material _material;
    RenderBuffer[] _mrt;

    void OnEnable()
    {
        var shader = Shader.Find("Hidden/MrtTest");
        _material = new Material(shader);
        _material.hideFlags = HideFlags.DontSave;
        _mrt = new RenderBuffer[2];
    }

    void OnDisable()
    {
        DestroyImmediate(_material);
        _material = null;
        _mrt = null;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var rtdepth = RenderTexture.GetTemporary(source.width, source.height, 32, RenderTextureFormat.Depth);
        var rt1 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default);
        var rt2 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.DefaultHDR);

        rtdepth.name = "test depth";
        rt1.name = "test rt1";
        rt2.name = "test rt2";

        _mrt[0] = rt1.colorBuffer;
        _mrt[1] = rt2.colorBuffer;

        // Blit with a MRT.
        Graphics.SetRenderTarget(_mrt, rtdepth.depthBuffer);
        Graphics.Blit(null, _material, 0);

        // Combine them and output to the destination.
        _material.SetTexture("_SecondTex", rt1);
        _material.SetTexture("_ThirdTex", rt2);
        Graphics.Blit(source, destination, _material, 1);

        RenderTexture.ReleaseTemporary(rt1);
        RenderTexture.ReleaseTemporary(rt2);
    }

    void OnGUI()
    {
        var text = "Supported MRT count: ";
        text += SystemInfo.supportedRenderTargetCount;
        GUI.Label(new Rect(0, 0, 200, 200), text);
    }
}
