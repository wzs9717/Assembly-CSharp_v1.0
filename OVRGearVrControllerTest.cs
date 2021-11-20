using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OVRGearVrControllerTest : MonoBehaviour
{
	public class BoolMonitor
	{
		public delegate bool BoolGenerator();

		private string m_name = string.Empty;

		private BoolGenerator m_generator;

		private bool m_currentValue;

		private float m_displayTimeout;

		private float m_displayTimer;

		public BoolMonitor(string name, BoolGenerator generator, float displayTimeout = 0.5f)
		{
			m_name = name;
			m_generator = generator;
			m_displayTimeout = displayTimeout;
		}

		public void Update()
		{
			m_currentValue = m_generator();
			if (m_currentValue)
			{
				m_displayTimer = m_displayTimeout;
			}
			m_displayTimer -= Time.deltaTime;
			if (m_displayTimer < 0f)
			{
				m_displayTimer = 0f;
			}
		}

		public override string ToString()
		{
			return "<b>" + m_name + ": </b>" + ((!(m_displayTimer > 0f)) ? "<color=black>" : "<color=red>") + m_currentValue + "</color>";
		}
	}

	public Text uiText;

	private List<BoolMonitor> monitors;

	private void Start()
	{
		if (uiText != null)
		{
			uiText.supportRichText = true;
		}
		monitors = new List<BoolMonitor>
		{
			new BoolMonitor("One", () => OVRInput.Get(OVRInput.Button.One)),
			new BoolMonitor("OneDown", () => OVRInput.GetDown(OVRInput.Button.One)),
			new BoolMonitor("OneUp", () => OVRInput.GetUp(OVRInput.Button.One)),
			new BoolMonitor("Two", () => OVRInput.Get(OVRInput.Button.Two)),
			new BoolMonitor("TwoDown", () => OVRInput.GetDown(OVRInput.Button.Two)),
			new BoolMonitor("TwoUp", () => OVRInput.GetUp(OVRInput.Button.Two)),
			new BoolMonitor("PrimaryIndexTrigger", () => OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)),
			new BoolMonitor("PrimaryIndexTriggerDown", () => OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)),
			new BoolMonitor("PrimaryIndexTriggerUp", () => OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)),
			new BoolMonitor("Up", () => OVRInput.Get(OVRInput.Button.Up)),
			new BoolMonitor("Down", () => OVRInput.Get(OVRInput.Button.Down)),
			new BoolMonitor("Left", () => OVRInput.Get(OVRInput.Button.Left)),
			new BoolMonitor("Right", () => OVRInput.Get(OVRInput.Button.Right)),
			new BoolMonitor("Touchpad (Touch)", () => OVRInput.Get(OVRInput.Touch.PrimaryTouchpad)),
			new BoolMonitor("TouchpadDown (Touch)", () => OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad)),
			new BoolMonitor("TouchpadUp (Touch)", () => OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad)),
			new BoolMonitor("Touchpad (Click)", () => OVRInput.Get(OVRInput.Button.PrimaryTouchpad)),
			new BoolMonitor("TouchpadDown (Click)", () => OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad)),
			new BoolMonitor("TouchpadUp (Click)", () => OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad)),
			new BoolMonitor("Start", () => OVRInput.Get(OVRInput.RawButton.Start)),
			new BoolMonitor("StartDown", () => OVRInput.GetDown(OVRInput.RawButton.Start)),
			new BoolMonitor("StartUp", () => OVRInput.GetUp(OVRInput.RawButton.Start)),
			new BoolMonitor("Back", () => OVRInput.Get(OVRInput.RawButton.Back)),
			new BoolMonitor("BackDown", () => OVRInput.GetDown(OVRInput.RawButton.Back)),
			new BoolMonitor("BackUp", () => OVRInput.GetUp(OVRInput.RawButton.Back)),
			new BoolMonitor("A", () => OVRInput.Get(OVRInput.RawButton.A)),
			new BoolMonitor("ADown", () => OVRInput.GetDown(OVRInput.RawButton.A)),
			new BoolMonitor("AUp", () => OVRInput.GetUp(OVRInput.RawButton.A))
		};
	}

	private void Update()
	{
		string text = string.Concat("<b>Active: </b>", OVRInput.GetActiveController(), "\n<b>Connected: </b>", OVRInput.GetConnectedControllers(), "\n");
		string text2 = text;
		text = string.Concat(text2, "Orientation: ", OVRInput.GetLocalControllerRotation(OVRInput.GetActiveController()), "\n");
		text2 = text;
		text = string.Concat(text2, "AngVel: ", OVRInput.GetLocalControllerAngularVelocity(OVRInput.GetActiveController()), "\n");
		text2 = text;
		text = string.Concat(text2, "AngAcc: ", OVRInput.GetLocalControllerAngularAcceleration(OVRInput.GetActiveController()), "\n");
		text2 = text;
		text = string.Concat(text2, "Position: ", OVRInput.GetLocalControllerPosition(OVRInput.GetActiveController()), "\n");
		text2 = text;
		text = string.Concat(text2, "Vel: ", OVRInput.GetLocalControllerVelocity(OVRInput.GetActiveController()), "\n");
		text2 = text;
		text = string.Concat(text2, "Acc: ", OVRInput.GetLocalControllerAcceleration(OVRInput.GetActiveController()), "\n");
		text2 = text;
		text = string.Concat(text2, "PrimaryTouchPad: ", OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad), "\n");
		text2 = text;
		text = string.Concat(text2, "SecondaryTouchPad: ", OVRInput.Get(OVRInput.Axis2D.SecondaryTouchpad), "\n");
		for (int i = 0; i < monitors.Count; i++)
		{
			monitors[i].Update();
			text = text + monitors[i].ToString() + "\n";
		}
		if (uiText != null)
		{
			uiText.text = text;
		}
	}
}
