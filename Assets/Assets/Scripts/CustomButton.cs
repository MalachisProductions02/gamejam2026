using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimatedObject
{
    public Animator animator;
    public string entryAnimation;

    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;
    [HideInInspector] public Vector3 originalScale;
}

public class CustomButton : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;
    [SerializeField] private GameObject hoverPanel;
    [SerializeField] private List<GameObject> objectsToDisable;
    [SerializeField] private List<AnimatedObject> anims;
    [SerializeField] private Animator animFondo;

    private void Start()
    {
        hoverObject.SetActive(false);
        hoverPanel.SetActive(false);
    }

    private void OnMouseEnter()
    {
        hoverObject.SetActive(true);
        Debug.Log("Mouse entered");
    }

    private void OnMouseExit()
    {
        hoverObject.SetActive(false);
        Debug.Log("Mouse exited");
    }

    private void OnMouseDown()
    {
        hoverObject.SetActive(false);
        hoverPanel.SetActive(true);

        foreach (AnimatedObject obj in anims)
        {
            if (obj.animator == null)
                continue;

            Transform t = obj.animator.transform;

            obj.originalPosition = t.localPosition;
            obj.originalRotation = t.localRotation;
            obj.originalScale = t.localScale;
        }

        if (animFondo != null)
        {
            animFondo.SetBool("Return", false);
            animFondo.SetBool("Bye", true);
        }
            

        foreach (AnimatedObject obj in anims)
        {
            if (obj.animator == null)
                continue;

            obj.animator.SetBool("Bye", true);
        }

        StartCoroutine(Disable());

        Debug.Log("Mouse clicked");
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        Debug.Log("Objects disabled");
    }

    public void Return()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (AnimatedObject obj in anims)
        {
            if (obj.animator == null)
                continue;

            Vector3 rotation = obj.animator.transform.localEulerAngles;
            rotation.z = 0f;
            obj.animator.transform.localEulerAngles = rotation;

            obj.animator.SetBool("Bye", false);
            obj.animator.Play(obj.entryAnimation, 0, 0f);
        }

        if (animFondo != null)
        {
            animFondo.SetBool("Bye", false);
        }

        Debug.Log("Objects returned");
    }
}
