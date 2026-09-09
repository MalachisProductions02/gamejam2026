using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    //[SerializeField] private Animator _animator;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    //private SpriteRenderer _spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        /*if (movementDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (movementDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isUp", false);
        _animator.SetBool("isDown", false);

        if (Mathf.Abs(movementDirection.x) == 1)
        {
            _animator.SetBool("isRunning", true);
        }
        else if (movementDirection.y == 1)
        {
            _animator.SetBool("isUp", true);
        }
        else if (movementDirection.y == -1)
        {
            _animator.SetBool("isDown", true);
        }*/


    }

    void FixedUpdate()
    {
        rb.velocity = movementDirection * speed;
    }
}
