using System;
using SimpleJSON;
using UnityEngine;

public class MaskControl : JSONStorable
{
	public enum MaskToShow
	{
		mask1,
		mask2,
		mask3,
		mask4
	}

	public Camera cameraToControl;

	public Camera cameraToControl2;

	public LayerMask mask1;

	public LayerMask mask2;

	public LayerMask mask3;

	public LayerMask mask4;

	public UIPopup maskPopup;

	public UIPopup maskPopupAlt;

	[SerializeField]
	protected MaskToShow _maskSelection;

	protected MaskToShow _startingMaskSelection;

	public MaskToShow maskSelection
	{
		get
		{
			return _maskSelection;
		}
		set
		{
			if (_maskSelection != value)
			{
				_maskSelection = value;
				if (maskPopup != null)
				{
					maskPopup.currentValue = _maskSelection.ToString();
				}
				if (maskPopupAlt != null)
				{
					maskPopupAlt.currentValue = _maskSelection.ToString();
				}
				SyncCamera();
			}
		}
	}

	protected void SyncCamera()
	{
		if (!(cameraToControl != null))
		{
			return;
		}
		switch (_maskSelection)
		{
		case MaskToShow.mask1:
			cameraToControl.cullingMask = mask1;
			if (cameraToControl2 != null)
			{
				cameraToControl2.cullingMask = mask1;
			}
			break;
		case MaskToShow.mask2:
			cameraToControl.cullingMask = mask2;
			if (cameraToControl2 != null)
			{
				cameraToControl2.cullingMask = mask2;
			}
			break;
		case MaskToShow.mask3:
			cameraToControl.cullingMask = mask3;
			if (cameraToControl2 != null)
			{
				cameraToControl2.cullingMask = mask3;
			}
			break;
		case MaskToShow.mask4:
			cameraToControl.cullingMask = mask4;
			if (cameraToControl2 != null)
			{
				cameraToControl2.cullingMask = mask4;
			}
			break;
		}
	}

	protected void SetMaskSelection(string maskSel)
	{
		try
		{
			MaskToShow maskToShow2 = (maskSelection = (MaskToShow)Enum.Parse(typeof(MaskToShow), maskSel));
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set mask type to " + maskSel + " which is not a valid mask type");
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (_maskSelection != _startingMaskSelection)
		{
			needsStore = true;
			jSON["maskSelection"] = _maskSelection.ToString();
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (jc["maskSelection"] != null)
		{
			SetMaskSelection(jc["maskSelection"]);
		}
	}

	protected void InitUI()
	{
		_startingMaskSelection = _maskSelection;
		SyncCamera();
		if (maskPopup != null)
		{
			maskPopup.currentValue = _maskSelection.ToString();
			UIPopup uIPopup = maskPopup;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetMaskSelection));
		}
		if (maskPopupAlt != null)
		{
			maskPopupAlt.currentValue = _maskSelection.ToString();
			UIPopup uIPopup2 = maskPopupAlt;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetMaskSelection));
		}
	}

	private void Start()
	{
		InitUI();
	}
}
