using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public GameObject headPrefab;
    public GameObject tailPrefab;
    public GameObject applePrefab;

    private Vector2 direction = Vector2.right;
    private Vector3 headPosition;
    private List<Vector3> positions = new List<Vector3>();
    private List<GameObject> tailSegments = new List<GameObject>();

    public float moveInterval = 0.2f;
    private float timer;

    private int minX = -20, maxX = 20, minY = -12, maxY = 10;
    private GameObject apple;

    void Start()
    {
        headPosition = transform.position;
        positions.Add(headPosition);
        SpawnApple();
    }

    void Update()
    {
        HandleInput();

        timer += Time.deltaTime;
        if (timer >= moveInterval)
        {
            Move();
            timer = 0;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
            direction = Vector2.right;
    }

    void Move()
    {
        positions.Insert(0, headPosition);
        headPosition += (Vector3)direction;

        transform.position = headPosition;

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
        Debug.Log("Game Over");
        Time.timeScale = 0f;
    }
}