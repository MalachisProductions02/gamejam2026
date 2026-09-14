using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimer : MonoBehaviour
{
    [SerializeField] private float tiempo = 85f;
    [SerializeField] private string escenaDestino;

    [Header("Fade Out")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool fadeOutAntesDeCambiar = true;
    [SerializeField] private float duracionFadeOut = 2f;

    private void Start()
    {
        StartCoroutine(CambiarEscena());
    }

    private IEnumerator CambiarEscena()
    {
        float tiempoEspera = tiempo;

        if (fadeOutAntesDeCambiar && audioSource != null)
        {
            tiempoEspera = Mathf.Max(0f, tiempo - duracionFadeOut);
        }

        yield return new WaitForSeconds(tiempoEspera);

        if (fadeOutAntesDeCambiar && audioSource != null)
        {
            yield return StartCoroutine(FadeOutAudio());
        }

        SceneManager.LoadScene(escenaDestino);
    }

    private IEnumerator FadeOutAudio()
    {
        float volumenInicial = audioSource.volume;
        float tiempo = 0f;

        while (tiempo < duracionFadeOut)
        {
            tiempo += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(
                volumenInicial,
                0f,
                tiempo / duracionFadeOut
            );

            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
