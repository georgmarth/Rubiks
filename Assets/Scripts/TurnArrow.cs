using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnArrow : MonoBehaviour {
    
    public CubeAxis axis;
    public bool clockWise;
    
    private Camera mainCamera;
    private MeshRenderer meshRenderer;
    private Collider arrowCollider;
    private CameraMovement cameraMovement;

    private bool gameNotStarted;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        meshRenderer = GetComponent<MeshRenderer>();
        arrowCollider = GetComponent<Collider>();
        cameraMovement = Camera.main.GetComponent<CameraMovement>();

        gameNotStarted = true;
	}
	
	// Update is called once per frame
	void Update () {

        

        if (PlayManager.Instance.GameOver)
        {
            Destroy(gameObject);
            return;
        }
        
        if (!PlayManager.Instance.GameRunning)
        {
            arrowCollider.enabled = false;
            meshRenderer.enabled = false;
            return;
        }
        
        // only update if Camera has moved
        if (cameraMovement.movedThisFrame || gameNotStarted)
        {
            gameNotStarted = false;
            UpdateArrow();
        }
    }

    private void UpdateArrow()
    {

        // fade out the arrow that is not facing the camera
        Vector3 cameraDirection = mainCamera.transform.position - transform.position;
        cameraDirection.Normalize();
        float angleToCamera = Vector3.Angle(cameraDirection, -transform.up);
        float alpha = Mathf.InverseLerp(
            Settings.Instance.endFadeAngle, Settings.Instance.startfadeAngle, angleToCamera);
        meshRenderer.material.color = new Color(1f, 1f, 1f, alpha);

        //disable collider when not visible
        arrowCollider.enabled = angleToCamera > Settings.Instance.endFadeAngle ? false : true;
        //also disable the mesh renderer when not visible
        meshRenderer.enabled = angleToCamera > Settings.Instance.endFadeAngle ? false : true;
    }

    public void OnClick(BaseEventData data)
    {
        if (data.GetType() == typeof(PointerEventData))
            {
            PointerEventData pointerData = (PointerEventData)data;
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                PlayManager.Instance.cube.StartRotation(axis, clockWise);
            }
        }
    }
}
