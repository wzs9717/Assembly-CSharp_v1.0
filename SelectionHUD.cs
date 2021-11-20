using UnityEngine;
using UnityEngine.UI;

public class SelectionHUD : MonoBehaviour
{
	public Text headerText;

	public Text[] selectionTexts;

	public Material selectionLineMaterial;

	public Material firstSelectionLineMaterial;

	public Vector3 drawFrom;

	public bool useDrawFromPosition;

	protected LineDrawer[] selectionLineDrawers;

	protected Transform[] selections;

	public int numSlots
	{
		get
		{
			if (selectionTexts != null)
			{
				return selectionTexts.Length;
			}
			return 0;
		}
	}

	public void ClearSelections()
	{
		for (int i = 0; i < selectionTexts.Length; i++)
		{
			SetSelection(i, null, string.Empty);
		}
	}

	public void SetSelection(int index, Transform selection, string name)
	{
		if (index < selectionTexts.Length)
		{
			selectionTexts[index].text = name;
			selections[index] = selection;
		}
	}

	private void Awake()
	{
		selectionLineDrawers = new LineDrawer[selectionTexts.Length];
		selections = new Transform[selectionTexts.Length];
		for (int i = 0; i < selectionTexts.Length; i++)
		{
			if (i == 0 && firstSelectionLineMaterial != null)
			{
				selectionLineDrawers[i] = new LineDrawer(firstSelectionLineMaterial);
			}
			else if (selectionLineMaterial != null)
			{
				selectionLineDrawers[i] = new LineDrawer(selectionLineMaterial);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		for (int i = 0; i < selectionTexts.Length; i++)
		{
			if (selections[i] != null && selectionLineDrawers[i] != null)
			{
				if (useDrawFromPosition)
				{
					selectionLineDrawers[i].SetLinePoints(drawFrom, selections[i].position);
				}
				else
				{
					selectionLineDrawers[i].SetLinePoints(selectionTexts[i].transform.position, selections[i].position);
				}
				selectionLineDrawers[i].Draw(base.gameObject.layer);
			}
		}
	}
}
