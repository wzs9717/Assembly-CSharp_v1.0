using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PerfMon : MonoBehaviour
{
	public static float physicsTime;

	public static float scriptsTime;

	public static float preRenderTime;

	public static float renderTime;

	public static float totalTime;

	public Toggle onToggle;

	[SerializeField]
	protected bool _on;

	public Transform perfMonUI;

	public Text totalTimeText;

	public Text scriptsTimeText;

	public Text renderTimeText;

	public Text physicsTimeText;

	public Text avgTotalTimeText;

	public Text avgScriptsTimeText;

	public Text avgRenderTimeText;

	public Text avgPhysicsTimeText;

	public int framesBetweenUpdate = 10;

	public float _frameStartTime;

	public float _frameStopTime;

	public float _preRenderStopTime;

	protected float _internalPhysicsStopTime;

	public float _physicsTime;

	public float _internalPhysicsTime;

	public float _totalTime;

	public float _preRenderTime;

	public float _scriptsTime;

	public float _renderTime;

	protected float _totPhysicsTime;

	protected float _totIntenalPhysicsTime;

	protected float _totTotalTime;

	protected float _totRenderTime;

	protected float _totScriptsTime;

	protected int _totFrames;

	public float _avgPhysicsTime;

	public float _avgInternalPhysicsTime;

	public float _avgTotalTime;

	public float _avgRenderTime;

	public float _avgScriptsTime;

	public int avgCalcStartFrame = -1;

	public int avgCalcNumFrames = 900;

	protected int cnt;

	public bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on != value)
			{
				_on = value;
				if (perfMonUI != null)
				{
					perfMonUI.gameObject.SetActive(_on);
				}
				if (onToggle != null)
				{
					onToggle.isOn = _on;
				}
			}
		}
	}

	public void RestartAverageCalc()
	{
		Debug.Log("RestartAverageCalc()");
		avgCalcStartFrame = _totFrames;
		_totPhysicsTime = 0f;
		_totIntenalPhysicsTime = 0f;
		_totScriptsTime = 0f;
		_totTotalTime = 0f;
		_totRenderTime = 0f;
	}

	protected void DoUpdate()
	{
		_frameStopTime = GlobalStopwatch.GetElapsedMilliseconds();
		_renderTime = _frameStopTime - PerfMonCamera.renderStartTime;
		_totalTime = _preRenderTime + _renderTime;
		physicsTime = _physicsTime;
		scriptsTime = _scriptsTime;
		preRenderTime = _preRenderTime;
		renderTime = _renderTime;
		totalTime = _totalTime;
		_totFrames++;
		int num = avgCalcStartFrame + avgCalcNumFrames;
		if (_totFrames > avgCalcStartFrame && _totFrames <= num)
		{
			_totPhysicsTime += _physicsTime;
			_totIntenalPhysicsTime += _internalPhysicsTime;
			_totScriptsTime += _scriptsTime;
			_totTotalTime += _totalTime;
			_totRenderTime += _renderTime;
			float num2 = 1f / (float)(_totFrames - avgCalcStartFrame);
			_avgPhysicsTime = _totPhysicsTime * num2;
			_avgInternalPhysicsTime = _totIntenalPhysicsTime * num2;
			_avgScriptsTime = _totScriptsTime * num2;
			_avgRenderTime = _totRenderTime * num2;
			_avgTotalTime = _totTotalTime * num2;
			if (_totFrames == num)
			{
				Debug.Log("Benchmark complete. Avg. tot time: " + _avgTotalTime.ToString("F2") + " Avg. physics time: " + _avgPhysicsTime.ToString("F2") + " Avg. internal physics time: " + _avgInternalPhysicsTime.ToString("F2") + " Avg. scripts time: " + _avgScriptsTime.ToString("F2") + " Avg. render time: " + _avgRenderTime.ToString("F2"));
			}
		}
		if (cnt == 0)
		{
			if (totalTimeText != null)
			{
				totalTimeText.text = _totalTime.ToString("F2");
			}
			if (renderTimeText != null)
			{
				renderTimeText.text = _renderTime.ToString("F2");
			}
			if (scriptsTimeText != null)
			{
				scriptsTimeText.text = _scriptsTime.ToString("F2");
			}
			if (physicsTimeText != null)
			{
				physicsTimeText.text = _physicsTime.ToString("F2");
			}
			if (avgTotalTimeText != null)
			{
				avgTotalTimeText.text = _avgTotalTime.ToString("F2");
			}
			if (avgRenderTimeText != null)
			{
				avgRenderTimeText.text = _avgRenderTime.ToString("F2");
			}
			if (avgScriptsTimeText != null)
			{
				avgScriptsTimeText.text = _avgScriptsTime.ToString("F2");
			}
			if (avgPhysicsTimeText != null)
			{
				avgPhysicsTimeText.text = _avgPhysicsTime.ToString("F2");
			}
		}
	}

	private void FixedUpdate()
	{
		_internalPhysicsStopTime = GlobalStopwatch.GetElapsedMilliseconds();
		_internalPhysicsTime = _internalPhysicsStopTime - PerfMonPre.physicsStartTime;
	}

	private void LateUpdate()
	{
		_frameStartTime = PerfMonPre.frameStartTime;
		_preRenderStopTime = GlobalStopwatch.GetElapsedMilliseconds();
		_preRenderTime = _preRenderStopTime - _frameStartTime;
		_physicsTime = PerfMonPre.physicsTime;
		_scriptsTime = _preRenderTime - _physicsTime;
		cnt++;
		if (cnt == framesBetweenUpdate)
		{
			cnt = 0;
		}
	}

	public IEnumerator Start()
	{
		if (perfMonUI != null)
		{
			perfMonUI.gameObject.SetActive(_on);
		}
		if (onToggle != null)
		{
			onToggle.isOn = _on;
			onToggle.onValueChanged.AddListener(delegate
			{
				on = onToggle.isOn;
			});
		}
		while (true)
		{
			yield return new WaitForEndOfFrame();
			DoUpdate();
		}
	}
}
