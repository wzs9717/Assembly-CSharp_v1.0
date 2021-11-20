using UnityEngine;

public class PersonGenderSelector : MonoBehaviour
{
	public enum Gender
	{
		None,
		Male,
		Female,
		Both
	}

	[HideInInspector]
	[SerializeField]
	protected Gender _gender;

	public Transform[] maleTransforms;

	public Transform[] femaleTransforms;

	public Transform rootBones;

	public string rootBonesName = "Genesis2";

	public string rootBonesNameMale = "Genesis2Male";

	public string rootBonesNameFemale = "Genesis2Female";

	public Gender gender
	{
		get
		{
			return _gender;
		}
		set
		{
			if (_gender != value)
			{
				_gender = value;
				SyncGender();
			}
		}
	}

	protected void SyncGender()
	{
		Transform[] array = maleTransforms;
		foreach (Transform transform in array)
		{
			if (_gender == Gender.Both || _gender == Gender.Male)
			{
				transform.gameObject.SetActive(value: true);
			}
			else
			{
				transform.gameObject.SetActive(value: false);
			}
		}
		Transform[] array2 = femaleTransforms;
		foreach (Transform transform2 in array2)
		{
			if (_gender == Gender.Both || _gender == Gender.Female)
			{
				transform2.gameObject.SetActive(value: true);
			}
			else
			{
				transform2.gameObject.SetActive(value: false);
			}
		}
		if (rootBones != null)
		{
			if (_gender == Gender.Male)
			{
				rootBones.name = rootBonesNameMale;
			}
			else if (_gender == Gender.Female)
			{
				rootBones.name = rootBonesNameFemale;
			}
			else
			{
				rootBones.name = rootBonesName;
			}
		}
	}
}
