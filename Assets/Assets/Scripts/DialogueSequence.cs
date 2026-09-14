using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Frase
{
    [TextArea(2, 5)]
    public string texto;

    public float duracion = 3f;

    public bool usarTipografiaAlternativa;

    public bool reproducirAudio;

    public AudioClip audio;
}

public class DialogueSequence : MonoBehaviour
{
    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI textUI;

    [Header("Tipografía alternativa")]
    [SerializeField] private TMP_FontAsset tipografiaAlternativa;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Frases")]
    [SerializeField] private Frase[] frases;

    [Header("Inicio")]
    [SerializeField] private float retrasoInicial = 3f;

    [Header("Al terminar")]
    [SerializeField] private GameObject objetoActivar;
    [SerializeField] private bool ocultarTextoAlFinal = true;

    [Header("Fade Out")]
    [SerializeField] private float duracionFadeOut = 2f;

    private TMP_FontAsset tipografiaOriginal;
    private AudioClip audioActual;

    private void Start()
    {
        tipografiaOriginal = textUI.font;

        textUI.gameObject.SetActive(false);

        if (objetoActivar != null)
        {
            objetoActivar.SetActive(false);
        }

        StartCoroutine(MostrarFrases());
    }

    private IEnumerator MostrarFrases()
    {
        yield return new WaitForSeconds(retrasoInicial);

        textUI.gameObject.SetActive(true);

        for (int i = 0; i < frases.Length; i++)
        {
            textUI.text = frases[i].texto;

            if (frases[i].usarTipografiaAlternativa && tipografiaAlternativa != null)
            {
                textUI.font = tipografiaAlternativa;
            }
            else
            {
                textUI.font = tipografiaOriginal;
            }

            if (frases[i].reproducirAudio && frases[i].audio != null && audioSource != null)
            {
                if (audioActual != frases[i].audio)
                {
                    audioSource.volume = 1f;
                    audioSource.clip = frases[i].audio;
                    audioSource.Play();

                    audioActual = frases[i].audio;
                }
            }

            yield return new WaitForSeconds(frases[i].duracion);
        }

        if (objetoActivar != null)
        {
            objetoActivar.SetActive(true);
        }

        if (ocultarTextoAlFinal)
        {
            textUI.gameObject.SetActive(false);
        }
    }

    public IEnumerator FadeOutAudio()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            yield break;
        }

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
