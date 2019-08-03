using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    private Vector3 MovingDirection = Vector3.left;    //initial movement direction
    public float startx;
    public float movementRange;

    // Start is called before the first frame update
    void Start()
    {
        startx = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }
    void UpdateMovement()
    {
        if (this.transform.position.x > startx + movementRange)
        {
            MovingDirection = Vector3.left;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;

        }
        else if (this.transform.position.x < startx - movementRange)
        {
            MovingDirection = Vector3.right;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;

        }
        this.transform.Translate(MovingDirection * Time.smoothDeltaTime);
    }
}
