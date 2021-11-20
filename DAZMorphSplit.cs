using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DAZMorphSplit : MonoBehaviour
{
	public DAZMorphBank bankToModify;

	public string inputMorphName;

	public string outputMorphName;

	public Dictionary<int, bool> filterVerticesDict;

	public float handleSize = 0.0001f;

	public bool showHandles;

	public bool showBackfaceHandles;

	public int subMeshSelection = -1;

	public int subMeshSelection2 = -1;

	protected DAZMorph _inputMorph;

	protected Dictionary<int, bool> _inputMorphVerticesDict;

	protected DAZMorph _outputMorph;

	protected Dictionary<int, int> _uvVertToBaseVertDict;

	public DAZMorph inputMorph => _inputMorph;

	public DAZMorph outputMorph => _outputMorph;

	public void ClickVertex(int vid)
	{
		if (IsFilterVertex(vid))
		{
			filterVerticesDict.Remove(vid);
		}
		else
		{
			filterVerticesDict.Add(vid, value: true);
		}
	}

	public void UpclickVertex(int vid)
	{
	}

	public void OnVertex(int vid)
	{
		if (!IsFilterVertex(vid) && IsInputMorphVertex(vid))
		{
			filterVerticesDict.Add(vid, value: true);
		}
	}

	public void OffVertex(int vid)
	{
		if (IsFilterVertex(vid))
		{
			filterVerticesDict.Remove(vid);
		}
	}

	public int GetBaseVertex(int vid)
	{
		if (_uvVertToBaseVertDict.TryGetValue(vid, out var value))
		{
			vid = value;
		}
		return vid;
	}

	public bool IsBaseVertex(int vid)
	{
		if (_uvVertToBaseVertDict != null)
		{
			return !_uvVertToBaseVertDict.ContainsKey(vid);
		}
		return true;
	}

	protected void CheckInputMorph()
	{
		if (bankToModify != null)
		{
			if (_inputMorph != null && !(_inputMorph.morphName != inputMorphName))
			{
				return;
			}
			_inputMorph = bankToModify.GetMorph(inputMorphName);
			_inputMorphVerticesDict = new Dictionary<int, bool>();
			if (_inputMorph != null)
			{
				for (int i = 0; i < _inputMorph.deltas.Length; i++)
				{
					int vertex = _inputMorph.deltas[i].vertex;
					_inputMorphVerticesDict.Add(vertex, value: true);
				}
			}
		}
		else
		{
			_inputMorph = null;
			_inputMorphVerticesDict = null;
		}
	}

	public bool IsValidInputMorph()
	{
		CheckInputMorph();
		if (_inputMorph != null)
		{
			return true;
		}
		return false;
	}

	public void InvalidateOutputMorph()
	{
		_outputMorph = null;
	}

	protected void CheckOutputMorph()
	{
		if (bankToModify != null)
		{
			if (_outputMorph == null || _outputMorph.morphName != outputMorphName)
			{
				_outputMorph = bankToModify.GetMorph(outputMorphName);
			}
		}
		else
		{
			_outputMorph = null;
		}
	}

	public bool IsExistingOutputMorph()
	{
		CheckOutputMorph();
		if (_outputMorph != null)
		{
			return true;
		}
		return false;
	}

	public bool IsInputMorphVertex(int vid)
	{
		CheckInputMorph();
		if (_inputMorphVerticesDict != null && _inputMorphVerticesDict.ContainsKey(vid))
		{
			return true;
		}
		return false;
	}

	public bool IsFilterVertex(int vid)
	{
		if (filterVerticesDict != null && filterVerticesDict.ContainsKey(vid))
		{
			return true;
		}
		return false;
	}

	protected void InitCaches(bool force = false)
	{
		if (filterVerticesDict == null)
		{
			filterVerticesDict = new Dictionary<int, bool>();
		}
		if (bankToModify != null && bankToModify.connectedMesh != null)
		{
			_uvVertToBaseVertDict = bankToModify.connectedMesh.uvVertToBaseVert;
		}
		else
		{
			_uvVertToBaseVertDict = new Dictionary<int, int>();
		}
	}

	public void FilterMorph()
	{
		if (!(bankToModify != null) || filterVerticesDict == null)
		{
			return;
		}
		DAZMesh connectedMesh = bankToModify.connectedMesh;
		CheckInputMorph();
		if (_inputMorph == null)
		{
			return;
		}
		CheckOutputMorph();
		if (_outputMorph == null)
		{
			_outputMorph = new DAZMorph(_inputMorph);
			List<DAZMorphVertex> list = new List<DAZMorphVertex>();
			DAZMorphVertex[] deltas = _inputMorph.deltas;
			foreach (DAZMorphVertex dAZMorphVertex in deltas)
			{
				if (filterVerticesDict.ContainsKey(dAZMorphVertex.vertex))
				{
					DAZMorphVertex dAZMorphVertex2 = new DAZMorphVertex();
					dAZMorphVertex2.delta = dAZMorphVertex.delta;
					dAZMorphVertex2.vertex = dAZMorphVertex.vertex;
					list.Add(dAZMorphVertex2);
				}
			}
			_outputMorph.morphName = outputMorphName;
			_outputMorph.displayName = outputMorphName;
			_outputMorph.deltas = list.ToArray();
			_outputMorph.numDeltas = _outputMorph.deltas.Length;
			bankToModify.AddMorph(_outputMorph);
		}
		else
		{
			Debug.Log("Output morph with name " + outputMorphName + " already exists");
		}
	}

	private void OnEnable()
	{
		if (Application.isEditor)
		{
			InitCaches(force: true);
		}
	}
}
