using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraMovement : MonoBehaviour {
    
    public float distance = 10f;
    public float xSpeed = 120f;
    public float ySpeed = 120f;
    public float zoomSpeed = 5f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public LayerMask dofFocusMask;

    public float cameraOffsetY = 0.5f;
    public LayerMask wallCollisionCheckMask;

    public PostProcessVolume volume;

    // Variable to reduce unnecessary updates
    [UnityEngine.HideInInspector]
    public bool movedThisFrame;

    private float x = 0f;
    private float y = 0f;

	// Use this for initialization
	void Start () {

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        SetDepthofFieldFocus();
        
	}
	
	
	void LateUpdate () {
        movedThisFrame = false;
        if (PlayManager.Instance.cube)
        {
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
                movedThisFrame = true;
            }
            Quaternion rotation = Quaternion.Euler(y, x, 0f);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, distanceMin, distanceMax);
            

            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f)
                movedThisFrame = true;

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + PlayManager.Instance.cube.transform.position;

            RaycastHit hit;
            if (Physics.Linecast(
                PlayManager.Instance.cube.transform.position, position, out hit, wallCollisionCheckMask))
            {
                distance = hit.distance;
                // recalculate distance and postion
                negDistance = new Vector3(0.0f, 0.0f, -distance);
                position = rotation * negDistance + PlayManager.Instance.cube.transform.position;
            }

            transform.rotation = rotation;
            transform.position = new Vector3(position.x, position.y + cameraOffsetY, position.z);
            transform.LookAt(PlayManager.Instance.cube.transform);
            SetDepthofFieldFocus();
        }
        
    }

    void SetDepthofFieldFocus()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, dofFocusMask))
        {
            DepthOfField settings = volume.profile.GetSetting<DepthOfField>();
            settings.focusDistance.value = hit.distance;
        }
    }
}

