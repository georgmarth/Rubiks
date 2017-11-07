using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{

    public RubiksCube cube;

    [HideInInspector]
    public float timer = 120f;

    private bool countDown;

    [HideInInspector]
    public bool GameOver { get; private set; }

    public Transform fireWorks;
    public float explosionForce = 20f;
    public float explosionRadius = 10f;

    public Transform forceFields;
    public GameObject explosion;

    public Transform cubePlaceholder;
    public ToggleGroup previewToggles;
    public Toggle timerToggle;
    public TMP_InputField scrambleInput;
    public TMP_InputField timerInput;

    public GameObject startButton;
    public GameObject loseButton;
    public GameObject restartButtons;

    public bool GameRunning {get; private set;}

    #region singleton
    public static PlayManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

    void Start()
    {
        GameOver = false;
        GameRunning = false;
    }

    void Update()
    {
        if (GameRunning)
        {
            if (countDown)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = 0f;
                    Lose();
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    public void StartGame()
    {
        GameRunning = true;
    }

    public void InitGame()
    {
        // Get Cube and instatiate
        IEnumerable<Toggle> activeToggles = previewToggles.ActiveToggles();
        foreach(Toggle cubeToggle in activeToggles)
        {
            if (cubeToggle.isOn)
            {
                CubeSelector selector = cubeToggle.GetComponent<CubeSelector>();
                if (selector)
                {
                    GameObject cubeGameObject = Instantiate(selector.cubePrefab, cubePlaceholder.position, cubePlaceholder.rotation);
                    cube = cubeGameObject.GetComponent<RubiksCube>();
                    break;
                }
            }
        }

        // set Timer
        countDown = timerToggle.isOn;
        timer = countDown ? float.Parse(timerInput.text) : 0f;

        // set Scrambles
        cube.StartScrambling(int.Parse(scrambleInput.text));

    }

    public void Win()
    {
        GameOver = true;
        GameRunning = false;

        // Spawn fireworks
        FireWorksSpawner[] spawners = fireWorks.GetComponentsInChildren<FireWorksSpawner>();
        foreach (FireWorksSpawner spawner in spawners)
        {
            spawner.Spawn();
        }

        MakeSingleBody();
        EnableForceFields();

        loseButton.SetActive(false);
        restartButtons.SetActive(true);

        SoundManager.Instance.PlayFireWorks();
    }

    private void MakeSingleBody()
    {
        float mass = 0f;
        foreach (CubePiece cubePiece in cube.allPieces)
        {
            Rigidbody rbPiece = cubePiece.GetComponent<Rigidbody>();
            mass += rbPiece.mass;
            Destroy(rbPiece);
        }
        Rigidbody rb = cube.gameObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.angularDrag = Settings.Instance.angularDrag;
        rb.drag = Settings.Instance.linearDrag;
    }

    private void EnableForceFields()
    {
        Collider[] colliders = forceFields.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }

    public void Lose()
    {
        GameOver = true;
        GameRunning = false;

        loseButton.SetActive(false);
        restartButtons.SetActive(true);

        ExplodePieces();

        SoundManager.Instance.PlayExplosion();
    }

    private void ExplodePieces()
    {
        
        foreach (CubePiece cubePiece in cube.allPieces)
        {
            Rigidbody rbPiece = cubePiece.GetComponent<Rigidbody>();
            rbPiece.isKinematic = false;
        }
        Instantiate<GameObject>(explosion, cube.transform.position, cube.transform.rotation);
    }

    public void DoneScrambling()
    {
        startButton.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
