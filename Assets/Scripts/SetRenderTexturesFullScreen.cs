using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetRenderTexturesFullScreen : MonoBehaviour
{
	public List<RenderTexture> renderTextures = new List<RenderTexture>();

	private void Awake()
	{
		WindowManager.OnScreenResize += this.Resize;		
	}

	private void Resize(int width, int height)
	{
		Debug.Log($"Resizing full-screen render textures ({width}, {height})");
		foreach (var renderTexture in renderTextures)
		{
			renderTexture.Release();
			renderTexture.width = width;
			renderTexture.height = height;
			renderTexture.Create();
		}
	}
}
