namespace ShaderToolboxPro.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

#if UNITY_6000_0_OR_NEWER
    using UnityEngine.Rendering.RenderGraphModule;
#endif

    [HelpURL("https://danielilett.com/shader-toolbox/camera-transparent-texture/")]
    [Tooltip("Renders an additional screen texture, like CameraOpaqueTexture, which captures the screen contents after transparent objects have been rendered.")]
    public class CameraTransparentTexture : ScriptableRendererFeature
    {
        CameraTransparentTexturePass pass;

        public override void Create()
        {
            pass = new CameraTransparentTexturePass();
            name = "CopyTransparentColor";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        class CameraTransparentTexturePass : ScriptableRenderPass
        {
            private RTHandle transparentTextureHandle;
            private string transparentTextureName = "_CameraTransparentTexture";

            public CameraTransparentTexturePass()
            {
                profilingSampler = new ProfilingSampler("CopyTransparentColor");
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

#if UNITY_6000_0_OR_NEWER
            requiresIntermediateTexture = false;
#endif
            }

            private static RenderTextureDescriptor GetCopyPassDescriptor(RenderTextureDescriptor descriptor)
            {
                descriptor.msaaSamples = 1;
                descriptor.depthBufferBits = (int)DepthBits.None;
                descriptor.autoGenerateMips = true;

                return descriptor;
            }

#if UNITY_6000_0_OR_NEWER
        [System.Obsolete]
#endif
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                ResetTarget();

                var descriptor = GetCopyPassDescriptor(cameraTextureDescriptor);
                RenderingUtils.ReAllocateIfNeeded(ref transparentTextureHandle, descriptor, name: transparentTextureName);

                base.Configure(cmd, cameraTextureDescriptor);
            }

#if UNITY_6000_0_OR_NEWER
        [System.Obsolete]
#endif
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.isPreviewCamera)
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get();

                RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

                using (new ProfilingScope(cmd, profilingSampler))
                {
                    Blitter.BlitCameraTexture(cmd, cameraTargetHandle, transparentTextureHandle);
                    cmd.SetGlobalTexture(transparentTextureName, transparentTextureHandle);
                }

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

#if UNITY_6000_0_OR_NEWER
        private class CopyPassData
        {
            public TextureHandle inputTexture;
        }

        private int cameraTransparentID = Shader.PropertyToID("_CameraTransparentTexture");

        private void CopyTransparentTexture(RasterCommandBuffer cmd, RTHandle source)
        {
            Blitter.BlitTexture(cmd, source, new Vector4(1, 1, 0, 0), 0.0f, false);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (cameraData.isPreviewCamera)
            {
                return;
            }

            var descriptor = GetCopyPassDescriptor(cameraData.cameraTargetDescriptor);
            TextureHandle transparentHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descriptor, "_CameraTransparentTexture", false);

            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("CopyTransparentColor", out var passData, profilingSampler))
            {
                passData.inputTexture = resourceData.activeColorTexture;

                builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
                builder.SetRenderAttachment(transparentHandle, 0, AccessFlags.Write);
                builder.SetGlobalTextureAfterPass(transparentHandle, cameraTransparentID);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) => CopyTransparentTexture(context.cmd, data.inputTexture));
            }
        }
#endif
        }
    }
}
