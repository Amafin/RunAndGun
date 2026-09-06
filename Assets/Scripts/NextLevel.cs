using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string nextSceneName;

    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelCompleted) return;

        if (collision.CompareTag("Player"))
        {
            levelCompleted = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}