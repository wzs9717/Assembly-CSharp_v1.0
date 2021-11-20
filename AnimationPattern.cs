using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPattern : CubicBezierCurve
{
	public Transform animatedTransform;

	public Toggle onToggle;

	[SerializeField]
	protected bool _on = true;

	public Atom containingAtom;

	public Slider speedSlider;

	public Slider speedSliderAlt;

	[SerializeField]
	protected float _speed = 1f;

	[SerializeField]
	protected AnimationStep[] _steps;

	public Atom animationStepPrefab;

	public LineDrawer rootLineDrawer;

	public Material rootLineDrawerMaterial;

	protected float stepTimer;

	protected int stepIndex;

	protected float lastTime;

	public bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on == value)
			{
				return;
			}
			_on = value;
			if (onToggle != null)
			{
				onToggle.isOn = _on;
			}
			if (animatedTransform != null)
			{
				MoveProducer component = animatedTransform.GetComponent<MoveProducer>();
				if (component != null)
				{
					component.on = _on;
				}
			}
		}
	}

	public string uid
	{
		get
		{
			if (containingAtom != null)
			{
				return containingAtom.uid;
			}
			return null;
		}
	}

	public float speed
	{
		get
		{
			return _speed;
		}
		set
		{
			if (_speed != value)
			{
				_speed = value;
				if (speedSlider != null)
				{
					speedSlider.value = _speed;
				}
				if (speedSliderAlt != null)
				{
					speedSliderAlt.value = _speed;
				}
			}
		}
	}

	public AnimationStep[] steps
	{
		get
		{
			return _steps;
		}
		set
		{
			_steps = value;
			base.points = value;
			ResetAnimation();
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (!on)
			{
				needsStore = true;
				jSON["on"].AsBool = on;
			}
			if (speedSlider != null)
			{
				SliderControl component = speedSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != speed)
				{
					needsStore = true;
					jSON["speed"].AsFloat = speed;
				}
			}
			if (steps != null)
			{
				needsStore = true;
				JSONArray jSONArray = new JSONArray();
				for (int i = 0; i < steps.Length; i++)
				{
					jSONArray[i] = steps[i].containingAtom.uid;
				}
				jSON["steps"] = jSONArray;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restorePhysical)
		{
			return;
		}
		if (jc["on"] != null)
		{
			on = jc["on"].AsBool;
		}
		else
		{
			on = true;
		}
		if (jc["speed"] != null)
		{
			speed = jc["speed"].AsFloat;
		}
		else if (speedSlider != null)
		{
			SliderControl component = speedSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				speed = component.defaultValue;
			}
		}
		if (jc["steps"] != null)
		{
			JSONArray asArray = jc["steps"].AsArray;
			steps = new AnimationStep[asArray.Count];
			for (int i = 0; i < asArray.Count; i++)
			{
				Atom atomByUid = SuperController.singleton.GetAtomByUid(asArray[i]);
				if (atomByUid != null)
				{
					if (atomByUid.animationSteps != null)
					{
						AnimationStep animationStep = atomByUid.animationSteps[0];
						steps[i] = animationStep;
						animationStep.animationParent = this;
					}
					else
					{
						Debug.LogError(string.Concat("Atom ", asArray[i], " does not contain an AnimationStep component"));
					}
				}
				else
				{
					Debug.LogError(string.Concat("Atom ", asArray[i], " referenced by animation pattern ", uid, " does not exist"));
				}
			}
		}
		else
		{
			steps = new AnimationStep[0];
		}
	}

	protected void DrawRootLine()
	{
		if (rootLineDrawer != null && _draw && _steps.Length > 0)
		{
			rootLineDrawer.SetLinePoints(base.transform.position, _steps[0].transform.position);
			rootLineDrawer.Draw(base.gameObject.layer);
		}
	}

	public AnimationStep CreateStepAtPosition(int position)
	{
		if (animationStepPrefab != null)
		{
			Transform transform = SuperController.singleton.AddAtom(animationStepPrefab);
			AnimationStep componentInChildren = transform.GetComponentInChildren<AnimationStep>();
			Atom component = transform.GetComponent<Atom>();
			if (containingAtom != null && component != null)
			{
				component.parentAtom = containingAtom;
			}
			else
			{
				transform.SetParent(base.transform, worldPositionStays: true);
			}
			transform.position = base.transform.position;
			transform.rotation = base.transform.rotation;
			if (_steps.Length >= 2)
			{
				if (position == 0)
				{
					if (base.loop)
					{
						componentInChildren.point.position = GetPositionFromPoint(_steps.Length - 1, 0.5f);
						componentInChildren.point.rotation = GetRotationFromPoint(_steps.Length - 1, 0.5f);
					}
					else
					{
						Vector3 vector = default(Vector3);
						if (_steps.Length > 1)
						{
							vector = _steps[0].point.position - _steps[1].point.position;
						}
						else
						{
							vector.x = 0f;
							vector.y = 0f;
							vector.z = 0f;
						}
						componentInChildren.point.position = _steps[0].point.position + vector;
						componentInChildren.point.rotation = _steps[0].point.rotation;
					}
				}
				else if (position >= _steps.Length)
				{
					if (base.loop)
					{
						componentInChildren.point.position = GetPositionFromPoint(_steps.Length - 1, 0.5f);
						componentInChildren.point.rotation = GetRotationFromPoint(_steps.Length - 1, 0.5f);
					}
					else
					{
						Vector3 vector2 = default(Vector3);
						if (_steps.Length > 1)
						{
							vector2 = _steps[_steps.Length - 1].point.position - _steps[_steps.Length - 2].point.position;
						}
						else
						{
							vector2.x = 0f;
							vector2.y = 0f;
							vector2.z = 0f;
						}
						componentInChildren.point.position = _steps[_steps.Length - 1].point.position + vector2;
						componentInChildren.point.rotation = _steps[_steps.Length - 1].point.rotation;
					}
				}
				else
				{
					componentInChildren.point.position = GetPositionFromPoint(position, 0.5f);
					componentInChildren.point.rotation = GetRotationFromPoint(position, 0.5f);
				}
			}
			else if (_steps.Length == 1)
			{
				Vector3 vector3 = default(Vector3);
				vector3.x = 0.1f;
				vector3.y = 0f;
				vector3.z = 0f;
				componentInChildren.point.position = transform.position + vector3;
				componentInChildren.point.rotation = transform.rotation;
			}
			else
			{
				Vector3 vector4 = default(Vector3);
				vector4.x = 0.1f;
				vector4.y = 0f;
				vector4.z = 0f;
				componentInChildren.point.position = transform.position - vector4;
				componentInChildren.point.rotation = transform.rotation;
			}
			componentInChildren.animationParent = this;
			AddStepAtPosition(componentInChildren, position);
			return componentInChildren;
		}
		return null;
	}

	public AnimationStep CreateStepBeforeStep(AnimationStep step)
	{
		int num = 0;
		AnimationStep[] array = _steps;
		foreach (AnimationStep animationStep in array)
		{
			if (animationStep == step)
			{
				break;
			}
			num++;
		}
		return CreateStepAtPosition(num);
	}

	public AnimationStep CreateStepAfterStep(AnimationStep step)
	{
		int num = 0;
		AnimationStep[] array = _steps;
		foreach (AnimationStep animationStep in array)
		{
			if (animationStep == step)
			{
				num++;
				break;
			}
			num++;
		}
		return CreateStepAtPosition(num);
	}

	public void CreateStepAtEnd()
	{
		CreateStepAtPosition(_steps.Length);
	}

	public void DestroyStep(AnimationStep step)
	{
		RemoveStep(step);
		if (Application.isPlaying)
		{
			if (step.containingAtom != null)
			{
				SuperController.singleton.RemoveAtom(step.containingAtom);
			}
			else
			{
				Object.DestroyImmediate(step.gameObject);
			}
		}
		else
		{
			Object.DestroyImmediate(step.gameObject);
		}
	}

	public void AddStepAtPosition(AnimationStep step, int position)
	{
		List<AnimationStep> list = new List<AnimationStep>();
		int num = 0;
		bool flag = false;
		AnimationStep[] array = _steps;
		foreach (AnimationStep item in array)
		{
			if (num == position)
			{
				flag = true;
				list.Add(step);
			}
			list.Add(item);
			num++;
		}
		if (!flag)
		{
			list.Add(step);
		}
		steps = list.ToArray();
		SyncStepPositionsNames();
	}

	public void SyncStepPositionsNames()
	{
		int num = 1;
		AnimationStep[] array = _steps;
		foreach (AnimationStep animationStep in array)
		{
			animationStep.stepNumber = num;
			num++;
		}
	}

	public void AddStepAtEnd(AnimationStep step)
	{
		AddStepAtPosition(step, _steps.Length);
	}

	public void RemoveStep(AnimationStep step)
	{
		List<AnimationStep> list = new List<AnimationStep>();
		AnimationStep[] array = _steps;
		foreach (AnimationStep animationStep in array)
		{
			if (animationStep != step)
			{
				list.Add(animationStep);
			}
		}
		steps = list.ToArray();
		SyncStepPositionsNames();
	}

	public void ResetAnimation()
	{
		stepTimer = 0f;
		stepIndex = 0;
	}

	protected void SetCurrentPositionAndRotation()
	{
		if (!_on || ((bool)SuperController.singleton && SuperController.singleton.freezeAnimation))
		{
			return;
		}
		int num = stepIndex + 1;
		bool flag = false;
		if (num == _steps.Length)
		{
			if (base.loop)
			{
				num = 0;
			}
			else
			{
				flag = true;
			}
		}
		if (flag || _steps == null || _steps.Length == 0)
		{
			return;
		}
		if (stepTimer >= _steps[num].transitionToTime)
		{
			stepTimer -= _steps[num].transitionToTime;
			stepIndex = num;
			num = stepIndex + 1;
			if (num == _steps.Length)
			{
				if (base.loop)
				{
					num = 0;
				}
				else
				{
					flag = true;
				}
			}
		}
		if (animatedTransform != null)
		{
			if (flag)
			{
				animatedTransform.position = _steps[stepIndex].point.position;
				animatedTransform.rotation = _steps[stepIndex].point.rotation;
				return;
			}
			float time = Mathf.Clamp01(stepTimer / _steps[num].transitionToTime);
			float t = _steps[stepIndex].curve.Evaluate(time);
			animatedTransform.position = GetPositionFromPoint(stepIndex, t);
			animatedTransform.rotation = GetRotationFromPoint(stepIndex, t);
		}
	}

	protected override void InitUI()
	{
		base.InitUI();
		if (onToggle != null)
		{
			onToggle.onValueChanged.AddListener(delegate
			{
				on = onToggle.isOn;
			});
		}
		if (speedSlider != null)
		{
			speedSlider.onValueChanged.AddListener(delegate
			{
				speed = speedSlider.value;
			});
		}
		if (speedSliderAlt != null)
		{
			speedSliderAlt.onValueChanged.AddListener(delegate
			{
				speed = speedSliderAlt.value;
			});
		}
		if (rootLineDrawerMaterial != null)
		{
			rootLineDrawer = new LineDrawer(rootLineDrawerMaterial);
		}
	}

	protected void FixedUpdate()
	{
		if (!SuperController.singleton || !SuperController.singleton.freezeAnimation)
		{
			float fixedTime = Time.fixedTime;
			if (lastTime == 0f)
			{
				lastTime = fixedTime;
			}
			float num = fixedTime - lastTime;
			stepTimer += num * speed;
			lastTime = fixedTime;
		}
		SetCurrentPositionAndRotation();
	}

	protected new void Update()
	{
		base.Update();
		if (!SuperController.singleton || !SuperController.singleton.freezeAnimation)
		{
			float time = Time.time;
			if (lastTime == 0f)
			{
				lastTime = time;
			}
			float num = time - lastTime;
			stepTimer += num * speed;
			lastTime = time;
		}
		SetCurrentPositionAndRotation();
		DrawRootLine();
	}
}
