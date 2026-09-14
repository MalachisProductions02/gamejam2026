using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene2 : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject objetoActivar;
    [SerializeField] private float tiempoAntesDeCambiar = 3f;
    [SerializeField] private string escenaDestino;

    public void CambiarEscena()
    {
        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }

        if (objetoActivar != null)
        {
            objetoActivar.SetActive(true);
        }

        yield return new WaitForSeconds(tiempoAntesDeCambiar);

        SceneManager.LoadScene(escenaDestino);
    }
}   
