using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubiksCube : MonoBehaviour {

    public LayerMask layerMask;

    public CubeAxis[] allAxis;

    public bool MiddlePiecesRotatable = false;

    [UnityEngine.HideInInspector]
    public CubePiece[] allPieces;

    private bool rotating = false;
    private bool startRotating = false;
    private bool scrambling = false;
    
    private CubeAxis rotatingAxis;
    private Vector3 axis;
    private float angle = 90f;
    private Transform[] rotationPieces;
    private Quaternion[] originalRotations;
    private bool cw = true;

    private CubeAxis lastRandomScramble;

    private Quaternion rotation;
    private float turnTime = 0f;

    private int scrambles;

    void Start()
    {
        allPieces = GetComponentsInChildren<CubePiece>();
    }

	void FixedUpdate () {
        if (!PlayManager.Instance.GameOver)
        {
            if (scrambling && !rotating)
                Scramble();

            if (startRotating)
                InitRotation();

            if (rotating)
                Rotate();
        }
	}

    public void StartScrambling(int amount)
    {
        if (scrambling)
            return;

        scrambles = amount;
        scrambling = true;
    }

    private void CheckSolved()
    {
        if (PlayManager.Instance.GameRunning && IsSolved())
        {
            PlayManager.Instance.Win();
        }
    }

    private void Scramble()
    {
        if (scrambles <= 0)
        {
            scrambling = false;
            PlayManager.Instance.DoneScrambling();
            return;
        }
        scrambles--;

        CubeAxis randomAxis = allAxis[Random.Range(0, allAxis.Length)];
        while (lastRandomScramble != null && lastRandomScramble == randomAxis)
        {
            randomAxis = allAxis[Random.Range(0, allAxis.Length)];
        }
        lastRandomScramble = randomAxis;
        bool clockwise = Random.value < 0.5f;
        StartRotation(randomAxis, clockwise);
    }

    private void Rotate()
    {
        float rotateTime = scrambling ? Settings.Instance.scrambleTime : Settings.Instance.rotateTime;

        turnTime = Mathf.Clamp(turnTime + Time.deltaTime, 0f, rotateTime);
        float t = Mathf.InverseLerp(0f, rotateTime, turnTime);
        for ( int i = 0; i < rotationPieces.Length; i++)
        {
            Transform piece = rotationPieces[i];
            Quaternion originalRotation = originalRotations[i];
            Quaternion targetRotation = rotation * originalRotation;
            piece.localRotation = Quaternion.Lerp(originalRotation, targetRotation, t);
        }
        if (turnTime == rotateTime)
        {
            rotating = false;
            CheckSolved();
        }
    }

    public void StartRotation(CubeAxis axis, bool clockwise)
    {
        cw = clockwise;
        StartRotation(axis);
    }

    public void StartRotation(CubeAxis axis)
    {
        if (rotating)
            return;

        rotatingAxis = axis;
        startRotating = true;
    }

    private void InitRotation()
    {
        BoxCollider selector = rotatingAxis.GetComponent<BoxCollider>();

        Vector3 position = selector.transform.position + (selector.transform.rotation * selector.center);
        Vector3 extends = new Vector3(
            selector.size.x / 2,
            selector.size.y / 2,
            selector.size.z / 2
            );

        Collider[] pieces = Physics.OverlapBox(position, extends, selector.transform.rotation, layerMask);



        rotationPieces = new Transform[pieces.Length];
        originalRotations = new Quaternion[pieces.Length];

        for (int i = 0; i < pieces.Length; i++)
        {
            Collider piece = pieces[i];
            rotationPieces[i] = piece.transform;
            originalRotations[i] = piece.transform.localRotation;

        }

        rotation = Quaternion.AngleAxis(cw ? angle : -angle, rotatingAxis.axis);


        rotating = true;
        startRotating = false;
        turnTime = 0f;

        SoundManager.Instance.PlayCubeSound();
    }

    public void enableGravity()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddTorque(new Vector3(5f, 0f, 0f));
    }

    public bool IsSolved()
    {
        foreach(CubePiece piece in allPieces)
        {
            if (!piece.IsSolved())
                return false;
        }
        return true;
    }
}
