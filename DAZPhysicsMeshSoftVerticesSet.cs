using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DAZPhysicsMeshSoftVerticesSet
{
	[SerializeField]
	protected string _uid;

	public int targetVertex = -1;

	public int anchorVertex = -1;

	public bool autoInfluenceAnchor;

	public int[] influenceVertices;

	public int highlightVertex;

	[SerializeField]
	protected List<string> _links;

	public float springMultiplier = 1f;

	public float sizeMultiplier = 1f;

	public float limitMultiplier = 1f;

	public bool forceLookAtReference;

	public Vector3 lastPosition;

	public Transform kinematicTransform;

	public Rigidbody kinematicRB;

	public Transform jointTransform;

	public Transform jointTrackerTransform;

	public Rigidbody jointRB;

	public ConfigurableJoint joint;

	public Collider jointCollider;

	public Collider jointCollider2;

	public float[] influenceVerticesDistances;

	public float[] influenceVerticesWeights;

	public SpringJoint[] linkJoints;

	public Vector3 initialTargetPosition;

	public Vector3 jointTargetPosition;

	public Vector3 lastJointTargetPosition;

	public Quaternion jointTargetLookAt;

	public Vector3 primaryMove;

	public Vector3 move;

	public string uid
	{
		get
		{
			if (_uid == null || _uid == string.Empty)
			{
				_uid = Guid.NewGuid().ToString();
			}
			return _uid;
		}
	}

	public List<string> links
	{
		get
		{
			if (_links == null)
			{
				_links = new List<string>();
			}
			return _links;
		}
	}

	public DAZPhysicsMeshSoftVerticesSet()
	{
		_uid = Guid.NewGuid().ToString();
		influenceVertices = new int[0];
		_links = new List<string>();
	}

	public void AddInfluenceVertex(int vid)
	{
		int[] array = new int[influenceVertices.Length + 1];
		bool flag = false;
		for (int i = 0; i < influenceVertices.Length; i++)
		{
			if (influenceVertices[i] == vid)
			{
				flag = true;
				break;
			}
			array[i] = influenceVertices[i];
		}
		if (!flag)
		{
			array[influenceVertices.Length] = vid;
			influenceVertices = array;
		}
	}

	public void RemoveInfluenceVertex(int vid)
	{
		int[] array = new int[influenceVertices.Length - 1];
		bool flag = false;
		int num = 0;
		for (int i = 0; i < influenceVertices.Length; i++)
		{
			if (influenceVertices[i] == vid)
			{
				flag = true;
				continue;
			}
			array[num] = influenceVertices[i];
			num++;
		}
		if (flag)
		{
			influenceVertices = array;
		}
	}
}
