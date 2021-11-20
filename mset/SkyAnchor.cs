using System;
using UnityEngine;

namespace mset
{
	public class SkyAnchor : MonoBehaviour
	{
		public enum AnchorBindType
		{
			Center,
			Offset,
			TargetTransform,
			TargetSky
		}

		public AnchorBindType BindType;

		public Transform AnchorTransform;

		public Vector3 AnchorOffset = Vector3.zero;

		public Sky AnchorSky;

		public Vector3 CachedCenter = Vector3.zero;

		public SkyApplicator CurrentApplicator;

		private bool isStatic;

		public bool HasLocalSky;

		public bool HasChanged = true;

		[SerializeField]
		private SkyBlender Blender = new SkyBlender();

		private Vector3 LastPosition = Vector3.zero;

		[NonSerialized]
		public Material[] materials;

		private bool firstFrame;

		public Sky CurrentSky => Blender.CurrentSky;

		public Sky PreviousSky => Blender.PreviousSky;

		public float BlendTime
		{
			get
			{
				return Blender.BlendTime;
			}
			set
			{
				Blender.BlendTime = value;
			}
		}

		public bool IsStatic => isStatic;

		private void Start()
		{
			if (BindType != AnchorBindType.TargetSky)
			{
				GetComponent<Renderer>().SetPropertyBlock(new MaterialPropertyBlock());
				SkyManager skyManager = SkyManager.Get();
				skyManager.RegisterNewRenderer(GetComponent<Renderer>());
				skyManager.ApplyCorrectSky(GetComponent<Renderer>());
				BlendTime = skyManager.LocalBlendTime;
				if ((bool)Blender.CurrentSky)
				{
					Blender.SnapToSky(Blender.CurrentSky);
				}
				else
				{
					Blender.SnapToSky(skyManager.GlobalSky);
				}
			}
			materials = GetComponent<Renderer>().materials;
			LastPosition = base.transform.position;
			HasChanged = true;
		}

		private void OnEnable()
		{
			isStatic = base.gameObject.isStatic;
			ComputeCenter(ref CachedCenter);
			firstFrame = true;
		}

		private void LateUpdate()
		{
			if (BindType == AnchorBindType.TargetSky)
			{
				HasChanged = AnchorSky != Blender.CurrentSky;
				if (AnchorSky != null)
				{
					CachedCenter = AnchorSky.transform.position;
				}
			}
			else if (BindType == AnchorBindType.TargetTransform)
			{
				if ((bool)AnchorTransform && (AnchorTransform.position.x != LastPosition.x || AnchorTransform.position.y != LastPosition.y || AnchorTransform.position.z != LastPosition.z))
				{
					HasChanged = true;
					LastPosition = AnchorTransform.position;
					CachedCenter.x = LastPosition.x;
					CachedCenter.y = LastPosition.y;
					CachedCenter.z = LastPosition.z;
				}
			}
			else if (!isStatic)
			{
				if (LastPosition.x != base.transform.position.x || LastPosition.y != base.transform.position.y || LastPosition.z != base.transform.position.z)
				{
					HasChanged = true;
					LastPosition = base.transform.position;
					ComputeCenter(ref CachedCenter);
				}
			}
			else
			{
				HasChanged = false;
			}
			HasChanged |= firstFrame;
			firstFrame = false;
			if (Blender.IsBlending || Blender.WasBlending(Time.deltaTime))
			{
				Apply();
			}
			else if (BindType == AnchorBindType.TargetSky)
			{
				if (HasChanged || Blender.CurrentSky.Dirty)
				{
					Apply();
				}
			}
			else if (HasLocalSky && (HasChanged || Blender.CurrentSky.Dirty))
			{
				Apply();
			}
		}

		public void UpdateMaterials()
		{
			materials = GetComponent<Renderer>().materials;
		}

		public void CleanUpMaterials()
		{
			if (materials != null)
			{
				Material[] array = materials;
				foreach (Material obj in array)
				{
					UnityEngine.Object.Destroy(obj);
				}
				materials = new Material[0];
			}
		}

		public void SnapToSky(Sky nusky)
		{
			if (!(nusky == null) && BindType != AnchorBindType.TargetSky)
			{
				Blender.SnapToSky(nusky);
				HasLocalSky = true;
			}
		}

		public void BlendToSky(Sky nusky)
		{
			if (!(nusky == null) && BindType != AnchorBindType.TargetSky)
			{
				Blender.BlendToSky(nusky);
				HasLocalSky = true;
			}
		}

		public void SnapToGlobalSky(Sky nusky)
		{
			SnapToSky(nusky);
			HasLocalSky = false;
		}

		public void BlendToGlobalSky(Sky nusky)
		{
			if (HasLocalSky)
			{
				BlendToSky(nusky);
			}
			HasLocalSky = false;
		}

		public void Apply()
		{
			if (BindType == AnchorBindType.TargetSky)
			{
				if ((bool)AnchorSky)
				{
					Blender.SnapToSky(AnchorSky);
				}
				else
				{
					Blender.SnapToSky(SkyManager.Get().GlobalSky);
				}
			}
			Blender.Apply(GetComponent<Renderer>(), materials);
		}

		public void GetCenter(ref Vector3 _center)
		{
			_center.x = CachedCenter.x;
			_center.y = CachedCenter.y;
			_center.z = CachedCenter.z;
		}

		private void ComputeCenter(ref Vector3 _center)
		{
			_center.x = base.transform.position.x;
			_center.y = base.transform.position.y;
			_center.z = base.transform.position.z;
			switch (BindType)
			{
			case AnchorBindType.TargetTransform:
				if ((bool)AnchorTransform)
				{
					_center.x = AnchorTransform.position.x;
					_center.y = AnchorTransform.position.y;
					_center.z = AnchorTransform.position.z;
				}
				break;
			case AnchorBindType.Center:
				_center.x = GetComponent<Renderer>().bounds.center.x;
				_center.y = GetComponent<Renderer>().bounds.center.y;
				_center.z = GetComponent<Renderer>().bounds.center.z;
				break;
			case AnchorBindType.Offset:
			{
				Vector3 vector = base.transform.localToWorldMatrix.MultiplyPoint3x4(AnchorOffset);
				_center.x = vector.x;
				_center.y = vector.y;
				_center.z = vector.z;
				break;
			}
			case AnchorBindType.TargetSky:
				if ((bool)AnchorSky)
				{
					_center.x = AnchorSky.transform.position.x;
					_center.y = AnchorSky.transform.position.y;
					_center.z = AnchorSky.transform.position.z;
				}
				break;
			}
		}

		private void OnDestroy()
		{
			CleanUpMaterials();
		}

		private void OnApplicationQuit()
		{
			CleanUpMaterials();
		}
	}
}
