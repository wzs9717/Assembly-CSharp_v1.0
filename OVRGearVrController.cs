using UnityEngine;

public class OVRGearVrController : MonoBehaviour
{
	public GameObject m_model;

	public OVRInput.Controller m_controller;

	private bool m_prevControllerConnected;

	private bool m_prevControllerConnectedCached;

	private void Update()
	{
		bool flag = OVRInput.IsControllerConnected(m_controller);
		if (flag != m_prevControllerConnected || !m_prevControllerConnectedCached)
		{
			m_model.SetActive(flag);
			m_prevControllerConnected = flag;
			m_prevControllerConnectedCached = true;
		}
		if (flag)
		{
		}
	}
}
