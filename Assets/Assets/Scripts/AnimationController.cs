using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void ReturnAnimation()
    {
        anim.SetBool("Bye", false);
        anim.SetBool("Return", true);
    }
}
