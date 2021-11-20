using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class OVRInput
{
	[Flags]
	public enum Button
	{
		None = 0x0,
		One = 0x1,
		Two = 0x2,
		Three = 0x4,
		Four = 0x8,
		Start = 0x100,
		Back = 0x200,
		PrimaryShoulder = 0x1000,
		PrimaryIndexTrigger = 0x2000,
		PrimaryHandTrigger = 0x4000,
		PrimaryThumbstick = 0x8000,
		PrimaryThumbstickUp = 0x10000,
		PrimaryThumbstickDown = 0x20000,
		PrimaryThumbstickLeft = 0x40000,
		PrimaryThumbstickRight = 0x80000,
		PrimaryTouchpad = 0x400,
		SecondaryShoulder = 0x100000,
		SecondaryIndexTrigger = 0x200000,
		SecondaryHandTrigger = 0x400000,
		SecondaryThumbstick = 0x800000,
		SecondaryThumbstickUp = 0x1000000,
		SecondaryThumbstickDown = 0x2000000,
		SecondaryThumbstickLeft = 0x4000000,
		SecondaryThumbstickRight = 0x8000000,
		SecondaryTouchpad = 0x800,
		DpadUp = 0x10,
		DpadDown = 0x20,
		DpadLeft = 0x40,
		DpadRight = 0x80,
		Up = 0x10000000,
		Down = 0x20000000,
		Left = 0x40000000,
		Right = int.MinValue,
		Any = -1
	}

	[Flags]
	public enum RawButton
	{
		None = 0x0,
		A = 0x1,
		B = 0x2,
		X = 0x100,
		Y = 0x200,
		Start = 0x100000,
		Back = 0x200000,
		LShoulder = 0x800,
		LIndexTrigger = 0x10000000,
		LHandTrigger = 0x20000000,
		LThumbstick = 0x400,
		LThumbstickUp = 0x10,
		LThumbstickDown = 0x20,
		LThumbstickLeft = 0x40,
		LThumbstickRight = 0x80,
		LTouchpad = 0x40000000,
		RShoulder = 0x8,
		RIndexTrigger = 0x4000000,
		RHandTrigger = 0x8000000,
		RThumbstick = 0x4,
		RThumbstickUp = 0x1000,
		RThumbstickDown = 0x2000,
		RThumbstickLeft = 0x4000,
		RThumbstickRight = 0x8000,
		RTouchpad = int.MinValue,
		DpadUp = 0x10000,
		DpadDown = 0x20000,
		DpadLeft = 0x40000,
		DpadRight = 0x80000,
		Any = -1
	}

	[Flags]
	public enum Touch
	{
		None = 0x0,
		One = 0x1,
		Two = 0x2,
		Three = 0x4,
		Four = 0x8,
		PrimaryIndexTrigger = 0x2000,
		PrimaryThumbstick = 0x8000,
		PrimaryThumbRest = 0x1000,
		PrimaryTouchpad = 0x400,
		SecondaryIndexTrigger = 0x200000,
		SecondaryThumbstick = 0x800000,
		SecondaryThumbRest = 0x100000,
		SecondaryTouchpad = 0x800,
		Any = -1
	}

	[Flags]
	public enum RawTouch
	{
		None = 0x0,
		A = 0x1,
		B = 0x2,
		X = 0x100,
		Y = 0x200,
		LIndexTrigger = 0x1000,
		LThumbstick = 0x400,
		LThumbRest = 0x800,
		LTouchpad = 0x40000000,
		RIndexTrigger = 0x10,
		RThumbstick = 0x4,
		RThumbRest = 0x8,
		RTouchpad = int.MinValue,
		Any = -1
	}

	[Flags]
	public enum NearTouch
	{
		None = 0x0,
		PrimaryIndexTrigger = 0x1,
		PrimaryThumbButtons = 0x2,
		SecondaryIndexTrigger = 0x4,
		SecondaryThumbButtons = 0x8,
		Any = -1
	}

	[Flags]
	public enum RawNearTouch
	{
		None = 0x0,
		LIndexTrigger = 0x1,
		LThumbButtons = 0x2,
		RIndexTrigger = 0x4,
		RThumbButtons = 0x8,
		Any = -1
	}

	[Flags]
	public enum Axis1D
	{
		None = 0x0,
		PrimaryIndexTrigger = 0x1,
		PrimaryHandTrigger = 0x4,
		SecondaryIndexTrigger = 0x2,
		SecondaryHandTrigger = 0x8,
		Any = -1
	}

	[Flags]
	public enum RawAxis1D
	{
		None = 0x0,
		LIndexTrigger = 0x1,
		LHandTrigger = 0x4,
		RIndexTrigger = 0x2,
		RHandTrigger = 0x8,
		Any = -1
	}

	[Flags]
	public enum Axis2D
	{
		None = 0x0,
		PrimaryThumbstick = 0x1,
		PrimaryTouchpad = 0x4,
		SecondaryThumbstick = 0x2,
		SecondaryTouchpad = 0x8,
		Any = -1
	}

	[Flags]
	public enum RawAxis2D
	{
		None = 0x0,
		LThumbstick = 0x1,
		LTouchpad = 0x4,
		RThumbstick = 0x2,
		RTouchpad = 0x8,
		Any = -1
	}

	[Flags]
	public enum Controller
	{
		None = 0x0,
		LTouch = 0x1,
		RTouch = 0x2,
		Touch = 0x3,
		Remote = 0x4,
		Gamepad = 0x10,
		Touchpad = 0x8000000,
		LTrackedRemote = 0x1000000,
		RTrackedRemote = 0x2000000,
		Active = int.MinValue,
		All = -1
	}

	private abstract class OVRControllerBase
	{
		public class VirtualButtonMap
		{
			public RawButton None;

			public RawButton One;

			public RawButton Two;

			public RawButton Three;

			public RawButton Four;

			public RawButton Start;

			public RawButton Back;

			public RawButton PrimaryShoulder;

			public RawButton PrimaryIndexTrigger;

			public RawButton PrimaryHandTrigger;

			public RawButton PrimaryThumbstick;

			public RawButton PrimaryThumbstickUp;

			public RawButton PrimaryThumbstickDown;

			public RawButton PrimaryThumbstickLeft;

			public RawButton PrimaryThumbstickRight;

			public RawButton PrimaryTouchpad;

			public RawButton SecondaryShoulder;

			public RawButton SecondaryIndexTrigger;

			public RawButton SecondaryHandTrigger;

			public RawButton SecondaryThumbstick;

			public RawButton SecondaryThumbstickUp;

			public RawButton SecondaryThumbstickDown;

			public RawButton SecondaryThumbstickLeft;

			public RawButton SecondaryThumbstickRight;

			public RawButton SecondaryTouchpad;

			public RawButton DpadUp;

			public RawButton DpadDown;

			public RawButton DpadLeft;

			public RawButton DpadRight;

			public RawButton Up;

			public RawButton Down;

			public RawButton Left;

			public RawButton Right;

			public RawButton ToRawMask(Button virtualMask)
			{
				RawButton rawButton = RawButton.None;
				if (virtualMask == Button.None)
				{
					return RawButton.None;
				}
				if ((virtualMask & Button.One) != 0)
				{
					rawButton |= One;
				}
				if ((virtualMask & Button.Two) != 0)
				{
					rawButton |= Two;
				}
				if ((virtualMask & Button.Three) != 0)
				{
					rawButton |= Three;
				}
				if ((virtualMask & Button.Four) != 0)
				{
					rawButton |= Four;
				}
				if ((virtualMask & Button.Start) != 0)
				{
					rawButton |= Start;
				}
				if ((virtualMask & Button.Back) != 0)
				{
					rawButton |= Back;
				}
				if ((virtualMask & Button.PrimaryShoulder) != 0)
				{
					rawButton |= PrimaryShoulder;
				}
				if ((virtualMask & Button.PrimaryIndexTrigger) != 0)
				{
					rawButton |= PrimaryIndexTrigger;
				}
				if ((virtualMask & Button.PrimaryHandTrigger) != 0)
				{
					rawButton |= PrimaryHandTrigger;
				}
				if ((virtualMask & Button.PrimaryThumbstick) != 0)
				{
					rawButton |= PrimaryThumbstick;
				}
				if ((virtualMask & Button.PrimaryThumbstickUp) != 0)
				{
					rawButton |= PrimaryThumbstickUp;
				}
				if ((virtualMask & Button.PrimaryThumbstickDown) != 0)
				{
					rawButton |= PrimaryThumbstickDown;
				}
				if ((virtualMask & Button.PrimaryThumbstickLeft) != 0)
				{
					rawButton |= PrimaryThumbstickLeft;
				}
				if ((virtualMask & Button.PrimaryThumbstickRight) != 0)
				{
					rawButton |= PrimaryThumbstickRight;
				}
				if ((virtualMask & Button.PrimaryTouchpad) != 0)
				{
					rawButton |= PrimaryTouchpad;
				}
				if ((virtualMask & Button.SecondaryShoulder) != 0)
				{
					rawButton |= SecondaryShoulder;
				}
				if ((virtualMask & Button.SecondaryIndexTrigger) != 0)
				{
					rawButton |= SecondaryIndexTrigger;
				}
				if ((virtualMask & Button.SecondaryHandTrigger) != 0)
				{
					rawButton |= SecondaryHandTrigger;
				}
				if ((virtualMask & Button.SecondaryThumbstick) != 0)
				{
					rawButton |= SecondaryThumbstick;
				}
				if ((virtualMask & Button.SecondaryThumbstickUp) != 0)
				{
					rawButton |= SecondaryThumbstickUp;
				}
				if ((virtualMask & Button.SecondaryThumbstickDown) != 0)
				{
					rawButton |= SecondaryThumbstickDown;
				}
				if ((virtualMask & Button.SecondaryThumbstickLeft) != 0)
				{
					rawButton |= SecondaryThumbstickLeft;
				}
				if ((virtualMask & Button.SecondaryThumbstickRight) != 0)
				{
					rawButton |= SecondaryThumbstickRight;
				}
				if ((virtualMask & Button.SecondaryTouchpad) != 0)
				{
					rawButton |= SecondaryTouchpad;
				}
				if ((virtualMask & Button.DpadUp) != 0)
				{
					rawButton |= DpadUp;
				}
				if ((virtualMask & Button.DpadDown) != 0)
				{
					rawButton |= DpadDown;
				}
				if ((virtualMask & Button.DpadLeft) != 0)
				{
					rawButton |= DpadLeft;
				}
				if ((virtualMask & Button.DpadRight) != 0)
				{
					rawButton |= DpadRight;
				}
				if ((virtualMask & Button.Up) != 0)
				{
					rawButton |= Up;
				}
				if ((virtualMask & Button.Down) != 0)
				{
					rawButton |= Down;
				}
				if ((virtualMask & Button.Left) != 0)
				{
					rawButton |= Left;
				}
				if (((uint)virtualMask & 0x80000000u) != 0)
				{
					rawButton |= Right;
				}
				return rawButton;
			}
		}

		public class VirtualTouchMap
		{
			public RawTouch None;

			public RawTouch One;

			public RawTouch Two;

			public RawTouch Three;

			public RawTouch Four;

			public RawTouch PrimaryIndexTrigger;

			public RawTouch PrimaryThumbstick;

			public RawTouch PrimaryThumbRest;

			public RawTouch PrimaryTouchpad;

			public RawTouch SecondaryIndexTrigger;

			public RawTouch SecondaryThumbstick;

			public RawTouch SecondaryThumbRest;

			public RawTouch SecondaryTouchpad;

			public RawTouch ToRawMask(Touch virtualMask)
			{
				RawTouch rawTouch = RawTouch.None;
				if (virtualMask == Touch.None)
				{
					return RawTouch.None;
				}
				if ((virtualMask & Touch.One) != 0)
				{
					rawTouch |= One;
				}
				if ((virtualMask & Touch.Two) != 0)
				{
					rawTouch |= Two;
				}
				if ((virtualMask & Touch.Three) != 0)
				{
					rawTouch |= Three;
				}
				if ((virtualMask & Touch.Four) != 0)
				{
					rawTouch |= Four;
				}
				if ((virtualMask & Touch.PrimaryIndexTrigger) != 0)
				{
					rawTouch |= PrimaryIndexTrigger;
				}
				if ((virtualMask & Touch.PrimaryThumbstick) != 0)
				{
					rawTouch |= PrimaryThumbstick;
				}
				if ((virtualMask & Touch.PrimaryThumbRest) != 0)
				{
					rawTouch |= PrimaryThumbRest;
				}
				if ((virtualMask & Touch.PrimaryTouchpad) != 0)
				{
					rawTouch |= PrimaryTouchpad;
				}
				if ((virtualMask & Touch.SecondaryIndexTrigger) != 0)
				{
					rawTouch |= SecondaryIndexTrigger;
				}
				if ((virtualMask & Touch.SecondaryThumbstick) != 0)
				{
					rawTouch |= SecondaryThumbstick;
				}
				if ((virtualMask & Touch.SecondaryThumbRest) != 0)
				{
					rawTouch |= SecondaryThumbRest;
				}
				if ((virtualMask & Touch.SecondaryTouchpad) != 0)
				{
					rawTouch |= SecondaryTouchpad;
				}
				return rawTouch;
			}
		}

		public class VirtualNearTouchMap
		{
			public RawNearTouch None;

			public RawNearTouch PrimaryIndexTrigger;

			public RawNearTouch PrimaryThumbButtons;

			public RawNearTouch SecondaryIndexTrigger;

			public RawNearTouch SecondaryThumbButtons;

			public RawNearTouch ToRawMask(NearTouch virtualMask)
			{
				RawNearTouch rawNearTouch = RawNearTouch.None;
				if (virtualMask == NearTouch.None)
				{
					return RawNearTouch.None;
				}
				if ((virtualMask & NearTouch.PrimaryIndexTrigger) != 0)
				{
					rawNearTouch |= PrimaryIndexTrigger;
				}
				if ((virtualMask & NearTouch.PrimaryThumbButtons) != 0)
				{
					rawNearTouch |= PrimaryThumbButtons;
				}
				if ((virtualMask & NearTouch.SecondaryIndexTrigger) != 0)
				{
					rawNearTouch |= SecondaryIndexTrigger;
				}
				if ((virtualMask & NearTouch.SecondaryThumbButtons) != 0)
				{
					rawNearTouch |= SecondaryThumbButtons;
				}
				return rawNearTouch;
			}
		}

		public class VirtualAxis1DMap
		{
			public RawAxis1D None;

			public RawAxis1D PrimaryIndexTrigger;

			public RawAxis1D PrimaryHandTrigger;

			public RawAxis1D SecondaryIndexTrigger;

			public RawAxis1D SecondaryHandTrigger;

			public RawAxis1D ToRawMask(Axis1D virtualMask)
			{
				RawAxis1D rawAxis1D = RawAxis1D.None;
				if (virtualMask == Axis1D.None)
				{
					return RawAxis1D.None;
				}
				if ((virtualMask & Axis1D.PrimaryIndexTrigger) != 0)
				{
					rawAxis1D |= PrimaryIndexTrigger;
				}
				if ((virtualMask & Axis1D.PrimaryHandTrigger) != 0)
				{
					rawAxis1D |= PrimaryHandTrigger;
				}
				if ((virtualMask & Axis1D.SecondaryIndexTrigger) != 0)
				{
					rawAxis1D |= SecondaryIndexTrigger;
				}
				if ((virtualMask & Axis1D.SecondaryHandTrigger) != 0)
				{
					rawAxis1D |= SecondaryHandTrigger;
				}
				return rawAxis1D;
			}
		}

		public class VirtualAxis2DMap
		{
			public RawAxis2D None;

			public RawAxis2D PrimaryThumbstick;

			public RawAxis2D PrimaryTouchpad;

			public RawAxis2D SecondaryThumbstick;

			public RawAxis2D SecondaryTouchpad;

			public RawAxis2D ToRawMask(Axis2D virtualMask)
			{
				RawAxis2D rawAxis2D = RawAxis2D.None;
				if (virtualMask == Axis2D.None)
				{
					return RawAxis2D.None;
				}
				if ((virtualMask & Axis2D.PrimaryThumbstick) != 0)
				{
					rawAxis2D |= PrimaryThumbstick;
				}
				if ((virtualMask & Axis2D.PrimaryTouchpad) != 0)
				{
					rawAxis2D |= PrimaryTouchpad;
				}
				if ((virtualMask & Axis2D.SecondaryThumbstick) != 0)
				{
					rawAxis2D |= SecondaryThumbstick;
				}
				if ((virtualMask & Axis2D.SecondaryTouchpad) != 0)
				{
					rawAxis2D |= SecondaryTouchpad;
				}
				return rawAxis2D;
			}
		}

		public Controller controllerType;

		public VirtualButtonMap buttonMap = new VirtualButtonMap();

		public VirtualTouchMap touchMap = new VirtualTouchMap();

		public VirtualNearTouchMap nearTouchMap = new VirtualNearTouchMap();

		public VirtualAxis1DMap axis1DMap = new VirtualAxis1DMap();

		public VirtualAxis2DMap axis2DMap = new VirtualAxis2DMap();

		public OVRPlugin.ControllerState2 previousState = default(OVRPlugin.ControllerState2);

		public OVRPlugin.ControllerState2 currentState = default(OVRPlugin.ControllerState2);

		public bool shouldApplyDeadzone = true;

		public OVRControllerBase()
		{
			ConfigureButtonMap();
			ConfigureTouchMap();
			ConfigureNearTouchMap();
			ConfigureAxis1DMap();
			ConfigureAxis2DMap();
		}

		public virtual Controller Update()
		{
			OVRPlugin.ControllerState2 controllerState = OVRPlugin.GetControllerState2((uint)controllerType);
			if (controllerState.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 268435456u;
			}
			if (controllerState.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 536870912u;
			}
			if (controllerState.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16u;
			}
			if (controllerState.LThumbstick.y <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32u;
			}
			if (controllerState.LThumbstick.x <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 64u;
			}
			if (controllerState.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 128u;
			}
			if (controllerState.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 67108864u;
			}
			if (controllerState.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 134217728u;
			}
			if (controllerState.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 4096u;
			}
			if (controllerState.RThumbstick.y <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 8192u;
			}
			if (controllerState.RThumbstick.x <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16384u;
			}
			if (controllerState.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32768u;
			}
			previousState = currentState;
			currentState = controllerState;
			return (Controller)((int)currentState.ConnectedControllers & (int)controllerType);
		}

		public virtual void SetControllerVibration(float frequency, float amplitude)
		{
			OVRPlugin.SetControllerVibration((uint)controllerType, frequency, amplitude);
		}

		public abstract void ConfigureButtonMap();

		public abstract void ConfigureTouchMap();

		public abstract void ConfigureNearTouchMap();

		public abstract void ConfigureAxis1DMap();

		public abstract void ConfigureAxis2DMap();

		public RawButton ResolveToRawMask(Button virtualMask)
		{
			return buttonMap.ToRawMask(virtualMask);
		}

		public RawTouch ResolveToRawMask(Touch virtualMask)
		{
			return touchMap.ToRawMask(virtualMask);
		}

		public RawNearTouch ResolveToRawMask(NearTouch virtualMask)
		{
			return nearTouchMap.ToRawMask(virtualMask);
		}

		public RawAxis1D ResolveToRawMask(Axis1D virtualMask)
		{
			return axis1DMap.ToRawMask(virtualMask);
		}

		public RawAxis2D ResolveToRawMask(Axis2D virtualMask)
		{
			return axis2DMap.ToRawMask(virtualMask);
		}
	}

	private class OVRControllerTouch : OVRControllerBase
	{
		public OVRControllerTouch()
		{
			controllerType = Controller.Touch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.A;
			buttonMap.Two = RawButton.B;
			buttonMap.Three = RawButton.X;
			buttonMap.Four = RawButton.Y;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.None;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.LHandTrigger;
			buttonMap.PrimaryThumbstick = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger = RawButton.RHandTrigger;
			buttonMap.SecondaryThumbstick = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.None;
			buttonMap.DpadDown = RawButton.None;
			buttonMap.DpadLeft = RawButton.None;
			buttonMap.DpadRight = RawButton.None;
			buttonMap.Up = RawButton.LThumbstickUp;
			buttonMap.Down = RawButton.LThumbstickDown;
			buttonMap.Left = RawButton.LThumbstickLeft;
			buttonMap.Right = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.A;
			touchMap.Two = RawTouch.B;
			touchMap.Three = RawTouch.X;
			touchMap.Four = RawTouch.Y;
			touchMap.PrimaryIndexTrigger = RawTouch.LIndexTrigger;
			touchMap.PrimaryThumbstick = RawTouch.LThumbstick;
			touchMap.PrimaryThumbRest = RawTouch.LThumbRest;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.RIndexTrigger;
			touchMap.SecondaryThumbstick = RawTouch.RThumbstick;
			touchMap.SecondaryThumbRest = RawTouch.RThumbRest;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.LIndexTrigger;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.LThumbButtons;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.RIndexTrigger;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.RThumbButtons;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.LHandTrigger;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.RHandTrigger;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}
	}

	private class OVRControllerLTouch : OVRControllerBase
	{
		public OVRControllerLTouch()
		{
			controllerType = Controller.LTouch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.X;
			buttonMap.Two = RawButton.Y;
			buttonMap.Three = RawButton.None;
			buttonMap.Four = RawButton.None;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.None;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.LHandTrigger;
			buttonMap.PrimaryThumbstick = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.None;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.None;
			buttonMap.SecondaryThumbstickUp = RawButton.None;
			buttonMap.SecondaryThumbstickDown = RawButton.None;
			buttonMap.SecondaryThumbstickLeft = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.None;
			buttonMap.DpadDown = RawButton.None;
			buttonMap.DpadLeft = RawButton.None;
			buttonMap.DpadRight = RawButton.None;
			buttonMap.Up = RawButton.LThumbstickUp;
			buttonMap.Down = RawButton.LThumbstickDown;
			buttonMap.Left = RawButton.LThumbstickLeft;
			buttonMap.Right = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.X;
			touchMap.Two = RawTouch.Y;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.LIndexTrigger;
			touchMap.PrimaryThumbstick = RawTouch.LThumbstick;
			touchMap.PrimaryThumbRest = RawTouch.LThumbRest;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.LIndexTrigger;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.LThumbButtons;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.LHandTrigger;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}
	}

	private class OVRControllerRTouch : OVRControllerBase
	{
		public OVRControllerRTouch()
		{
			controllerType = Controller.RTouch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.A;
			buttonMap.Two = RawButton.B;
			buttonMap.Three = RawButton.None;
			buttonMap.Four = RawButton.None;
			buttonMap.Start = RawButton.None;
			buttonMap.Back = RawButton.None;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.RIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.RHandTrigger;
			buttonMap.PrimaryThumbstick = RawButton.RThumbstick;
			buttonMap.PrimaryThumbstickUp = RawButton.RThumbstickUp;
			buttonMap.PrimaryThumbstickDown = RawButton.RThumbstickDown;
			buttonMap.PrimaryThumbstickLeft = RawButton.RThumbstickLeft;
			buttonMap.PrimaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.None;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.None;
			buttonMap.SecondaryThumbstickUp = RawButton.None;
			buttonMap.SecondaryThumbstickDown = RawButton.None;
			buttonMap.SecondaryThumbstickLeft = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.None;
			buttonMap.DpadDown = RawButton.None;
			buttonMap.DpadLeft = RawButton.None;
			buttonMap.DpadRight = RawButton.None;
			buttonMap.Up = RawButton.RThumbstickUp;
			buttonMap.Down = RawButton.RThumbstickDown;
			buttonMap.Left = RawButton.RThumbstickLeft;
			buttonMap.Right = RawButton.RThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.A;
			touchMap.Two = RawTouch.B;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.RIndexTrigger;
			touchMap.PrimaryThumbstick = RawTouch.RThumbstick;
			touchMap.PrimaryThumbRest = RawTouch.RThumbRest;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.RIndexTrigger;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.RThumbButtons;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.RIndexTrigger;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.RHandTrigger;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.RThumbstick;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}
	}

	private class OVRControllerRemote : OVRControllerBase
	{
		public OVRControllerRemote()
		{
			controllerType = Controller.Remote;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.Start;
			buttonMap.Two = RawButton.Back;
			buttonMap.Three = RawButton.None;
			buttonMap.Four = RawButton.None;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.None;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.None;
			buttonMap.PrimaryThumbstickUp = RawButton.None;
			buttonMap.PrimaryThumbstickDown = RawButton.None;
			buttonMap.PrimaryThumbstickLeft = RawButton.None;
			buttonMap.PrimaryThumbstickRight = RawButton.None;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.None;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.None;
			buttonMap.SecondaryThumbstickUp = RawButton.None;
			buttonMap.SecondaryThumbstickDown = RawButton.None;
			buttonMap.SecondaryThumbstickLeft = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.DpadUp;
			buttonMap.Down = RawButton.DpadDown;
			buttonMap.Left = RawButton.DpadLeft;
			buttonMap.Right = RawButton.DpadRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}
	}

	private class OVRControllerGamepadPC : OVRControllerBase
	{
		public OVRControllerGamepadPC()
		{
			controllerType = Controller.Gamepad;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.A;
			buttonMap.Two = RawButton.B;
			buttonMap.Three = RawButton.X;
			buttonMap.Four = RawButton.Y;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.LThumbstickUp;
			buttonMap.Down = RawButton.LThumbstickDown;
			buttonMap.Left = RawButton.LThumbstickLeft;
			buttonMap.Right = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}
	}

	private class OVRControllerGamepadMac : OVRControllerBase
	{
		private enum AxisGPC
		{
			None = -1,
			LeftXAxis,
			LeftYAxis,
			RightXAxis,
			RightYAxis,
			LeftTrigger,
			RightTrigger,
			DPad_X_Axis,
			DPad_Y_Axis,
			Max
		}

		public enum ButtonGPC
		{
			None = -1,
			A,
			B,
			X,
			Y,
			Up,
			Down,
			Left,
			Right,
			Start,
			Back,
			LStick,
			RStick,
			LeftShoulder,
			RightShoulder,
			Max
		}

		private bool initialized;

		private const string DllName = "OVRGamepad";

		public OVRControllerGamepadMac()
		{
			controllerType = Controller.Gamepad;
			initialized = OVR_GamepadController_Initialize();
		}

		~OVRControllerGamepadMac()
		{
			if (initialized)
			{
				OVR_GamepadController_Destroy();
			}
		}

		public override Controller Update()
		{
			if (!initialized)
			{
				return Controller.None;
			}
			OVRPlugin.ControllerState2 controllerState = default(OVRPlugin.ControllerState2);
			if (OVR_GamepadController_Update())
			{
				controllerState.ConnectedControllers = 16u;
			}
			if (OVR_GamepadController_GetButton(0))
			{
				controllerState.Buttons |= 1u;
			}
			if (OVR_GamepadController_GetButton(1))
			{
				controllerState.Buttons |= 2u;
			}
			if (OVR_GamepadController_GetButton(2))
			{
				controllerState.Buttons |= 256u;
			}
			if (OVR_GamepadController_GetButton(3))
			{
				controllerState.Buttons |= 512u;
			}
			if (OVR_GamepadController_GetButton(4))
			{
				controllerState.Buttons |= 65536u;
			}
			if (OVR_GamepadController_GetButton(5))
			{
				controllerState.Buttons |= 131072u;
			}
			if (OVR_GamepadController_GetButton(6))
			{
				controllerState.Buttons |= 262144u;
			}
			if (OVR_GamepadController_GetButton(7))
			{
				controllerState.Buttons |= 524288u;
			}
			if (OVR_GamepadController_GetButton(8))
			{
				controllerState.Buttons |= 1048576u;
			}
			if (OVR_GamepadController_GetButton(9))
			{
				controllerState.Buttons |= 2097152u;
			}
			if (OVR_GamepadController_GetButton(10))
			{
				controllerState.Buttons |= 1024u;
			}
			if (OVR_GamepadController_GetButton(11))
			{
				controllerState.Buttons |= 4u;
			}
			if (OVR_GamepadController_GetButton(12))
			{
				controllerState.Buttons |= 2048u;
			}
			if (OVR_GamepadController_GetButton(13))
			{
				controllerState.Buttons |= 8u;
			}
			controllerState.LThumbstick.x = OVR_GamepadController_GetAxis(0);
			controllerState.LThumbstick.y = OVR_GamepadController_GetAxis(1);
			controllerState.RThumbstick.x = OVR_GamepadController_GetAxis(2);
			controllerState.RThumbstick.y = OVR_GamepadController_GetAxis(3);
			controllerState.LIndexTrigger = OVR_GamepadController_GetAxis(4);
			controllerState.RIndexTrigger = OVR_GamepadController_GetAxis(5);
			if (controllerState.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 268435456u;
			}
			if (controllerState.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 536870912u;
			}
			if (controllerState.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16u;
			}
			if (controllerState.LThumbstick.y <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32u;
			}
			if (controllerState.LThumbstick.x <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 64u;
			}
			if (controllerState.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 128u;
			}
			if (controllerState.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 67108864u;
			}
			if (controllerState.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 134217728u;
			}
			if (controllerState.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 4096u;
			}
			if (controllerState.RThumbstick.y <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 8192u;
			}
			if (controllerState.RThumbstick.x <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16384u;
			}
			if (controllerState.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32768u;
			}
			previousState = currentState;
			currentState = controllerState;
			return (Controller)((int)currentState.ConnectedControllers & (int)controllerType);
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.A;
			buttonMap.Two = RawButton.B;
			buttonMap.Three = RawButton.X;
			buttonMap.Four = RawButton.Y;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.LThumbstickUp;
			buttonMap.Down = RawButton.LThumbstickDown;
			buttonMap.Left = RawButton.LThumbstickLeft;
			buttonMap.Right = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}

		public override void SetControllerVibration(float frequency, float amplitude)
		{
			int node = 0;
			float frequency2 = frequency * 200f;
			OVR_GamepadController_SetVibration(node, amplitude, frequency2);
		}

		[DllImport("OVRGamepad", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Initialize();

		[DllImport("OVRGamepad", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Destroy();

		[DllImport("OVRGamepad", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Update();

		[DllImport("OVRGamepad", CallingConvention = CallingConvention.Cdecl)]
		private static extern float OVR_GamepadController_GetAxis(int axis);

		[DllImport("OVRGamepad", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_GetButton(int button);

		[DllImport("OVRGamepad", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_SetVibration(int node, float strength, float frequency);
	}

	private class OVRControllerGamepadAndroid : OVRControllerBase
	{
		private static class AndroidButtonNames
		{
			public static readonly KeyCode A = KeyCode.JoystickButton0;

			public static readonly KeyCode B = KeyCode.JoystickButton1;

			public static readonly KeyCode X = KeyCode.JoystickButton2;

			public static readonly KeyCode Y = KeyCode.JoystickButton3;

			public static readonly KeyCode Start = KeyCode.JoystickButton10;

			public static readonly KeyCode Back = KeyCode.JoystickButton11;

			public static readonly KeyCode LThumbstick = KeyCode.JoystickButton8;

			public static readonly KeyCode RThumbstick = KeyCode.JoystickButton9;

			public static readonly KeyCode LShoulder = KeyCode.JoystickButton4;

			public static readonly KeyCode RShoulder = KeyCode.JoystickButton5;
		}

		private static class AndroidAxisNames
		{
			public static readonly string LThumbstickX = "Oculus_GearVR_LThumbstickX";

			public static readonly string LThumbstickY = "Oculus_GearVR_LThumbstickY";

			public static readonly string RThumbstickX = "Oculus_GearVR_RThumbstickX";

			public static readonly string RThumbstickY = "Oculus_GearVR_RThumbstickY";

			public static readonly string LIndexTrigger = "Oculus_GearVR_LIndexTrigger";

			public static readonly string RIndexTrigger = "Oculus_GearVR_RIndexTrigger";

			public static readonly string DpadX = "Oculus_GearVR_DpadX";

			public static readonly string DpadY = "Oculus_GearVR_DpadY";
		}

		private bool joystickDetected;

		private float joystickCheckInterval = 1f;

		private float joystickCheckTime;

		public OVRControllerGamepadAndroid()
		{
			controllerType = Controller.Gamepad;
		}

		private bool ShouldUpdate()
		{
			if (Time.realtimeSinceStartup - joystickCheckTime > joystickCheckInterval)
			{
				joystickCheckTime = Time.realtimeSinceStartup;
				joystickDetected = false;
				string[] joystickNames = Input.GetJoystickNames();
				for (int i = 0; i < joystickNames.Length; i++)
				{
					if (joystickNames[i] != string.Empty)
					{
						joystickDetected = true;
						break;
					}
				}
			}
			return joystickDetected;
		}

		public override Controller Update()
		{
			if (!ShouldUpdate())
			{
				return Controller.None;
			}
			OVRPlugin.ControllerState2 controllerState = default(OVRPlugin.ControllerState2);
			controllerState.ConnectedControllers = 16u;
			if (Input.GetKey(AndroidButtonNames.A))
			{
				controllerState.Buttons |= 1u;
			}
			if (Input.GetKey(AndroidButtonNames.B))
			{
				controllerState.Buttons |= 2u;
			}
			if (Input.GetKey(AndroidButtonNames.X))
			{
				controllerState.Buttons |= 256u;
			}
			if (Input.GetKey(AndroidButtonNames.Y))
			{
				controllerState.Buttons |= 512u;
			}
			if (Input.GetKey(AndroidButtonNames.Start))
			{
				controllerState.Buttons |= 1048576u;
			}
			if (Input.GetKey(AndroidButtonNames.Back) || Input.GetKey(KeyCode.Escape))
			{
				controllerState.Buttons |= 2097152u;
			}
			if (Input.GetKey(AndroidButtonNames.LThumbstick))
			{
				controllerState.Buttons |= 1024u;
			}
			if (Input.GetKey(AndroidButtonNames.RThumbstick))
			{
				controllerState.Buttons |= 4u;
			}
			if (Input.GetKey(AndroidButtonNames.LShoulder))
			{
				controllerState.Buttons |= 2048u;
			}
			if (Input.GetKey(AndroidButtonNames.RShoulder))
			{
				controllerState.Buttons |= 8u;
			}
			controllerState.LThumbstick.x = Input.GetAxisRaw(AndroidAxisNames.LThumbstickX);
			controllerState.LThumbstick.y = Input.GetAxisRaw(AndroidAxisNames.LThumbstickY);
			controllerState.RThumbstick.x = Input.GetAxisRaw(AndroidAxisNames.RThumbstickX);
			controllerState.RThumbstick.y = Input.GetAxisRaw(AndroidAxisNames.RThumbstickY);
			controllerState.LIndexTrigger = Input.GetAxisRaw(AndroidAxisNames.LIndexTrigger);
			controllerState.RIndexTrigger = Input.GetAxisRaw(AndroidAxisNames.RIndexTrigger);
			if (controllerState.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 268435456u;
			}
			if (controllerState.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 536870912u;
			}
			if (controllerState.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16u;
			}
			if (controllerState.LThumbstick.y <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32u;
			}
			if (controllerState.LThumbstick.x <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 64u;
			}
			if (controllerState.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 128u;
			}
			if (controllerState.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 67108864u;
			}
			if (controllerState.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 134217728u;
			}
			if (controllerState.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 4096u;
			}
			if (controllerState.RThumbstick.y <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 8192u;
			}
			if (controllerState.RThumbstick.x <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16384u;
			}
			if (controllerState.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32768u;
			}
			float axisRaw = Input.GetAxisRaw(AndroidAxisNames.DpadX);
			float axisRaw2 = Input.GetAxisRaw(AndroidAxisNames.DpadY);
			if (axisRaw <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 262144u;
			}
			if (axisRaw >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 524288u;
			}
			if (axisRaw2 <= 0f - AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 131072u;
			}
			if (axisRaw2 >= AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 65536u;
			}
			previousState = currentState;
			currentState = controllerState;
			return (Controller)((int)currentState.ConnectedControllers & (int)controllerType);
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.A;
			buttonMap.Two = RawButton.B;
			buttonMap.Three = RawButton.X;
			buttonMap.Four = RawButton.Y;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad = RawButton.None;
			buttonMap.SecondaryShoulder = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.LThumbstickUp;
			buttonMap.Down = RawButton.LThumbstickDown;
			buttonMap.Left = RawButton.LThumbstickLeft;
			buttonMap.Right = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.None;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}

		public override void SetControllerVibration(float frequency, float amplitude)
		{
		}
	}

	private class OVRControllerTouchpad : OVRControllerBase
	{
		private OVRPlugin.Vector2f moveAmount;

		private float maxTapMagnitude = 0.1f;

		private float minMoveMagnitude = 0.15f;

		public OVRControllerTouchpad()
		{
			controllerType = Controller.Touchpad;
		}

		public override Controller Update()
		{
			Controller result = base.Update();
			if (GetDown(RawTouch.LTouchpad, Controller.Touchpad))
			{
				moveAmount = currentState.LTouchpad;
			}
			if (GetUp(RawTouch.LTouchpad, Controller.Touchpad))
			{
				moveAmount.x = previousState.LTouchpad.x - moveAmount.x;
				moveAmount.y = previousState.LTouchpad.y - moveAmount.y;
				Vector2 vector = new Vector2(moveAmount.x, moveAmount.y);
				float magnitude = vector.magnitude;
				if (magnitude < maxTapMagnitude)
				{
					currentState.Buttons |= 1048576u;
					currentState.Buttons |= 1073741824u;
				}
				else if (magnitude >= minMoveMagnitude)
				{
					vector.Normalize();
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						if (vector.x < 0f)
						{
							currentState.Buttons |= 262144u;
						}
						else
						{
							currentState.Buttons |= 524288u;
						}
					}
					else if (vector.y < 0f)
					{
						currentState.Buttons |= 131072u;
					}
					else
					{
						currentState.Buttons |= 65536u;
					}
				}
			}
			return result;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.Start;
			buttonMap.Two = RawButton.Back;
			buttonMap.Three = RawButton.None;
			buttonMap.Four = RawButton.None;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.None;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.None;
			buttonMap.PrimaryThumbstickUp = RawButton.None;
			buttonMap.PrimaryThumbstickDown = RawButton.None;
			buttonMap.PrimaryThumbstickLeft = RawButton.None;
			buttonMap.PrimaryThumbstickRight = RawButton.None;
			buttonMap.PrimaryTouchpad = RawButton.LTouchpad;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.None;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.None;
			buttonMap.SecondaryThumbstickUp = RawButton.None;
			buttonMap.SecondaryThumbstickDown = RawButton.None;
			buttonMap.SecondaryThumbstickLeft = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.DpadUp;
			buttonMap.Down = RawButton.DpadDown;
			buttonMap.Left = RawButton.DpadLeft;
			buttonMap.Right = RawButton.DpadRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.LTouchpad;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad = RawAxis2D.LTouchpad;
			axis2DMap.SecondaryThumbstick = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}
	}

	private class OVRControllerLTrackedRemote : OVRControllerBase
	{
		private bool emitSwipe;

		private OVRPlugin.Vector2f moveAmount;

		private float minMoveMagnitude = 0.3f;

		public OVRControllerLTrackedRemote()
		{
			controllerType = Controller.LTrackedRemote;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.LTouchpad;
			buttonMap.Two = RawButton.Back;
			buttonMap.Three = RawButton.None;
			buttonMap.Four = RawButton.None;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.None;
			buttonMap.PrimaryThumbstickUp = RawButton.None;
			buttonMap.PrimaryThumbstickDown = RawButton.None;
			buttonMap.PrimaryThumbstickLeft = RawButton.None;
			buttonMap.PrimaryThumbstickRight = RawButton.None;
			buttonMap.PrimaryTouchpad = RawButton.LTouchpad;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.None;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.None;
			buttonMap.SecondaryThumbstickUp = RawButton.None;
			buttonMap.SecondaryThumbstickDown = RawButton.None;
			buttonMap.SecondaryThumbstickLeft = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.DpadUp;
			buttonMap.Down = RawButton.DpadDown;
			buttonMap.Left = RawButton.DpadLeft;
			buttonMap.Right = RawButton.DpadRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.LTouchpad;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad = RawAxis2D.LTouchpad;
			axis2DMap.SecondaryThumbstick = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}

		public override Controller Update()
		{
			Controller result = base.Update();
			if (GetDown(RawTouch.LTouchpad, Controller.LTrackedRemote))
			{
				emitSwipe = true;
				moveAmount = currentState.LTouchpad;
			}
			if (GetDown(RawButton.LTouchpad, Controller.LTrackedRemote))
			{
				emitSwipe = false;
			}
			if (GetUp(RawTouch.LTouchpad, Controller.LTrackedRemote) && emitSwipe)
			{
				emitSwipe = false;
				moveAmount.x = previousState.LTouchpad.x - moveAmount.x;
				moveAmount.y = previousState.LTouchpad.y - moveAmount.y;
				Vector2 vector = new Vector2(moveAmount.x, moveAmount.y);
				if (vector.magnitude >= minMoveMagnitude)
				{
					vector.Normalize();
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						if (vector.x < 0f)
						{
							currentState.Buttons |= 262144u;
						}
						else
						{
							currentState.Buttons |= 524288u;
						}
					}
					else if (vector.y < 0f)
					{
						currentState.Buttons |= 131072u;
					}
					else
					{
						currentState.Buttons |= 65536u;
					}
				}
			}
			return result;
		}
	}

	private class OVRControllerRTrackedRemote : OVRControllerBase
	{
		private bool emitSwipe;

		private OVRPlugin.Vector2f moveAmount;

		private float minMoveMagnitude = 0.3f;

		public OVRControllerRTrackedRemote()
		{
			controllerType = Controller.RTrackedRemote;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None = RawButton.None;
			buttonMap.One = RawButton.RTouchpad;
			buttonMap.Two = RawButton.Back;
			buttonMap.Three = RawButton.None;
			buttonMap.Four = RawButton.None;
			buttonMap.Start = RawButton.Start;
			buttonMap.Back = RawButton.Back;
			buttonMap.PrimaryShoulder = RawButton.None;
			buttonMap.PrimaryIndexTrigger = RawButton.RIndexTrigger;
			buttonMap.PrimaryHandTrigger = RawButton.None;
			buttonMap.PrimaryThumbstick = RawButton.None;
			buttonMap.PrimaryThumbstickUp = RawButton.None;
			buttonMap.PrimaryThumbstickDown = RawButton.None;
			buttonMap.PrimaryThumbstickLeft = RawButton.None;
			buttonMap.PrimaryThumbstickRight = RawButton.None;
			buttonMap.PrimaryTouchpad = RawButton.RTouchpad;
			buttonMap.SecondaryShoulder = RawButton.None;
			buttonMap.SecondaryIndexTrigger = RawButton.None;
			buttonMap.SecondaryHandTrigger = RawButton.None;
			buttonMap.SecondaryThumbstick = RawButton.None;
			buttonMap.SecondaryThumbstickUp = RawButton.None;
			buttonMap.SecondaryThumbstickDown = RawButton.None;
			buttonMap.SecondaryThumbstickLeft = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad = RawButton.None;
			buttonMap.DpadUp = RawButton.DpadUp;
			buttonMap.DpadDown = RawButton.DpadDown;
			buttonMap.DpadLeft = RawButton.DpadLeft;
			buttonMap.DpadRight = RawButton.DpadRight;
			buttonMap.Up = RawButton.DpadUp;
			buttonMap.Down = RawButton.DpadDown;
			buttonMap.Left = RawButton.DpadLeft;
			buttonMap.Right = RawButton.DpadRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None = RawTouch.None;
			touchMap.One = RawTouch.None;
			touchMap.Two = RawTouch.None;
			touchMap.Three = RawTouch.None;
			touchMap.Four = RawTouch.None;
			touchMap.PrimaryIndexTrigger = RawTouch.None;
			touchMap.PrimaryThumbstick = RawTouch.None;
			touchMap.PrimaryThumbRest = RawTouch.None;
			touchMap.PrimaryTouchpad = RawTouch.RTouchpad;
			touchMap.SecondaryIndexTrigger = RawTouch.None;
			touchMap.SecondaryThumbstick = RawTouch.None;
			touchMap.SecondaryThumbRest = RawTouch.None;
			touchMap.SecondaryTouchpad = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad = RawAxis2D.RTouchpad;
			axis2DMap.SecondaryThumbstick = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad = RawAxis2D.None;
		}

		public override Controller Update()
		{
			Controller result = base.Update();
			if (GetDown(RawTouch.RTouchpad, Controller.RTrackedRemote))
			{
				emitSwipe = true;
				moveAmount = currentState.RTouchpad;
			}
			if (GetDown(RawButton.RTouchpad, Controller.RTrackedRemote))
			{
				emitSwipe = false;
			}
			if (GetUp(RawTouch.RTouchpad, Controller.RTrackedRemote) && emitSwipe)
			{
				emitSwipe = false;
				moveAmount.x = previousState.RTouchpad.x - moveAmount.x;
				moveAmount.y = previousState.RTouchpad.y - moveAmount.y;
				Vector2 vector = new Vector2(moveAmount.x, moveAmount.y);
				if (vector.magnitude >= minMoveMagnitude)
				{
					vector.Normalize();
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						if (vector.x < 0f)
						{
							currentState.Buttons |= 262144u;
						}
						else
						{
							currentState.Buttons |= 524288u;
						}
					}
					else if (vector.y < 0f)
					{
						currentState.Buttons |= 131072u;
					}
					else
					{
						currentState.Buttons |= 65536u;
					}
				}
			}
			return result;
		}
	}

	private static readonly float AXIS_AS_BUTTON_THRESHOLD;

	private static readonly float AXIS_DEADZONE_THRESHOLD;

	private static List<OVRControllerBase> controllers;

	private static Controller activeControllerType;

	private static Controller connectedControllerTypes;

	private static OVRPlugin.Step stepType;

	private static int fixedUpdateCount;

	private static bool _pluginSupportsActiveController;

	private static bool _pluginSupportsActiveControllerCached;

	private static Version _pluginSupportsActiveControllerMinVersion;

	private static bool pluginSupportsActiveController
	{
		get
		{
			if (!_pluginSupportsActiveControllerCached)
			{
				_pluginSupportsActiveController = true && OVRPlugin.version >= _pluginSupportsActiveControllerMinVersion;
				_pluginSupportsActiveControllerCached = true;
			}
			return _pluginSupportsActiveController;
		}
	}

	static OVRInput()
	{
		AXIS_AS_BUTTON_THRESHOLD = 0.5f;
		AXIS_DEADZONE_THRESHOLD = 0.2f;
		activeControllerType = Controller.None;
		connectedControllerTypes = Controller.None;
		stepType = OVRPlugin.Step.Render;
		fixedUpdateCount = 0;
		_pluginSupportsActiveController = false;
		_pluginSupportsActiveControllerCached = false;
		_pluginSupportsActiveControllerMinVersion = new Version(1, 9, 0);
		controllers = new List<OVRControllerBase>
		{
			new OVRControllerGamepadPC(),
			new OVRControllerTouch(),
			new OVRControllerLTouch(),
			new OVRControllerRTouch(),
			new OVRControllerRemote()
		};
	}

	public static void Update()
	{
		connectedControllerTypes = Controller.None;
		stepType = OVRPlugin.Step.Render;
		fixedUpdateCount = 0;
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			connectedControllerTypes |= oVRControllerBase.Update();
			if ((connectedControllerTypes & oVRControllerBase.controllerType) != 0 && (Get(RawButton.Any, oVRControllerBase.controllerType) || Get(RawTouch.Any, oVRControllerBase.controllerType)))
			{
				activeControllerType = oVRControllerBase.controllerType;
			}
		}
		if (activeControllerType == Controller.LTouch || activeControllerType == Controller.RTouch)
		{
			activeControllerType = Controller.Touch;
		}
		if ((connectedControllerTypes & activeControllerType) == 0)
		{
			activeControllerType = Controller.None;
		}
		if (activeControllerType == Controller.None)
		{
			if ((connectedControllerTypes & Controller.RTrackedRemote) != 0)
			{
				activeControllerType = Controller.RTrackedRemote;
			}
			else if ((connectedControllerTypes & Controller.LTrackedRemote) != 0)
			{
				activeControllerType = Controller.LTrackedRemote;
			}
		}
		if (pluginSupportsActiveController)
		{
			connectedControllerTypes = (Controller)OVRPlugin.GetConnectedControllers();
			activeControllerType = (Controller)OVRPlugin.GetActiveController();
		}
	}

	public static void FixedUpdate()
	{
		stepType = OVRPlugin.Step.Physics;
		double predictionSeconds = (double)fixedUpdateCount * (double)Time.fixedDeltaTime / (double)Mathf.Max(Time.timeScale, 1E-06f);
		fixedUpdateCount++;
		OVRPlugin.UpdateNodePhysicsPoses(0, predictionSeconds);
	}

	public static bool GetControllerOrientationTracked(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.HandLeft);
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.HandRight);
		default:
			return false;
		}
	}

	public static bool GetControllerPositionTracked(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.HandLeft);
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.HandRight);
		default:
			return false;
		}
	}

	public static Vector3 GetLocalControllerPosition(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, stepType).ToOVRPose().position;
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, stepType).ToOVRPose().position;
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 GetLocalControllerVelocity(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodeVelocity(OVRPlugin.Node.HandLeft, stepType).FromFlippedZVector3f();
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodeVelocity(OVRPlugin.Node.HandRight, stepType).FromFlippedZVector3f();
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 GetLocalControllerAcceleration(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodeAcceleration(OVRPlugin.Node.HandLeft, stepType).FromFlippedZVector3f();
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodeAcceleration(OVRPlugin.Node.HandRight, stepType).FromFlippedZVector3f();
		default:
			return Vector3.zero;
		}
	}

	public static Quaternion GetLocalControllerRotation(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, stepType).ToOVRPose().orientation;
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, stepType).ToOVRPose().orientation;
		default:
			return Quaternion.identity;
		}
	}

	public static Vector3 GetLocalControllerAngularVelocity(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodeAngularVelocity(OVRPlugin.Node.HandLeft, stepType).FromFlippedZVector3f();
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodeAngularVelocity(OVRPlugin.Node.HandRight, stepType).FromFlippedZVector3f();
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 GetLocalControllerAngularAcceleration(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.LTouch:
		case Controller.LTrackedRemote:
			return OVRPlugin.GetNodeAngularAcceleration(OVRPlugin.Node.HandLeft, stepType).FromFlippedZVector3f();
		case Controller.RTouch:
		case Controller.RTrackedRemote:
			return OVRPlugin.GetNodeAngularAcceleration(OVRPlugin.Node.HandRight, stepType).FromFlippedZVector3f();
		default:
			return Vector3.zero;
		}
	}

	public static bool Get(Button virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButton(virtualMask, RawButton.None, controllerMask);
	}

	public static bool Get(RawButton rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButton(Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButton(Button virtualMask, RawButton rawMask, Controller controllerMask)
	{
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawButton rawButton = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.currentState.Buttons & (uint)rawButton) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetDown(Button virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonDown(virtualMask, RawButton.None, controllerMask);
	}

	public static bool GetDown(RawButton rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonDown(Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButtonDown(Button virtualMask, RawButton rawMask, Controller controllerMask)
	{
		bool result = false;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawButton rawButton = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.previousState.Buttons & (uint)rawButton) != 0)
				{
					return false;
				}
				if ((oVRControllerBase.currentState.Buttons & (uint)rawButton) != 0 && (oVRControllerBase.previousState.Buttons & (uint)rawButton) == 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool GetUp(Button virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonUp(virtualMask, RawButton.None, controllerMask);
	}

	public static bool GetUp(RawButton rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonUp(Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButtonUp(Button virtualMask, RawButton rawMask, Controller controllerMask)
	{
		bool result = false;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawButton rawButton = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.currentState.Buttons & (uint)rawButton) != 0)
				{
					return false;
				}
				if ((oVRControllerBase.currentState.Buttons & (uint)rawButton) == 0 && (oVRControllerBase.previousState.Buttons & (uint)rawButton) != 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool Get(Touch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouch(virtualMask, RawTouch.None, controllerMask);
	}

	public static bool Get(RawTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouch(Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouch(Touch virtualMask, RawTouch rawMask, Controller controllerMask)
	{
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawTouch rawTouch = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.currentState.Touches & (uint)rawTouch) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetDown(Touch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchDown(virtualMask, RawTouch.None, controllerMask);
	}

	public static bool GetDown(RawTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchDown(Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouchDown(Touch virtualMask, RawTouch rawMask, Controller controllerMask)
	{
		bool result = false;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawTouch rawTouch = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.previousState.Touches & (uint)rawTouch) != 0)
				{
					return false;
				}
				if ((oVRControllerBase.currentState.Touches & (uint)rawTouch) != 0 && (oVRControllerBase.previousState.Touches & (uint)rawTouch) == 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool GetUp(Touch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchUp(virtualMask, RawTouch.None, controllerMask);
	}

	public static bool GetUp(RawTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchUp(Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouchUp(Touch virtualMask, RawTouch rawMask, Controller controllerMask)
	{
		bool result = false;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawTouch rawTouch = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.currentState.Touches & (uint)rawTouch) != 0)
				{
					return false;
				}
				if ((oVRControllerBase.currentState.Touches & (uint)rawTouch) == 0 && (oVRControllerBase.previousState.Touches & (uint)rawTouch) != 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool Get(NearTouch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouch(virtualMask, RawNearTouch.None, controllerMask);
	}

	public static bool Get(RawNearTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouch(NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouch(NearTouch virtualMask, RawNearTouch rawMask, Controller controllerMask)
	{
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawNearTouch rawNearTouch = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.currentState.NearTouches & (uint)rawNearTouch) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetDown(NearTouch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchDown(virtualMask, RawNearTouch.None, controllerMask);
	}

	public static bool GetDown(RawNearTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchDown(NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouchDown(NearTouch virtualMask, RawNearTouch rawMask, Controller controllerMask)
	{
		bool result = false;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawNearTouch rawNearTouch = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.previousState.NearTouches & (uint)rawNearTouch) != 0)
				{
					return false;
				}
				if ((oVRControllerBase.currentState.NearTouches & (uint)rawNearTouch) != 0 && (oVRControllerBase.previousState.NearTouches & (uint)rawNearTouch) == 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool GetUp(NearTouch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchUp(virtualMask, RawNearTouch.None, controllerMask);
	}

	public static bool GetUp(RawNearTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchUp(NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouchUp(NearTouch virtualMask, RawNearTouch rawMask, Controller controllerMask)
	{
		bool result = false;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				RawNearTouch rawNearTouch = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
				if ((oVRControllerBase.currentState.NearTouches & (uint)rawNearTouch) != 0)
				{
					return false;
				}
				if ((oVRControllerBase.currentState.NearTouches & (uint)rawNearTouch) == 0 && (oVRControllerBase.previousState.NearTouches & (uint)rawNearTouch) != 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static float Get(Axis1D virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis1D(virtualMask, RawAxis1D.None, controllerMask);
	}

	public static float Get(RawAxis1D rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis1D(Axis1D.None, rawMask, controllerMask);
	}

	private static float GetResolvedAxis1D(Axis1D virtualMask, RawAxis1D rawMask, Controller controllerMask)
	{
		float num = 0f;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (!ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				continue;
			}
			RawAxis1D rawAxis1D = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
			if ((RawAxis1D.LIndexTrigger & rawAxis1D) != 0)
			{
				float num2 = oVRControllerBase.currentState.LIndexTrigger;
				if (oVRControllerBase.shouldApplyDeadzone)
				{
					num2 = CalculateDeadzone(num2, AXIS_DEADZONE_THRESHOLD);
				}
				num = CalculateAbsMax(num, num2);
			}
			if ((RawAxis1D.RIndexTrigger & rawAxis1D) != 0)
			{
				float num3 = oVRControllerBase.currentState.RIndexTrigger;
				if (oVRControllerBase.shouldApplyDeadzone)
				{
					num3 = CalculateDeadzone(num3, AXIS_DEADZONE_THRESHOLD);
				}
				num = CalculateAbsMax(num, num3);
			}
			if ((RawAxis1D.LHandTrigger & rawAxis1D) != 0)
			{
				float num4 = oVRControllerBase.currentState.LHandTrigger;
				if (oVRControllerBase.shouldApplyDeadzone)
				{
					num4 = CalculateDeadzone(num4, AXIS_DEADZONE_THRESHOLD);
				}
				num = CalculateAbsMax(num, num4);
			}
			if ((RawAxis1D.RHandTrigger & rawAxis1D) != 0)
			{
				float num5 = oVRControllerBase.currentState.RHandTrigger;
				if (oVRControllerBase.shouldApplyDeadzone)
				{
					num5 = CalculateDeadzone(num5, AXIS_DEADZONE_THRESHOLD);
				}
				num = CalculateAbsMax(num, num5);
			}
		}
		return num;
	}

	public static Vector2 Get(Axis2D virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis2D(virtualMask, RawAxis2D.None, controllerMask);
	}

	public static Vector2 Get(RawAxis2D rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis2D(Axis2D.None, rawMask, controllerMask);
	}

	private static Vector2 GetResolvedAxis2D(Axis2D virtualMask, RawAxis2D rawMask, Controller controllerMask)
	{
		Vector2 vector = Vector2.zero;
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (!ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				continue;
			}
			RawAxis2D rawAxis2D = rawMask | oVRControllerBase.ResolveToRawMask(virtualMask);
			if ((RawAxis2D.LThumbstick & rawAxis2D) != 0)
			{
				Vector2 vector2 = new Vector2(oVRControllerBase.currentState.LThumbstick.x, oVRControllerBase.currentState.LThumbstick.y);
				if (oVRControllerBase.shouldApplyDeadzone)
				{
					vector2 = CalculateDeadzone(vector2, AXIS_DEADZONE_THRESHOLD);
				}
				vector = CalculateAbsMax(vector, vector2);
			}
			if ((RawAxis2D.LTouchpad & rawAxis2D) != 0)
			{
				Vector2 b = new Vector2(oVRControllerBase.currentState.LTouchpad.x, oVRControllerBase.currentState.LTouchpad.y);
				vector = CalculateAbsMax(vector, b);
			}
			if ((RawAxis2D.RThumbstick & rawAxis2D) != 0)
			{
				Vector2 vector3 = new Vector2(oVRControllerBase.currentState.RThumbstick.x, oVRControllerBase.currentState.RThumbstick.y);
				if (oVRControllerBase.shouldApplyDeadzone)
				{
					vector3 = CalculateDeadzone(vector3, AXIS_DEADZONE_THRESHOLD);
				}
				vector = CalculateAbsMax(vector, vector3);
			}
			if ((RawAxis2D.RTouchpad & rawAxis2D) != 0)
			{
				Vector2 b2 = new Vector2(oVRControllerBase.currentState.RTouchpad.x, oVRControllerBase.currentState.RTouchpad.y);
				vector = CalculateAbsMax(vector, b2);
			}
		}
		return vector;
	}

	public static Controller GetConnectedControllers()
	{
		return connectedControllerTypes;
	}

	public static bool IsControllerConnected(Controller controller)
	{
		return (connectedControllerTypes & controller) == controller;
	}

	public static Controller GetActiveController()
	{
		return activeControllerType;
	}

	public static void SetControllerVibration(float frequency, float amplitude, Controller controllerMask = Controller.Active)
	{
		if (((uint)controllerMask & 0x80000000u) != 0)
		{
			controllerMask |= activeControllerType;
		}
		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase oVRControllerBase = controllers[i];
			if (ShouldResolveController(oVRControllerBase.controllerType, controllerMask))
			{
				oVRControllerBase.SetControllerVibration(frequency, amplitude);
			}
		}
	}

	private static Vector2 CalculateAbsMax(Vector2 a, Vector2 b)
	{
		float sqrMagnitude = a.sqrMagnitude;
		float sqrMagnitude2 = b.sqrMagnitude;
		if (sqrMagnitude >= sqrMagnitude2)
		{
			return a;
		}
		return b;
	}

	private static float CalculateAbsMax(float a, float b)
	{
		float num = ((!(a >= 0f)) ? (0f - a) : a);
		float num2 = ((!(b >= 0f)) ? (0f - b) : b);
		if (num >= num2)
		{
			return a;
		}
		return b;
	}

	private static Vector2 CalculateDeadzone(Vector2 a, float deadzone)
	{
		if (a.sqrMagnitude <= deadzone * deadzone)
		{
			return Vector2.zero;
		}
		a *= (a.magnitude - deadzone) / (1f - deadzone);
		if (a.sqrMagnitude > 1f)
		{
			return a.normalized;
		}
		return a;
	}

	private static float CalculateDeadzone(float a, float deadzone)
	{
		float num = ((!(a >= 0f)) ? (0f - a) : a);
		if (num <= deadzone)
		{
			return 0f;
		}
		a *= (num - deadzone) / (1f - deadzone);
		if (a * a > 1f)
		{
			return (!(a >= 0f)) ? (-1f) : 1f;
		}
		return a;
	}

	private static bool ShouldResolveController(Controller controllerType, Controller controllerMask)
	{
		bool result = false;
		if ((controllerType & controllerMask) == controllerType)
		{
			result = true;
		}
		if ((controllerMask & Controller.Touch) == Controller.Touch && (controllerType & Controller.Touch) != 0 && (controllerType & Controller.Touch) != Controller.Touch)
		{
			result = false;
		}
		return result;
	}
}
