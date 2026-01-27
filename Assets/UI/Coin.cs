using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    public int scoreValue = 100;
    public float rotateSpeed = 90f;

    public AudioClip pickupSound;
    [Range(0f, 1f)] public float volume = 1f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(scoreValue);
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);

            Destroy(gameObject);
        }
    }
}
