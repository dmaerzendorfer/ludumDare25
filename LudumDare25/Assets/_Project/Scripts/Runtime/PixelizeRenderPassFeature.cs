using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.Runtime
{
    
    //based on https://www.youtube.com/watch?v=U8PygjYAF7A
    //could have also just used a full screen pass renderer feature, but wanted to look into the code a bit
    public class PixelizeRenderPassFeature : ScriptableRendererFeature
    {
        public RenderPassEvent injecitonPoint = RenderPassEvent.AfterRenderingPostProcessing;


        private PixelizeRenderPass m_ScriptablePass;
        public Material material;

       
        /// <inheritdoc/>
        public override void Create()
        {
            m_ScriptablePass = new PixelizeRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = injecitonPoint;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material == null)
            {
                Debug.LogWarning("PixelizeEffectRendererFEature material is null and will be skipped.");
                return;
            }

            m_ScriptablePass.Setup(material);

            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(m_ScriptablePass);
            }
        }

        class PixelizeRenderPass : ScriptableRenderPass
        {
            private const string m_PassName = "PixelizeEffectPass";
            Material m_BlitMaterial;

            public void Setup(Material mat)
            {
                m_BlitMaterial = mat;
                requiresIntermediateTexture = true;
            }

            // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
            // It is used to execute draw commands.
            static void ExecutePass(RasterGraphContext context) { }

            // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
            // FrameData is a context container through which URP resources can be accessed and managed.
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                if (resourceData.isActiveTargetBackBuffer)
                {
                    Debug.Log(
                        $"Skipping render pass, pixelizeEffectRendererFeature requires and intermediate ColorTexture," +
                        $" we can't sue the BackBuffer as a texture input.");
                    return;
                }

                var source = resourceData.activeColorTexture;
                var destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = $"$CameraColor-{m_PassName}";
                destinationDesc.clearBuffer = false;

                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
                RenderGraphUtils.BlitMaterialParameters para = new(source, destination, m_BlitMaterial, 0);
                renderGraph.AddBlitPass(para, passName: m_PassName);

                resourceData.cameraColor = destination;
            }
        }
    }
}

[Serializable]
public class PixelizeSettings
{
    public int screenHeight = 144;
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
}