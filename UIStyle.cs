using System.Collections.Generic;
using UnityEngine;

public abstract class UIStyle : MonoBehaviour
{
	public string styleName;

	public abstract List<Object> GetControlledObjects();

	public abstract List<Object> UpdateStyle();
}
