using System;
using UnityEngine;

namespace PrefabEvolution
{
	[SelectionBase]
	[AddComponentMenu("")]
	public class PEPrefabScript : MonoBehaviour, ISerializationCallbackReceiver
	{
		public static class EditorBridge
		{
			public static Action<PEPrefabScript> OnValidate;

			public static Func<GameObject, string> GetAssetGuid;

			public static Func<string, GameObject> GetAssetByGuid;
		}

		private class PrefabInternalData
		{
			private readonly PEExposedProperties Properties;

			private readonly PELinkage Links;

			private readonly PEModifications Modifications;

			private readonly string ParentPrefabGUID;

			private readonly string PrefabGUID;

			public PrefabInternalData(PEPrefabScript script)
			{
				Properties = script.Properties;
				Links = script.Links;
				Modifications = script.Modifications;
				ParentPrefabGUID = script.ParentPrefabGUID;
				PrefabGUID = script.PrefabGUID;
			}

			public void Fill(PEPrefabScript script)
			{
				script.Properties = Properties;
				script.Links = Links;
				script.Modifications = Modifications;
				script.ParentPrefabGUID = ParentPrefabGUID;
				script.PrefabGUID = PrefabGUID;
			}
		}

		[HideInInspector]
		public PEExposedProperties Properties = Utils.Create<PEExposedProperties>();

		[HideInInspector]
		public PELinkage Links = Utils.Create<PELinkage>();

		[HideInInspector]
		public PEModifications Modifications;

		public string ParentPrefabGUID;

		public string PrefabGUID;

		private PrefabInternalData _prefabInternalData;

		public GameObject ParentPrefab
		{
			get
			{
				return EditorBridge.GetAssetByGuid(ParentPrefabGUID);
			}
			set
			{
				string text = EditorBridge.GetAssetGuid(value);
				if (!string.IsNullOrEmpty(text))
				{
					ParentPrefabGUID = text;
				}
			}
		}

		public GameObject Prefab
		{
			get
			{
				return EditorBridge.GetAssetByGuid(PrefabGUID);
			}
			set
			{
				string text = EditorBridge.GetAssetGuid(value);
				if (!string.IsNullOrEmpty(text))
				{
					PrefabGUID = text;
				}
			}
		}

		public event Action OnBuildModifications;

		private void OnValidate()
		{
			base.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			if (EditorBridge.OnValidate != null)
			{
				EditorBridge.OnValidate(this);
			}
		}

		public void InvokeOnBuildModifications()
		{
			if (this.OnBuildModifications != null)
			{
				this.OnBuildModifications();
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		private void ClearInternalData()
		{
			Properties = null;
			Links = null;
			Modifications = null;
			ParentPrefabGUID = null;
			PrefabGUID = "STRIPPED";
		}
	}
}
