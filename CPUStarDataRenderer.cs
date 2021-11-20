using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUStarDataRenderer : BaseStarDataRenderer
{
	public override IEnumerator ComputeStarData()
	{
		SendProgress(0f);
		Texture2D tex = new Texture2D((int)imageSize, (int)imageSize, TextureFormat.RGBAFloat, mipmap: false);
		int tileSize = (int)imageSize / 2;
		List<StarPoint> starPoints = GenerateRandomStarsPoints(density, tileSize, tileSize);
		Vector2 origin = new Vector2(0f, tileSize);
		SendProgress(0f);
		Vector2 rotationOrigin = new Vector2(tileSize, tileSize);
		for (int yIndex = 0; yIndex < tileSize; yIndex++)
		{
			float yPercent = (float)yIndex / (float)(tileSize - 1);
			float yPosition = SphereUtility.PercentToHeight(yPercent);
			for (int i = 0; i < tileSize; i++)
			{
				float percent = (float)i / (float)(tileSize - 1);
				float radAngle = SphereUtility.PercentToRadAngle(percent);
				Vector3 spot = SphereUtility.SphericalToPoint(yPosition, radAngle);
				StarPoint starPoint = NearestStarPoint(spot, starPoints);
				tex.SetPixel(color: new Color(starPoint.position.x, starPoint.position.y, starPoint.position.z, starPoint.noise), x: (int)origin.x + i, y: (int)origin.y + yIndex);
				SphereUtility.CalculateStarRotation(starPoint.position, out var xRotationAngle, out var yRotationAngle);
				tex.SetPixel(color: new Color(xRotationAngle, yRotationAngle, 0f, 1f), x: (int)rotationOrigin.x + i, y: (int)rotationOrigin.y + yIndex);
			}
			float totalProgress = (float)((yIndex + 1) * tileSize) / (float)(tileSize * tileSize);
			SendProgress(totalProgress);
			yield return null;
		}
		tex.Apply(updateMipmaps: false);
		SendCompletion(tex, success: true);
	}

	private List<StarPoint> GenerateRandomStarsPoints(float density, int imageWidth, int imageHeight)
	{
		int num = Mathf.FloorToInt((float)imageWidth * (float)imageHeight * Mathf.Clamp(density, 0f, 1f));
		List<StarPoint> list = new List<StarPoint>(num + 1);
		for (int i = 0; i < num; i++)
		{
			Vector3 position = Random.onUnitSphere * sphereRadius;
			StarPoint item = new StarPoint(position, Random.Range(0.5f, 1f), 0f, 0f);
			list.Add(item);
		}
		return list;
	}

	private StarPoint NearestStarPoint(Vector3 spot, List<StarPoint> starPoints)
	{
		StarPoint result = new StarPoint(Vector3.zero, 0f, 0f, 0f);
		if (starPoints == null)
		{
			return result;
		}
		float num = -1f;
		for (int i = 0; i < starPoints.Count; i++)
		{
			StarPoint starPoint = starPoints[i];
			float num2 = Vector3.Distance(spot, starPoint.position);
			if (num == -1f || num2 < num)
			{
				result = starPoint;
				num = num2;
			}
		}
		return result;
	}
}
