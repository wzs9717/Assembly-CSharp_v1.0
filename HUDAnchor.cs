using UnityEngine;

public class HUDAnchor : MonoBehaviour
{
	public enum AnchorNum
	{
		One,
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
		Eight
	}

	public AnchorNum anchorNum;

	public static HUDAnchor anchor1;

	public static HUDAnchor anchor2;

	public static HUDAnchor anchor3;

	public static HUDAnchor anchor4;

	public static HUDAnchor anchor5;

	public static HUDAnchor anchor6;

	public static HUDAnchor anchor7;

	public static HUDAnchor anchor8;

	public Transform reference;

	public Transform referenceAlt;

	public static void SetAnchorsToReference()
	{
		if (anchor1 != null)
		{
			anchor1.SetAnchorToReference();
		}
		if (anchor2 != null)
		{
			anchor2.SetAnchorToReference();
		}
		if (anchor3 != null)
		{
			anchor3.SetAnchorToReference();
		}
		if (anchor4 != null)
		{
			anchor4.SetAnchorToReference();
		}
		if (anchor5 != null)
		{
			anchor5.SetAnchorToReference();
		}
		if (anchor6 != null)
		{
			anchor6.SetAnchorToReference();
		}
		if (anchor7 != null)
		{
			anchor7.SetAnchorToReference();
		}
		if (anchor8 != null)
		{
			anchor8.SetAnchorToReference();
		}
	}

	public static void AdjustAnchorHeights(float adj)
	{
		if (anchor1 != null)
		{
			anchor1.AdjustHeight(adj);
		}
		if (anchor2 != null)
		{
			anchor2.AdjustHeight(adj);
		}
		if (anchor3 != null)
		{
			anchor3.AdjustHeight(adj);
		}
		if (anchor4 != null)
		{
			anchor4.AdjustHeight(adj);
		}
		if (anchor5 != null)
		{
			anchor5.AdjustHeight(adj);
		}
		if (anchor6 != null)
		{
			anchor6.AdjustHeight(adj);
		}
		if (anchor7 != null)
		{
			anchor7.AdjustHeight(adj);
		}
		if (anchor8 != null)
		{
			anchor8.AdjustHeight(adj);
		}
	}

	public static Transform GetAnchorTransform(AnchorNum anchorNum)
	{
		Transform result = null;
		switch (anchorNum)
		{
		case AnchorNum.One:
			if (anchor1 != null)
			{
				result = anchor1.transform;
			}
			break;
		case AnchorNum.Two:
			if (anchor2 != null)
			{
				result = anchor2.transform;
			}
			break;
		case AnchorNum.Three:
			if (anchor3 != null)
			{
				result = anchor3.transform;
			}
			break;
		case AnchorNum.Four:
			if (anchor4 != null)
			{
				result = anchor4.transform;
			}
			break;
		case AnchorNum.Five:
			if (anchor5 != null)
			{
				result = anchor5.transform;
			}
			break;
		case AnchorNum.Six:
			if (anchor6 != null)
			{
				result = anchor6.transform;
			}
			break;
		case AnchorNum.Seven:
			if (anchor7 != null)
			{
				result = anchor7.transform;
			}
			break;
		case AnchorNum.Eight:
			if (anchor8 != null)
			{
				result = anchor8.transform;
			}
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	public void SetAnchorToReference()
	{
		if (reference != null && reference.gameObject.activeInHierarchy)
		{
			base.transform.position = reference.position;
			base.transform.rotation = reference.rotation;
		}
		if (referenceAlt != null && referenceAlt.gameObject.activeInHierarchy)
		{
			base.transform.position = referenceAlt.position;
			base.transform.rotation = referenceAlt.rotation;
		}
	}

	public void AdjustHeight(float adj)
	{
		if (reference != null)
		{
			Vector3 position = base.transform.position;
			position.y += adj;
			base.transform.position = position;
		}
	}

	private void Update()
	{
		switch (anchorNum)
		{
		case AnchorNum.One:
			anchor1 = this;
			break;
		case AnchorNum.Two:
			anchor2 = this;
			break;
		case AnchorNum.Three:
			anchor3 = this;
			break;
		case AnchorNum.Four:
			anchor4 = this;
			break;
		case AnchorNum.Five:
			anchor5 = this;
			break;
		case AnchorNum.Six:
			anchor6 = this;
			break;
		case AnchorNum.Seven:
			anchor7 = this;
			break;
		case AnchorNum.Eight:
			anchor8 = this;
			break;
		}
	}
}
