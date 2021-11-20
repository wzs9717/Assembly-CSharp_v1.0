using UnityEngine;

[ExecuteInEditMode]
public class SyncMaterialParameters : MonoBehaviour
{
	public bool sync;

	public Material master;

	public Material[] slaves;

	public string[] syncParams;

	public string[] syncColorParams;

	private void Start()
	{
	}

	private void Sync()
	{
		string[] array = syncParams;
		foreach (string text in array)
		{
			if (!master.HasProperty(text))
			{
				continue;
			}
			Material[] array2 = slaves;
			foreach (Material material in array2)
			{
				if (material.HasProperty(text))
				{
					material.SetFloat(text, master.GetFloat(text));
				}
			}
		}
		string[] array3 = syncColorParams;
		foreach (string text2 in array3)
		{
			if (!master.HasProperty(text2))
			{
				continue;
			}
			Material[] array4 = slaves;
			foreach (Material material2 in array4)
			{
				if (material2.HasProperty(text2))
				{
					material2.SetColor(text2, master.GetColor(text2));
				}
			}
		}
	}

	private void Update()
	{
		if (sync)
		{
			sync = false;
			Sync();
		}
	}
}
