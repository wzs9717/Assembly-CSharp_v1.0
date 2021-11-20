using System.Collections;
using UnityEngine;

public abstract class BaseStarDataRenderer
{
	public delegate void StarDataProgress(BaseStarDataRenderer renderer, float progress);

	public delegate void StarDataComplete(BaseStarDataRenderer renderer, Texture2D texture, bool success);

	public float density;

	public float imageSize;

	protected float sphereRadius = 1f;

	public event StarDataProgress progressCallback;

	public event StarDataComplete completionCallback;

	public abstract IEnumerator ComputeStarData();

	protected void SendProgress(float progress)
	{
		if (this.progressCallback != null)
		{
			this.progressCallback(this, progress);
		}
	}

	protected void SendCompletion(Texture2D texture, bool success)
	{
		if (this.completionCallback != null)
		{
			this.completionCallback(this, texture, success);
		}
	}
}
