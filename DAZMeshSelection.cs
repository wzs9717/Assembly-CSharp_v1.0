using System.Collections.Generic;
using UnityEngine;

public class DAZMeshSelection : MonoBehaviour
{
	public Transform meshTransform;

	public DAZMesh mesh;

	public string selectionName;

	[SerializeField]
	protected int _subMeshSelection = -1;

	[SerializeField]
	protected List<int> _selectedVertices;

	protected Dictionary<int, bool> selectedVerticesDict;

	public bool showSelection;

	public bool showBackfaces;

	public float handleSize = 0.002f;

	[SerializeField]
	protected bool _changed;

	public int subMeshSelection
	{
		get
		{
			return _subMeshSelection;
		}
		set
		{
			if (value != _subMeshSelection)
			{
				_subMeshSelection = value;
			}
		}
	}

	public List<int> selectedVertices => _selectedVertices;

	public bool changed => _changed;

	public void clearChanged()
	{
		_changed = false;
	}

	protected void InitList(bool force = false)
	{
		if (_selectedVertices == null || force)
		{
			_selectedVertices = new List<int>();
		}
	}

	protected void InitDict(bool force = false)
	{
		InitList(force);
		if (selectedVerticesDict != null && !force)
		{
			return;
		}
		selectedVerticesDict = new Dictionary<int, bool>();
		foreach (int selectedVertex in selectedVertices)
		{
			selectedVerticesDict.Add(selectedVertex, value: true);
		}
	}

	public bool IsVertexSelected(int vid)
	{
		InitDict();
		return selectedVerticesDict.ContainsKey(vid);
	}

	public void SelectVertex(int vid)
	{
		InitDict();
		if (vid >= 0 && vid <= mesh.numBaseVertices && !selectedVerticesDict.ContainsKey(vid))
		{
			_changed = true;
			_selectedVertices.Add(vid);
			selectedVerticesDict.Add(vid, value: true);
		}
	}

	public void DeselectVertex(int vid)
	{
		InitDict();
		if (selectedVerticesDict.ContainsKey(vid))
		{
			_changed = true;
			_selectedVertices.Remove(vid);
			selectedVerticesDict.Remove(vid);
		}
	}

	public void ToggleVertexSelection(int vid)
	{
		InitDict();
		_changed = true;
		if (selectedVerticesDict.ContainsKey(vid))
		{
			_selectedVertices.Remove(vid);
			selectedVerticesDict.Remove(vid);
		}
		else
		{
			_selectedVertices.Add(vid);
			selectedVerticesDict.Add(vid, value: true);
		}
	}

	public void ClearSelection()
	{
		_changed = true;
		InitDict(force: true);
	}

	public void Start()
	{
		InitList();
	}
}
