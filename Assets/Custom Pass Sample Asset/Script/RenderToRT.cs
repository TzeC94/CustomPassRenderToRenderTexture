using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

class RenderToRT : CustomPass
{
    public LayerMask layerMask;
    public RenderTexture renderTexture;
    public bool overrideAlpha = true;

    private ShaderTagId[] depthTags;

    private static int OverrideAlphaID = Shader.PropertyToID("_AlphaClippingOverride");

    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in an performance manner.
    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        // Setup code here
        depthTags = new ShaderTagId[2] { HDShaderPassNames.s_DepthOnlyName, HDShaderPassNames.s_DepthForwardOnlyName };
    }

    protected override void Execute(CustomPassContext ctx)
    {
        // Executed every frame for all the camera inside the pass volume.
        // The context contains the command buffer to use to enqueue graphics commands.

        if (renderTexture == null) return;

        //Change render target to render texture
        CoreUtils.SetRenderTarget(ctx.cmd, renderTexture.colorBuffer, renderTexture.depthBuffer, ClearFlag.All);

        //Depth Pass first
        CustomPassUtils.DrawRenderers(ctx, depthTags, layerMask, RenderQueueType.AllOpaque);

        //Clear the color to prepare render normally
        CoreUtils.ClearRenderTarget(ctx.cmd, ClearFlag.Color, Color.clear);

        //Create the Render State Block to override the depth
        RenderStateMask mask = RenderStateMask.Depth;
        DepthState depth = new DepthState(false, CompareFunction.LessEqual);
        RenderStateBlock block = new RenderStateBlock(mask)
        {
            depthState = depth
        };

        //Render Opaque without alpha test
        CustomPassUtils.DrawRenderers(ctx, layerMask, RenderQueueType.OpaqueNoAlphaTest, overrideRenderState: block);

        //Render Opaque with Alpha Test
        depth.compareFunction = CompareFunction.Equal;
        block.depthState = depth;

        //Issue command buffer to set the alpha override
        if(overrideAlpha)
            ctx.cmd.SetGlobalFloat(OverrideAlphaID, 1f);

        CustomPassUtils.DrawRenderers(ctx, layerMask, RenderQueueType.OpaqueAlphaTest, overrideRenderState: block);

        //Reset after draw
        if(overrideAlpha)
            ctx.cmd.SetGlobalFloat(OverrideAlphaID, 0f);
    }

    protected override void Cleanup()
    {
        // Cleanup code
    }
}