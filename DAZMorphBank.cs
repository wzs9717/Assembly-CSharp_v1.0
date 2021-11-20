using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
public class DAZMorphBank : MonoBehaviour
{
	[SerializeField]
	protected DAZMesh _connectedMesh;

	public string geometryId;

	public DAZBones morphBones;

	public DAZBones morphBones2;

	public DAZMorphBank copyMorphsFrom;

	public bool enableMCMMorphs = true;

	private bool bonesDirty;

	[SerializeField]
	protected List<DAZMorph> _morphs;

	public bool showMorphs;

	protected Dictionary<string, DAZMorph> _morphsByName;

	public Dictionary<string, bool> showRegion;

	protected Dictionary<string, List<DAZMorph>> _morphsByRegion;

	public bool useThreadedMorphing;

	protected DAZMorphTaskInfo applyMorphsTask;

	protected bool _threadsRunning;

	protected bool triggerThreadResetMorphs;

	protected bool threadedVerticesChanged;

	protected int[] threadedChangedVertices;

	protected int numThreadedChangedVertices;

	protected int numMaxThreadedChangedVertices;

	protected bool checkAllThreadedVertices;

	protected bool visibleThreadedVerticesChanged;

	public bool visibleVerticesChanged;

	protected DAZMorph[] threadedChangedMorphs;

	protected int numThreadedChangedMorphs;

	protected Vector3[] _threadedMorphedUVVertices;

	protected Vector3[] _threadedVisibleMorphedUVVertices;

	public bool _threadedNormalsDirtyThisFrame;

	public bool _threadedTangentsDirtyThisFrame;

	protected int totalFrameCount;

	protected int missedFrameCount;

	public DAZMesh connectedMesh
	{
		get
		{
			return _connectedMesh;
		}
		set
		{
			if (_connectedMesh != value)
			{
				_connectedMesh = value;
				ResetMorphs();
			}
		}
	}

	public List<DAZMorph> morphs
	{
		get
		{
			if (_morphs == null)
			{
				_morphs = new List<DAZMorph>();
			}
			return _morphs;
		}
	}

	[SerializeField]
	public int numMorphs
	{
		get
		{
			if (_morphs == null)
			{
				_morphs = new List<DAZMorph>();
			}
			return _morphs.Count;
		}
	}

	public Dictionary<string, List<DAZMorph>> morphsByRegion
	{
		get
		{
			if (_morphsByRegion == null || showRegion == null)
			{
				RebuildMorphsByRegion();
			}
			return _morphsByRegion;
		}
	}

	public DAZMorph GetMorph(string morphName)
	{
		if (_morphsByName == null)
		{
			_morphsByName = new Dictionary<string, DAZMorph>();
			foreach (DAZMorph morph in _morphs)
			{
				_morphsByName.Add(morph.morphName, morph);
			}
		}
		if (_morphsByName.TryGetValue(morphName, out var value))
		{
			return value;
		}
		return null;
	}

	protected void CopyMorph(DAZMorph dm)
	{
		DAZMorph morph = GetMorph(dm.morphName);
		if (morph != null)
		{
			morph.disable = dm.disable;
			morph.visible = dm.visible;
			morph.isPoseControl = dm.isPoseControl;
			morph.max = dm.max;
			morph.min = dm.min;
			morph.region = dm.region;
			morph.group = dm.group;
		}
		else
		{
			DAZMorph dAZMorph = new DAZMorph(dm);
			if (_morphsByName != null)
			{
				_morphsByName.Add(dAZMorph.morphName, dAZMorph);
			}
			_morphs.Add(dAZMorph);
		}
	}

	public void CopyMorphs()
	{
		if (!(copyMorphsFrom != null))
		{
			return;
		}
		foreach (DAZMorph morph in copyMorphsFrom.morphs)
		{
			CopyMorph(morph);
		}
		RebuildMorphsByRegion();
	}

	protected void RebuildMorphsByRegion()
	{
		_morphsByRegion = new Dictionary<string, List<DAZMorph>>();
		showRegion = new Dictionary<string, bool>();
		if (_morphs == null)
		{
			return;
		}
		for (int i = 0; i < _morphs.Count; i++)
		{
			string text = _morphs[i].region;
			if (text == null)
			{
				text = "None";
			}
			if (!showRegion.TryGetValue(text, out var _))
			{
				showRegion.Add(text, value: false);
			}
			if (!_morphsByRegion.TryGetValue(text, out var value2))
			{
				value2 = new List<DAZMorph>();
				_morphsByRegion.Add(text, value2);
			}
			value2.Add(_morphs[i]);
		}
	}

	protected void MTTask(object info)
	{
		DAZMorphTaskInfo dAZMorphTaskInfo = (DAZMorphTaskInfo)info;
		while (_threadsRunning)
		{
			dAZMorphTaskInfo.resetEvent.WaitOne(-1, exitContext: true);
			if (dAZMorphTaskInfo.kill)
			{
				break;
			}
			if (dAZMorphTaskInfo.taskType == DAZMorphTaskType.ApplyMorphs)
			{
				Thread.Sleep(0);
				ApplyMorphsThreaded();
			}
			dAZMorphTaskInfo.working = false;
		}
	}

	protected void StopThreads()
	{
		_threadsRunning = false;
		if (applyMorphsTask != null)
		{
			applyMorphsTask.kill = true;
			applyMorphsTask.resetEvent.Set();
			while (applyMorphsTask.thread.IsAlive)
			{
			}
			applyMorphsTask = null;
		}
	}

	protected void StartThreads()
	{
		if (!_threadsRunning)
		{
			_threadsRunning = true;
			applyMorphsTask = new DAZMorphTaskInfo();
			applyMorphsTask.threadIndex = 0;
			applyMorphsTask.name = "ApplyMorphsTask";
			applyMorphsTask.resetEvent = new AutoResetEvent(initialState: false);
			applyMorphsTask.thread = new Thread(MTTask);
			applyMorphsTask.thread.Priority = System.Threading.ThreadPriority.Lowest;
			applyMorphsTask.taskType = DAZMorphTaskType.ApplyMorphs;
			applyMorphsTask.thread.Start(applyMorphsTask);
		}
	}

	public void ApplyMorphsThreadedStart()
	{
		if (_morphs == null)
		{
			_morphs = new List<DAZMorph>();
		}
		if (threadedChangedVertices == null)
		{
			numMaxThreadedChangedVertices = _threadedMorphedUVVertices.Length;
			threadedChangedVertices = new int[numMaxThreadedChangedVertices];
		}
		checkAllThreadedVertices = false;
		numThreadedChangedVertices = 0;
		if (threadedChangedMorphs == null)
		{
			threadedChangedMorphs = new DAZMorph[_morphs.Count];
		}
		numThreadedChangedMorphs = 0;
		threadedVerticesChanged = false;
		visibleThreadedVerticesChanged = false;
		bonesDirty = false;
	}

	public void ApplyMorphsThreaded()
	{
		bool flag = true;
		int num = 5;
		while (flag)
		{
			num--;
			if (num == 0)
			{
				break;
			}
			flag = false;
			for (int i = 0; i < _morphs.Count; i++)
			{
				DAZMorph dAZMorph = _morphs[i];
				if (dAZMorph.disable)
				{
					continue;
				}
				float morphValue = dAZMorph.morphValue;
				float appliedValue = dAZMorph.appliedValue;
				if (appliedValue != morphValue)
				{
					DAZMorphFormula[] formulas = dAZMorph.formulas;
					foreach (DAZMorphFormula dAZMorphFormula in formulas)
					{
						if (dAZMorphFormula.targetType == DAZMorphFormulaTargetType.MorphValue)
						{
							DAZMorph morph = GetMorph(dAZMorphFormula.target);
							if (morph != null)
							{
								morph.morphValue = dAZMorphFormula.multiplier * morphValue;
								continue;
							}
							Debug.LogWarning(string.Concat("Could not find slave morph ", morph, " in morph ", dAZMorph.morphName));
						}
					}
				}
				if (enableMCMMorphs)
				{
					DAZMorphFormula[] formulas2 = dAZMorph.formulas;
					foreach (DAZMorphFormula dAZMorphFormula2 in formulas2)
					{
						if (dAZMorphFormula2.targetType == DAZMorphFormulaTargetType.MCM)
						{
							DAZMorph morph2 = GetMorph(dAZMorphFormula2.target);
							if (morph2 != null)
							{
								dAZMorph.morphValue = morph2.morphValue * dAZMorphFormula2.multiplier;
								morphValue = dAZMorph.morphValue;
							}
							else
							{
								dAZMorph.morphValue = 0f;
								morphValue = dAZMorph.morphValue;
							}
						}
						else if (dAZMorphFormula2.targetType == DAZMorphFormulaTargetType.MCMMult)
						{
							DAZMorph morph3 = GetMorph(dAZMorphFormula2.target);
							if (morph3 != null)
							{
								dAZMorph.morphValue *= morph3.morphValue;
								morphValue = dAZMorph.morphValue;
							}
							else
							{
								dAZMorph.morphValue = 0f;
								morphValue = dAZMorph.morphValue;
							}
						}
					}
				}
				if (appliedValue == morphValue)
				{
					continue;
				}
				flag = true;
				threadedChangedMorphs[numThreadedChangedMorphs] = dAZMorph;
				numThreadedChangedMorphs++;
				if (dAZMorph.deltas.Length > 0)
				{
					threadedVerticesChanged = true;
					if (dAZMorph.visible)
					{
						visibleThreadedVerticesChanged = true;
						float num2 = morphValue - appliedValue;
						DAZMorphVertex[] deltas = dAZMorph.deltas;
						foreach (DAZMorphVertex dAZMorphVertex in deltas)
						{
							if (numThreadedChangedVertices < numMaxThreadedChangedVertices)
							{
								threadedChangedVertices[numThreadedChangedVertices] = dAZMorphVertex.vertex;
								numThreadedChangedVertices++;
							}
							else
							{
								checkAllThreadedVertices = true;
							}
							_threadedMorphedUVVertices[dAZMorphVertex.vertex].x += dAZMorphVertex.delta.x * num2;
							_threadedMorphedUVVertices[dAZMorphVertex.vertex].y += dAZMorphVertex.delta.y * num2;
							_threadedMorphedUVVertices[dAZMorphVertex.vertex].z += dAZMorphVertex.delta.z * num2;
							_threadedVisibleMorphedUVVertices[dAZMorphVertex.vertex].x += dAZMorphVertex.delta.x * num2;
							_threadedVisibleMorphedUVVertices[dAZMorphVertex.vertex].y += dAZMorphVertex.delta.y * num2;
							_threadedVisibleMorphedUVVertices[dAZMorphVertex.vertex].z += dAZMorphVertex.delta.z * num2;
						}
					}
					else
					{
						float num3 = morphValue - appliedValue;
						DAZMorphVertex[] deltas2 = dAZMorph.deltas;
						foreach (DAZMorphVertex dAZMorphVertex2 in deltas2)
						{
							if (numThreadedChangedVertices < numMaxThreadedChangedVertices)
							{
								threadedChangedVertices[numThreadedChangedVertices] = dAZMorphVertex2.vertex;
								numThreadedChangedVertices++;
							}
							else
							{
								checkAllThreadedVertices = true;
							}
							_threadedMorphedUVVertices[dAZMorphVertex2.vertex].x += dAZMorphVertex2.delta.x * num3;
							_threadedMorphedUVVertices[dAZMorphVertex2.vertex].y += dAZMorphVertex2.delta.y * num3;
							_threadedMorphedUVVertices[dAZMorphVertex2.vertex].z += dAZMorphVertex2.delta.z * num3;
						}
					}
				}
				dAZMorph.appliedValue = morphValue;
			}
		}
	}

	public void ApplyMorphsThreadedFinish()
	{
		if (triggerThreadResetMorphs)
		{
			ApplyMorphsInternal();
			if (connectedMesh != null)
			{
				connectedMesh.ResetMorphedVertices();
				_threadedMorphedUVVertices = (Vector3[])connectedMesh.UVVertices.Clone();
				_threadedVisibleMorphedUVVertices = (Vector3[])connectedMesh.UVVertices.Clone();
			}
			else
			{
				Debug.LogWarning("ResetMorphs called when connected mesh was not set. Vertices were not reset.");
			}
			if (_morphs != null)
			{
				for (int i = 0; i < _morphs.Count; i++)
				{
					_morphs[i].appliedValue = 0f;
				}
			}
			ZeroAllBoneMorphs();
			triggerThreadResetMorphs = false;
			visibleVerticesChanged = true;
			return;
		}
		if (threadedChangedMorphs != null)
		{
			for (int j = 0; j < numThreadedChangedMorphs; j++)
			{
				DAZMorph morph = threadedChangedMorphs[j];
				ApplyBoneMorphs(morphBones, morph);
				ApplyBoneMorphs(morphBones2, morph);
			}
		}
		if (bonesDirty)
		{
			if (morphBones != null)
			{
				morphBones.SetMorphedTransform();
			}
			if (morphBones2 != null)
			{
				morphBones2.SetMorphedTransform();
			}
			bonesDirty = false;
		}
		visibleVerticesChanged = visibleThreadedVerticesChanged;
		if (!threadedVerticesChanged || !(connectedMesh != null))
		{
			return;
		}
		Vector3[] morphedBaseVertices = connectedMesh.morphedBaseVertices;
		Vector3[] rawMorphedUVVertices = connectedMesh.rawMorphedUVVertices;
		Vector3[] visibleMorphedUVVertices = connectedMesh.visibleMorphedUVVertices;
		if (checkAllThreadedVertices)
		{
			int numBaseVertices = connectedMesh.numBaseVertices;
			for (int k = 0; k < numBaseVertices; k++)
			{
				ref Vector3 reference = ref morphedBaseVertices[k];
				reference = _threadedMorphedUVVertices[k];
				ref Vector3 reference2 = ref rawMorphedUVVertices[k];
				reference2 = _threadedMorphedUVVertices[k];
				ref Vector3 reference3 = ref visibleMorphedUVVertices[k];
				reference3 = _threadedVisibleMorphedUVVertices[k];
			}
		}
		else
		{
			for (int l = 0; l < numThreadedChangedVertices; l++)
			{
				int num = threadedChangedVertices[l];
				ref Vector3 reference4 = ref morphedBaseVertices[num];
				reference4 = _threadedMorphedUVVertices[num];
				ref Vector3 reference5 = ref rawMorphedUVVertices[num];
				reference5 = _threadedMorphedUVVertices[num];
				ref Vector3 reference6 = ref visibleMorphedUVVertices[num];
				reference6 = _threadedVisibleMorphedUVVertices[num];
			}
		}
		connectedMesh.ApplyMorphVertices(visibleVerticesChanged);
	}

	protected void InitMorphs()
	{
		if (_morphs != null)
		{
			for (int i = 0; i < _morphs.Count; i++)
			{
				_morphs[i].startValue = _morphs[i].morphValue;
			}
		}
	}

	public void ResetMorphs()
	{
		if (useThreadedMorphing && Application.isPlaying)
		{
			triggerThreadResetMorphs = true;
			return;
		}
		ApplyMorphsInternal();
		if (connectedMesh != null)
		{
			connectedMesh.ResetMorphedVertices();
		}
		if (_morphs != null)
		{
			for (int i = 0; i < _morphs.Count; i++)
			{
				_morphs[i].appliedValue = 0f;
			}
		}
		ZeroAllBoneMorphs();
	}

	public void AddMorph(DAZMorph dm)
	{
		bool flag = false;
		if (_morphs == null)
		{
			_morphs = new List<DAZMorph>();
		}
		for (int i = 0; i < _morphs.Count; i++)
		{
			if (_morphs[i] != null && _morphs[i].morphName == dm.morphName)
			{
				flag = true;
				dm.CopyParameters(_morphs[i]);
				_morphs[i] = dm;
				if (_morphsByName != null && _morphsByName.ContainsKey(dm.morphName))
				{
					_morphsByName.Remove(dm.morphName);
					_morphsByName.Add(dm.morphName, dm);
				}
				break;
			}
		}
		if (!flag)
		{
			_morphs.Add(dm);
			if (_morphsByName != null)
			{
				_morphsByName.Add(dm.morphName, dm);
			}
		}
		RebuildMorphsByRegion();
		ResetMorphs();
	}

	public void RemoveAllMorphs()
	{
		for (int i = 0; i < _morphs.Count; i++)
		{
			_morphs[i].morphValue = 0f;
		}
		ApplyMorphs();
		_morphs.RemoveRange(0, _morphs.Count);
		_morphsByName = null;
		RebuildMorphsByRegion();
		ResetMorphs();
	}

	public void RemoveMorph(string morphName)
	{
		for (int i = 0; i < _morphs.Count; i++)
		{
			if (_morphs[i] != null && _morphs[i].morphName == morphName)
			{
				RemoveMorph(i);
				break;
			}
		}
	}

	public void RemoveMorph(int index)
	{
		if (index < _morphs.Count && _morphs[index] != null)
		{
			_morphs[index].morphValue = 0f;
			ApplyMorphs();
			_morphs.Remove(_morphs[index]);
			if (_morphsByName != null && _morphsByName.ContainsKey(_morphs[index].morphName))
			{
				_morphsByName.Remove(_morphs[index].morphName);
			}
			RebuildMorphsByRegion();
			ResetMorphs();
		}
	}

	protected void ApplyBoneMorphs(DAZBones bones, DAZMorph morph, bool zero = false)
	{
		if (!(bones != null))
		{
			return;
		}
		DAZMorphFormula[] formulas = morph.formulas;
		foreach (DAZMorphFormula dAZMorphFormula in formulas)
		{
			switch (dAZMorphFormula.targetType)
			{
			case DAZMorphFormulaTargetType.GeneralScale:
				if (zero)
				{
					bones.SetGeneralScale(morph.morphName, 0f);
				}
				else
				{
					bones.SetGeneralScale(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
				}
				break;
			case DAZMorphFormulaTargetType.BoneCenterX:
			{
				bonesDirty = true;
				DAZBone dAZBone = bones.GetDAZBone(dAZMorphFormula.target);
				if (dAZBone != null)
				{
					if (zero)
					{
						dAZBone.SetBoneXOffset(morph.morphName, 0f);
					}
					else
					{
						dAZBone.SetBoneXOffset(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
					}
				}
				else
				{
					Debug.LogWarning("Could not get bone " + dAZMorphFormula.target + " for morph " + morph.morphName);
				}
				break;
			}
			case DAZMorphFormulaTargetType.BoneCenterY:
			{
				bonesDirty = true;
				DAZBone dAZBone = bones.GetDAZBone(dAZMorphFormula.target);
				if (dAZBone != null)
				{
					if (zero)
					{
						dAZBone.SetBoneYOffset(morph.morphName, 0f);
					}
					else
					{
						dAZBone.SetBoneYOffset(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
					}
				}
				else
				{
					Debug.LogWarning("Could not get bone " + dAZMorphFormula.target + " for morph " + morph.morphName);
				}
				break;
			}
			case DAZMorphFormulaTargetType.BoneCenterZ:
			{
				bonesDirty = true;
				DAZBone dAZBone = bones.GetDAZBone(dAZMorphFormula.target);
				if (dAZBone != null)
				{
					if (zero)
					{
						dAZBone.SetBoneZOffset(morph.morphName, 0f);
					}
					else
					{
						dAZBone.SetBoneZOffset(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
					}
				}
				else
				{
					Debug.LogWarning("Could not get bone " + dAZMorphFormula.target + " for morph " + morph.morphName);
				}
				break;
			}
			case DAZMorphFormulaTargetType.OrientationX:
			{
				bonesDirty = true;
				DAZBone dAZBone = bones.GetDAZBone(dAZMorphFormula.target);
				if (dAZBone != null)
				{
					if (zero)
					{
						dAZBone.SetBoneOrientationXOffset(morph.morphName, 0f);
					}
					else
					{
						dAZBone.SetBoneOrientationXOffset(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
					}
				}
				else
				{
					Debug.LogWarning("Could not get bone " + dAZMorphFormula.target + " for morph " + morph.morphName);
				}
				break;
			}
			case DAZMorphFormulaTargetType.OrientationY:
			{
				bonesDirty = true;
				DAZBone dAZBone = bones.GetDAZBone(dAZMorphFormula.target);
				if (dAZBone != null)
				{
					if (zero)
					{
						dAZBone.SetBoneOrientationYOffset(morph.morphName, 0f);
					}
					else
					{
						dAZBone.SetBoneOrientationYOffset(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
					}
				}
				else
				{
					Debug.LogWarning("Could not get bone " + dAZMorphFormula.target + " for morph " + morph.morphName);
				}
				break;
			}
			case DAZMorphFormulaTargetType.OrientationZ:
			{
				bonesDirty = true;
				DAZBone dAZBone = bones.GetDAZBone(dAZMorphFormula.target);
				if (dAZBone != null)
				{
					if (zero)
					{
						dAZBone.SetBoneOrientationZOffset(morph.morphName, 0f);
					}
					else
					{
						dAZBone.SetBoneOrientationZOffset(morph.morphName, dAZMorphFormula.multiplier * morph.morphValue);
					}
				}
				else
				{
					Debug.LogWarning("Could not get bone " + dAZMorphFormula.target + " for morph " + morph.morphName);
				}
				break;
			}
			}
		}
	}

	protected void ApplyAllBoneMorphs()
	{
		for (int i = 0; i < _morphs.Count; i++)
		{
			ApplyBoneMorphs(morphBones, _morphs[i]);
			ApplyBoneMorphs(morphBones2, _morphs[i]);
		}
		if (morphBones != null)
		{
			morphBones.SetMorphedTransform();
		}
		if (morphBones2 != null)
		{
			morphBones2.SetMorphedTransform();
		}
		bonesDirty = false;
	}

	protected void ZeroAllBoneMorphs()
	{
		for (int i = 0; i < _morphs.Count; i++)
		{
			ApplyBoneMorphs(morphBones, _morphs[i], zero: true);
			ApplyBoneMorphs(morphBones2, _morphs[i], zero: true);
		}
		if (morphBones != null)
		{
			morphBones.SetMorphedTransform();
		}
		if (morphBones2 != null)
		{
			morphBones2.SetMorphedTransform();
		}
		bonesDirty = false;
	}

	protected void ApplyMorphsInternal(bool force = false)
	{
		if (_morphs == null)
		{
			_morphs = new List<DAZMorph>();
		}
		visibleVerticesChanged = false;
		bool flag = false;
		bool flag2 = true;
		int num = 5;
		bonesDirty = false;
		Vector3[] array = null;
		Vector3[] array2 = null;
		Vector3[] array3 = null;
		if (connectedMesh != null)
		{
			array = connectedMesh.morphedBaseVertices;
			array2 = connectedMesh.rawMorphedUVVertices;
			array3 = connectedMesh.visibleMorphedUVVertices;
		}
		while (flag2)
		{
			num--;
			if (num == 0)
			{
				break;
			}
			flag2 = false;
			for (int i = 0; i < _morphs.Count; i++)
			{
				DAZMorph dAZMorph = _morphs[i];
				if (dAZMorph.disable)
				{
					continue;
				}
				float morphValue = dAZMorph.morphValue;
				float appliedValue = dAZMorph.appliedValue;
				if (appliedValue != morphValue)
				{
					DAZMorphFormula[] formulas = dAZMorph.formulas;
					foreach (DAZMorphFormula dAZMorphFormula in formulas)
					{
						if (dAZMorphFormula.targetType == DAZMorphFormulaTargetType.MorphValue)
						{
							DAZMorph morph = GetMorph(dAZMorphFormula.target);
							if (morph != null)
							{
								morph.morphValue = dAZMorphFormula.multiplier * morphValue;
								continue;
							}
							Debug.LogWarning(string.Concat("Could not find slave morph ", morph, " in morph ", dAZMorph.morphName));
						}
					}
				}
				if (enableMCMMorphs)
				{
					DAZMorphFormula[] formulas2 = dAZMorph.formulas;
					foreach (DAZMorphFormula dAZMorphFormula2 in formulas2)
					{
						if (dAZMorphFormula2.targetType == DAZMorphFormulaTargetType.MCM)
						{
							DAZMorph morph2 = GetMorph(dAZMorphFormula2.target);
							if (morph2 != null)
							{
								dAZMorph.morphValue = morph2.morphValue * dAZMorphFormula2.multiplier;
								morphValue = dAZMorph.morphValue;
							}
							else
							{
								dAZMorph.morphValue = 0f;
								morphValue = dAZMorph.morphValue;
							}
						}
						else if (dAZMorphFormula2.targetType == DAZMorphFormulaTargetType.MCMMult)
						{
							DAZMorph morph3 = GetMorph(dAZMorphFormula2.target);
							if (morph3 != null)
							{
								dAZMorph.morphValue *= morph3.morphValue;
								morphValue = dAZMorph.morphValue;
							}
							else
							{
								dAZMorph.morphValue = 0f;
								morphValue = dAZMorph.morphValue;
							}
						}
					}
				}
				if (appliedValue == morphValue)
				{
					continue;
				}
				flag2 = true;
				ApplyBoneMorphs(morphBones, dAZMorph);
				ApplyBoneMorphs(morphBones2, dAZMorph);
				if (dAZMorph.deltas.Length > 0 && connectedMesh != null && array2 != null)
				{
					flag = true;
					if (dAZMorph.visible)
					{
						visibleVerticesChanged = true;
						float num2 = morphValue - appliedValue;
						DAZMorphVertex[] deltas = dAZMorph.deltas;
						foreach (DAZMorphVertex dAZMorphVertex in deltas)
						{
							array2[dAZMorphVertex.vertex].x += dAZMorphVertex.delta.x * num2;
							array[dAZMorphVertex.vertex].x = array2[dAZMorphVertex.vertex].x;
							array3[dAZMorphVertex.vertex].x = array2[dAZMorphVertex.vertex].x;
							array2[dAZMorphVertex.vertex].y += dAZMorphVertex.delta.y * num2;
							array[dAZMorphVertex.vertex].y = array2[dAZMorphVertex.vertex].y;
							array3[dAZMorphVertex.vertex].y = array2[dAZMorphVertex.vertex].y;
							array2[dAZMorphVertex.vertex].z += dAZMorphVertex.delta.z * num2;
							array[dAZMorphVertex.vertex].z = array2[dAZMorphVertex.vertex].z;
							array3[dAZMorphVertex.vertex].z = array2[dAZMorphVertex.vertex].z;
							if (dAZMorph.triggerNormalRecalc)
							{
								connectedMesh.morphedBaseDirtyVertices[dAZMorphVertex.vertex] = true;
								connectedMesh.morphedNormalsDirty = true;
							}
							if (dAZMorph.triggerTangentRecalc)
							{
								connectedMesh.morphedUVDirtyVertices[dAZMorphVertex.vertex] = true;
								connectedMesh.morphedTangentsDirty = true;
							}
						}
					}
					else
					{
						float num3 = morphValue - appliedValue;
						DAZMorphVertex[] deltas2 = dAZMorph.deltas;
						foreach (DAZMorphVertex dAZMorphVertex2 in deltas2)
						{
							array2[dAZMorphVertex2.vertex].x += dAZMorphVertex2.delta.x * num3;
							array[dAZMorphVertex2.vertex].x = array2[dAZMorphVertex2.vertex].x;
							array2[dAZMorphVertex2.vertex].y += dAZMorphVertex2.delta.y * num3;
							array[dAZMorphVertex2.vertex].y = array2[dAZMorphVertex2.vertex].y;
							array2[dAZMorphVertex2.vertex].z += dAZMorphVertex2.delta.z * num3;
							array[dAZMorphVertex2.vertex].z = array2[dAZMorphVertex2.vertex].z;
							if (dAZMorph.triggerNormalRecalc)
							{
								connectedMesh.morphedBaseDirtyVertices[dAZMorphVertex2.vertex] = true;
								connectedMesh.morphedNormalsDirty = true;
							}
							if (dAZMorph.triggerTangentRecalc)
							{
								connectedMesh.morphedUVDirtyVertices[dAZMorphVertex2.vertex] = true;
								connectedMesh.morphedTangentsDirty = true;
							}
						}
					}
				}
				dAZMorph.appliedValue = morphValue;
			}
		}
		if (bonesDirty)
		{
			if (morphBones != null)
			{
				morphBones.SetMorphedTransform();
			}
			if (morphBones2 != null)
			{
				morphBones2.SetMorphedTransform();
			}
			bonesDirty = false;
		}
		if ((flag || force) && connectedMesh != null)
		{
			connectedMesh.ApplyMorphVertices(visibleVerticesChanged);
		}
	}

	public void ApplyMorphs(bool force = false)
	{
		if ((bool)connectedMesh && connectedMesh.wasInit)
		{
			connectedMesh.StartMorph();
			if (!Application.isPlaying || !useThreadedMorphing)
			{
				ApplyMorphsInternal(force);
			}
		}
	}

	public void ApplyMorphsImmediate()
	{
		if (Application.isPlaying && useThreadedMorphing)
		{
			while (applyMorphsTask.working)
			{
				Thread.Sleep(0);
			}
			ApplyMorphsThreadedFinish();
			ApplyMorphsThreadedStart();
			applyMorphsTask.working = true;
			applyMorphsTask.resetEvent.Set();
			while (applyMorphsTask.working)
			{
				Thread.Sleep(0);
			}
			ApplyMorphsThreadedFinish();
			ApplyMorphsThreadedStart();
			applyMorphsTask.working = true;
			applyMorphsTask.resetEvent.Set();
			visibleVerticesChanged = true;
			if (connectedMesh != null)
			{
				connectedMesh.ApplyMorphVertices(visibleVerticesChanged);
			}
		}
		else
		{
			ApplyMorphsInternal();
		}
	}

	private void Update()
	{
		if (!connectedMesh || !connectedMesh.wasInit)
		{
			return;
		}
		connectedMesh.StartMorph();
		if (Application.isPlaying && useThreadedMorphing)
		{
			StartThreads();
			totalFrameCount++;
			if (!applyMorphsTask.working)
			{
				ApplyMorphsThreadedFinish();
				ApplyMorphsThreadedStart();
				applyMorphsTask.working = true;
				applyMorphsTask.resetEvent.Set();
			}
			else if (OVRManager.isHmdPresent)
			{
				missedFrameCount++;
				Debug.LogWarning("ApplyMorphsTask did not complete in 1 frame. Missed " + missedFrameCount + " out of total " + totalFrameCount);
				DebugHUD.Msg("ApplyMorphsTask miss " + missedFrameCount + " out of total " + totalFrameCount);
				DebugHUD.Alert2();
			}
		}
		else
		{
			ApplyMorphsInternal();
		}
	}

	private void Init()
	{
		if (morphBones != null)
		{
			morphBones.Init();
		}
		if (morphBones2 != null)
		{
			morphBones2.Init();
		}
		_morphsByName = null;
		if (connectedMesh != null)
		{
			connectedMesh.Init();
			if (_threadedMorphedUVVertices == null)
			{
				_threadedMorphedUVVertices = (Vector3[])connectedMesh.UVVertices.Clone();
			}
			if (_threadedVisibleMorphedUVVertices == null)
			{
				_threadedVisibleMorphedUVVertices = (Vector3[])connectedMesh.UVVertices.Clone();
			}
		}
		ResetMorphs();
		RebuildMorphsByRegion();
		ApplyMorphs(force: true);
	}

	private void OnDisable()
	{
		ZeroAllBoneMorphs();
	}

	private void OnEnable()
	{
		Init();
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			InitMorphs();
		}
	}

	private void Start()
	{
		Init();
	}

	protected void OnApplicationQuit()
	{
		if (Application.isPlaying)
		{
			StopThreads();
		}
	}
}
