using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;

[RequireComponent(typeof(Camera))]
public class SteamVR_Camera : MonoBehaviour
{
	[SerializeField]
	private Transform _head;

	[SerializeField]
	private Transform _ears;

	public bool wireframe;

	private static Hashtable values;

	private const string eyeSuffix = " (eye)";

	private const string earsSuffix = " (ears)";

	private const string headSuffix = " (head)";

	private const string originSuffix = " (origin)";

	public Transform head => _head;

	public Transform offset => _head;

	public Transform origin => _head.parent;

	public Camera camera { get; private set; }

	public Transform ears => _ears;

	public static float sceneResolutionScale
	{
		get
		{
			return VRSettings.get_renderScale();
		}
		set
		{
			VRSettings.set_renderScale(value);
		}
	}

	public string baseName => (!base.name.EndsWith(" (eye)")) ? base.name : base.name.Substring(0, base.name.Length - " (eye)".Length);

	public Ray GetRay()
	{
		return new Ray(_head.position, _head.forward);
	}

	private void OnDisable()
	{
		SteamVR_Render.Remove(this);
	}

	private void OnEnable()
	{
		SteamVR instance = SteamVR.instance;
		if (instance == null)
		{
			if (head != null)
			{
				head.GetComponent<SteamVR_TrackedObject>().enabled = false;
			}
			base.enabled = false;
			return;
		}
		Transform transform = base.transform;
		if (head != transform)
		{
			Expand();
			transform.parent = origin;
			while (head.childCount > 0)
			{
				head.GetChild(0).parent = transform;
			}
			head.parent = transform;
			head.localPosition = Vector3.zero;
			head.localRotation = Quaternion.identity;
			head.localScale = Vector3.one;
			head.gameObject.SetActive(value: false);
			_head = transform;
		}
		if (ears == null)
		{
			SteamVR_Ears componentInChildren = base.transform.GetComponentInChildren<SteamVR_Ears>();
			if (componentInChildren != null)
			{
				_ears = componentInChildren.transform;
			}
		}
		if (ears != null)
		{
			ears.GetComponent<SteamVR_Ears>().vrcam = this;
		}
		SteamVR_Render.Add(this);
	}

	private void Awake()
	{
		camera = GetComponent<Camera>();
		ForceLast();
	}

	public void ForceLast()
	{
		if (values != null)
		{
			foreach (DictionaryEntry value in values)
			{
				FieldInfo fieldInfo = value.Key as FieldInfo;
				fieldInfo.SetValue(this, value.Value);
			}
			values = null;
			return;
		}
		Component[] components = GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			SteamVR_Camera steamVR_Camera = components[i] as SteamVR_Camera;
			if (steamVR_Camera != null && steamVR_Camera != this)
			{
				Object.DestroyImmediate(steamVR_Camera);
			}
		}
		components = GetComponents<Component>();
		if (!(this != components[components.Length - 1]))
		{
			return;
		}
		values = new Hashtable();
		FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo2 in array)
		{
			if (fieldInfo2.IsPublic || fieldInfo2.IsDefined(typeof(SerializeField), inherit: true))
			{
				values[fieldInfo2] = fieldInfo2.GetValue(this);
			}
		}
		GameObject gameObject = base.gameObject;
		Object.DestroyImmediate(this);
		gameObject.AddComponent<SteamVR_Camera>().ForceLast();
	}

	public void Expand()
	{
		Transform parent = base.transform.parent;
		if (parent == null)
		{
			parent = new GameObject(base.name + " (origin)").transform;
			parent.localPosition = base.transform.localPosition;
			parent.localRotation = base.transform.localRotation;
			parent.localScale = base.transform.localScale;
		}
		if (head == null)
		{
			_head = new GameObject(base.name + " (head)", typeof(SteamVR_TrackedObject)).transform;
			head.parent = parent;
			head.position = base.transform.position;
			head.rotation = base.transform.rotation;
			head.localScale = Vector3.one;
			head.tag = base.tag;
		}
		if (base.transform.parent != head)
		{
			base.transform.parent = head;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			while (base.transform.childCount > 0)
			{
				base.transform.GetChild(0).parent = head;
			}
			GUILayer component = GetComponent<GUILayer>();
			if (component != null)
			{
				Object.DestroyImmediate(component);
				head.gameObject.AddComponent<GUILayer>();
			}
			AudioListener component2 = GetComponent<AudioListener>();
			if (component2 != null)
			{
				Object.DestroyImmediate(component2);
				_ears = new GameObject(base.name + " (ears)", typeof(SteamVR_Ears)).transform;
				ears.parent = _head;
				ears.localPosition = Vector3.zero;
				ears.localRotation = Quaternion.identity;
				ears.localScale = Vector3.one;
			}
		}
		if (!base.name.EndsWith(" (eye)"))
		{
			base.name += " (eye)";
		}
	}

	public void Collapse()
	{
		base.transform.parent = null;
		while (head.childCount > 0)
		{
			head.GetChild(0).parent = base.transform;
		}
		GUILayer component = head.GetComponent<GUILayer>();
		if (component != null)
		{
			Object.DestroyImmediate(component);
			base.gameObject.AddComponent<GUILayer>();
		}
		if (ears != null)
		{
			while (ears.childCount > 0)
			{
				ears.GetChild(0).parent = base.transform;
			}
			Object.DestroyImmediate(ears.gameObject);
			_ears = null;
			base.gameObject.AddComponent(typeof(AudioListener));
		}
		if (origin != null)
		{
			if (origin.name.EndsWith(" (origin)"))
			{
				Transform transform = origin;
				while (transform.childCount > 0)
				{
					transform.GetChild(0).parent = transform.parent;
				}
				Object.DestroyImmediate(transform.gameObject);
			}
			else
			{
				base.transform.parent = origin;
			}
		}
		Object.DestroyImmediate(head.gameObject);
		_head = null;
		if (base.name.EndsWith(" (eye)"))
		{
			base.name = base.name.Substring(0, base.name.Length - " (eye)".Length);
		}
	}
}
