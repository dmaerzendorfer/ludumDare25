using System;
using _Project.Scripts.Runtime.PixelateRenderFeature;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixelizeFeature : ScriptableRendererFeature
{
    [Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int screenheight = 144;
    }

    [SerializeField]
    private CustomPassSettings settings;

    private PixelizePass _customPass;
    
    public override void Create()
    {
        _customPass = new PixelizePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        #if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(_customPass);
    }
}
