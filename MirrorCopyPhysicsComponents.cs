using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class MirrorCopyPhysicsComponents : MonoBehaviour
{
	public enum InvertAxis
	{
		X,
		Y,
		Z
	}

	public InvertAxis invertAxis;

	public Transform copyFromRoot;

	private Dictionary<string, Rigidbody> copyFromRBMap;

	private Dictionary<string, Rigidbody> RBMap;

	private Dictionary<string, Transform> copyFromTransformMap;

	private Dictionary<string, Transform> transformMap;

	private Dictionary<string, ConfigurableJoint> copyFromJointMap;

	private Dictionary<string, ConfigurableJoint> jointMap;

	public string replaceRegexp = "^r";

	public string withString = "l";

	private Vector3 MirrorVector(Vector3 inVector)
	{
		Vector3 result = inVector;
		switch (invertAxis)
		{
		case InvertAxis.X:
			result.x = 0f - result.x;
			break;
		case InvertAxis.Y:
			result.y = 0f - result.y;
			break;
		case InvertAxis.Z:
			result.z = 0f - result.z;
			break;
		}
		return result;
	}

	private Quaternion MirrorQuaternion(Quaternion inQuat)
	{
		Quaternion result = inQuat;
		switch (invertAxis)
		{
		case InvertAxis.X:
			result.y = 0f - result.y;
			result.z = 0f - result.z;
			break;
		case InvertAxis.Y:
			result.x = 0f - result.x;
			result.z = 0f - result.z;
			break;
		case InvertAxis.Z:
			result.x = 0f - result.x;
			result.y = 0f - result.y;
			break;
		}
		return result;
	}

	public void init()
	{
		transformMap = new Dictionary<string, Transform>();
		Transform[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (!transformMap.ContainsKey(transform.name))
			{
				transformMap.Add(transform.name, transform);
			}
		}
		RBMap = new Dictionary<string, Rigidbody>();
		Rigidbody[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array2 = componentsInChildren2;
		foreach (Rigidbody rigidbody in array2)
		{
			RBMap.Add(rigidbody.name, rigidbody);
		}
		jointMap = new Dictionary<string, ConfigurableJoint>();
		ConfigurableJoint[] componentsInChildren3 = GetComponentsInChildren<ConfigurableJoint>();
		ConfigurableJoint[] array3 = componentsInChildren3;
		foreach (ConfigurableJoint configurableJoint in array3)
		{
			jointMap.Add(configurableJoint.name, configurableJoint);
		}
		if (!(copyFromRoot != null))
		{
			return;
		}
		copyFromTransformMap = new Dictionary<string, Transform>();
		Transform[] componentsInChildren4 = copyFromRoot.GetComponentsInChildren<Transform>();
		Transform[] array4 = componentsInChildren4;
		foreach (Transform transform2 in array4)
		{
			if (!copyFromTransformMap.ContainsKey(transform2.name))
			{
				copyFromTransformMap.Add(transform2.name, transform2);
			}
		}
		copyFromRBMap = new Dictionary<string, Rigidbody>();
		Rigidbody[] componentsInChildren5 = copyFromRoot.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array5 = componentsInChildren5;
		foreach (Rigidbody rigidbody2 in array5)
		{
			copyFromRBMap.Add(rigidbody2.name, rigidbody2);
		}
		copyFromJointMap = new Dictionary<string, ConfigurableJoint>();
		ConfigurableJoint[] componentsInChildren6 = copyFromRoot.GetComponentsInChildren<ConfigurableJoint>();
		ConfigurableJoint[] array6 = componentsInChildren6;
		foreach (ConfigurableJoint configurableJoint2 in array6)
		{
			copyFromJointMap.Add(configurableJoint2.name, configurableJoint2);
		}
	}

	public ICollection<string> getRBJointNames()
	{
		if (RBMap == null)
		{
			init();
		}
		return RBMap.Keys;
	}

	public ICollection<string> getCopyFromRBJointNames()
	{
		if (copyFromRBMap == null)
		{
			init();
		}
		if (copyFromRBMap != null)
		{
			return copyFromRBMap.Keys;
		}
		return null;
	}

	public ICollection<string> getJointNames()
	{
		if (jointMap == null)
		{
			init();
		}
		return jointMap.Keys;
	}

	public ICollection<string> getCopyFromJointNames()
	{
		if (copyFromJointMap == null)
		{
			init();
		}
		if (copyFromJointMap != null)
		{
			return copyFromJointMap.Keys;
		}
		return null;
	}

	public ConfigurableJoint getJoint(string joint)
	{
		if (jointMap == null)
		{
			init();
		}
		if (jointMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	public ConfigurableJoint getCopyFromJoint(string joint)
	{
		if (copyFromJointMap == null)
		{
			init();
		}
		if (copyFromJointMap != null && copyFromJointMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	public Transform getTransform(string trans)
	{
		if (transformMap == null)
		{
			init();
		}
		if (transformMap.TryGetValue(trans, out var value))
		{
			return value;
		}
		return null;
	}

	public Transform getCopyFromTransform(string trans)
	{
		if (copyFromTransformMap == null)
		{
			init();
		}
		if (copyFromTransformMap != null && copyFromTransformMap.TryGetValue(trans, out var value))
		{
			return value;
		}
		return null;
	}

	public Rigidbody getRB(string joint)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	public Rigidbody getCopyFromRB(string joint)
	{
		if (copyFromRBMap == null)
		{
			init();
		}
		if (copyFromRBMap != null && copyFromRBMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	private void copyCapsuleCollider(GameObject refgo, GameObject go)
	{
		CapsuleCollider component = refgo.GetComponent<CapsuleCollider>();
		CapsuleCollider capsuleCollider = go.GetComponent<CapsuleCollider>();
		if (component != null)
		{
			if (capsuleCollider == null)
			{
				capsuleCollider = go.AddComponent<CapsuleCollider>();
			}
			capsuleCollider.isTrigger = component.isTrigger;
			capsuleCollider.sharedMaterial = component.sharedMaterial;
			Vector3 vector2 = (capsuleCollider.center = MirrorVector(component.center));
			capsuleCollider.radius = component.radius;
			capsuleCollider.height = component.height;
			capsuleCollider.direction = component.direction;
		}
	}

	private void copyBoxCollider(GameObject refgo, GameObject go)
	{
		BoxCollider component = refgo.GetComponent<BoxCollider>();
		BoxCollider boxCollider = go.GetComponent<BoxCollider>();
		if (component != null)
		{
			if (boxCollider == null)
			{
				boxCollider = go.AddComponent<BoxCollider>();
			}
			boxCollider.isTrigger = component.isTrigger;
			boxCollider.sharedMaterial = component.sharedMaterial;
			Vector3 vector2 = (boxCollider.center = MirrorVector(component.center));
			boxCollider.size = component.size;
		}
	}

	public void deleteColliders()
	{
		foreach (string rBJointName in getRBJointNames())
		{
			Rigidbody value = null;
			if (!RBMap.TryGetValue(rBJointName, out value))
			{
				continue;
			}
			if (value == null)
			{
				Debug.LogError("RB " + rBJointName + " in RBMap was null");
				RBMap.Remove(rBJointName);
				continue;
			}
			CapsuleCollider component = value.GetComponent<CapsuleCollider>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			SphereCollider component2 = value.GetComponent<SphereCollider>();
			if (component2 != null)
			{
				UnityEngine.Object.DestroyImmediate(component2);
			}
			BoxCollider component3 = value.GetComponent<BoxCollider>();
			if (component3 != null)
			{
				UnityEngine.Object.DestroyImmediate(component3);
			}
			List<GameObject> list = new List<GameObject>();
			foreach (Transform item in value.transform)
			{
				Rigidbody component4 = item.GetComponent<Rigidbody>();
				if (component4 == null)
				{
					CapsuleCollider component5 = item.GetComponent<CapsuleCollider>();
					BoxCollider component6 = item.GetComponent<BoxCollider>();
					if (component5 != null || component6 != null)
					{
						list.Add(item.gameObject);
					}
				}
			}
			foreach (GameObject item2 in list)
			{
				UnityEngine.Object.DestroyImmediate(item2);
			}
		}
	}

	private void copyComponent(Component src, GameObject dest)
	{
		Type type = src.GetType();
		Component component = dest.GetComponent(type);
		if (component == null)
		{
			component = dest.AddComponent(type);
		}
		FieldInfo[] fields = type.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			fieldInfo.SetValue(component, fieldInfo.GetValue(src));
		}
		PropertyInfo[] properties = type.GetProperties();
		PropertyInfo[] array2 = properties;
		foreach (PropertyInfo propertyInfo in array2)
		{
			if (propertyInfo.CanWrite && propertyInfo.Name != "name")
			{
				propertyInfo.SetValue(component, propertyInfo.GetValue(src, null), null);
			}
		}
	}

	public void copyRigidbody()
	{
		if (!copyFromRoot)
		{
			return;
		}
		foreach (string copyFromRBJointName in getCopyFromRBJointNames())
		{
			Rigidbody value = null;
			Rigidbody copyFromRB = getCopyFromRB(copyFromRBJointName);
			string text = Regex.Replace(copyFromRBJointName, replaceRegexp, withString);
			if (!RBMap.TryGetValue(text, out value))
			{
				Transform transform = getTransform(text);
				if (transform == null)
				{
					string input = copyFromRB.transform.parent.name;
					string text2 = Regex.Replace(input, replaceRegexp, withString);
					Transform transform2 = getTransform(text2);
					if (transform2 != null)
					{
						GameObject gameObject = new GameObject(text);
						gameObject.name = text;
						if (!transformMap.ContainsKey(text))
						{
							transformMap.Add(text, gameObject.transform);
						}
						gameObject.transform.parent = transform2;
						gameObject.transform.localPosition = MirrorVector(copyFromRB.transform.localPosition);
						gameObject.transform.localRotation = copyFromRB.transform.localRotation;
						gameObject.transform.localScale = copyFromRB.transform.localScale;
						value = gameObject.AddComponent<Rigidbody>();
						RBMap.Add(text, value);
					}
					else
					{
						Debug.LogError("could not find parent transform " + text2 + " during copy");
					}
				}
				else
				{
					value = transform.gameObject.AddComponent<Rigidbody>();
					RBMap.Add(text, value);
				}
			}
			if (!(value != null) || !(copyFromRB != null))
			{
				continue;
			}
			value.gameObject.layer = copyFromRB.gameObject.layer;
			value.mass = copyFromRB.mass;
			value.drag = copyFromRB.drag;
			value.angularDrag = copyFromRB.angularDrag;
			value.useGravity = copyFromRB.useGravity;
			value.interpolation = copyFromRB.interpolation;
			value.collisionDetectionMode = copyFromRB.collisionDetectionMode;
			value.isKinematic = copyFromRB.isKinematic;
			value.constraints = copyFromRB.constraints;
			copyCapsuleCollider(copyFromRB.gameObject, value.gameObject);
			copyBoxCollider(copyFromRB.gameObject, value.gameObject);
			foreach (Transform item in copyFromRB.transform)
			{
				Rigidbody component = item.GetComponent<Rigidbody>();
				if (!(component == null))
				{
					continue;
				}
				CapsuleCollider component2 = item.GetComponent<CapsuleCollider>();
				BoxCollider component3 = item.GetComponent<BoxCollider>();
				if (!(component2 != null) && !(component3 != null))
				{
					continue;
				}
				string input2 = item.name;
				GameObject gameObject2 = null;
				string text3 = Regex.Replace(input2, replaceRegexp, withString);
				foreach (Transform item2 in value.transform)
				{
					if (item2.name == text3)
					{
						gameObject2 = item2.gameObject;
					}
				}
				if (gameObject2 == null)
				{
					gameObject2 = new GameObject(text3);
					gameObject2.name = text3;
					if (!transformMap.ContainsKey(text3))
					{
						transformMap.Add(text3, gameObject2.transform);
					}
					gameObject2.transform.parent = value.transform;
					gameObject2.transform.localPosition = MirrorVector(item.localPosition);
					gameObject2.transform.localRotation = MirrorQuaternion(item.localRotation);
					gameObject2.transform.localScale = item.localScale;
				}
				copyCapsuleCollider(item.gameObject, gameObject2);
				copyBoxCollider(item.gameObject, gameObject2);
				gameObject2.layer = item.gameObject.layer;
			}
		}
	}

	public void copyConfigurableJoint()
	{
		if (!copyFromRoot)
		{
			return;
		}
		foreach (string copyFromJointName in getCopyFromJointNames())
		{
			ConfigurableJoint value = null;
			ConfigurableJoint copyFromJoint = getCopyFromJoint(copyFromJointName);
			string text = Regex.Replace(copyFromJointName, replaceRegexp, withString);
			if (!jointMap.TryGetValue(text, out value))
			{
				Transform transform = getTransform(text);
				if (transform != null)
				{
					value = transform.gameObject.AddComponent<ConfigurableJoint>();
					jointMap.Add(text, value);
					if ((bool)copyFromJoint.connectedBody)
					{
						string input = copyFromJoint.connectedBody.name;
						string text2 = Regex.Replace(input, replaceRegexp, withString);
						if (RBMap.TryGetValue(text2, out var value2))
						{
							value.connectedBody = value2;
						}
						else
						{
							Debug.LogError("could not find parentRB " + text2 + " during copy");
						}
					}
					else
					{
						Debug.LogError("ref joint " + text + " doesn't have a connected body during copy");
					}
				}
				else
				{
					Debug.LogError("could not find transform " + text + " during copy");
				}
			}
			if (value != null && copyFromJoint != null)
			{
				value.axis = copyFromJoint.axis;
				value.secondaryAxis = copyFromJoint.secondaryAxis;
				value.xMotion = copyFromJoint.xMotion;
				value.yMotion = copyFromJoint.yMotion;
				value.zMotion = copyFromJoint.zMotion;
				value.angularXMotion = copyFromJoint.angularXMotion;
				value.angularYMotion = copyFromJoint.angularYMotion;
				value.angularZMotion = copyFromJoint.angularZMotion;
				value.linearLimit = copyFromJoint.linearLimit;
				if ((value.axis.x == 1f && invertAxis != 0) || (value.axis.y == 1f && invertAxis != InvertAxis.Y) || (value.axis.z == 1f && invertAxis != InvertAxis.Z))
				{
					SoftJointLimit lowAngularXLimit = copyFromJoint.lowAngularXLimit;
					lowAngularXLimit.limit = 0f - lowAngularXLimit.limit;
					value.highAngularXLimit = lowAngularXLimit;
					lowAngularXLimit = copyFromJoint.highAngularXLimit;
					lowAngularXLimit.limit = 0f - lowAngularXLimit.limit;
					value.lowAngularXLimit = lowAngularXLimit;
				}
				else
				{
					value.lowAngularXLimit = copyFromJoint.lowAngularXLimit;
					value.highAngularXLimit = copyFromJoint.highAngularXLimit;
				}
				value.angularYLimit = copyFromJoint.angularYLimit;
				value.angularZLimit = copyFromJoint.angularZLimit;
				value.targetPosition = copyFromJoint.targetPosition;
				value.targetVelocity = copyFromJoint.targetVelocity;
				value.xDrive = copyFromJoint.xDrive;
				value.yDrive = copyFromJoint.yDrive;
				value.zDrive = copyFromJoint.zDrive;
				value.targetRotation = copyFromJoint.targetRotation;
				value.targetAngularVelocity = copyFromJoint.targetAngularVelocity;
				value.rotationDriveMode = copyFromJoint.rotationDriveMode;
				value.angularXDrive = copyFromJoint.angularXDrive;
				value.angularYZDrive = copyFromJoint.angularYZDrive;
				value.slerpDrive = copyFromJoint.slerpDrive;
				value.projectionAngle = copyFromJoint.projectionAngle;
				value.projectionDistance = copyFromJoint.projectionDistance;
				value.projectionMode = copyFromJoint.projectionMode;
			}
		}
	}

	public void copyOthers()
	{
		List<Component> list = new List<Component>();
		IgnoreChildColliders[] componentsInChildren = copyFromRoot.GetComponentsInChildren<IgnoreChildColliders>();
		IgnoreChildColliders[] array = componentsInChildren;
		foreach (Component item in array)
		{
			list.Add(item);
		}
		AdjustJointSpring[] componentsInChildren2 = copyFromRoot.GetComponentsInChildren<AdjustJointSpring>();
		AdjustJointSpring[] array2 = componentsInChildren2;
		foreach (Component item2 in array2)
		{
			list.Add(item2);
		}
		AdjustJointSprings[] componentsInChildren3 = copyFromRoot.GetComponentsInChildren<AdjustJointSprings>();
		AdjustJointSprings[] array3 = componentsInChildren3;
		foreach (Component item3 in array3)
		{
			list.Add(item3);
		}
		AdjustJointTarget[] componentsInChildren4 = copyFromRoot.GetComponentsInChildren<AdjustJointTarget>();
		AdjustJointTarget[] array4 = componentsInChildren4;
		foreach (Component item4 in array4)
		{
			list.Add(item4);
		}
		AdjustJointTargets[] componentsInChildren5 = copyFromRoot.GetComponentsInChildren<AdjustJointTargets>();
		AdjustJointTargets[] array5 = componentsInChildren5;
		foreach (Component item5 in array5)
		{
			list.Add(item5);
		}
		ForceReceiver[] componentsInChildren6 = copyFromRoot.GetComponentsInChildren<ForceReceiver>();
		ForceReceiver[] array6 = componentsInChildren6;
		foreach (Component item6 in array6)
		{
			list.Add(item6);
		}
		foreach (Component item7 in list)
		{
			string input = item7.name;
			string key = Regex.Replace(input, replaceRegexp, withString);
			if (transformMap.TryGetValue(key, out var value))
			{
				copyComponent(item7, value.gameObject);
			}
		}
	}

	public void copy()
	{
		copyRigidbody();
		copyConfigurableJoint();
		copyOthers();
	}
}
