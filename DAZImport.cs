using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.GZip;
using SimpleJSON;
using UnityEngine;

public class DAZImport : MonoBehaviour
{
	protected const float geoScale = 0.01f;

	public string DAZLibraryDirectory = "C:/Users/Public/Documents/My DAZ 3D Library/";

	public string alternateDAZLibraryDirectory = string.Empty;

	public string DAZSceneDufFile;

	public string DAZSceneModifierOverrideDufFile;

	public string modifierOverrideUrl;

	public bool createMaterials = true;

	public bool replaceExistingMaterials;

	public bool isMaleCharacter;

	public bool createMorphBank;

	[NonSerialized]
	public string standardShader = "Custom/Subsurface/Cull";

	[NonSerialized]
	public string transparentShader = "Custom/Subsurface/Transparent";

	[NonSerialized]
	public string reflTransparentShader = "Legacy Shaders/Transparent/Specular";

	[NonSerialized]
	public string subsurfaceStandardShader = "Custom/Subsurface/Cull";

	[NonSerialized]
	public string subsurfaceAltStandardShader = "Custom/Subsurface/NoCull";

	[NonSerialized]
	public string subsurfaceTransparentShader = "Custom/Subsurface/Transparent";

	[NonSerialized]
	public string subsurfaceReflTransparentShader = "Legacy Shaders/Transparent/Specular";

	[NonSerialized]
	public string subsurfaceAltReflTransparentShader = "Marmoset/Transparent/Simple Glass/Specular IBL";

	[NonSerialized]
	public string hairStandardShader = "Custom/Hair/Main";

	[NonSerialized]
	public string hairTransparentShader = "Custom/Hair/MainSeperateAlpha";

	[NonSerialized]
	public string unityStandardShader = "Legacy Shaders/Specular";

	[NonSerialized]
	public string unityTransparentShader = "Legacy Shaders/Transparent/Diffuse";

	[NonSerialized]
	public string unityReflTransparentShader = "Legacy Shaders/Transparent/Specular";

	[NonSerialized]
	public string marmosetStandardShader = "Marmoset/Specular IBL Soft";

	[NonSerialized]
	public string marmosetTransparentShader = "Marmoset/Transparent/Specular IBL";

	[NonSerialized]
	public string marmosetReflTransparentShader = "Marmoset/Transparent/Simple Glass/Specular IBL";

	public string MaterialCreationDirectory = "Assets/Materials";

	private string MaterialFolder;

	public Color subdermisColor = Color.white;

	public float specularMultiplier = 2f;

	public bool createSkinsAndNodes = true;

	public bool nestObjects;

	public bool embedMeshAndSkinOnNodes;

	public DAZSkinV2.PhysicsType skinPhysicsType;

	public Transform container;

	public Transform nodePrefab;

	private Dictionary<string, JSONNode> DAZ_uv_library;

	private Dictionary<string, DAZUVMap> DAZ_uv_map;

	private Dictionary<string, Material> DAZ_material_map;

	private Dictionary<string, JSONNode> DAZ_geometry_library;

	private Dictionary<string, JSONNode> DAZ_modifier_library;

	private Dictionary<string, JSONNode> DAZ_node_library;

	private Dictionary<string, JSONNode> DAZ_material_library;

	private Dictionary<string, Transform> sceneNodeIDToTransform;

	private Dictionary<string, string> graftIDToMainID;

	private Dictionary<string, string> geometryIDToNodeID;

	private Dictionary<string, string> sceneGeometryIDToSceneNodeID;

	private DAZSkinV2[] dazSkins;

	private JSONNode ReadJSON(string path)
	{
		if (FileChecker.IsGzipped(path))
		{
			FileStream fileStream = File.OpenRead(path);
			Stream stream = new GZipInputStream(fileStream);
			StreamReader streamReader = new StreamReader(stream);
			string aJSON = streamReader.ReadToEnd();
			streamReader.Close();
			stream.Close();
			fileStream.Close();
			return JSON.Parse(aJSON);
		}
		StreamReader streamReader2 = new StreamReader(path);
		string aJSON2 = streamReader2.ReadToEnd();
		streamReader2.Close();
		return JSON.Parse(aJSON2);
	}

	private List<string> ProcessDAZLibraries(string url, JSONNode jl)
	{
		if (DAZ_node_library == null)
		{
			DAZ_node_library = new Dictionary<string, JSONNode>();
		}
		JSONNode jSONNode = jl["node_library"];
		if (jSONNode != null)
		{
			foreach (JSONNode item in jSONNode.AsArray)
			{
				string text = item["id"];
				string key = url + "#" + text;
				if (url == string.Empty && DAZ_node_library.ContainsKey(key))
				{
					DAZ_node_library.Remove(key);
				}
				DAZ_node_library.Add(key, item);
			}
		}
		if (DAZ_geometry_library == null)
		{
			DAZ_geometry_library = new Dictionary<string, JSONNode>();
		}
		JSONNode jSONNode3 = jl["geometry_library"];
		if (jSONNode3 != null)
		{
			foreach (JSONNode item2 in jSONNode3.AsArray)
			{
				string text2 = item2["id"];
				string key2 = url + "#" + text2;
				if (url == string.Empty && DAZ_geometry_library.ContainsKey(key2))
				{
					DAZ_geometry_library.Remove(key2);
				}
				DAZ_geometry_library.Add(key2, item2);
			}
		}
		if (DAZ_modifier_library == null)
		{
			DAZ_modifier_library = new Dictionary<string, JSONNode>();
		}
		List<string> list = new List<string>();
		JSONNode jSONNode5 = jl["modifier_library"];
		if (jSONNode5 != null)
		{
			foreach (JSONNode item3 in jSONNode5.AsArray)
			{
				string text3 = item3["id"];
				string text4 = url + "#" + text3;
				list.Add(text4);
				if (url == string.Empty && DAZ_modifier_library.ContainsKey(text4))
				{
					DAZ_modifier_library.Remove(text4);
				}
				DAZ_modifier_library.Add(text4, item3);
			}
		}
		if (DAZ_uv_library == null)
		{
			DAZ_uv_library = new Dictionary<string, JSONNode>();
		}
		JSONNode jSONNode7 = jl["uv_set_library"];
		if (jSONNode7 != null)
		{
			foreach (JSONNode item4 in jSONNode7.AsArray)
			{
				string text5 = item4["id"];
				string key3 = url + "#" + text5;
				if (url == string.Empty && DAZ_uv_library.ContainsKey(key3))
				{
					DAZ_uv_library.Remove(key3);
				}
				DAZ_uv_library.Add(key3, item4);
			}
		}
		if (DAZ_material_library == null)
		{
			DAZ_material_library = new Dictionary<string, JSONNode>();
		}
		JSONNode jSONNode9 = jl["material_library"];
		if (jSONNode9 != null)
		{
			foreach (JSONNode item5 in jSONNode9.AsArray)
			{
				string text6 = item5["id"];
				string key4 = url + "#" + text6;
				if (url == string.Empty && DAZ_material_library.ContainsKey(key4))
				{
					DAZ_material_library.Remove(key4);
				}
				DAZ_material_library.Add(key4, item5);
			}
			return list;
		}
		return list;
	}

	private List<string> ProcessAltModifiers(string url, JSONNode jl)
	{
		if (DAZ_modifier_library == null)
		{
			DAZ_modifier_library = new Dictionary<string, JSONNode>();
		}
		List<string> list = new List<string>();
		JSONNode jSONNode = jl["modifier_library"];
		if (jSONNode != null)
		{
			foreach (JSONNode item in jSONNode.AsArray)
			{
				string text = item["id"];
				string text2 = url + "#" + text;
				Debug.Log("Process alt modifier " + text2);
				list.Add(text2);
				if (DAZ_modifier_library.ContainsKey(text2))
				{
					Debug.Log("Replacing existing");
					DAZ_modifier_library.Remove(text2);
				}
				DAZ_modifier_library.Add(text2, item);
			}
			return list;
		}
		return list;
	}

	public static string DAZurlFix(string url)
	{
		url = url.Replace("%20", " ");
		url = url.Replace("%21", "!");
		url = url.Replace("%27", "'");
		url = url.Replace("%28", "(");
		url = url.Replace("%29", ")");
		return url;
	}

	public static string DAZurlToPathKey(string url)
	{
		url = DAZurlFix(url);
		return Regex.Replace(url, "#.*", string.Empty);
	}

	public static string DAZurlToId(string url)
	{
		url = DAZurlFix(url);
		return Regex.Replace(url, ".*#", string.Empty);
	}

	public List<string> ReadDAZdsf(string url)
	{
		string text = DAZurlToPathKey(url);
		List<string> result = null;
		if (text != string.Empty)
		{
			string text2 = DAZLibraryDirectory + text;
			if (text2 != string.Empty)
			{
				if (File.Exists(text2))
				{
					JSONNode jl = ReadJSON(text2);
					result = ProcessDAZLibraries(text, jl);
				}
				else
				{
					text2 = alternateDAZLibraryDirectory + text;
					if (File.Exists(text2))
					{
						JSONNode jl2 = ReadJSON(text2);
						result = ProcessDAZLibraries(text, jl2);
					}
					else
					{
						Debug.LogError("File " + text2 + " could not be found in libraries. Check DAZ and alternate library paths for correctness.");
					}
				}
			}
		}
		else
		{
			Debug.LogError("Could not get path key from url " + url);
		}
		return result;
	}

	private JSONNode GetMaterial(string url)
	{
		if (DAZ_material_library == null)
		{
			DAZ_material_library = new Dictionary<string, JSONNode>();
		}
		url = DAZurlFix(url);
		if (!DAZ_material_library.ContainsKey(url))
		{
			ReadDAZdsf(url);
		}
		if (!DAZ_material_library.TryGetValue(url, out var value))
		{
			Debug.LogError("Could not find material at " + url);
		}
		return value;
	}

	private JSONNode GetUV(string url)
	{
		if (DAZ_uv_library == null)
		{
			DAZ_uv_library = new Dictionary<string, JSONNode>();
		}
		url = DAZurlFix(url);
		if (!DAZ_uv_library.ContainsKey(url))
		{
			ReadDAZdsf(url);
		}
		if (!DAZ_uv_library.TryGetValue(url, out var value))
		{
			Debug.LogError("Could not find uv at " + url);
		}
		return value;
	}

	private JSONNode GetGeometry(string url)
	{
		if (DAZ_geometry_library == null)
		{
			DAZ_geometry_library = new Dictionary<string, JSONNode>();
		}
		url = DAZurlFix(url);
		if (!DAZ_geometry_library.ContainsKey(url))
		{
			ReadDAZdsf(url);
		}
		if (!DAZ_geometry_library.TryGetValue(url, out var value))
		{
			Debug.LogError("Could not find geometry at " + url);
		}
		return value;
	}

	private JSONNode GetNode(string url)
	{
		if (DAZ_node_library == null)
		{
			DAZ_node_library = new Dictionary<string, JSONNode>();
		}
		url = DAZurlFix(url);
		if (!DAZ_node_library.ContainsKey(url))
		{
			ReadDAZdsf(url);
		}
		if (!DAZ_node_library.TryGetValue(url, out var value))
		{
			Debug.LogError("Could not find node at " + url);
		}
		return value;
	}

	private JSONNode GetModifier(string url)
	{
		if (DAZ_modifier_library == null)
		{
			DAZ_modifier_library = new Dictionary<string, JSONNode>();
		}
		url = DAZurlFix(url);
		if (!DAZ_modifier_library.ContainsKey(url))
		{
			ReadDAZdsf(url);
		}
		if (!DAZ_modifier_library.TryGetValue(url, out var value))
		{
			Debug.LogError("Could not find modifier at " + url);
		}
		return value;
	}

	private DAZUVMap ProcessUV(string uvurl)
	{
		DAZUVMap dAZUVMap;
		if (uvurl == null)
		{
			dAZUVMap = new DAZUVMap();
		}
		else
		{
			if (DAZ_uv_map.TryGetValue(uvurl, out dAZUVMap))
			{
				return dAZUVMap;
			}
			dAZUVMap = new DAZUVMap();
			DAZ_uv_map.Add(uvurl, dAZUVMap);
			JSONNode uV = GetUV(uvurl);
			dAZUVMap.Import(uV);
		}
		return dAZUVMap;
	}

	private DAZMesh GetDAZMeshBySceneGeometryId(string sceneGeometryId)
	{
		DAZMesh[] components = GetComponents<DAZMesh>();
		DAZMesh dAZMesh = null;
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh2 in array)
		{
			if (dAZMesh2.sceneGeometryId == sceneGeometryId)
			{
				dAZMesh = dAZMesh2;
				break;
			}
		}
		if (dAZMesh == null && container != null && embedMeshAndSkinOnNodes)
		{
			components = container.GetComponentsInChildren<DAZMesh>();
			DAZMesh[] array2 = components;
			foreach (DAZMesh dAZMesh3 in array2)
			{
				if (dAZMesh3.sceneGeometryId == sceneGeometryId)
				{
					dAZMesh = dAZMesh3;
					break;
				}
			}
		}
		return dAZMesh;
	}

	private DAZMesh GetDAZMeshByGeometryId(string geometryId)
	{
		DAZMesh[] components = GetComponents<DAZMesh>();
		DAZMesh dAZMesh = null;
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh2 in array)
		{
			if (dAZMesh2.geometryId == geometryId)
			{
				dAZMesh = dAZMesh2;
				break;
			}
		}
		if (dAZMesh == null && container != null && embedMeshAndSkinOnNodes)
		{
			components = container.GetComponentsInChildren<DAZMesh>();
			DAZMesh[] array2 = components;
			foreach (DAZMesh dAZMesh3 in array2)
			{
				if (dAZMesh3.geometryId == geometryId)
				{
					dAZMesh = dAZMesh3;
					break;
				}
			}
		}
		return dAZMesh;
	}

	private DAZMesh GetDAZMeshByNodeId(string nodeId)
	{
		DAZMesh[] components = GetComponents<DAZMesh>();
		DAZMesh dAZMesh = null;
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh2 in array)
		{
			if (dAZMesh2.nodeId == nodeId)
			{
				dAZMesh = dAZMesh2;
				break;
			}
		}
		if (dAZMesh == null && container != null && embedMeshAndSkinOnNodes)
		{
			components = container.GetComponentsInChildren<DAZMesh>();
			DAZMesh[] array2 = components;
			foreach (DAZMesh dAZMesh3 in array2)
			{
				if (dAZMesh3.nodeId == nodeId)
				{
					dAZMesh = dAZMesh3;
					break;
				}
			}
		}
		return dAZMesh;
	}

	private DAZMesh GetDAZMeshBySceneNodeId(string sceneNodeId)
	{
		DAZMesh[] components = GetComponents<DAZMesh>();
		DAZMesh dAZMesh = null;
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh2 in array)
		{
			if (dAZMesh2.sceneNodeId == sceneNodeId)
			{
				dAZMesh = dAZMesh2;
				break;
			}
		}
		if (dAZMesh == null && container != null && embedMeshAndSkinOnNodes)
		{
			components = container.GetComponentsInChildren<DAZMesh>();
			DAZMesh[] array2 = components;
			foreach (DAZMesh dAZMesh3 in array2)
			{
				if (dAZMesh3.sceneNodeId == sceneNodeId)
				{
					dAZMesh = dAZMesh3;
					break;
				}
			}
		}
		return dAZMesh;
	}

	private DAZMesh CreateDAZMesh(string sceneGeometryId, string geometryId, string sceneNodeId, string nodeId, GameObject meshContainer)
	{
		DAZMesh dAZMesh = meshContainer.AddComponent<DAZMesh>();
		dAZMesh.sceneGeometryId = sceneGeometryId;
		dAZMesh.geometryId = geometryId;
		dAZMesh.nodeId = nodeId;
		dAZMesh.sceneNodeId = sceneNodeId;
		return dAZMesh;
	}

	private DAZMesh GetOrCreateDAZMesh(string sceneGeometryId, string geometryId, string sceneNodeId, string nodeId, GameObject meshContainer)
	{
		DAZMesh dAZMesh = GetDAZMeshBySceneGeometryId(sceneGeometryId);
		if (dAZMesh == null)
		{
			dAZMesh = CreateDAZMesh(sceneGeometryId, geometryId, sceneNodeId, nodeId, meshContainer);
		}
		else
		{
			dAZMesh.sceneNodeId = sceneNodeId;
			dAZMesh.nodeId = nodeId;
		}
		return dAZMesh;
	}

	private DAZSkinV2 GetDAZSkin(string skinId, GameObject skinContainer)
	{
		DAZSkinV2[] components = skinContainer.GetComponents<DAZSkinV2>();
		DAZSkinV2 result = null;
		DAZSkinV2[] array = components;
		foreach (DAZSkinV2 dAZSkinV in array)
		{
			if (dAZSkinV.skinId == skinId)
			{
				result = dAZSkinV;
				break;
			}
		}
		return result;
	}

	private DAZSkinV2 CreateDAZSkin(string skinId, string skinUrl, GameObject skinContainer)
	{
		DAZSkinV2 dAZSkinV = skinContainer.AddComponent<DAZSkinV2>();
		dAZSkinV.skinId = skinId;
		dAZSkinV.skinUrl = DAZurlFix(skinUrl);
		dAZSkinV.physicsType = skinPhysicsType;
		return dAZSkinV;
	}

	private DAZSkinV2 GetOrCreateDAZSkin(string skinId, string skinUrl, GameObject skinContainer)
	{
		DAZSkinV2 dAZSkinV = GetDAZSkin(skinId, skinContainer);
		if (dAZSkinV == null)
		{
			dAZSkinV = CreateDAZSkin(skinId, skinUrl, skinContainer);
		}
		return dAZSkinV;
	}

	private DAZMesh ProcessGeometry(string geourl, string sceneGeometryId, DAZUVMap[] uvmaplist, string sceneNodeId, string nodeId, GameObject meshContainer)
	{
		JSONNode geometry = GetGeometry(geourl);
		string geometryId = geometry["id"];
		DAZMesh orCreateDAZMesh = GetOrCreateDAZMesh(sceneGeometryId, geometryId, sceneNodeId, nodeId, meshContainer);
		if (orCreateDAZMesh != null)
		{
			orCreateDAZMesh.Import(geometry, uvmaplist[0], DAZ_material_map);
		}
		return orCreateDAZMesh;
	}

	private string ProcessNode(JSONNode sn, bool isRoot)
	{
		string url = DAZurlFix(sn["url"]);
		string text = sn["id"];
		JSONNode node = GetNode(url);
		string result = node["id"];
		if (sn["conform_target"] != null)
		{
			string text2 = DAZurlToId(sn["conform_target"]);
			if (graftIDToMainID == null)
			{
				graftIDToMainID = new Dictionary<string, string>();
			}
			graftIDToMainID.Add(text, text2);
			result = text2;
		}
		else if (createSkinsAndNodes)
		{
			if (sceneNodeIDToTransform == null)
			{
				sceneNodeIDToTransform = new Dictionary<string, Transform>();
			}
			Transform value2;
			if (sn["parent"] != null)
			{
				string text3 = DAZurlToId(sn["parent"]);
				if (graftIDToMainID != null && graftIDToMainID.TryGetValue(text3, out var value))
				{
					text3 = value;
				}
				if (!sceneNodeIDToTransform.TryGetValue(text3, out value2))
				{
					Debug.LogWarning("Could not find parent transform " + text3);
					value2 = container;
				}
			}
			else
			{
				value2 = container;
			}
			string text4 = node["name"];
			if (isRoot)
			{
				text4 = text;
			}
			Vector3 zero = Vector3.zero;
			if (sn["translation"] != null)
			{
				foreach (JSONNode item in sn["translation"].AsArray)
				{
					switch ((string)item["id"])
					{
					case "x":
						zero.x = item["current_value"].AsFloat * -0.01f;
						break;
					case "y":
						zero.y = item["current_value"].AsFloat * 0.01f;
						break;
					case "z":
						zero.z = item["current_value"].AsFloat * 0.01f;
						break;
					}
				}
			}
			Vector3 zero2 = Vector3.zero;
			if (sn["rotation"] != null)
			{
				foreach (JSONNode item2 in sn["rotation"].AsArray)
				{
					switch ((string)item2["id"])
					{
					case "x":
						zero2.x = item2["current_value"].AsFloat;
						break;
					case "y":
						zero2.y = 0f - item2["current_value"].AsFloat;
						break;
					case "z":
						zero2.z = 0f - item2["current_value"].AsFloat;
						break;
					}
				}
			}
			GameObject gameObject = null;
			foreach (Transform item3 in value2)
			{
				if (item3.name == text4)
				{
					gameObject = item3.gameObject;
					break;
				}
			}
			if (gameObject == null)
			{
				if (isRoot && nodePrefab != null)
				{
					gameObject = UnityEngine.Object.Instantiate(nodePrefab.gameObject);
					gameObject.name = text4;
					gameObject.transform.parent = value2;
					Transform transform2 = gameObject.transform.Find("object");
					if (transform2 != null)
					{
						sceneNodeIDToTransform.Add(text, transform2.transform);
					}
					else
					{
						sceneNodeIDToTransform.Add(text, gameObject.transform);
					}
				}
				else
				{
					gameObject = new GameObject(text4);
					gameObject.transform.parent = value2;
					sceneNodeIDToTransform.Add(text, gameObject.transform);
				}
			}
			else if (isRoot && nodePrefab != null)
			{
				Transform transform3 = gameObject.transform.Find("object");
				if (transform3 != null)
				{
					sceneNodeIDToTransform.Add(text, transform3.transform);
				}
				else
				{
					sceneNodeIDToTransform.Add(text, gameObject.transform);
				}
			}
			else
			{
				sceneNodeIDToTransform.Add(text, gameObject.transform);
			}
			if (isRoot)
			{
				gameObject.transform.localPosition = zero;
				gameObject.transform.localEulerAngles = zero2;
				if (!nestObjects)
				{
					gameObject.transform.parent = container;
				}
			}
			else
			{
				DAZBone dAZBone = gameObject.GetComponent<DAZBone>();
				if (dAZBone == null)
				{
					dAZBone = gameObject.AddComponent<DAZBone>();
				}
				dAZBone.ImportNode(node, isMaleCharacter);
				dAZBone.presetLocalTranslation = zero;
				dAZBone.presetLocalRotation = zero2;
				dAZBone.exclude = skinPhysicsType != DAZSkinV2.PhysicsType.None;
			}
		}
		return result;
	}

	public DAZMorph ProcessMorph(string modurl, string sceneId = null)
	{
		JSONNode modifier = GetModifier(modurl);
		if (modifier != null)
		{
			string text = DAZurlToId(modifier["parent"]);
			if (modifier["formulas"].Count > 0)
			{
				foreach (JSONNode item in modifier["formulas"].AsArray)
				{
					string input = item["output"];
					string text2 = Regex.Replace(input, "^.*\\?", string.Empty);
					if (text2 == "value")
					{
						string input2 = Regex.Replace(input, "^.*:", string.Empty);
						input2 = Regex.Replace(input2, "\\?.*", string.Empty);
						if (Regex.IsMatch(input2, "^/"))
						{
							ProcessMorph(input2, sceneId);
						}
					}
				}
			}
			DAZMorph dAZMorph = new DAZMorph();
			dAZMorph.Import(modifier);
			DAZMesh dAZMesh;
			if (sceneId != null)
			{
				dAZMesh = GetDAZMeshBySceneGeometryId(sceneId);
				if (dAZMesh == null)
				{
					dAZMesh = GetDAZMeshBySceneNodeId(sceneId);
				}
			}
			else
			{
				dAZMesh = GetDAZMeshByGeometryId(text);
				if (dAZMesh == null)
				{
					dAZMesh = GetDAZMeshByNodeId(text);
				}
			}
			if (dAZMesh != null)
			{
				if (dAZMesh.morphBank == null && createMorphBank)
				{
					DAZMorphBank dAZMorphBank = dAZMesh.gameObject.AddComponent<DAZMorphBank>();
					dAZMorphBank.connectedMesh = dAZMesh;
					dAZMesh.morphBank = dAZMorphBank;
				}
				if (dAZMesh.morphBank != null)
				{
					dAZMesh.morphBank.AddMorph(dAZMorph);
				}
			}
			else if (sceneId != null)
			{
				Debug.LogWarning("Could not find scene id " + sceneId + " when processing morph " + dAZMorph.morphName);
			}
			else
			{
				Debug.LogWarning("Could not find base id " + text + " when processing morph " + dAZMorph.morphName);
			}
			return dAZMorph;
		}
		Debug.LogError("Could not process morph " + modurl);
		return null;
	}

	private DAZSkinV2 ProcessSkin(JSONNode sn, GameObject skinContainer)
	{
		string text = sn["url"];
		string skinId = sn["id"];
		JSONNode modifier = GetModifier(text);
		DAZSkinV2 orCreateDAZSkin = GetOrCreateDAZSkin(skinId, text, skinContainer);
		if (orCreateDAZSkin != null)
		{
			string text2 = (orCreateDAZSkin.sceneGeometryId = DAZurlToId(sn["parent"]));
			if (sceneGeometryIDToSceneNodeID.TryGetValue(text2, out var value))
			{
				if (graftIDToMainID.TryGetValue(value, out var value2))
				{
					value = value2;
				}
				if (sceneNodeIDToTransform.TryGetValue(value, out var value3))
				{
					DAZBones component = value3.GetComponent<DAZBones>();
					if (component != null)
					{
						orCreateDAZSkin.root = component;
					}
					orCreateDAZSkin.Import(modifier);
				}
				else
				{
					Debug.LogError("Could not find root bone " + value + " during ProcessSkin for geometry " + text2);
				}
			}
		}
		return orCreateDAZSkin;
	}

	public void ImportDuf()
	{
		if (container == null)
		{
			container = base.transform;
		}
		DAZ_modifier_library = new Dictionary<string, JSONNode>();
		DAZ_geometry_library = new Dictionary<string, JSONNode>();
		DAZ_node_library = new Dictionary<string, JSONNode>();
		DAZ_uv_library = new Dictionary<string, JSONNode>();
		DAZ_uv_map = new Dictionary<string, DAZUVMap>();
		DAZ_material_map = new Dictionary<string, Material>();
		sceneNodeIDToTransform = new Dictionary<string, Transform>();
		graftIDToMainID = new Dictionary<string, string>();
		if (DAZSceneDufFile == null || !(DAZSceneDufFile != string.Empty))
		{
			return;
		}
		JSONNode jSONNode = ReadJSON(DAZSceneDufFile);
		ProcessDAZLibraries(string.Empty, jSONNode);
		JSONNode jSONNode2 = jSONNode["scene"]["materials"];
		Dictionary<string, List<DAZUVMap>> dictionary = new Dictionary<string, List<DAZUVMap>>();
		foreach (JSONNode item2 in jSONNode2.AsArray)
		{
			string key = DAZurlToId(item2["geometry"]);
			string uvurl = item2["uv_set"];
			DAZUVMap item = ProcessUV(uvurl);
			if (!dictionary.TryGetValue(key, out var value))
			{
				value = new List<DAZUVMap>();
				dictionary.Add(key, value);
			}
			value.Add(item);
		}
		JSONNode jSONNode4 = jSONNode["scene"]["modifiers"];
		JSONNode jSONNode5 = jSONNode["scene"]["nodes"];
		sceneGeometryIDToSceneNodeID = new Dictionary<string, string>();
		foreach (JSONNode item3 in jSONNode5.AsArray)
		{
			string text = item3["id"];
			bool flag = false;
			if (item3["geometries"] != null)
			{
				flag = true;
			}
			string nodeId = ProcessNode(item3, flag);
			if (!flag)
			{
				continue;
			}
			DAZBones dAZBones = null;
			Transform transform = base.transform;
			if (sceneNodeIDToTransform.TryGetValue(text, out var value2))
			{
				if (embedMeshAndSkinOnNodes)
				{
					if (item3["conform_target"] != null)
					{
						string sceneNodeId = DAZurlToId(item3["conform_target"]);
						DAZMesh dAZMeshBySceneNodeId = GetDAZMeshBySceneNodeId(sceneNodeId);
						transform = ((!(dAZMeshBySceneNodeId != null)) ? value2 : dAZMeshBySceneNodeId.transform);
					}
					else
					{
						transform = value2;
					}
				}
				dAZBones = value2.GetComponent<DAZBones>();
				if (dAZBones == null)
				{
					dAZBones = value2.gameObject.AddComponent<DAZBones>();
				}
			}
			DAZMesh dAZMesh = null;
			foreach (JSONNode item4 in item3["geometries"].AsArray)
			{
				string text2 = item4["url"];
				if (text2 != null)
				{
					string text3 = item4["id"];
					sceneGeometryIDToSceneNodeID.Add(text3, text);
					if (dictionary.TryGetValue(text3, out var value3))
					{
						dAZMesh = ProcessGeometry(text2, text3, value3.ToArray(), text, nodeId, transform.gameObject);
					}
					else
					{
						Debug.LogError("Could not find materials for " + text3);
					}
				}
				else
				{
					Debug.LogError("Could not find geometries url");
				}
			}
			if (item3["conform_target"] != null)
			{
				string sceneNodeId2 = DAZurlToId(item3["conform_target"]);
				DAZMesh dAZMeshBySceneNodeId2 = GetDAZMeshBySceneNodeId(sceneNodeId2);
				if (dAZMeshBySceneNodeId2 != null && dAZMesh != null)
				{
					dAZMesh.graftTo = dAZMeshBySceneNodeId2;
				}
			}
		}
		foreach (JSONNode item5 in jSONNode4.AsArray)
		{
			string text4 = item5["extra"][0]["type"];
			if (!(text4 != "skin_settings"))
			{
				continue;
			}
			string sceneId = DAZurlToId(item5["parent"]);
			DAZMorph dAZMorph = ProcessMorph(item5["url"], sceneId);
			if (dAZMorph != null && !dAZMorph.preserveValueOnReimport)
			{
				float num = item5["channel"]["current_value"].AsFloat;
				if (num <= 0.001f && num >= -0.001f)
				{
					num = 0f;
				}
				dAZMorph.importValue = num;
				dAZMorph.morphValue = num;
			}
		}
		if (container != null)
		{
			DAZBones[] componentsInChildren = container.GetComponentsInChildren<DAZBones>();
			foreach (DAZBones dAZBones2 in componentsInChildren)
			{
				dAZBones2.Reset();
			}
		}
		DAZMesh[] components = GetComponents<DAZMesh>();
		foreach (DAZMesh dAZMesh2 in components)
		{
			dAZMesh2.ResetMorphs();
			dAZMesh2.ApplyMorphs();
		}
		DAZMergedMesh[] components2 = GetComponents<DAZMergedMesh>();
		foreach (DAZMergedMesh dAZMergedMesh in components2)
		{
			dAZMergedMesh.ManualUpdate();
		}
		if (DAZSceneModifierOverrideDufFile != null && DAZSceneModifierOverrideDufFile != string.Empty)
		{
			JSONNode jl = ReadJSON(DAZSceneModifierOverrideDufFile);
			string url = DAZurlFix(modifierOverrideUrl);
			ProcessAltModifiers(url, jl);
		}
		if (!createSkinsAndNodes || !(container != null))
		{
			return;
		}
		foreach (JSONNode item6 in jSONNode4.AsArray)
		{
			string text5 = item6["extra"][0]["type"];
			if (!(text5 == "skin_settings"))
			{
				continue;
			}
			string skinId = item6["id"];
			string skinUrl = item6["url"];
			string text6 = DAZurlToId(item6["parent"]);
			Transform transform2 = base.transform;
			if (embedMeshAndSkinOnNodes && text6 != null)
			{
				DAZMesh dAZMeshBySceneGeometryId = GetDAZMeshBySceneGeometryId(text6);
				if (dAZMeshBySceneGeometryId != null)
				{
					transform2 = dAZMeshBySceneGeometryId.transform;
				}
				else
				{
					Debug.LogWarning("Could not find DAZMesh with scene geometry ID " + text6);
					transform2 = base.transform;
				}
			}
			DAZSkinV2 orCreateDAZSkin = GetOrCreateDAZSkin(skinId, skinUrl, transform2.gameObject);
			orCreateDAZSkin.ImportStart();
			foreach (JSONNode item7 in jSONNode5.AsArray)
			{
				string url2 = DAZurlFix(item7["url"]);
				JSONNode node = GetNode(url2);
				orCreateDAZSkin.ImportNode(node, url2);
			}
			ProcessSkin(item6, transform2.gameObject);
			if (orCreateDAZSkin.dazMesh.graftTo != null)
			{
				DAZMergedMesh dAZMergedMesh2 = transform2.GetComponent<DAZMergedMesh>();
				if (dAZMergedMesh2 == null)
				{
					dAZMergedMesh2 = transform2.gameObject.AddComponent<DAZMergedMesh>();
				}
				dAZMergedMesh2.Merge();
				DAZMergedSkinV2 dAZMergedSkinV = transform2.GetComponent<DAZMergedSkinV2>();
				if (dAZMergedSkinV == null)
				{
					dAZMergedSkinV = transform2.gameObject.AddComponent<DAZMergedSkinV2>();
				}
				dAZMergedSkinV.root = orCreateDAZSkin.root;
				dAZMergedSkinV.Merge();
				dAZMergedSkinV.skin = true;
				dAZMergedSkinV.useSmoothing = true;
				dAZMergedSkinV.useSmoothVertsForNormalTangentRecalc = true;
				dAZMergedSkinV.skinMethod = DAZSkinV2.SkinMethod.GPU;
				dAZMergedSkinV.CopyMaterials();
			}
		}
	}
}
