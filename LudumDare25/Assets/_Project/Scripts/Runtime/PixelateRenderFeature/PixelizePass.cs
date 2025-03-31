using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.Runtime.PixelateRenderFeature
{
    public class PixelizePass : ScriptableRenderPass
    {
        private PixelizeFeature.CustomPassSettings _settings;
        private RenderTargetIdentifier _colorBuffer, _pixelBuffer;
        private int _pixelBufferID = Shader.PropertyToID("_PixelBuffer");

        private Material _material;
        private int _pixelScreenHeight, _pixelScreenWidth;

        public PixelizePass(PixelizeFeature.CustomPassSettings settings)
        {
            _settings = settings;
            renderPassEvent = settings.renderPassEvent;
            if (_material == null) _material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
        }
        
        // override on

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            base.Execute(context, ref renderingData);
        }
    }
}