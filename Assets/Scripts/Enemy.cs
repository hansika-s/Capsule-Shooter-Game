using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float initialHealth = 5;
    private GameObject player;
    private float enemyHealth;
    private Renderer meshRenderer;
    private float enemySpeed = 0.1f;
    PlayerController playerController;


    void Awake()
    {
        enemyHealth = initialHealth;
        meshRenderer = GetComponent<Renderer>();
        //reference to player to update score
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

    }
    void Update()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, enemySpeed * Time.deltaTime);
    }

    //checking collisions with the bullet from player
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Bullet"))
        {
            enemyHealth--;
            enemyHealth = Math.Max(enemyHealth, 0);
            UpdateColor();

            if (enemyHealth == 0) { Destroy(gameObject); playerController.score++; }
        }
    }

    // setting enemy's look corresponding to the health
    private void UpdateColor()
    {
        float healthPercent = enemyHealth / initialHealth;
        Color newColor;
        if (healthPercent > 0.5f)
        {
            newColor = Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2f);
        }
        else
        {
            newColor = Color.Lerp(Color.red, Color.yellow, healthPercent * 2f);
        }
        SetColor(newColor);
    }
    private void SetColor(Color color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }

    }
}
