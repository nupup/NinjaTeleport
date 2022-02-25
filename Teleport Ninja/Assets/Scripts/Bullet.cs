using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 30;

    private void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
