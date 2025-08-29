using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int spawnInterval = 5;
    private GameObject plane;
    private Vector3 spawnPosition;

    void Start()
    {
        plane = GameObject.FindGameObjectWithTag("Ground");
        InvokeRepeating("SpawnEnemy", 2, spawnInterval);
    }

    //spawning enemy objects at random positions on the plane
    private void SpawnEnemy()
    {
        Vector3 planeSize = plane.GetComponent<Renderer>().bounds.size;
        float halfX = planeSize.x / 2f;
        float halfZ = planeSize.z / 2f;
        spawnPosition = new Vector3(Random.Range(-halfX, halfX), plane.transform.position.y + 0.5f, Random.Range(-halfZ, halfZ));
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
