using SimpleJSON;
using UnityEngine;

public class DAZUVMap
{
	public string id;

	public Vector2[] uvs;

	public DAZVertexMap[] vertexMap;

	public void Import(JSONNode jsonUV)
	{
		string text = (id = jsonUV["id"]);
		int num = 0;
		int asInt = jsonUV["uvs"]["count"].AsInt;
		uvs = new Vector2[asInt];
		foreach (JSONNode item in jsonUV["uvs"]["values"].AsArray)
		{
			float asFloat = item[0].AsFloat;
			float asFloat2 = item[1].AsFloat;
			uvs[num].x = asFloat;
			uvs[num].y = asFloat2;
			num++;
		}
		num = 0;
		JSONArray asArray = jsonUV["polygon_vertex_indices"].AsArray;
		int count = asArray.Count;
		vertexMap = new DAZVertexMap[count];
		foreach (JSONNode item2 in asArray)
		{
			vertexMap[num] = new DAZVertexMap();
			int asInt2 = item2[0].AsInt;
			int asInt3 = item2[1].AsInt;
			int asInt4 = item2[2].AsInt;
			vertexMap[num].polyindex = asInt2;
			vertexMap[num].fromvert = asInt3;
			vertexMap[num].tovert = asInt4;
			num++;
		}
	}
}
