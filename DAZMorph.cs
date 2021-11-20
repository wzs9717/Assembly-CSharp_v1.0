using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJSON;
using UnityEngine;

[Serializable]
public class DAZMorph
{
	private const float geoScale = 0.01f;

	public bool visible;

	public bool preserveValueOnReimport;

	public bool disable;

	public bool isPoseControl;

	public string morphName;

	public string displayName;

	public string region;

	public string group;

	public float importValue;

	public float startValue;

	[SerializeField]
	private float _morphValue;

	public float appliedValue;

	public float min;

	public float max;

	public int numDeltas;

	public bool triggerNormalRecalc = true;

	public bool triggerTangentRecalc = true;

	public DAZMorphVertex[] deltas;

	public DAZMorphFormula[] formulas;

	public float morphValue
	{
		get
		{
			return _morphValue;
		}
		set
		{
			_morphValue = value;
		}
	}

	public DAZMorph()
	{
	}

	public DAZMorph(DAZMorph copyFrom)
	{
		CopyParameters(copyFrom);
		numDeltas = copyFrom.numDeltas;
		deltas = new DAZMorphVertex[copyFrom.deltas.Length];
		for (int i = 0; i < deltas.Length; i++)
		{
			deltas[i] = new DAZMorphVertex();
			deltas[i].vertex = copyFrom.deltas[i].vertex;
			deltas[i].delta = copyFrom.deltas[i].delta;
		}
		formulas = new DAZMorphFormula[copyFrom.formulas.Length];
		for (int j = 0; j < formulas.Length; j++)
		{
			formulas[j] = new DAZMorphFormula();
			formulas[j].targetType = copyFrom.formulas[j].targetType;
			formulas[j].target = copyFrom.formulas[j].target;
			formulas[j].multiplier = copyFrom.formulas[j].multiplier;
		}
	}

	public void CopyParameters(DAZMorph copyFrom)
	{
		group = copyFrom.group;
		region = copyFrom.region;
		morphName = copyFrom.morphName;
		displayName = copyFrom.displayName;
		preserveValueOnReimport = copyFrom.preserveValueOnReimport;
		min = copyFrom.min;
		max = copyFrom.max;
		visible = copyFrom.visible;
		disable = copyFrom.disable;
		isPoseControl = copyFrom.isPoseControl;
		_morphValue = copyFrom.morphValue;
		appliedValue = copyFrom.appliedValue;
		triggerNormalRecalc = copyFrom.triggerNormalRecalc;
		triggerTangentRecalc = copyFrom.triggerTangentRecalc;
	}

	private bool ProcessFormula(JSONNode fn, DAZMorphFormula formula, string morphName)
	{
		JSONNode jSONNode = fn["operations"];
		string url = jSONNode[0]["url"];
		string text = DAZImport.DAZurlToId(url);
		if (text == morphName + "?value")
		{
			string text2 = jSONNode[2]["op"];
			if (text2 == "mult")
			{
				float num = (formula.multiplier = jSONNode[1]["val"].AsFloat);
				return true;
			}
			Debug.LogWarning("Morph " + morphName + ": Found unknown formula " + text2);
		}
		else if (formula.target == morphName)
		{
			string text3 = fn["stage"];
			if (text3 != null)
			{
				if (text3 == "mult")
				{
					formula.targetType = DAZMorphFormulaTargetType.MCMMult;
					text = (formula.target = Regex.Replace(text, "\\?.*", string.Empty));
					return true;
				}
				Debug.LogWarning("Morph " + morphName + ": Found unknown stage " + text3);
			}
			else
			{
				formula.targetType = DAZMorphFormulaTargetType.MCM;
				text = (formula.target = Regex.Replace(text, "\\?.*", string.Empty));
				string text4 = jSONNode[2]["op"];
				if (text4 == "mult")
				{
					float num2 = (formula.multiplier = jSONNode[1]["val"].AsFloat);
					return true;
				}
				Debug.LogWarning("Morph " + morphName + ": Found unknown formula " + text4);
			}
		}
		else
		{
			Debug.LogWarning("Morph " + morphName + ": Found unknown operation url " + text);
		}
		return false;
	}

	public void Import(JSONNode mn)
	{
		morphName = mn["id"];
		_morphValue = 0f;
		appliedValue = 0f;
		if (mn["group"] != null)
		{
			group = Regex.Replace(mn["group"], "^/", string.Empty);
		}
		else
		{
			group = string.Empty;
		}
		displayName = mn["channel"]["label"];
		region = mn["region"];
		if (region == null)
		{
			region = group;
		}
		min = mn["channel"]["min"].AsFloat;
		if (min == -100000f)
		{
			min = -1f;
		}
		max = mn["channel"]["max"].AsFloat;
		if (max == 100000f)
		{
			max = 1f;
		}
		if (mn["formulas"].Count > 0)
		{
			List<DAZMorphFormula> list = new List<DAZMorphFormula>();
			foreach (JSONNode item in mn["formulas"].AsArray)
			{
				DAZMorphFormula dAZMorphFormula = new DAZMorphFormula();
				string text = item["output"];
				string input = Regex.Replace(text, "^.*#", string.Empty);
				input = (dAZMorphFormula.target = Regex.Replace(input, "\\?.*", string.Empty));
				string text2 = Regex.Replace(text, "^.*\\?", string.Empty);
				switch (text2)
				{
				case "value":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.MorphValue;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						list.Add(dAZMorphFormula);
					}
					continue;
				case "scale/general":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.GeneralScale;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						list.Add(dAZMorphFormula);
					}
					continue;
				case "scale/x":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.ScaleX;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						list.Add(dAZMorphFormula);
					}
					continue;
				case "scale/y":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.ScaleY;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						list.Add(dAZMorphFormula);
					}
					continue;
				case "scale/z":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.ScaleZ;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						list.Add(dAZMorphFormula);
					}
					continue;
				case "center_point/x":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.BoneCenterX;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						dAZMorphFormula.multiplier = (0f - dAZMorphFormula.multiplier) * 0.01f;
						list.Add(dAZMorphFormula);
					}
					continue;
				case "center_point/y":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.BoneCenterY;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						dAZMorphFormula.multiplier *= 0.01f;
						list.Add(dAZMorphFormula);
					}
					continue;
				case "center_point/z":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.BoneCenterZ;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						dAZMorphFormula.multiplier *= 0.01f;
						list.Add(dAZMorphFormula);
					}
					continue;
				}
				if (Regex.IsMatch(text2, "^end_point"))
				{
					continue;
				}
				switch (text2)
				{
				case "orientation/x":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.OrientationX;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						list.Add(dAZMorphFormula);
					}
					break;
				case "orientation/y":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.OrientationY;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						dAZMorphFormula.multiplier *= -1f;
						list.Add(dAZMorphFormula);
					}
					break;
				case "orientation/z":
					dAZMorphFormula.targetType = DAZMorphFormulaTargetType.OrientationZ;
					if (ProcessFormula(item, dAZMorphFormula, morphName))
					{
						dAZMorphFormula.multiplier *= -1f;
						list.Add(dAZMorphFormula);
					}
					break;
				default:
					Debug.LogWarning("Morph " + morphName + " has unknown output type " + text);
					break;
				}
			}
			formulas = list.ToArray();
		}
		else
		{
			formulas = new DAZMorphFormula[0];
		}
		numDeltas = mn["morph"]["deltas"]["count"].AsInt;
		deltas = new DAZMorphVertex[numDeltas];
		int num = 0;
		Vector3 delta = default(Vector3);
		foreach (JSONNode item2 in mn["morph"]["deltas"]["values"].AsArray)
		{
			int asInt = item2[0].AsInt;
			delta.x = (0f - item2[1].AsFloat) * 0.01f;
			delta.y = item2[2].AsFloat * 0.01f;
			delta.z = item2[3].AsFloat * 0.01f;
			DAZMorphVertex dAZMorphVertex = new DAZMorphVertex();
			dAZMorphVertex.vertex = asInt;
			dAZMorphVertex.delta = delta;
			deltas[num] = dAZMorphVertex;
			num++;
		}
	}
}
