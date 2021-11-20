using UnityEngine;

public class LinkedPoint
{
	public LinkedPoint previous;

	public LinkedPoint next;

	public Vector3 previous_position;

	public Vector3 stiff_position;

	public float stiffness;

	public Vector3 unconstrained_position;

	public Vector3 position;

	public Vector3 delta_position;

	public Vector3 velocity;

	public Vector3 force;

	public bool collided;

	public bool had_collided;

	public LinkedPoint(Vector3 pt)
	{
		position = pt;
		previous_position = pt;
		velocity = Vector3.zero;
		force = Vector3.zero;
	}
}
