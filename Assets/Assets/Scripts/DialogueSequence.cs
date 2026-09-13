using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSequence : MonoBehaviour
{
    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI textUI;

    [TextArea(2, 5)]
    [SerializeField] private string[] frases;

    [Header("Duración de cada frase")]
    [SerializeField] private float[] duracionFrases;

    [Header("Inicio")]
    [SerializeField] private float retrasoInicial = 3f;

    [Header("Al terminar")]
    [SerializeField] private GameObject objetoActivar;
    [SerializeField] private bool ocultarTextoAlFinal = true;

    private void Start()
    {
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
            textUI.text = frases[i];

            // Duración de esta frase
            float duracion = 3f;

            if (i < duracionFrases.Length)
            {
                duracion = duracionFrases[i];
            }

            yield return new WaitForSeconds(duracion);
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
}
