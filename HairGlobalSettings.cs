using UnityEngine;

public class HairGlobalSettings
{
	public HairStripV2.HairDrawType hairDrawType = HairStripV2.HairDrawType.GPULines;

	public int numberSegments = 10;

	public float invNumberSegments;

	public int numHairsMin = 1;

	public int numHairsMax = 1;

	public HairStripV2.HairBundleType bundleType = HairStripV2.HairBundleType.Circular;

	public float subHairXOffsetMax = 0.01f;

	public float subHairYOffsetMax = 0.01f;

	public float subHairZOffsetBend;

	public Collider[] colliders;

	public bool useExtendedColliders;

	public ExtendedCapsuleCollider[] extendedColliders;

	public Vector3[] colliderCenters;

	public bool createTangents;

	public bool ownMesh = true;

	public float deltaTime;

	public float deltaTimeSqr;

	public float invDeltaTime;

	public bool drawFromAnchor = true;

	public float hairLength = 0.15f;

	public float segmentLength;

	public float quarterSegmentLength;

	public float hairWidth = 0.0005f;

	public float hairHalfWidth;

	public bool roundSheetHairs = true;

	public float sheetHairRoundness = 0.5f;

	public Material hairMaterial;

	public Vector3 gravityForce;

	public bool staticFriction;

	public float staticMoveDistance = 0.001f;

	public float staticMoveDistanceSqr;

	public float velocityFactor = 0.98f;

	public float stiffnessRoot;

	public float stiffnessEnd;

	public float stiffnessVariance;

	public bool enableCollision = true;

	public bool enableSimulation = true;

	public float slowCollidingPoints = 0.5f;

	public float dampenFactor = 0.9f;

	public float invdtdampen;

	public bool clampAcceleration = true;

	public bool clampVelocity = true;

	public float accelerationClamp = 0.015f;

	public float velocityClamp = 0.1f;

	public bool castShadows = true;

	public bool receiveShadows = true;

	public float debugWidth = 0.005f;
}
