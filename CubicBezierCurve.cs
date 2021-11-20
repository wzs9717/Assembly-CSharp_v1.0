using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CubicBezierCurve : JSONStorable
{
	public enum CurveType
	{
		Auto,
		Smooth,
		Disjoint
	}

	[SerializeField]
	protected bool _draw;

	[SerializeField]
	protected CubicBezierPoint[] _points;

	public Toggle loopToggle;

	[SerializeField]
	protected bool _loop = true;

	[SerializeField]
	protected CurveType _curveType;

	public UIPopup curveTypeSelector;

	[SerializeField]
	protected int _curveSmooth = 10;

	public Material material;

	protected int[] indices;

	protected Vector3[] vertices;

	protected Mesh mesh;

	protected Vector3[] K;

	protected Vector3[] r;

	protected float[] a;

	protected float[] b;

	protected float[] c;

	public bool draw
	{
		get
		{
			return _draw;
		}
		set
		{
			if (_draw != value)
			{
				_draw = value;
			}
		}
	}

	public CubicBezierPoint[] points
	{
		get
		{
			return _points;
		}
		set
		{
			_points = value;
			ResyncControlPoints();
			RegenerateMesh();
		}
	}

	public bool loop
	{
		get
		{
			return _loop;
		}
		set
		{
			if (_loop != value)
			{
				_loop = value;
				if (loopToggle != null)
				{
					loopToggle.isOn = _loop;
				}
				RegenerateMesh();
			}
		}
	}

	public CurveType curveType
	{
		get
		{
			return _curveType;
		}
		set
		{
			if (_curveType != value)
			{
				_curveType = value;
				ResyncControlPoints();
			}
		}
	}

	public int curveSmooth
	{
		get
		{
			return _curveSmooth;
		}
		set
		{
			if (_curveSmooth != value)
			{
				_curveSmooth = value;
				RegenerateMesh();
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (!loop)
			{
				needsStore = true;
				jSON["loop"].AsBool = loop;
			}
			if (curveType != 0)
			{
				needsStore = true;
				jSON["curveType"] = curveType.ToString();
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (restorePhysical)
		{
			if (jc["loop"] != null)
			{
				loop = jc["loop"].AsBool;
			}
			else
			{
				loop = true;
			}
			if (jc["curveType"] != null)
			{
				SetCurveType(jc["curveType"]);
			}
			else
			{
				SetCurveType("Auto");
			}
		}
	}

	public void SetCurveType(string type)
	{
		try
		{
			CurveType curveType2 = (this.curveType = (CurveType)Enum.Parse(typeof(CurveType), type));
			if (curveTypeSelector != null)
			{
				curveTypeSelector.currentValue = type;
			}
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set curve type to " + type + " which is not a valid curve type");
		}
	}

	protected void AutoComputeControlPoints()
	{
		if (_points == null || _points.Length == 0)
		{
			return;
		}
		int num = _points.Length;
		switch (num)
		{
		case 1:
			_points[0].controlPointIn.transform.position = _points[0].point.position;
			_points[0].controlPointOut.transform.position = _points[0].point.position;
			return;
		case 2:
			if (!_loop)
			{
				_points[0].controlPointIn.transform.position = _points[0].point.position;
				_points[0].controlPointOut.transform.position = _points[0].point.position;
				_points[1].controlPointIn.transform.position = _points[1].point.position;
				_points[1].controlPointOut.transform.position = _points[1].point.position;
				return;
			}
			break;
		}
		int num2 = ((!loop) ? (num - 1) : (num + 1));
		if (K == null || K.Length < num2 + 1)
		{
			K = new Vector3[num2 + 1];
		}
		if (loop)
		{
			ref Vector3 reference = ref K[0];
			reference = _points[num - 1].point.position;
			for (int i = 1; i < num2; i++)
			{
				ref Vector3 reference2 = ref K[i];
				reference2 = _points[i - 1].point.position;
			}
			ref Vector3 reference3 = ref K[num2];
			reference3 = _points[0].point.position;
		}
		else
		{
			for (int j = 0; j < num; j++)
			{
				ref Vector3 reference4 = ref K[j];
				reference4 = _points[j].point.position;
			}
		}
		if (a == null || a.Length < num2)
		{
			a = new float[num2];
		}
		if (b == null || b.Length < num2)
		{
			b = new float[num2];
		}
		if (c == null || c.Length < num2)
		{
			c = new float[num2];
		}
		if (r == null || r.Length < num2)
		{
			r = new Vector3[num2];
		}
		a[0] = 0f;
		b[0] = 2f;
		c[0] = 1f;
		ref Vector3 reference5 = ref r[0];
		reference5 = K[0] + 2f * K[1];
		for (int k = 1; k < num2 - 1; k++)
		{
			a[k] = 1f;
			b[k] = 4f;
			c[k] = 1f;
			ref Vector3 reference6 = ref r[k];
			reference6 = 4f * K[k] + 2f * K[k + 1];
		}
		a[num2 - 1] = 2f;
		b[num2 - 1] = 7f;
		c[num2 - 1] = 0f;
		ref Vector3 reference7 = ref r[num2 - 1];
		reference7 = 8f * K[num2 - 1] + K[num2];
		for (int l = 1; l < num2; l++)
		{
			float num3 = a[l] / b[l - 1];
			b[l] -= num3 * c[l - 1];
			ref Vector3 reference8 = ref r[l];
			reference8 = r[l] - num3 * r[l - 1];
		}
		if (loop)
		{
			Vector3 vector = r[num2 - 1] / b[num2 - 1];
			_points[num2 - 2].controlPointOut.transform.position = (r[num2 - 1] - c[num2 - 1] * vector) / b[num2 - 1];
			for (int num4 = num2 - 3; num4 >= 0; num4--)
			{
				_points[num4].controlPointOut.transform.position = (r[num4 + 1] - c[num4 + 1] * _points[num4 + 1].controlPointOut.transform.position) / b[num4 + 1];
			}
		}
		else
		{
			_points[num2].controlPointOut.transform.position = _points[num2].point.position;
			_points[num2 - 1].controlPointOut.transform.position = r[num2 - 1] / b[num2 - 1];
			for (int num5 = num2 - 2; num5 >= 0; num5--)
			{
				_points[num5].controlPointOut.transform.position = (r[num5] - c[num5] * _points[num5 + 1].controlPointOut.transform.position) / b[num5];
			}
		}
		if (loop)
		{
			for (int m = 0; m < num2 - 1; m++)
			{
				_points[m].controlPointIn.transform.position = 2f * K[m + 1] - _points[m].controlPointOut.transform.position;
			}
			return;
		}
		_points[0].controlPointIn.transform.position = _points[0].point.position;
		for (int n = 1; n < num2; n++)
		{
			_points[n].controlPointIn.transform.position = 2f * K[n] - _points[n].controlPointOut.transform.position;
		}
		_points[num2].controlPointIn.transform.position = 0.5f * (K[num2] + _points[num2 - 1].controlPointOut.transform.position);
	}

	protected void SlaveOutputControlPoints()
	{
		for (int i = 0; i < _points.Length; i++)
		{
			if (_points[i] != null && _points[i].controlPointIn != null && _points[i].controlPointOut != null)
			{
				Vector3 vector = _points[i].point.transform.position - _points[i].controlPointIn.transform.position;
				_points[i].controlPointOut.transform.position = _points[i].point.transform.position + vector;
			}
		}
	}

	public void RegenerateMesh()
	{
		if (_points != null && _points.Length > 1)
		{
			int num = _points.Length - 1;
			if (_loop)
			{
				num++;
			}
			mesh = new Mesh();
			indices = new int[num * _curveSmooth];
			vertices = new Vector3[num * _curveSmooth];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < _curveSmooth; j++)
				{
					indices[num2] = num2;
					num2++;
				}
			}
			mesh.vertices = vertices;
			mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
		}
		else
		{
			mesh = new Mesh();
		}
	}

	protected void UpdateMesh()
	{
		if (!(mesh != null) || _points.Length <= 1)
		{
			return;
		}
		int num = _points.Length - 1;
		if (_loop)
		{
			num++;
		}
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			Vector3 position = _points[i].point.position;
			Vector3 position2 = _points[i].controlPointOut.transform.position;
			Vector3 position3;
			Vector3 position4;
			if (loop && i == _points.Length - 1)
			{
				position3 = _points[0].controlPointIn.transform.position;
				position4 = _points[0].point.position;
			}
			else
			{
				position3 = _points[i + 1].controlPointIn.transform.position;
				position4 = _points[i + 1].point.position;
			}
			for (int j = 0; j < _curveSmooth; j++)
			{
				float num3 = (float)j * 1f / (float)(_curveSmooth - 1);
				float num4 = 1f - num3;
				float num5 = num4 * num4;
				float num6 = num5 * num4;
				float num7 = num3 * num3;
				float num8 = num7 * num3;
				Vector3 vector = position * num6 + 3f * position2 * num5 * num3 + 3f * position3 * num4 * num7 + position4 * num8;
				vertices[num2] = vector;
				num2++;
			}
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
	}

	public void ResyncControlPoints()
	{
		switch (_curveType)
		{
		case CurveType.Auto:
			SetCurveAuto();
			break;
		case CurveType.Smooth:
			SetCurveSmooth();
			break;
		case CurveType.Disjoint:
			SetCurveDisjoint();
			break;
		}
	}

	protected void SetCurveAuto()
	{
		for (int i = 0; i < _points.Length; i++)
		{
			if (_points[i] != null)
			{
				if (_points[i].controlPointIn != null)
				{
					_points[i].controlPointIn.gameObject.SetActive(value: false);
				}
				if (_points[i].controlPointOut != null)
				{
					_points[i].controlPointOut.gameObject.SetActive(value: false);
				}
			}
		}
	}

	protected void SetCurveSmooth()
	{
		for (int i = 0; i < _points.Length; i++)
		{
			if (_points[i] != null)
			{
				if (_points[i].controlPointIn != null)
				{
					_points[i].controlPointIn.gameObject.SetActive(value: true);
				}
				if (_points[i].controlPointOut != null)
				{
					_points[i].controlPointOut.gameObject.SetActive(value: false);
				}
			}
		}
	}

	protected void SetCurveDisjoint()
	{
		for (int i = 0; i < _points.Length; i++)
		{
			if (_points[i] != null)
			{
				if (_points[i].controlPointIn != null)
				{
					_points[i].controlPointIn.gameObject.SetActive(value: true);
				}
				if (_points[i].controlPointOut != null)
				{
					_points[i].controlPointOut.gameObject.SetActive(value: true);
				}
			}
		}
	}

	protected void SetControlPoints()
	{
		switch (_curveType)
		{
		case CurveType.Auto:
			AutoComputeControlPoints();
			break;
		case CurveType.Smooth:
			SlaveOutputControlPoints();
			break;
		}
	}

	protected void DrawMesh()
	{
		if (mesh != null && material != null && _draw)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			Graphics.DrawMesh(mesh, identity, material, base.gameObject.layer, null, 0, null, castShadows: false, receiveShadows: false);
		}
	}

	public Vector3 GetPositionFromPoint(int fromPoint, float t)
	{
		Vector3 position = _points[fromPoint].point.position;
		Vector3 position2 = _points[fromPoint].controlPointOut.transform.position;
		if (_points.Length == 1)
		{
			return position;
		}
		Vector3 position3;
		Vector3 position4;
		if (fromPoint == _points.Length - 1)
		{
			if (!loop)
			{
				return position;
			}
			position3 = _points[0].controlPointIn.transform.position;
			position4 = _points[0].point.position;
		}
		else
		{
			position3 = _points[fromPoint + 1].controlPointIn.transform.position;
			position4 = _points[fromPoint + 1].point.position;
		}
		float num = 1f - t;
		float num2 = num * num;
		float num3 = num2 * num;
		float num4 = t * t;
		float num5 = num4 * t;
		return position * num3 + 3f * position2 * num2 * t + 3f * position3 * num * num4 + position4 * num5;
	}

	public Quaternion GetRotationFromPoint(int fromPoint, float t)
	{
		Quaternion rotation = _points[fromPoint].point.rotation;
		if (_points.Length == 1)
		{
			return rotation;
		}
		Quaternion rotation2;
		if (fromPoint == _points.Length - 1)
		{
			if (!loop)
			{
				return rotation;
			}
			rotation2 = _points[0].point.rotation;
		}
		else
		{
			rotation2 = _points[fromPoint + 1].point.rotation;
		}
		return Quaternion.Lerp(rotation, rotation2, t);
	}

	protected virtual void InitUI()
	{
		if (loopToggle != null)
		{
			loopToggle.onValueChanged.AddListener(delegate
			{
				loop = loopToggle.isOn;
			});
		}
		if (curveTypeSelector != null)
		{
			UIPopup uIPopup = curveTypeSelector;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetCurveType));
		}
	}

	protected void Start()
	{
		InitUI();
	}

	protected void Update()
	{
		if (mesh == null)
		{
			RegenerateMesh();
		}
		SetControlPoints();
		UpdateMesh();
		DrawMesh();
	}
}
