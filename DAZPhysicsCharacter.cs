using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class DAZPhysicsCharacter : MonoBehaviour
{
	public DAZPhysicsCharacter copyFrom;

	public Transform followTree;

	public Transform targets;

	public string layerPrefix = "1";

	private Dictionary<string, Transform> TransformMap;

	private Dictionary<string, Rigidbody> RBMap;

	private Dictionary<string, SetCenterOfGravity> CGMap;

	private Dictionary<string, FollowPhysicallySingle> FPMap;

	private Dictionary<string, Transform> FollowMap;

	private Dictionary<string, FreeControllerV3> TargetsMap;

	private Dictionary<string, ConfigurableJoint> JointMap;

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
			capsuleCollider.center = component.center;
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
			boxCollider.center = component.center;
			boxCollider.size = component.size;
		}
	}

	public void deleteColliders()
	{
		if (!copyFrom)
		{
			return;
		}
		foreach (string rBJointName in copyFrom.getRBJointNames())
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
			BoxCollider component2 = value.GetComponent<BoxCollider>();
			if (component2 != null)
			{
				UnityEngine.Object.DestroyImmediate(component2);
			}
			List<GameObject> list = new List<GameObject>();
			foreach (Transform item in value.transform)
			{
				Rigidbody component3 = item.GetComponent<Rigidbody>();
				if (component3 == null)
				{
					CapsuleCollider component4 = item.GetComponent<CapsuleCollider>();
					BoxCollider component5 = item.GetComponent<BoxCollider>();
					if (component4 != null || component5 != null)
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
	}

	public void copyOthers()
	{
		Blinker[] componentsInChildren = copyFrom.GetComponentsInChildren<Blinker>();
		Blinker[] array = componentsInChildren;
		foreach (Component component in array)
		{
			if (TransformMap.TryGetValue(component.name, out var value))
			{
				copyComponent(component, value.gameObject);
				Blinker component2 = value.GetComponent<Blinker>();
				component2.skin = value.GetComponent<SkinnedMeshRenderer>();
			}
		}
		LookAtWithLimits[] componentsInChildren2 = copyFrom.GetComponentsInChildren<LookAtWithLimits>();
		LookAtWithLimits[] array2 = componentsInChildren2;
		foreach (Component component3 in array2)
		{
			if (TransformMap.TryGetValue(component3.name, out var value2))
			{
				copyComponent(component3, value2.gameObject);
			}
		}
		SyncMaterialParameters[] componentsInChildren3 = copyFrom.GetComponentsInChildren<SyncMaterialParameters>();
		SyncMaterialParameters[] array3 = componentsInChildren3;
		foreach (Component component4 in array3)
		{
			if (TransformMap.TryGetValue(component4.name, out var value3))
			{
				copyComponent(component4, value3.gameObject);
			}
		}
		AdjustJointTargetAndCOG[] componentsInChildren4 = copyFrom.GetComponentsInChildren<AdjustJointTargetAndCOG>();
		AdjustJointTargetAndCOG[] array4 = componentsInChildren4;
		foreach (Component component5 in array4)
		{
			if (TransformMap.TryGetValue(component5.name, out var value4))
			{
				copyComponent(component5, value4.gameObject);
			}
		}
		BakedSkinMesh[] componentsInChildren5 = copyFrom.GetComponentsInChildren<BakedSkinMesh>();
		BakedSkinMesh[] array5 = componentsInChildren5;
		foreach (Component component6 in array5)
		{
			if (TransformMap.TryGetValue(component6.name, out var value5))
			{
				copyComponent(component6, value5.gameObject);
			}
		}
		DAZHairMesh[] componentsInChildren6 = copyFrom.GetComponentsInChildren<DAZHairMesh>();
		DAZHairMesh[] array6 = componentsInChildren6;
		foreach (Component component7 in array6)
		{
			if (TransformMap.TryGetValue(component7.name, out var value6))
			{
				copyComponent(component7, value6.gameObject);
			}
		}
		ForceReceiver[] componentsInChildren7 = copyFrom.GetComponentsInChildren<ForceReceiver>();
		ForceReceiver[] array7 = componentsInChildren7;
		foreach (Component component8 in array7)
		{
			if (TransformMap.TryGetValue(component8.name, out var value7))
			{
				copyComponent(component8, value7.gameObject);
			}
		}
	}

	public void copyRigidbody(bool useLocal = false)
	{
		if (!copyFrom)
		{
			return;
		}
		foreach (string rBJointName in copyFrom.getRBJointNames())
		{
			Rigidbody value = null;
			Rigidbody rB = copyFrom.getRB(rBJointName);
			if (!RBMap.TryGetValue(rBJointName, out value))
			{
				Transform transform = getTransform(rBJointName);
				if (transform == null)
				{
					string text = rB.transform.parent.name;
					Transform transform2 = getTransform(text);
					if (transform2 != null)
					{
						GameObject gameObject = new GameObject(rBJointName);
						gameObject.name = rBJointName;
						if (!TransformMap.ContainsKey(rBJointName))
						{
							TransformMap.Add(rBJointName, gameObject.transform);
						}
						gameObject.transform.parent = transform2;
						if (useLocal)
						{
							gameObject.transform.localPosition = rB.transform.localPosition;
							gameObject.transform.localRotation = rB.transform.localRotation;
						}
						else
						{
							gameObject.transform.position = rB.transform.position;
							gameObject.transform.rotation = rB.transform.rotation;
						}
						gameObject.transform.localScale = rB.transform.localScale;
						value = gameObject.AddComponent<Rigidbody>();
						RBMap.Add(rBJointName, value);
					}
					else
					{
						Debug.LogError("could not find parent transform " + text + " during copy");
					}
				}
				else
				{
					value = transform.gameObject.AddComponent<Rigidbody>();
					RBMap.Add(rBJointName, value);
				}
			}
			if (!(value != null) || !(rB != null))
			{
				continue;
			}
			string input = LayerMask.LayerToName(rB.gameObject.layer);
			string text2 = Regex.Replace(input, "^[0-9]", layerPrefix);
			int num = LayerMask.NameToLayer(text2);
			if (num != -1)
			{
				value.gameObject.layer = num;
			}
			else
			{
				Debug.LogError("could not find layer " + text2 + " during RB copy");
			}
			value.mass = rB.mass;
			value.drag = rB.drag;
			value.angularDrag = rB.angularDrag;
			value.useGravity = rB.useGravity;
			value.interpolation = rB.interpolation;
			value.collisionDetectionMode = rB.collisionDetectionMode;
			value.isKinematic = rB.isKinematic;
			value.constraints = rB.constraints;
			copyCapsuleCollider(rB.gameObject, value.gameObject);
			copyBoxCollider(rB.gameObject, value.gameObject);
			foreach (Transform item in rB.transform)
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
				string text3 = item.name;
				GameObject gameObject2 = null;
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
					if (!TransformMap.ContainsKey(text3))
					{
						TransformMap.Add(text3, gameObject2.transform);
					}
					gameObject2.transform.parent = value.transform;
					if (useLocal)
					{
						gameObject2.transform.localPosition = item.localPosition;
						gameObject2.transform.localRotation = item.localRotation;
					}
					else
					{
						gameObject2.transform.position = item.position;
						gameObject2.transform.rotation = item.rotation;
					}
					gameObject2.transform.localScale = item.localScale;
				}
				copyCapsuleCollider(item.gameObject, gameObject2);
				copyBoxCollider(item.gameObject, gameObject2);
				input = LayerMask.LayerToName(item.gameObject.layer);
				text2 = Regex.Replace(input, "^[0-9]", layerPrefix);
				num = LayerMask.NameToLayer(text2);
				if (num != -1)
				{
					gameObject2.layer = num;
				}
				else
				{
					Debug.LogError("could not find layer " + text2 + " during RB copy");
				}
			}
		}
	}

	public void createForceReceivers()
	{
		string[] array = new string[19]
		{
			"hip", "abdomen", "abdomen2", "chest", "lCollar", "lShldr", "lForeArm", "lHand", "rCollar", "rShldr",
			"rForeArm", "rHand", "pelvis", "lThigh", "lShin", "lFoot", "rThigh", "rShin", "rFoot"
		};
		string[] array2 = array;
		foreach (string key in array2)
		{
			if (TransformMap.TryGetValue(key, out var value))
			{
				ForceReceiver component = value.GetComponent<ForceReceiver>();
				if (component == null)
				{
					value.gameObject.AddComponent<ForceReceiver>();
				}
			}
		}
	}

	public void copyConfigurableJoint()
	{
		if (!copyFrom)
		{
			return;
		}
		foreach (string jointName in copyFrom.getJointNames())
		{
			ConfigurableJoint value = null;
			ConfigurableJoint joint = copyFrom.getJoint(jointName);
			if (!JointMap.TryGetValue(jointName, out value))
			{
				Transform transform = getTransform(jointName);
				if (transform != null)
				{
					value = transform.gameObject.AddComponent<ConfigurableJoint>();
					JointMap.Add(jointName, value);
					if ((bool)joint.connectedBody)
					{
						string text = joint.connectedBody.name;
						if (RBMap.TryGetValue(text, out var value2))
						{
							value.connectedBody = value2;
						}
						else
						{
							Debug.LogError("could not find parentRB " + text + " during copy");
						}
					}
					else
					{
						Debug.LogError("ref joint " + jointName + " doesn't have a connected body during copy");
					}
				}
				else
				{
					Debug.LogError("could not find transform " + jointName + " during copy");
				}
			}
			if (value != null && joint != null)
			{
				value.axis = joint.axis;
				value.secondaryAxis = joint.secondaryAxis;
				value.xMotion = joint.xMotion;
				value.yMotion = joint.yMotion;
				value.zMotion = joint.zMotion;
				value.angularXMotion = joint.angularXMotion;
				value.angularYMotion = joint.angularYMotion;
				value.angularZMotion = joint.angularZMotion;
				value.linearLimit = joint.linearLimit;
				value.lowAngularXLimit = joint.lowAngularXLimit;
				value.highAngularXLimit = joint.highAngularXLimit;
				value.angularYLimit = joint.angularYLimit;
				value.angularZLimit = joint.angularZLimit;
				value.targetPosition = joint.targetPosition;
				value.targetVelocity = joint.targetVelocity;
				value.xDrive = joint.xDrive;
				value.yDrive = joint.yDrive;
				value.zDrive = joint.zDrive;
				value.targetRotation = joint.targetRotation;
				value.targetAngularVelocity = joint.targetAngularVelocity;
				value.rotationDriveMode = joint.rotationDriveMode;
				value.angularXDrive = joint.angularXDrive;
				value.angularYZDrive = joint.angularYZDrive;
				value.slerpDrive = joint.slerpDrive;
				value.projectionAngle = joint.projectionAngle;
				value.projectionDistance = joint.projectionDistance;
				value.projectionMode = joint.projectionMode;
			}
		}
	}

	public void copySetCenterOfGravity()
	{
		if (!copyFrom)
		{
			return;
		}
		foreach (string cGJointName in copyFrom.getCGJointNames())
		{
			SetCenterOfGravity value = null;
			SetCenterOfGravity cG = copyFrom.getCG(cGJointName);
			if (!CGMap.TryGetValue(cGJointName, out value))
			{
				Transform transform = getTransform(cGJointName);
				if (transform != null)
				{
					value = transform.gameObject.AddComponent<SetCenterOfGravity>();
					CGMap.Add(cGJointName, value);
				}
				else
				{
					Debug.LogError("could not find transform " + cGJointName + " during copy");
				}
			}
			if (value != null && cG != null)
			{
				value.enabled = cG.enabled;
				value.liveUpdate = cG.liveUpdate;
				value.centerOfGravity = cG.centerOfGravity;
			}
		}
	}

	public void copyFollowPhysically()
	{
		if (!copyFrom)
		{
			return;
		}
		foreach (string fPJointName in copyFrom.getFPJointNames())
		{
			FollowPhysicallySingle value = null;
			FollowPhysicallySingle fP = copyFrom.getFP(fPJointName);
			if (!FPMap.TryGetValue(fPJointName, out value))
			{
				Transform transform = getTransform(fPJointName);
				if (transform != null)
				{
					value = transform.gameObject.AddComponent<FollowPhysicallySingle>();
					FPMap.Add(fPJointName, value);
				}
				else
				{
					Debug.LogError("could not find transform " + fPJointName + " during copy");
				}
			}
			if (!(value != null) || !(fP != null))
			{
				continue;
			}
			value.on = fP.on;
			value.drivePosition = fP.drivePosition;
			value.driveRotation = fP.driveRotation;
			value.useGlobalForceMultiplier = fP.useGlobalForceMultiplier;
			value.useGlobalTorqueMultiplier = fP.useGlobalTorqueMultiplier;
			if ((bool)targets && TargetsMap.TryGetValue(fPJointName, out var value2))
			{
				value.follow = value2.transform;
				value2.followWhenOff = value.transform;
				if ((bool)followTree && FollowMap.TryGetValue(fPJointName, out var value3))
				{
					value2.follow = value3;
				}
			}
			else if ((bool)followTree)
			{
				if (FollowMap.TryGetValue(fPJointName, out var value4))
				{
					value.follow = value4;
				}
			}
			else
			{
				value.follow = fP.follow;
				value2 = value.follow.GetComponent<FreeControllerV3>();
				if (value2 != null)
				{
					value2.followWhenOff = value.transform;
				}
			}
			value.moveMethod = fP.moveMethod;
			value.rotateMethod = fP.rotateMethod;
			value.PIDpresentFactorRot = fP.PIDpresentFactorRot;
			value.PIDintegralFactorRot = fP.PIDintegralFactorRot;
			value.PIDderivFactorRot = fP.PIDderivFactorRot;
			value.PIDpresentFactorPos = fP.PIDpresentFactorPos;
			value.PIDintegralFactorPos = fP.PIDintegralFactorPos;
			value.PIDderivFactorPos = fP.PIDderivFactorPos;
			value.forcePosition = fP.forcePosition;
			value.ForceMultiplier = fP.ForceMultiplier;
			value.TorqueMultiplier = fP.TorqueMultiplier;
			value.TorqueMultiplier2 = fP.TorqueMultiplier2;
			value.MaxForce = fP.MaxForce;
			value.MaxTorque = fP.MaxTorque;
			value.freezeMass = fP.freezeMass;
			value.controlledJointSpring = fP.controlledJointSpring;
			value.controlledJointMaxForce = fP.controlledJointMaxForce;
			value.debugForce = fP.debugForce;
			value.debugTorque = fP.debugTorque;
			value.lineMaterial = fP.lineMaterial;
			value.rotationLineMaterial = fP.rotationLineMaterial;
			if (fP.onIfAllFCV3sFollowing == null || !targets)
			{
				continue;
			}
			value.onIfAllFCV3sFollowing = new FreeControllerV3[fP.onIfAllFCV3sFollowing.Length];
			int num = 0;
			FreeControllerV3[] onIfAllFCV3sFollowing = fP.onIfAllFCV3sFollowing;
			foreach (FreeControllerV3 freeControllerV in onIfAllFCV3sFollowing)
			{
				if (TargetsMap.TryGetValue(freeControllerV.name, out var value5))
				{
					value.onIfAllFCV3sFollowing[num] = value5;
				}
				num++;
			}
		}
	}

	public void copy()
	{
		copyRigidbody();
		copyConfigurableJoint();
		copySetCenterOfGravity();
		copyFollowPhysically();
		copyOthers();
	}

	public ICollection<string> getAllNames()
	{
		if (TransformMap == null)
		{
			init();
		}
		return TransformMap.Keys;
	}

	public ICollection<string> getJointNames()
	{
		if (JointMap == null)
		{
			init();
		}
		return JointMap.Keys;
	}

	public ICollection<string> getRBJointNames()
	{
		if (RBMap == null)
		{
			init();
		}
		return RBMap.Keys;
	}

	public ICollection<string> getCGJointNames()
	{
		if (CGMap == null)
		{
			init();
		}
		return CGMap.Keys;
	}

	public ICollection<string> getFPJointNames()
	{
		if (FPMap == null)
		{
			init();
		}
		return FPMap.Keys;
	}

	public Transform getTransform(string trans)
	{
		if (TransformMap.TryGetValue(trans, out var value))
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

	public SetCenterOfGravity getCG(string joint)
	{
		if (CGMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	public FollowPhysicallySingle getFP(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	public ConfigurableJoint getJoint(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value;
		}
		return null;
	}

	public float getLowAngularXLimit(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.lowAngularXLimit.limit;
		}
		return 0f;
	}

	public void setLowAngularXLimit(string joint, float limit)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			SoftJointLimit lowAngularXLimit = value.lowAngularXLimit;
			lowAngularXLimit.limit = limit;
			value.lowAngularXLimit = lowAngularXLimit;
		}
	}

	public float getHighAngularXLimit(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.highAngularXLimit.limit;
		}
		return 0f;
	}

	public void setHighAngularXLimit(string joint, float limit)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			SoftJointLimit highAngularXLimit = value.highAngularXLimit;
			highAngularXLimit.limit = limit;
			value.highAngularXLimit = highAngularXLimit;
		}
	}

	public float getAngularYLimit(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularYLimit.limit;
		}
		return 0f;
	}

	public void setAngularYLimit(string joint, float limit)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			SoftJointLimit angularYLimit = value.angularYLimit;
			angularYLimit.limit = limit;
			value.angularYLimit = angularYLimit;
		}
	}

	public float getAngularZLimit(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularZLimit.limit;
		}
		return 0f;
	}

	public void setAngularZLimit(string joint, float limit)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			SoftJointLimit angularZLimit = value.angularZLimit;
			angularZLimit.limit = limit;
			value.angularZLimit = angularZLimit;
		}
	}

	public RotationDriveMode getRotationDriveMode(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.rotationDriveMode;
		}
		return RotationDriveMode.Slerp;
	}

	public void setRotationDriveMode(string joint, RotationDriveMode mode)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			value.rotationDriveMode = mode;
		}
	}

	public float getAngularXDriveSpring(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularXDrive.positionSpring;
		}
		return 0f;
	}

	public void setAngularXDriveSpring(string joint, float spring)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive angularXDrive = value.angularXDrive;
			angularXDrive.positionSpring = spring;
			value.angularXDrive = angularXDrive;
		}
	}

	public float getAngularYZDriveSpring(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularYZDrive.positionSpring;
		}
		return 0f;
	}

	public void setAngularYZDriveSpring(string joint, float spring)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive angularYZDrive = value.angularYZDrive;
			angularYZDrive.positionSpring = spring;
			value.angularYZDrive = angularYZDrive;
		}
	}

	public float getAngularXDriveDamper(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularXDrive.positionDamper;
		}
		return 0f;
	}

	public void setAngularXDriveDamper(string joint, float damper)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive angularXDrive = value.angularXDrive;
			angularXDrive.positionDamper = damper;
			value.angularXDrive = angularXDrive;
		}
	}

	public float getAngularYZDriveDamper(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularYZDrive.positionDamper;
		}
		return 0f;
	}

	public void setAngularYZDriveDamper(string joint, float damper)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive angularYZDrive = value.angularYZDrive;
			angularYZDrive.positionDamper = damper;
			value.angularYZDrive = angularYZDrive;
		}
	}

	public float getAngularXDriveMaxForce(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularXDrive.maximumForce;
		}
		return 0f;
	}

	public void setAngularXDriveMaxForce(string joint, float force)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive angularXDrive = value.angularXDrive;
			angularXDrive.maximumForce = force;
			value.angularXDrive = angularXDrive;
		}
	}

	public float getAngularYZDriveMaxForce(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.angularYZDrive.maximumForce;
		}
		return 0f;
	}

	public void setAngularYZDriveMaxForce(string joint, float force)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive angularYZDrive = value.angularYZDrive;
			angularYZDrive.maximumForce = force;
			value.angularYZDrive = angularYZDrive;
		}
	}

	public float getSlerpDriveSpring(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.slerpDrive.positionSpring;
		}
		return 0f;
	}

	public void setSlerpDriveSpring(string joint, float spring)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive slerpDrive = value.slerpDrive;
			slerpDrive.positionSpring = spring;
			value.slerpDrive = slerpDrive;
		}
	}

	public float getSlerpDriveDamper(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.slerpDrive.positionDamper;
		}
		return 0f;
	}

	public void setSlerpDriveDamper(string joint, float damper)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive slerpDrive = value.slerpDrive;
			slerpDrive.positionDamper = damper;
			value.slerpDrive = slerpDrive;
		}
	}

	public float getSlerpDriveMaxForce(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			return value.slerpDrive.maximumForce;
		}
		return 0f;
	}

	public void setSlerpDriveMaxForce(string joint, float force)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			JointDrive slerpDrive = value.slerpDrive;
			slerpDrive.maximumForce = force;
			value.slerpDrive = slerpDrive;
		}
	}

	public bool isProjectionOn(string joint)
	{
		if (JointMap.TryGetValue(joint, out var value))
		{
			if (value.projectionMode == JointProjectionMode.None)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public float getMass(string joint)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			return value.mass;
		}
		return 0f;
	}

	public void setMass(string joint, float mass)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			value.mass = mass;
		}
	}

	public float getDrag(string joint)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			return value.drag;
		}
		return 0f;
	}

	public void setDrag(string joint, float drag)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			value.drag = drag;
		}
	}

	public float getAngularDrag(string joint)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			return value.angularDrag;
		}
		return 0f;
	}

	public void setAngularDrag(string joint, float angDrag)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			value.angularDrag = angDrag;
		}
	}

	public bool getUseGravity(string joint)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			return value.useGravity;
		}
		return false;
	}

	public void setUseGravity(string joint, bool use)
	{
		if (RBMap.TryGetValue(joint, out var value))
		{
			value.useGravity = use;
		}
	}

	public bool getSetCenterOfGravity(string joint)
	{
		if (CGMap.TryGetValue(joint, out var value))
		{
			return value.enabled;
		}
		return false;
	}

	public void setSetCenterOfGravity(string joint, bool use)
	{
		if (CGMap.TryGetValue(joint, out var value))
		{
			value.enabled = use;
		}
	}

	public float getPIDpresentFactorPos(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.PIDpresentFactorPos;
		}
		return 0f;
	}

	public void setPIDpresentFactorPos(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.PIDpresentFactorPos = val;
		}
	}

	public float getPIDintegralFactorPos(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.PIDintegralFactorPos;
		}
		return 0f;
	}

	public void setPIDintegralFactorPos(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.PIDintegralFactorPos = val;
		}
	}

	public float getPIDderivFactorPos(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.PIDderivFactorPos;
		}
		return 0f;
	}

	public void setAllPIDderivFactorPosFromUISlider()
	{
	}

	public void setPIDderivFactorPos(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.PIDderivFactorPos = val;
		}
	}

	public float getPIDpresentFactorRot(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.PIDpresentFactorRot;
		}
		return 0f;
	}

	public void setPIDpresentFactorRot(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.PIDpresentFactorRot = val;
		}
	}

	public float getPIDintegralFactorRot(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.PIDintegralFactorRot;
		}
		return 0f;
	}

	public void setPIDintegralFactorRot(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.PIDintegralFactorRot = val;
		}
	}

	public float getPIDderivFactorRot(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.PIDderivFactorRot;
		}
		return 0f;
	}

	public void setPIDderivFactorRot(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.PIDderivFactorRot = val;
		}
	}

	public FollowPhysicallySingle.ForcePosition getForcePosition(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.forcePosition;
		}
		return FollowPhysicallySingle.ForcePosition.HingePoint;
	}

	public void setForcePosition(string joint, FollowPhysicallySingle.ForcePosition val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.forcePosition = val;
		}
	}

	public bool getUseGlobalForceMultiplier(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.useGlobalForceMultiplier;
		}
		return false;
	}

	public void setUseGlobalForceMultiplier(string joint, bool val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.useGlobalForceMultiplier = val;
		}
	}

	public float getForceMultiplier(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.ForceMultiplier;
		}
		return 0f;
	}

	public void setForceMultiplier(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.ForceMultiplier = val;
		}
	}

	public bool getUseGlobalTorqueMultiplier(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.useGlobalTorqueMultiplier;
		}
		return false;
	}

	public void setUseGlobalTorqueMultiplier(string joint, bool val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.useGlobalTorqueMultiplier = val;
		}
	}

	public float getTorqueMultiplier(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.TorqueMultiplier;
		}
		return 0f;
	}

	public void setTorqueMultiplier(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.TorqueMultiplier = val;
		}
	}

	public float getTorqueMultiplier2(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.TorqueMultiplier2;
		}
		return 0f;
	}

	public void setTorqueMultiplier2(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.TorqueMultiplier2 = val;
		}
	}

	public float getMaxForce(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.MaxForce;
		}
		return 0f;
	}

	public void setMaxForce(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.MaxForce = val;
		}
	}

	public float getMaxTorque(string joint)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			return value.MaxTorque;
		}
		return 0f;
	}

	public void setMaxTorque(string joint, float val)
	{
		if (FPMap.TryGetValue(joint, out var value))
		{
			value.MaxTorque = val;
		}
	}

	public void init()
	{
		TransformMap = new Dictionary<string, Transform>();
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (!TransformMap.ContainsKey(transform.name))
			{
				TransformMap.Add(transform.name, transform);
			}
		}
		RBMap = new Dictionary<string, Rigidbody>();
		Rigidbody[] componentsInChildren2 = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array2 = componentsInChildren2;
		foreach (Rigidbody rigidbody in array2)
		{
			RBMap.Add(rigidbody.name, rigidbody);
		}
		CGMap = new Dictionary<string, SetCenterOfGravity>();
		SetCenterOfGravity[] componentsInChildren3 = GetComponentsInChildren<SetCenterOfGravity>();
		SetCenterOfGravity[] array3 = componentsInChildren3;
		foreach (SetCenterOfGravity setCenterOfGravity in array3)
		{
			CGMap.Add(setCenterOfGravity.name, setCenterOfGravity);
		}
		FPMap = new Dictionary<string, FollowPhysicallySingle>();
		FollowPhysicallySingle[] componentsInChildren4 = GetComponentsInChildren<FollowPhysicallySingle>();
		FollowPhysicallySingle[] array4 = componentsInChildren4;
		foreach (FollowPhysicallySingle followPhysicallySingle in array4)
		{
			FPMap.Add(followPhysicallySingle.name, followPhysicallySingle);
		}
		JointMap = new Dictionary<string, ConfigurableJoint>();
		ConfigurableJoint[] componentsInChildren5 = GetComponentsInChildren<ConfigurableJoint>();
		ConfigurableJoint[] array5 = componentsInChildren5;
		foreach (ConfigurableJoint configurableJoint in array5)
		{
			if (!JointMap.ContainsKey(configurableJoint.name))
			{
				JointMap.Add(configurableJoint.name, configurableJoint);
			}
		}
		FollowMap = new Dictionary<string, Transform>();
		if ((bool)followTree)
		{
			Transform[] componentsInChildren6 = followTree.GetComponentsInChildren<Transform>();
			foreach (Transform transform2 in componentsInChildren6)
			{
				FollowMap.Add(transform2.name, transform2);
			}
		}
		TargetsMap = new Dictionary<string, FreeControllerV3>();
		if ((bool)targets)
		{
			FreeControllerV3[] componentsInChildren7 = targets.GetComponentsInChildren<FreeControllerV3>();
			foreach (FreeControllerV3 freeControllerV in componentsInChildren7)
			{
				TargetsMap.Add(freeControllerV.name, freeControllerV);
			}
		}
	}
}
