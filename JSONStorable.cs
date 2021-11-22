using SimpleJSON;
using UnityEngine;

public class JSONStorable : MonoBehaviour
{
	public bool exclude;

	public bool onlyStoreIfActive;

	public bool needsStore;

	public string overrideId;

	public string storeId
	{ 

		
		get
		{
			if (overrideId == null || overrideId == string.Empty)
			{
				return base.name;
			}
			return overrideId;
		}
	}

	public virtual JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass["id"] = storeId;
		needsStore = false;
		return jSONClass;
	}

	public virtual void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
	}

	public virtual void LateRestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
	}

	public virtual void PreRestore()
	{
	}

	public virtual void PostRestore()
	{
	}
}
