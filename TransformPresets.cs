using UnityEngine;

public class TransformPresets : MonoBehaviour
{
	[SerializeField]
	private Vector3[] _positions;

	[SerializeField]
	private Quaternion[] _rotations;

	[SerializeField]
	private string[] _labels;

	[SerializeField]
	private int _numPresets;

	public Vector3[] positions => _positions;

	public Quaternion[] rotations => _rotations;

	public string[] labels => _labels;

	public int numPresets
	{
		get
		{
			return _numPresets;
		}
		set
		{
			if (_numPresets != value && value > 0)
			{
				int num = _numPresets;
				_numPresets = value;
				Vector3[] array = new Vector3[_numPresets];
				Quaternion[] array2 = new Quaternion[_numPresets];
				string[] array3 = new string[_numPresets];
				for (int i = 0; i < _numPresets; i++)
				{
					array2[i].x = 0f;
					array2[i].y = 0f;
					array2[i].z = 0f;
					array2[i].w = 1f;
				}
				for (int j = 0; j < num && j != _numPresets; j++)
				{
					ref Vector3 reference = ref array[j];
					reference = _positions[j];
					ref Quaternion reference2 = ref array2[j];
					reference2 = _rotations[j];
					array3[j] = _labels[j];
				}
				_positions = array;
				_rotations = array2;
				_labels = array3;
			}
		}
	}

	public void SetPresetFromTransform(int presetNum)
	{
		if (presetNum < _numPresets && presetNum >= 0)
		{
			ref Vector3 reference = ref _positions[presetNum];
			reference = base.transform.position;
			ref Quaternion reference2 = ref _rotations[presetNum];
			reference2 = base.transform.rotation;
		}
	}

	public void SetTransformFromPreset(int presetNum)
	{
		if (presetNum < _numPresets && presetNum >= 0)
		{
			base.transform.position = _positions[presetNum];
			base.transform.rotation = _rotations[presetNum];
		}
	}
}
