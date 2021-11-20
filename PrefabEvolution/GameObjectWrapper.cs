using UnityEngine;

namespace PrefabEvolution
{
	public class GameObjectWrapper
	{
		public GameObject target;

		public bool m_IsActive
		{
			get
			{
				return target.activeSelf;
			}
			set
			{
				target.SetActive(value);
			}
		}

		public GameObjectWrapper(GameObject target)
		{
			this.target = target;
		}
	}
}
