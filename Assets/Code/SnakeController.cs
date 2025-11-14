using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeController : MonoBehaviour
{

    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap ;
    public float shiftspeed;
    private int maxFoodCount = 20;


    // References
    public GameObject BodyPrefab;
    public GameObject FoodPrefab;  // 🟢 Yeni eklendi: Yem prefab'ı referansı
    public Vector3 SpawnArea = new Vector3(); // 🟢 Yem doğma alanı

    // Lists
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        GrowSnake();
        GrowSnake();

        for (int i = 0; i < maxFoodCount; i++)
        {
            SpawnFood();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = 10;
            BodySpeed = 10;
        }
        else
        {
            MoveSpeed = 5;
            BodySpeed = 5;
        }
            // Move forward
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        // Steer
        float steerDirection = Input.GetAxis("Horizontal"); // Returns value -1, 0, or 1
        transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

        // Store position history
        PositionsHistory.Insert(0, transform.position);

        // Move body parts
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

            // Move body towards the point along the snakes path
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed * Time.deltaTime;

            // Rotate body towards the point along the snakes path
            body.transform.LookAt(point);

            index++;
        }
    }

    private void GrowSnake()
    {
        // Instantiate body instance and
        // add it to the list
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }
    private void SpawnFood()
    {
        // Eğer sahnede zaten maxFoodCount kadar yem varsa, yeni oluşturma
        if (GameObject.FindGameObjectsWithTag("Food").Length >= maxFoodCount)
            return;
        Vector3 randomPos = new Vector3(
            Random.Range(-SpawnArea.x, SpawnArea.x),
            0.5f,
            Random.Range(-SpawnArea.z, SpawnArea.z)
        );
        Instantiate(FoodPrefab, randomPos, Quaternion.identity);
    }

    // 🟢 Yeni eklendi: Yemle çarpışma kontrolü
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);  // Yemi yok et
            GrowSnake();                // Yılanı büyüt
            SpawnFood();                // Yeni yem oluştur
        }
    }
}