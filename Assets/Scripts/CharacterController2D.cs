using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
    //Player Gamefeel Values
    [Header("Movement Values")]
    public float walkingSpeed;
    public float sprintingSpeed;
    public float maxMovementSpeed;
    public float jumpVelocity;
    public float thrustVelocity;

    [Header("Game Conditions")]
    public float deathBoundary;
    public Text countText;
    public Text winText;

    [Header("Audio SFX")]
    public AudioSource SFX;
    public AudioClip jumpSFX;
    public AudioClip coinSFX;
    public AudioClip loseSFX;

    //Input Variables
    private bool left_input = false;
    private bool right_input = false;
    private bool jump_input = false;
    private bool sprint_input = false;

    [Header("Debug Values")]
    //Movement Variables
    [SerializeField]
    private int playerLives = 3;
    [SerializeField]
    private bool playerDead = false;
    [SerializeField]
    private bool levelCompleted = false;
    [SerializeField]
    private float timeout = 10.0f;
    [SerializeField]
    private int pointsNeeded;
    [SerializeField]
    private int count;
    [SerializeField]
    private Vector2 PlayerStartPosition;
    [SerializeField]
    private float movementSpeed = 1.0f;
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool isLedgeGrabbing;
    [SerializeField]
    private bool canThrust = true;
    private Vector2 groundedCheckPosition;
    private Vector2 groundedCheckLeft;
    private Vector2 groundedCheckRight;

    //Components
    Animator animator;
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;

    void OnDrawGizmosSelected()
    {
        // Draws a blue line from this transform to the target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(-1000000.0f, deathBoundary), new Vector2(1000000.0f, deathBoundary));
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayerStartPosition = transform.position;
        pointsNeeded = GameObject.FindGameObjectsWithTag("PickUp").Length;
        count = 0;
        winText.text = "";
        SetCountText();

        
    }

    void FixedUpdate()
    {
        //Utility Scripts
        isGroundedUpdate();

        //Get Input
        GetInput();

        //Move Player
        if (!playerDead)
        {
            PlayerMovement();
        }
        
        if (levelCompleted)
        {
            timeout -= Time.deltaTime;

            if (timeout < 0)
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (SceneManager.sceneCount > nextSceneIndex)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
            }
        }
    }

    void GetInput()
    {
        //Update Input Values
        left_input = Input.GetKey("a") || Input.GetKey("left") ? true : false;
        right_input = Input.GetKey("d") || Input.GetKey("right") ? true : false;
        sprint_input = Input.GetKey("left shift") ? true : false;
        jump_input = Input.GetKey("space") ? true : false;
    }

    void PlayerMovement()
    {
        string playerMovementAnimation = "Player Walking";
        float movementSpeedCap = maxMovementSpeed;
        if (isGrounded && !canThrust)
        {
            canThrust = true;
        }
        if (sprint_input)
        {
            movementSpeed = sprintingSpeed;
            playerMovementAnimation = "Player Running";
            movementSpeedCap = maxMovementSpeed;
        }
        else
        {
            movementSpeed = walkingSpeed;
            playerMovementAnimation = "Player Walking";
            movementSpeedCap = maxMovementSpeed * 0.5f;
        }
        if (left_input)
        {
            rb2d.AddForce(new Vector2(-movementSpeed, 0), ForceMode2D.Impulse);
            
        }

        if (right_input)
        {
            rb2d.AddForce(new Vector2(movementSpeed, 0), ForceMode2D.Impulse);
        }

        if (jump_input && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpVelocity);
            SFX.clip = jumpSFX;
            SFX.Play();
        }
        if (jump_input && !isGrounded && isLedgeGrabbing && canThrust)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, thrustVelocity);
            canThrust = false;
            SFX.clip = jumpSFX;
            SFX.Play();
        }

        if (isGrounded)
        {
            if (rb2d.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
                animator.Play(playerMovementAnimation);
            }
            if (rb2d.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
                animator.Play(playerMovementAnimation);
            }
            if (rb2d.velocity.x > -0.5f && rb2d.velocity.x < 0.5f)
            {
                animator.Play("Player Idle");
            }
        }
        else
        {
            if (rb2d.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            if (rb2d.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            if (isLedgeGrabbing)
            {
                animator.Play("Player Ledge Grab");
            }
            else
            {
                if (rb2d.velocity.y > 0)
                {
                    animator.Play("Player Jumping");
                }
                else
                {
                    animator.Play("Player Falling");
                }
                
            }
        }
        rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -movementSpeedCap, movementSpeedCap), rb2d.velocity.y);

        if ((transform.position.y < deathBoundary) && (playerLives > 0))
        {
            playerLives -= 1;
            
            if (playerLives <= 0)
            {
                playerDead = true;
                winText.text = "You Lost!";
            }
            else
            {
                transform.position = PlayerStartPosition;
                SFX.clip = loseSFX;
                SFX.Play();
            }
        }
        
    }

    void isGroundedUpdate()
    {
        groundedCheckPosition = new Vector2(transform.position.x, transform.position.y - 1);
        groundedCheckLeft = new Vector2(transform.position.x - 1, transform.position.y);
        groundedCheckRight = new Vector2(transform.position.x + 1, transform.position.y);

        //Check If Grounded on the floor
        if (Physics2D.Linecast(transform.position, groundedCheckPosition, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }


        if ((Physics2D.Linecast(transform.position, groundedCheckLeft, 1 << LayerMask.NameToLayer("Ground"))) || (Physics2D.Linecast(transform.position, groundedCheckRight, 1 << LayerMask.NameToLayer("Ground"))))
        {
            isLedgeGrabbing = true;
        }
        else
        {
            isLedgeGrabbing = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            SFX.clip = coinSFX;
            SFX.Play();
            count = count + 1;
            SetCountText();
        }

        if (other.gameObject.CompareTag("enemy"))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                playerLives -= 1;

                if (playerLives <= 0)
                {
                    playerDead = true;
                    winText.text = "You Lost!";
                    transform.position = PlayerStartPosition;
                }
                else
                {
                    transform.position = PlayerStartPosition;
                    SFX.clip = loseSFX;
                    SFX.Play();
                }
            }

        }
    }

    void SetCountText()
    {
        countText.text = "Coins Collected: " + count.ToString() + "/" + pointsNeeded.ToString() + "\nLives: " + playerLives.ToString();

        if (count >= pointsNeeded)
        {
            winText.text = "You win!";
            levelCompleted = true;
        }
    }
}
