using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Currently broken
public class FullScreenRenderTexture : RenderTexture
{
	public FullScreenRenderTexture(int depth=0)
		: base(256, 256, depth)
	{
		WindowManager.OnScreenResize += this.Resize;
		int width = WindowManager.Instance.GetScreenWidth();
		int height = WindowManager.Instance.GetScreenHeight(); 
		Resize( width <= 0 ? 1 : width, height <= 0 ? 1 : height );
	}

	void Resize(int width, int height)
	{
		Debug.Log($"Resizing render texture: {width}, {height}");
		Release();
		this.width = width;
		this.height = height;
		Create();
	}

	[MenuItem("Assets/Create/Fullscreen Render Texture")]
	public static void CreateMyAsset()
	{
		FullScreenRenderTexture asset = new FullScreenRenderTexture();

		AssetDatabase.CreateAsset(asset, "Assets/FullScreenRenderTexture.renderTexture");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
}
