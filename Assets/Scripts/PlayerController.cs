using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PrimeTween;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 10.0f;
    [SerializeField] private float fireSpeed = 20f;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    private GameObject plane;
    private Rigidbody rb;

    //reference to UI elements
    private GameObject UIElements;
    private GameObject gameOverText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI highScoreText;
    private Button restart;
    private Button exit;

    //reference to Input Actions
    private InputActionMap UIActionMap;
    private InputActionMap PlayerActionMap;
    private InputAction moveAction;
    private InputAction pointAction;
    private InputAction leftClickAction;
    private Vector2 movementVector;
    private Vector2 mousePos;
    private Vector3 hitPoint;
    private int playerHealth = 5;
    private int highScore;
    [HideInInspector] public int score;

    void Awake()
    {
        //Fetching the action maps - Player and UI
        if (inputActions != null)
        {
            PlayerActionMap = inputActions.FindActionMap("Player");
            UIActionMap = inputActions.FindActionMap("UI");
        }

        //reset player score
        score = 0;
    }

    void OnEnable()
    {
        PlayerActionMap.Enable();
        UIActionMap.Enable();
    }

    void OnDisable()
    {
        PlayerActionMap.Disable();
        UIActionMap.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //fetching relvant actiosn for moving and rotating the player 
        if (PlayerActionMap != null)
        {
            moveAction = PlayerActionMap.FindAction("Move");
        }

        if (UIActionMap != null)
        {
            pointAction = UIActionMap.FindAction("Point");
            leftClickAction = UIActionMap.FindAction("Click");
        }

        plane = GameObject.FindGameObjectWithTag("Ground");

        //fetching UI elements
        UIElements = GameObject.Find("UIElements");
        if (UIElements != null)
        {
            healthText = UIElements.transform.Find("Health").GetComponent<TextMeshProUGUI>();
            scoreText = UIElements.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            highScoreText = UIElements.transform.Find("HighScore").GetComponent<TextMeshProUGUI>();
            gameOverText = UIElements.transform.Find("GameOver").gameObject;
            restart = UIElements.transform.Find("Restart").GetComponent<Button>();
            exit = UIElements.transform.Find("Exit").GetComponent<Button>();

        }

        //Setting up UI
        gameOverText.SetActive(false);
        restart.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);
        healthText.text = "Health: " + playerHealth.ToString();
        scoreText.text = "Score: " + score.ToString();
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = "HighScore: " + highScore.ToString();

        restart.onClick.AddListener(RestartGame);
        exit.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        movementVector = moveAction.ReadValue<Vector2>();
        mousePos = pointAction.ReadValue<Vector2>();

        //Rotating the player following the cursor
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == plane)
            {
                hitPoint = hit.point;
            }
        }
        transform.LookAt(hitPoint);

        // fetching input for firing projectile
        if (leftClickAction.WasPressedThisFrame())
        {
            FireProjectile();
        }

        //UI Update
        scoreText.text = "Score: " + score.ToString();

    }

    void FixedUpdate()
    {
        //moving player 
        Vector3 positionVector = transform.right * movementVector.x + transform.forward * movementVector.y;
        rb.linearVelocity = positionVector * playerSpeed;
    }
    private void FireProjectile()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector3 targetPosition = hitPoint;
        float distance = Vector3.Distance(firePoint.position, targetPosition);
        float duration = distance / fireSpeed;
        //using PrimeTween to animate bullet from player towards the aim(cursor)
        Tween.Position(bullet.transform, bullet.transform.position, targetPosition, duration)
            .OnComplete(() => Destroy(bullet));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerHealth--;
            healthText.text = "Health: " + playerHealth.ToString();

            playerHealth = Math.Max(playerHealth, 0);
            if (playerHealth <= 0)
            {
                gameOverText.SetActive(true);
                restart.gameObject.SetActive(true);
                exit.gameObject.SetActive(true);

            }

        }
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void OnApplicationQuit()
    {
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }


}
