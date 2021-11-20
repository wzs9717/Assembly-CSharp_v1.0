using System;
using UnityEngine;

namespace mset
{
	[Serializable]
	public class SkyBlender
	{
		public Sky CurrentSky;

		public Sky PreviousSky;

		[SerializeField]
		private float blendTime = 0.25f;

		private float currentBlendTime = 0.25f;

		private float endStamp;

		public float BlendTime
		{
			get
			{
				return blendTime;
			}
			set
			{
				blendTime = value;
			}
		}

		private float blendTimer
		{
			get
			{
				return endStamp - Time.time;
			}
			set
			{
				endStamp = Time.time + value;
			}
		}

		public float BlendWeight => 1f - Mathf.Clamp01(blendTimer / currentBlendTime);

		public bool IsBlending => Time.time < endStamp;

		public bool WasBlending(float secAgo)
		{
			return Time.time - secAgo < endStamp;
		}

		public void Apply()
		{
			if (IsBlending)
			{
				Sky.EnableGlobalProjection(CurrentSky.HasDimensions || PreviousSky.HasDimensions);
				Sky.EnableGlobalBlending(enable: true);
				CurrentSky.Apply(0);
				PreviousSky.Apply(1);
				Sky.SetBlendWeight(BlendWeight);
			}
			else
			{
				Sky.EnableGlobalProjection(CurrentSky.HasDimensions);
				Sky.EnableGlobalBlending(enable: false);
				CurrentSky.Apply(0);
			}
		}

		public void Apply(Material target)
		{
			if (IsBlending)
			{
				Sky.EnableBlending(target, enable: true);
				Sky.EnableProjection(target, CurrentSky.HasDimensions || PreviousSky.HasDimensions);
				CurrentSky.Apply(target, 0);
				PreviousSky.Apply(target, 1);
				Sky.SetBlendWeight(target, BlendWeight);
			}
			else
			{
				Sky.EnableBlending(target, enable: false);
				Sky.EnableProjection(target, CurrentSky.HasDimensions);
				CurrentSky.Apply(target, 0);
			}
		}

		public void Apply(Renderer target, Material[] materials)
		{
			if (IsBlending)
			{
				Sky.EnableBlending(target, materials, enable: true);
				Sky.EnableProjection(target, materials, CurrentSky.HasDimensions || PreviousSky.HasDimensions);
				CurrentSky.ApplyFast(target, 0);
				PreviousSky.ApplyFast(target, 1);
				Sky.SetBlendWeight(target, BlendWeight);
			}
			else
			{
				Sky.EnableBlending(target, materials, enable: false);
				Sky.EnableProjection(target, materials, CurrentSky.HasDimensions);
				CurrentSky.ApplyFast(target, 0);
			}
		}

		public void ApplyToTerrain()
		{
			if (IsBlending)
			{
				Sky.EnableTerrainBlending(enable: true);
			}
			else
			{
				Sky.EnableTerrainBlending(enable: false);
			}
		}

		public void SnapToSky(Sky nusky)
		{
			if (!(nusky == null))
			{
				CurrentSky = (PreviousSky = nusky);
				blendTimer = 0f;
			}
		}

		public void BlendToSky(Sky nusky)
		{
			if (!(nusky == null) && CurrentSky != nusky)
			{
				if (CurrentSky == null)
				{
					PreviousSky = (CurrentSky = nusky);
					blendTimer = 0f;
					return;
				}
				PreviousSky = CurrentSky;
				CurrentSky = nusky;
				currentBlendTime = blendTime;
				blendTimer = currentBlendTime;
			}
		}

		public void SkipTime(float sec)
		{
			blendTimer -= sec;
		}
	}
}
