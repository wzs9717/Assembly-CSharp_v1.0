using UnityEngine;
using UnityEngine.UI;

public class FreeControllerV3GUI : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	protected FreeControllerV3 _controller;

	public Text UIDText;

	public ToggleGroupValue positionToggleGroup;

	public ToggleGroupValue rotationToggleGroup;

	public Slider holdPositionSpringSlider;

	public Slider holdPositionDamperSlider;

	public Slider holdPositionMaxForceSlider;

	public Slider holdRotationSpringSlider;

	public Slider holdRotationDamperSlider;

	public Slider holdRotationMaxForceSlider;

	public Slider linkPositionSpringSlider;

	public Slider linkPositionDamperSlider;

	public Slider linkPositionMaxForceSlider;

	public Slider linkRotationSpringSlider;

	public Slider linkRotationDamperSlider;

	public Slider linkRotationMaxForceSlider;

	public UIPopup linkToSelectionPopup;

	public UIPopup linkToAtomSelectionPopup;

	public Slider jointRotationDriveSpringSlider;

	public Slider jointRotationDriveDamperSlider;

	public Slider jointRotationDriveMaxForceSlider;

	public Slider jointRotationDriveXTargetSlider;

	public Slider jointRotationDriveYTargetSlider;

	public Slider jointRotationDriveZTargetSlider;

	public Button selectLinkToFromSceneButton;

	public Button selectAlignToFromSceneButton;

	public Slider massSlider;

	public Toggle physicsEnabledToggle;

	public Toggle collisionEnabledToggle;

	public Toggle useGravityWhenOffToggle;

	public Toggle interactableInPlayModeToggle;

	public SetTextFromFloat xPositionText;

	public Button xPositionMinus1Button;

	public Button xPositionMinusPoint1Button;

	public Button xPositionMinusPoint01Button;

	public Button xPosition0Button;

	public Button xPositionPlusPoint01Button;

	public Button xPositionPlusPoint1Button;

	public Button xPositionPlus1Button;

	public Button xPositionSnapPoint1Button;

	public Toggle xPositionLockToggle;

	public SetTextFromFloat yPositionText;

	public Button yPositionMinus1Button;

	public Button yPositionMinusPoint1Button;

	public Button yPositionMinusPoint01Button;

	public Button yPosition0Button;

	public Button yPositionPlusPoint01Button;

	public Button yPositionPlusPoint1Button;

	public Button yPositionPlus1Button;

	public Button yPositionSnapPoint1Button;

	public Toggle yPositionLockToggle;

	public SetTextFromFloat zPositionText;

	public Button zPositionMinus1Button;

	public Button zPositionMinusPoint1Button;

	public Button zPositionMinusPoint01Button;

	public Button zPosition0Button;

	public Button zPositionPlusPoint01Button;

	public Button zPositionPlusPoint1Button;

	public Button zPositionPlus1Button;

	public Button zPositionSnapPoint1Button;

	public Toggle zPositionLockToggle;

	public SetTextFromFloat xRotationText;

	public Button xRotationMinus45Button;

	public Button xRotationMinus5Button;

	public Button xRotationMinusPoint5Button;

	public Button xRotation0Button;

	public Button xRotationPlusPoint5Button;

	public Button xRotationPlus5Button;

	public Button xRotationPlus45Button;

	public Button xRotationSnap1Button;

	public Toggle xRotationLockToggle;

	public SetTextFromFloat yRotationText;

	public Button yRotationMinus45Button;

	public Button yRotationMinus5Button;

	public Button yRotationMinusPoint5Button;

	public Button yRotation0Button;

	public Button yRotationPlusPoint5Button;

	public Button yRotationPlus5Button;

	public Button yRotationPlus45Button;

	public Button yRotationSnap1Button;

	public Toggle yRotationLockToggle;

	public SetTextFromFloat zRotationText;

	public Button zRotationMinus45Button;

	public Button zRotationMinus5Button;

	public Button zRotationMinusPoint5Button;

	public Button zRotation0Button;

	public Button zRotationPlusPoint5Button;

	public Button zRotationPlus5Button;

	public Button zRotationPlus45Button;

	public Button zRotationSnap1Button;

	public Toggle zRotationLockToggle;

	public FreeControllerV3 controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(_controller != value))
			{
				return;
			}
			_controller = value;
			if (_controller != null)
			{
				_controller.positionToggleGroup = positionToggleGroup;
				_controller.rotationToggleGroup = rotationToggleGroup;
				_controller.holdPositionSpringSlider = holdPositionSpringSlider;
				_controller.holdPositionDamperSlider = holdPositionDamperSlider;
				_controller.holdPositionMaxForceSlider = holdPositionMaxForceSlider;
				_controller.holdRotationSpringSlider = holdRotationSpringSlider;
				_controller.holdRotationDamperSlider = holdRotationDamperSlider;
				_controller.holdRotationMaxForceSlider = holdRotationMaxForceSlider;
				_controller.linkPositionSpringSlider = linkPositionSpringSlider;
				_controller.linkPositionDamperSlider = linkPositionDamperSlider;
				_controller.linkPositionMaxForceSlider = linkPositionMaxForceSlider;
				_controller.linkRotationSpringSlider = linkRotationSpringSlider;
				_controller.linkRotationDamperSlider = linkRotationDamperSlider;
				_controller.linkRotationMaxForceSlider = linkRotationMaxForceSlider;
				_controller.linkToSelectionPopup = linkToSelectionPopup;
				_controller.linkToAtomSelectionPopup = linkToAtomSelectionPopup;
				_controller.selectLinkToFromSceneButton = selectLinkToFromSceneButton;
				_controller.selectAlignToFromSceneButton = selectAlignToFromSceneButton;
				_controller.jointRotationDriveSpringSlider = jointRotationDriveSpringSlider;
				_controller.jointRotationDriveDamperSlider = jointRotationDriveDamperSlider;
				_controller.jointRotationDriveMaxForceSlider = jointRotationDriveMaxForceSlider;
				_controller.jointRotationDriveXTargetSlider = jointRotationDriveXTargetSlider;
				_controller.jointRotationDriveYTargetSlider = jointRotationDriveYTargetSlider;
				_controller.jointRotationDriveZTargetSlider = jointRotationDriveZTargetSlider;
				_controller.RBMassSlider = massSlider;
				_controller.physicsEnabledToggle = physicsEnabledToggle;
				_controller.collisionEnabledToggle = collisionEnabledToggle;
				_controller.useGravityOnRBWhenOffToggle = useGravityWhenOffToggle;
				_controller.interactableInPlayModeToggle = interactableInPlayModeToggle;
				_controller.xLockToggle = xPositionLockToggle;
				_controller.yLockToggle = yPositionLockToggle;
				_controller.zLockToggle = zPositionLockToggle;
				_controller.xRotLockToggle = xRotationLockToggle;
				_controller.yRotLockToggle = yRotationLockToggle;
				_controller.zRotLockToggle = zRotationLockToggle;
				_controller.xPositionMinus1Button = xPositionMinus1Button;
				_controller.xPositionMinusPoint1Button = xPositionMinusPoint1Button;
				_controller.xPositionMinusPoint01Button = xPositionMinusPoint01Button;
				_controller.xPositionPlusPoint01Button = xPositionPlusPoint01Button;
				_controller.xPositionPlusPoint1Button = xPositionPlusPoint1Button;
				_controller.xPositionPlus1Button = xPositionPlus1Button;
				_controller.xPosition0Button = xPosition0Button;
				_controller.xPositionText = xPositionText;
				_controller.xPositionSnapPoint1Button = xPositionSnapPoint1Button;
				_controller.yPositionMinus1Button = yPositionMinus1Button;
				_controller.yPositionMinusPoint1Button = yPositionMinusPoint1Button;
				_controller.yPositionMinusPoint01Button = yPositionMinusPoint01Button;
				_controller.yPositionPlusPoint01Button = yPositionPlusPoint01Button;
				_controller.yPositionPlusPoint1Button = yPositionPlusPoint1Button;
				_controller.yPositionPlus1Button = yPositionPlus1Button;
				_controller.yPosition0Button = yPosition0Button;
				_controller.yPositionText = yPositionText;
				_controller.yPositionSnapPoint1Button = yPositionSnapPoint1Button;
				_controller.zPositionMinus1Button = zPositionMinus1Button;
				_controller.zPositionMinusPoint1Button = zPositionMinusPoint1Button;
				_controller.zPositionMinusPoint01Button = zPositionMinusPoint01Button;
				_controller.zPositionPlusPoint01Button = zPositionPlusPoint01Button;
				_controller.zPositionPlusPoint1Button = zPositionPlusPoint1Button;
				_controller.zPositionPlus1Button = zPositionPlus1Button;
				_controller.zPosition0Button = zPosition0Button;
				_controller.zPositionText = zPositionText;
				_controller.zPositionSnapPoint1Button = zPositionSnapPoint1Button;
				_controller.xRotationMinus45Button = xRotationMinus45Button;
				_controller.xRotationMinus5Button = xRotationMinus5Button;
				_controller.xRotationMinusPoint5Button = xRotationMinusPoint5Button;
				_controller.xRotationPlusPoint5Button = xRotationPlusPoint5Button;
				_controller.xRotationPlus5Button = xRotationPlus5Button;
				_controller.xRotationPlus45Button = xRotationPlus45Button;
				_controller.xRotation0Button = xRotation0Button;
				_controller.xRotationText = xRotationText;
				_controller.xRotationSnap1Button = xRotationSnap1Button;
				_controller.yRotationMinus45Button = yRotationMinus45Button;
				_controller.yRotationMinus5Button = yRotationMinus5Button;
				_controller.yRotationMinusPoint5Button = yRotationMinusPoint5Button;
				_controller.yRotationPlusPoint5Button = yRotationPlusPoint5Button;
				_controller.yRotationPlus5Button = yRotationPlus5Button;
				_controller.yRotationPlus45Button = yRotationPlus45Button;
				_controller.yRotation0Button = yRotation0Button;
				_controller.yRotationText = yRotationText;
				_controller.yRotationSnap1Button = yRotationSnap1Button;
				_controller.zRotationMinus45Button = zRotationMinus45Button;
				_controller.zRotationMinus5Button = zRotationMinus5Button;
				_controller.zRotationMinusPoint5Button = zRotationMinusPoint5Button;
				_controller.zRotationPlusPoint5Button = zRotationPlusPoint5Button;
				_controller.zRotationPlus5Button = zRotationPlus5Button;
				_controller.zRotationPlus45Button = zRotationPlus45Button;
				_controller.zRotation0Button = zRotation0Button;
				_controller.zRotationText = zRotationText;
				_controller.zRotationSnap1Button = zRotationSnap1Button;
				_controller.UITransforms = new Transform[1];
				_controller.UITransforms[0] = base.transform;
				if (UIDText != null)
				{
					UIDText.text = _controller.name;
				}
				_controller.UIDText = UIDText;
			}
		}
	}
}
