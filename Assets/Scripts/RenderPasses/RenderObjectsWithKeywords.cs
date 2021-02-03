using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

namespace CustomRendering
{
	public enum RenderTarget { CameraColor, RenderTextureObject };

	public class RenderObjectsWithKeywords : ScriptableRendererFeature
	{
		// Render feature Settings
		[System.Serializable]
		public class Settings {
			public string passTag = "RenderObjectsWithKeywordsFeature";
			public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;

			public FilterSettings filter = new FilterSettings();

			public List<string> shaderKeywords = new List<string>();

			public RenderTarget dstType = RenderTarget.CameraColor;
			public RenderTexture dstTextureObject;
			public bool forceClearDst = true;
		}

		[System.Serializable]
		public class FilterSettings
		{
			public RenderQueueType renderQueueType;
			public LayerMask layerMask;
			public string[] passNames;

			public FilterSettings()
			{
				renderQueueType = RenderQueueType.Opaque;
				layerMask = 0;
			}
		}

		public Settings settings = new Settings();

		RenderObjectsWithKeywordsPass mScriptablePass;

		public override void Create()
		{
				// Render Objects pass doesn't support events before rendering prepasses.
				// The camera is not setup before this point and all rendering is monoscopic.
				// Events before BeforeRenderingPrepasses should be used for input texture passes (shadow map, LUT, etc) that doesn't depend on the camera.
				// These events are filtering in the UI, but we still should prevent users from changing it from code or
				// by changing the serialized data.
				if (settings.passEvent < RenderPassEvent.BeforeRenderingPrepasses)
						settings.passEvent = RenderPassEvent.BeforeRenderingPrepasses;

				mScriptablePass = new RenderObjectsWithKeywordsPass(settings.passTag, settings);
		}

		// Here you can inject one or multiple render passes in the renderer.
		// This method is called when setting up the renderer once per-camera.
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
				renderer.EnqueuePass(mScriptablePass);
		}
	}
}

