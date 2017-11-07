using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.RequireComponent(typeof(Collider))]
public class ForceField : MonoBehaviour {

    public Vector3 force;
    public Vector3 torque;

	void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            float distance = (transform.position - other.transform.position).magnitude;
            float multiplier = 1f / distance;

            other.attachedRigidbody.AddForce(force * multiplier);
            other.attachedRigidbody.AddTorque(torque * multiplier);
        }
    }
}
