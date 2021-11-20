using UnityEngine;

public class SetDAZMorphFromBoneAngleToggler : MonoBehaviour
{
	private bool _morphsEnabled = true;

	public string[] morphNames;

	public bool morphsEnabled
	{
		get
		{
			return _morphsEnabled;
		}
		set
		{
			_morphsEnabled = value;
			SetDAZMorphFromBoneAngle[] components = GetComponents<SetDAZMorphFromBoneAngle>();
			string[] array = morphNames;
			foreach (string text in array)
			{
				SetDAZMorphFromBoneAngle[] array2 = components;
				foreach (SetDAZMorphFromBoneAngle setDAZMorphFromBoneAngle in array2)
				{
					if (setDAZMorphFromBoneAngle.morph1Name == text)
					{
						setDAZMorphFromBoneAngle.enabled = value;
						break;
					}
				}
			}
		}
	}
}
