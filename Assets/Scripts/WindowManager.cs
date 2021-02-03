using System.Collections;
using UnityEngine;

public sealed class WindowManager : Singleton<WindowManager>
{
	public delegate void ScreenResizeEventHandler(int width, int height); // Define a delegate for resize event
	public static event ScreenResizeEventHandler OnScreenResize;          // Define resize event
	private void ResizeScreen(int width, int height)                      // Function to invoke event
	{
		if (OnScreenResize != null)
		{
			OnScreenResize(width, height);
		}
	}

	private const float CHECK_RESIZE_SECONDS = 0.5f; 
	private bool checkForResize = true;
	private Vector2Int screenSize;

	public int GetScreenWidth() { return this.screenSize.x; }
	public int GetScreenHeight() { return this.screenSize.y; }

	void Awake()
	{
		this.screenSize = new Vector2Int(Screen.width, Screen.height);
		ResizeScreen(screenSize.x, screenSize.y);

		Debug.Log($"Starting Resize Coroutine. Initial screen size: {screenSize}");
		StartCoroutine( CheckForResize() );
	}

	IEnumerator CheckForResize()
	{
		while( checkForResize )
		{
			Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height); 
			if (this.screenSize != screenSize)
			{
				this.screenSize = screenSize;
				ResizeScreen(Screen.width, Screen.height);
				Debug.Log($"Screen size changed: {screenSize}");
			}
			yield return new WaitForSeconds(CHECK_RESIZE_SECONDS);
		}
	}

	private void OnDestroy() {
		checkForResize = false;
	}
}
