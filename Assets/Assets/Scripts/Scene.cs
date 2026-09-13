using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string escenaDestino;

    public void CambiarEscena()
    {
        SceneManager.LoadScene(escenaDestino);
    }
}
