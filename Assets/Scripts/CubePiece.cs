using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CubePiece : MonoBehaviour {

    public CubePiece centerPiece;
    public bool middlePiece = false;
    public Vector3 testRotations = new Vector3(1f, 1f, 1f);

    [HideInInspector]
    public Quaternion initalRotation;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        initalRotation = transform.localRotation;
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsSolved()
    {
        Vector3 correctRotation = (centerPiece.transform.localRotation * 
            Quaternion.Inverse(centerPiece.initalRotation))
            .eulerAngles;
        Vector3 currentRotation = (transform.localRotation * Quaternion.Inverse(initalRotation)).eulerAngles;

        if (PlayManager.Instance.cube.MiddlePiecesRotatable && middlePiece)
        {
            correctRotation = new Vector3(
                correctRotation.x * testRotations.x,
                correctRotation.y * testRotations.y,
                correctRotation.z * testRotations.z
                );
            currentRotation = new Vector3(
                currentRotation.x * testRotations.x,
                currentRotation.y * testRotations.y,
                currentRotation.z * testRotations.z
                );
        }
        return correctRotation == currentRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 3)
        {
            audioSource.pitch = Random.Range(.8f, 1.2f);
            audioSource.Play();
        }
    }
}
