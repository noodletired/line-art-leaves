using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

namespace CustomRendering
{
	public class RenderObjectsWithKeywordsPass : ScriptableRenderPass
	{
		private RenderObjectsWithKeywords.Settings mSettings;
		private RenderTargetIdentifier mDestination;

		string mProfilerTag;
		ProfilingSampler mProfilingSampler;

		FilteringSettings mFilteringSettings;
		RenderStateBlock mRenderStateBlock;
		List<ShaderTagId> mShaderTagIdList = new List<ShaderTagId>();

		// Constructor
		// Copies over settings and sets up member variables
		public RenderObjectsWithKeywordsPass(string profilerTag, RenderObjectsWithKeywords.Settings settings)
		{
			mProfilerTag = profilerTag;
			mProfilingSampler = new ProfilingSampler(profilerTag);
			mSettings = settings;

			RenderQueueRange renderQueueRange = (settings.filter.renderQueueType == RenderQueueType.Transparent)
					? RenderQueueRange.transparent
					: RenderQueueRange.opaque;
			mFilteringSettings = new FilteringSettings(renderQueueRange, settings.filter.layerMask);

			mShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
			mShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
			mShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));

			mRenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

			if (settings.dstType == RenderTarget.RenderTextureObject) {
				mDestination = settings.dstTextureObject;
			} else {
				mDestination = new RenderTargetIdentifier();
			}

			mRenderStateBlock.mask |= RenderStateMask.Depth;
			mRenderStateBlock.depthState = new DepthState(true, CompareFunction.Less);
		}

		// Execute
		// Runs the render pass, enabling and disabling keywords before and after
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			// Set up camera context
			ref CameraData cameraData = ref renderingData.cameraData;
			ConfigureTarget(mDestination);

			SortingCriteria sortingCriteria = (mSettings.filter.renderQueueType == RenderQueueType.Transparent)
				? SortingCriteria.CommonTransparent
				: renderingData.cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawingSettings = CreateDrawingSettings(mShaderTagIdList, ref renderingData, sortingCriteria);

			CommandBuffer cmd = CommandBufferPool.Get(mProfilerTag);
			using (new ProfilingScope(cmd, mProfilingSampler))
			{
				if (mSettings.forceClearDst)
					cmd.ClearRenderTarget(true, true, new Color(0,0,0));
				foreach (string keyword in mSettings.shaderKeywords)
					cmd.EnableShaderKeyword(keyword);
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();

				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref mFilteringSettings,
					ref mRenderStateBlock);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
				// Restore camera context
				foreach (string keyword in mSettings.shaderKeywords)
						cmd.DisableShaderKeyword(keyword);
		}
	}
}