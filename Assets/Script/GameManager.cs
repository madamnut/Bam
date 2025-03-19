using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public GameObject headPrefab;
    public GameObject tailPrefab;
    public GameObject applePrefab;

    [Header("UI")]
    public GameObject pauseCanvas;
    public GameObject gameOverCanvas;
    public Text[] scoreTexts;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip pauseSFX;
    public AudioClip unpauseSFX;
    public AudioClip gameOverSFX;
    public AudioClip appleEatSFX;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float gameOverVolume = .6f;
    [Range(0f, 1f)] public float appleEatVolume = 1f;

    private AudioSource audioSource;

    private Vector2 direction = Vector2.right;
    private Vector3 headPosition;
    private GameObject head;

    private List<Vector3> positions = new List<Vector3>();
    private List<GameObject> tailSegments = new List<GameObject>();

    public float initialInterval = 0.1f;
    public float minimumInterval = 0.03f;
    private float timer;

    private int minX = -20, maxX = 20, minY = -12, maxY = 10;
    private GameObject apple;

    public int Score => tailSegments.Count;

    void Start()
    {
        Instance = this;
        headPosition = transform.position;
        head = Instantiate(headPrefab, headPosition, Quaternion.identity);
        positions.Add(headPosition);
        SpawnApple();

        pauseCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        
        audioSource = gameObject.AddComponent<AudioSource>();
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (Time.timeScale == 0f)
            return;

        HandleInput();

        timer += Time.deltaTime;
        if (timer >= GetCurrentInterval())
        {
            Move();
            timer = 0;
        }

        UpdateScoreTexts();
    }

    void UpdateScoreTexts()
    {
        foreach (Text txt in scoreTexts)
            txt.text = Score.ToString();
    }

    float GetCurrentInterval()
    {
        float interval = initialInterval - ((initialInterval - minimumInterval) * (Score / 100f));
        return Mathf.Max(interval, minimumInterval);
    }

    void HandleInput()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && direction != Vector2.down)
            direction = Vector2.up;
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && direction != Vector2.up)
            direction = Vector2.down;
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && direction != Vector2.right)
            direction = Vector2.left;
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && direction != Vector2.left)
            direction = Vector2.right;
    }

    void Move()
    {
        positions.Insert(0, headPosition);
        headPosition += (Vector3)direction;

        head.transform.position = headPosition;

        if (positions.Count > tailSegments.Count)
            positions.RemoveAt(positions.Count - 1);

        for (int i = 0; i < tailSegments.Count; i++)
            tailSegments[i].transform.position = positions[i];

        CheckCollision();
    }

    void SpawnApple()
    {
        Vector3 spawnPos;
        do
        {
            spawnPos = new Vector3(Random.Range(minX, maxX + 1), Random.Range(minY, maxY + 1), 0);
        } while (positions.Contains(spawnPos) || spawnPos == headPosition);

        apple = Instantiate(applePrefab, spawnPos, Quaternion.identity);
    }

    void Grow()
    {
        GameObject segment = Instantiate(tailPrefab, positions[positions.Count - 1], Quaternion.identity);
        tailSegments.Add(segment);
    }

    void CheckCollision()
    {
        if (headPosition.x < minX || headPosition.x > maxX || headPosition.y < minY || headPosition.y > maxY)
            GameOver();

        if (headPosition == apple.transform.position)
        {
            if (audioSource != null && appleEatSFX != null)
                audioSource.PlayOneShot(appleEatSFX, appleEatVolume);
            
            Grow();
            Destroy(apple);
            SpawnApple();
        }

        for (int i = 0; i < tailSegments.Count; i++)
        {
            if (headPosition == tailSegments[i].transform.position)
                GameOver();
        }
    }

    void GameOver()
    {
        if (audioSource != null && gameOverSFX != null)
            audioSource.PlayOneShot(gameOverSFX, gameOverVolume);

        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
    }

    void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f;
        Time.timeScale = isPaused ? 1f : 0f;
        pauseCanvas.SetActive(!isPaused);

        if (audioSource != null)
        {
            if (isPaused && unpauseSFX != null)
                audioSource.PlayOneShot(unpauseSFX);
            else if (!isPaused && pauseSFX != null)
                audioSource.PlayOneShot(pauseSFX);
        }
    }
}
