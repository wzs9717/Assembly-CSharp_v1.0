using System;
using UnityEngine;

[Serializable]
public class MaterialOptionTextureGroup
{
	public string name;

	public string textureName;

	public string secondaryTextureName;

	public Texture2D autoCreateDefaultTexture;

	public string autoCreateDefaultSetName;

	public string autoCreateTextureFilePrefix;

	public string autoCreateSetNamePrefix;

	public int[] materialSlots;

	public MaterialOptionTextureSet[] sets;
}
