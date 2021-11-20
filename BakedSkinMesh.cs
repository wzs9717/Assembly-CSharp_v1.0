using UnityEngine;

[AddComponentMenu("My Scripts/Geometry/BakedSkinMesh")]
public class BakedSkinMesh : MonoBehaviour
{
	public enum AlignType
	{
		up,
		right,
		forward
	}

	public bool on = true;

	public bool draw = true;

	public bool useUnityBake = true;

	public bool alignTangents = true;

	public AlignType alignType = AlignType.forward;

	private AlignType _alignType;

	private Mesh _mesh;

	private Mesh _originalMesh;

	public string debugBone;

	public Mesh debugMesh;

	public Material debugMaterial1;

	public Material debugMaterial2;

	private SkinnedMeshRenderer smr;

	private MeshFilter mf;

	private MeshRenderer mr;

	private int numBones;

	private Matrix4x4[] bindposes;

	private BoneWeight[] boneWeights;

	private Transform[] boneTransforms;

	private Matrix4x4[] boneMatrices;

	private Vector3[] originalVertices;

	private Vector3[] originalNormals;

	private Vector4[] originalTangents;

	private int numVertices;

	private Vector3[] bakedVertices;

	private Vector3[] bakedNormals;

	private Vector4[] bakedTangents;

	private Matrix4x4 vertexMatrix;

	public Mesh BakedMesh => _mesh;

	public Mesh OriginalMesh => _originalMesh;

	private void BakeMesh()
	{
		if ((bool)smr)
		{
			if (_alignType != alignType)
			{
				_alignType = alignType;
				InitSkinnedMeshVars();
			}
			if (useUnityBake)
			{
				smr.BakeMesh(_mesh);
			}
			else
			{
				SelfBake1();
			}
		}
	}

	private void DrawMesh()
	{
		if (draw)
		{
			if ((bool)smr)
			{
				Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
				Graphics.DrawMesh(_mesh, localToWorldMatrix, smr.sharedMaterial, 0, null, 0, null, smr.shadowCastingMode, smr.receiveShadows);
			}
			else if ((bool)mf && (bool)mr)
			{
				Matrix4x4 localToWorldMatrix2 = base.transform.localToWorldMatrix;
				Graphics.DrawMesh(_mesh, localToWorldMatrix2, mr.sharedMaterial, 0, null, 0, null, mr.shadowCastingMode, mr.receiveShadows);
			}
		}
	}

	private void DrawDebug(Matrix4x4 m, Material mat)
	{
		if (debugMesh != null && mat != null)
		{
			Graphics.DrawMesh(debugMesh, m, mat, 0, null, 0, null, castShadows: false, receiveShadows: false);
		}
	}

	private void SelfBake1()
	{
		bool flag = true;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int i = 0; i < numBones; i++)
		{
			ref Matrix4x4 reference = ref boneMatrices[i];
			reference = boneTransforms[i].localToWorldMatrix * bindposes[i];
			if (debugBone != null && debugBone != string.Empty && boneTransforms[i].name == debugBone)
			{
				DrawDebug(boneMatrices[i], debugMaterial1);
				DrawDebug(boneTransforms[i].localToWorldMatrix, debugMaterial2);
			}
		}
		Vector3 vector2 = default(Vector3);
		for (int j = 0; j < numVertices; j++)
		{
			int boneIndex = boneWeights[j].boneIndex0;
			int boneIndex2 = boneWeights[j].boneIndex1;
			int boneIndex3 = boneWeights[j].boneIndex2;
			int boneIndex4 = boneWeights[j].boneIndex3;
			float weight = boneWeights[j].weight0;
			float weight2 = boneWeights[j].weight1;
			float weight3 = boneWeights[j].weight2;
			float weight4 = boneWeights[j].weight3;
			for (int k = 0; k < 16; k++)
			{
				vertexMatrix[k] = boneMatrices[boneIndex][k] * weight + boneMatrices[boneIndex2][k] * weight2 + boneMatrices[boneIndex3][k] * weight3 + boneMatrices[boneIndex4][k] * weight4;
			}
			ref Vector3 reference2 = ref bakedVertices[j];
			reference2 = vertexMatrix.MultiplyPoint3x4(originalVertices[j]);
			if (flag)
			{
				flag = false;
				zero.x = bakedVertices[j].x;
				zero2.x = bakedVertices[j].x;
				zero.y = bakedVertices[j].y;
				zero2.y = bakedVertices[j].y;
				zero.z = bakedVertices[j].z;
				zero2.z = bakedVertices[j].z;
			}
			else
			{
				if (bakedVertices[j].x < zero.x)
				{
					zero.x = bakedVertices[j].x;
				}
				else if (bakedVertices[j].x > zero2.x)
				{
					zero2.x = bakedVertices[j].x;
				}
				if (bakedVertices[j].y < zero.y)
				{
					zero.y = bakedVertices[j].y;
				}
				else if (bakedVertices[j].y > zero2.y)
				{
					zero2.y = bakedVertices[j].y;
				}
				if (bakedVertices[j].z < zero.z)
				{
					zero.z = bakedVertices[j].z;
				}
				else if (bakedVertices[j].z > zero2.z)
				{
					zero2.z = bakedVertices[j].z;
				}
			}
			Vector3 vector = vertexMatrix.MultiplyVector(originalNormals[j]);
			float num = 1f / vector.magnitude;
			vector.x *= num;
			vector.y *= num;
			vector.z *= num;
			bakedNormals[j] = vector;
			vector2.x = originalTangents[j].x;
			vector2.y = originalTangents[j].y;
			vector2.z = originalTangents[j].z;
			Vector3 vector3 = vertexMatrix.MultiplyVector(vector2);
			num = 1f / vector3.magnitude;
			bakedTangents[j].x = vector3.x * num;
			bakedTangents[j].y = vector3.y * num;
			bakedTangents[j].z = vector3.z * num;
			bakedTangents[j].w = originalTangents[j].w;
		}
		_mesh.vertices = bakedVertices;
		_mesh.normals = bakedNormals;
		_mesh.tangents = bakedTangents;
		Bounds bounds = default(Bounds);
		bounds.min = zero;
		bounds.max = zero2;
		_mesh.bounds = bounds;
	}

	private void SelfBake2()
	{
		for (int i = 0; i < numBones; i++)
		{
			ref Matrix4x4 reference = ref boneMatrices[i];
			reference = boneTransforms[i].localToWorldMatrix * bindposes[i];
		}
		for (int j = 0; j < numVertices; j++)
		{
			ref Vector3 reference2 = ref bakedVertices[j];
			reference2 = Vector3.zero;
		}
		for (int k = 0; k < numVertices; k++)
		{
			BoneWeight boneWeight = boneWeights[k];
			if (boneWeight.weight0 != 0f)
			{
				bakedVertices[k] += boneMatrices[boneWeight.boneIndex0].MultiplyPoint3x4(originalVertices[k]) * boneWeight.weight0;
			}
			if (boneWeight.weight1 != 0f)
			{
				bakedVertices[k] += boneMatrices[boneWeight.boneIndex1].MultiplyPoint3x4(originalVertices[k]) * boneWeight.weight1;
			}
			if (boneWeight.weight2 != 0f)
			{
				bakedVertices[k] += boneMatrices[boneWeight.boneIndex2].MultiplyPoint3x4(originalVertices[k]) * boneWeight.weight2;
			}
			if (boneWeight.weight3 != 0f)
			{
				bakedVertices[k] += boneMatrices[boneWeight.boneIndex3].MultiplyPoint3x4(originalVertices[k]) * boneWeight.weight3;
			}
		}
		_mesh.vertices = bakedVertices;
	}

	private void InitSkinnedMeshVars()
	{
		numBones = smr.bones.Length;
		bindposes = _originalMesh.bindposes;
		boneWeights = _originalMesh.boneWeights;
		boneTransforms = new Transform[numBones];
		originalVertices = _originalMesh.vertices;
		numVertices = originalVertices.Length;
		originalNormals = _originalMesh.normals;
		originalTangents = _originalMesh.tangents;
		bakedVertices = new Vector3[numVertices];
		bakedNormals = new Vector3[numVertices];
		bakedTangents = new Vector4[numVertices];
		boneMatrices = new Matrix4x4[numBones];
		vertexMatrix = default(Matrix4x4);
		for (int i = 0; i < numBones; i++)
		{
			boneTransforms[i] = smr.bones[i].transform;
			ref Matrix4x4 reference = ref boneMatrices[i];
			reference = boneTransforms[i].localToWorldMatrix;
		}
		if (!alignTangents)
		{
			return;
		}
		Mesh mesh = CloneMesh(_originalMesh);
		Vector4 vector = default(Vector4);
		vector.w = 1f;
		Vector3 vector2 = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		if (_alignType == AlignType.forward)
		{
			vector2 = Vector3.forward;
			vector3 = Vector3.right;
		}
		else if (_alignType == AlignType.right)
		{
			vector2 = Vector3.right;
			vector3 = Vector3.up;
		}
		else if (_alignType == AlignType.up)
		{
			vector2 = Vector3.up;
			vector3 = Vector3.forward;
		}
		for (int j = 0; j < numVertices; j++)
		{
			vector.x = originalNormals[j].y * vector2.z - originalNormals[j].z * vector2.y;
			vector.y = originalNormals[j].z * vector2.x - originalNormals[j].x * vector2.z;
			vector.z = originalNormals[j].x * vector2.y - originalNormals[j].y * vector2.x;
			float num = 1f / vector.magnitude;
			vector.x *= num;
			vector.y *= num;
			vector.z *= num;
			if (vector.sqrMagnitude <= 0.5f)
			{
				vector.x = originalNormals[j].y * vector3.z - originalNormals[j].z * vector3.y;
				vector.y = originalNormals[j].z * vector3.x - originalNormals[j].x * vector3.z;
				vector.z = originalNormals[j].x * vector3.y - originalNormals[j].y * vector3.x;
				num = 1f / vector.magnitude;
				vector.x *= num;
				vector.y *= num;
				vector.z *= num;
			}
			originalTangents[j] = vector;
		}
		mesh.tangents = originalTangents;
		smr.sharedMesh = mesh;
	}

	private Mesh CloneMesh(Mesh inMesh)
	{
		return Object.Instantiate(inMesh);
	}

	private void Start()
	{
		smr = GetComponent<SkinnedMeshRenderer>();
		_alignType = alignType;
		if ((bool)smr)
		{
			_originalMesh = smr.sharedMesh;
			_mesh = CloneMesh(_originalMesh);
			_mesh.MarkDynamic();
			InitSkinnedMeshVars();
		}
		else
		{
			mr = GetComponent<MeshRenderer>();
			mf = GetComponent<MeshFilter>();
			if ((bool)mf)
			{
				_mesh = mf.sharedMesh;
				_originalMesh = _mesh;
			}
		}
		BakeMesh();
	}

	private void LateUpdate()
	{
		if (on)
		{
			base.transform.position = Vector3.zero;
			base.transform.rotation = Quaternion.identity;
			BakeMesh();
			DrawMesh();
		}
	}
}
