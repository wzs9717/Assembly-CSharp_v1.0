using System.Collections.Generic;
using UnityEngine;

namespace mset
{
	[RequireComponent(typeof(Sky))]
	public class SkyApplicator : MonoBehaviour
	{
		public Sky TargetSky;

		public bool TriggerIsActive = true;

		[SerializeField]
		private Bounds triggerDimensions = new Bounds(Vector3.zero, Vector3.one);

		public bool HasChanged = true;

		public SkyApplicator ParentApplicator;

		public List<SkyApplicator> Children = new List<SkyApplicator>();

		private HashSet<Renderer> AffectedRenderers = new HashSet<Renderer>();

		private Vector3 LastPosition = Vector3.zero;

		private Vector3 _center;

		public Bounds TriggerDimensions
		{
			get
			{
				return triggerDimensions;
			}
			set
			{
				HasChanged = true;
				triggerDimensions = value;
			}
		}

		private void Awake()
		{
			TargetSky = GetComponent<Sky>();
		}

		private void Start()
		{
		}

		private void OnEnable()
		{
			base.gameObject.isStatic = true;
			base.transform.root.gameObject.isStatic = true;
			LastPosition = base.transform.position;
			if (ParentApplicator == null && base.transform.parent != null && base.transform.parent.GetComponent<SkyApplicator>() != null)
			{
				ParentApplicator = base.transform.parent.GetComponent<SkyApplicator>();
			}
			if (ParentApplicator != null)
			{
				ParentApplicator.Children.Add(this);
				return;
			}
			SkyManager skyManager = SkyManager.Get();
			if (skyManager != null)
			{
				skyManager.RegisterApplicator(this);
			}
		}

		private void OnDisable()
		{
			if (ParentApplicator != null)
			{
				ParentApplicator.Children.Remove(this);
			}
			SkyManager skyManager = SkyManager.Get();
			if ((bool)skyManager)
			{
				skyManager.UnregisterApplicator(this, AffectedRenderers);
				AffectedRenderers.Clear();
			}
		}

		public void RemoveRenderer(Renderer rend)
		{
			if (AffectedRenderers.Contains(rend))
			{
				AffectedRenderers.Remove(rend);
				SkyAnchor component = rend.GetComponent<SkyAnchor>();
				if ((bool)component && component.CurrentApplicator == this)
				{
					component.CurrentApplicator = null;
				}
			}
		}

		public void AddRenderer(Renderer rend)
		{
			SkyAnchor component = rend.GetComponent<SkyAnchor>();
			if (component != null)
			{
				if (component.CurrentApplicator != null)
				{
					component.CurrentApplicator.RemoveRenderer(rend);
				}
				component.CurrentApplicator = this;
			}
			AffectedRenderers.Add(rend);
		}

		public bool ApplyInside(Renderer rend)
		{
			if (TargetSky == null || !TriggerIsActive)
			{
				return false;
			}
			SkyAnchor component = rend.gameObject.GetComponent<SkyAnchor>();
			if ((bool)component && component.BindType == SkyAnchor.AnchorBindType.TargetSky && component.AnchorSky == TargetSky)
			{
				TargetSky.Apply(rend, 0);
				component.Apply();
				return true;
			}
			foreach (SkyApplicator child in Children)
			{
				if (child.ApplyInside(rend))
				{
					return true;
				}
			}
			Vector3 center = rend.bounds.center;
			if ((bool)component)
			{
				component.GetCenter(ref center);
			}
			center = base.transform.worldToLocalMatrix.MultiplyPoint(center);
			if (TriggerDimensions.Contains(center))
			{
				TargetSky.Apply(rend, 0);
				return true;
			}
			return false;
		}

		public bool RendererInside(Renderer rend)
		{
			SkyAnchor skyAnchor = rend.gameObject.GetComponent<SkyAnchor>();
			if ((bool)skyAnchor && skyAnchor.BindType == SkyAnchor.AnchorBindType.TargetSky && skyAnchor.AnchorSky == TargetSky)
			{
				AddRenderer(rend);
				skyAnchor.Apply();
				return true;
			}
			if (!TriggerIsActive)
			{
				return false;
			}
			foreach (SkyApplicator child in Children)
			{
				if (child.RendererInside(rend))
				{
					return true;
				}
			}
			if (skyAnchor == null)
			{
				skyAnchor = rend.gameObject.AddComponent(typeof(SkyAnchor)) as SkyAnchor;
			}
			skyAnchor.GetCenter(ref _center);
			_center = base.transform.worldToLocalMatrix.MultiplyPoint(_center);
			if (TriggerDimensions.Contains(_center))
			{
				if (!AffectedRenderers.Contains(rend))
				{
					AddRenderer(rend);
					if (!skyAnchor.HasLocalSky)
					{
						skyAnchor.SnapToSky(SkyManager.Get().GlobalSky);
					}
					skyAnchor.BlendToSky(TargetSky);
				}
				return true;
			}
			RemoveRenderer(rend);
			return false;
		}

		private void LateUpdate()
		{
			if (TargetSky.Dirty)
			{
				foreach (Renderer affectedRenderer in AffectedRenderers)
				{
					if (!(affectedRenderer == null))
					{
						TargetSky.Apply(affectedRenderer, 0);
					}
				}
				TargetSky.Dirty = false;
			}
			if (base.transform.position != LastPosition)
			{
				HasChanged = true;
			}
		}
	}
}
