// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

using UnityEngine;
using Rewired;
using Obi;
using cakeslice;

/// <summary>
/// Rewired Controller for both characters
/// </summary>
[AddComponentMenu("")]
[RequireComponent(typeof(Rigidbody))]
public class InputController : MonoBehaviour
{
    [Header("Set Manually")]
    public bool noInput = false;
    public EndAnims endAnims;
    public int playerId = 0; // The Rewired player id of this character
    public float moveSpeed = 5.5f; // Speed of horizontal movement
    public float slowMoveSpeed = 3.0f; // Speed of grandma when holding box
    public float jumpSpeed = 3.0f; // Strength of jump
    public float fallMult = 5f; // To increase gravity
    public float smallFallMult = 4f;
    public float upFallMult = 2f;
    public float ground = 1f; // How far the ground is from the game object
    
    public bool debug = true; // Used for editor raycast
    public float raycastHeadOffset = 1.0f;
    public ScarfObiController scarfController;
    public GameObject fakeBox;
    public ParticleSystem yarnLengthenParticles;
    public ParticleSystem yarnShortenParticles;
    public TrailRenderer yarnTrail;
    public ParticleSystem deathParticles;
    public ParticleSystem jumpParticles;
    public ParticleSystem landParticles;
   

    [Header("Set Dynamically")]
    public GameManager gameManager;
    public GameObject pickupObject;
    public InputController grandma;
    public InputController baby;
    public PickupController gmaPickupCont;
    public Vector3 moveVector;
    public Vector3 movementDir;
    public bool allowRotate = true;
    public bool slowed = false;
    public bool bePicked = false;
    public Outline[] m_Outlines;
    private float m_OutlineShowTimeDelay = 0.2f;
    private float tmp_m_OutlineShowTimeDelay = 0.0f;

    [Header("Input boolean")]
    public bool thrown = false;
    public bool grab = false;
    public bool meow = false;
    public bool jump; // Press jump button
    public bool restart; // Press reset button
    public bool ropeChanging = false;
    private GameObject cameraAngle;
    private GameObject cam;
    private Player player; // The Rewired Player
    private Rigidbody rb;
    private AudioManager audioManager;
    
    // Status boolean
    public bool canMove = true;
    public bool haveBbcat = false;
    public bool haveBox = false;
    public bool changeScarfInc = false;
    public bool changeScarfDec = false;
    public bool grounded;
    public bool midJump = false;
    public bool firstGrounded = true;

    [System.NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
    private bool initialized;
    private PickupController pickupCont;
    private ObiRope rope;
    private AudioSource meowSound;
    private bool disableAnim = true;


    private void Start()
    {
        // Get the rigidbody on this game object
        rb = GetComponent<Rigidbody>();

        // Initializes an empty game object that tracks the
        // camera Y angle for player movement direction
        cameraAngle = GameObject.Find("GetsCameraYAngle");

        cam = Camera.main.gameObject;
        gameManager = GameManager.instance;
        audioManager = AudioManager.instance;
        rope = scarfController.gameObject.GetComponent<ObiRope>();
        
        pickupCont = GetComponent<PickupController>();

        // Find all outline scripts
        m_Outlines = GetComponentsInChildren<Outline>();
        // Disable them at start
        foreach (Outline outline in m_Outlines)
        {
            outline.enabled = false;
        }

        meowSound = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Get the Rewired Player object for this player.
    /// </summary>
    private void Initialize()
    {
        player = ReInput.players.GetPlayer(playerId);

        initialized = true;
    }

    /// <summary>
    /// Call functions for getting input
    /// </summary>
    void Update()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (!initialized) Initialize(); // Reinitialize after a recompile in the editor
        if (noInput) return;
        else if (!noInput && disableAnim)
        {
            disableAnim = false;
            if(GetComponent<Animator>())
            {
                GetComponent<Animator>().enabled = false;
                endAnims.enabled = false;
            }
        }

        grounded = IsGrounded();

        if(grounded && firstGrounded && landParticles)
        {
            firstGrounded = false;
            landParticles.Play();
        }
        if(!grounded)
        {
            firstGrounded = true;
        }

        // Turn on/off the dust particles
        if (!grounded || (rb.velocity.z == 0 && rb.velocity.x == 0))
        {
            GetComponent<ParticleSystem>().Stop(false);
        }
        else if (!GetComponent<ParticleSystem>().isPlaying)
        {
            GetComponent<ParticleSystem>().Play(false);
        }

        GetInput();
        
        if (tmp_m_OutlineShowTimeDelay >= 0.0f)
        {
            tmp_m_OutlineShowTimeDelay -= Time.deltaTime;
        }
        CheckViewBlock();

        // Check if thrown cat has landed
        if(thrown && grounded)
        {
            thrown = false;
        }
        if(midJump && grounded)
        {
            midJump = false;
        }

        if(haveBox && rb.velocity.magnitude > 0.01 && grounded)
        {
            audioManager.PlayMoveBox();
        }
        else if(haveBox)
        {
            audioManager.StopMoveBox();
        }
    }

    void FixedUpdate()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (!initialized) Initialize(); // Reinitialize after a recompile in the editor
        if (noInput) return;

        ProcessInput();

        // Make the jump have more weight to it
        if(!grounded && !thrown)
        {
            if (rb.velocity.y < 0)
            {
                // Moving downward
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMult) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !player.GetButton("Jump"))
            {
                // Moving upward and let go of jump button
                rb.velocity += Vector3.up * Physics.gravity.y * (smallFallMult) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0)
            {
                // Moving upward and holding jump
                rb.velocity += Vector3.up * Physics.gravity.y * (upFallMult) * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Get Rewired inputs
    /// </summary>
    private void GetInput()
    {
        // Direction of movement based on camera
        if(player.GetAxis("Move Horizontal") != 0 || player.GetAxis("Move Vertical") != 0)
        {
            movementDir = (cameraAngle.transform.right * player.GetAxis("Move Horizontal"))
                      + (cameraAngle.transform.forward * player.GetAxis("Move Vertical"));
        }

        float maxSpeed = 0;

        if (canMove && !thrown) // got rid of && grounded
        {
            // forward movement should be forward from camera
            if (slowed == false)
            {
                moveVector = (cameraAngle.transform.right * player.GetAxis("Move Horizontal") * moveSpeed) +
                            (cameraAngle.transform.forward * player.GetAxis("Move Vertical") * moveSpeed);
                moveVector.y = 0; // don't affect the y
                maxSpeed = moveSpeed;
                //Debug.Log("fast");
            }
            else
            {
                moveVector = (cameraAngle.transform.right * player.GetAxis("Move Horizontal") * slowMoveSpeed) +
                            (cameraAngle.transform.forward * player.GetAxis("Move Vertical") * slowMoveSpeed);
                moveVector.y = 0; // don't affect the y
                maxSpeed = slowMoveSpeed;
                //Debug.Log("slow");
            }
        }
        /*
        else if ((player.GetAxis("Move Horizontal") != 0 || player.GetAxis("Move Vertical") != 0) && !thrown && canMove) // added to stop from random rotations
        {
            Debug.Log("Other");
            // decreased input when in the air
            moveVector = rb.velocity/1.1f + (cameraAngle.transform.right * player.GetAxis("Move Horizontal")/4f)
                            +(cameraAngle.transform.forward * player.GetAxis("Move Vertical")/4f);
            moveVector.y = 0; // don't affect the y
            maxSpeed = moveSpeed;
        }
        */

        // clamp diagonal movement to max speed
        if (!thrown && moveVector.magnitude > maxSpeed)
        {
            moveVector = moveVector.normalized * maxSpeed;
        }

        // Check jump
        if (player.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
        else
        {
            jump = false;
        }

        // Increase and decrease scarf
        if ((player.GetButton("Inc") && rope.RestLength < scarfController.maxLength)
            || (player.GetButton("Dec") && rope.RestLength > scarfController.minLength))
        {
            if(player.GetButton("Inc") && player.GetButton("Dec"))
            {
                changeScarfInc = false;
                changeScarfDec = false;
                ropeChanging = false;

                audioManager.StopExtend(playerId);
                audioManager.StopShorten(playerId);

                if (yarnLengthenParticles.isPlaying)
                {
                    yarnLengthenParticles.Stop();
                    if (yarnTrail != null)
                    {
                        yarnTrail.emitting = false;
                    }
                }
                if (yarnShortenParticles.isPlaying)
                {
                    yarnShortenParticles.Stop();
                    if (yarnTrail != null)
                    {
                        yarnTrail.emitting = false;
                    }
                }
            }
            else
            {
                if (player.GetButton("Inc"))
                {
                    changeScarfInc = true;
                    //Debug.Log("inc");
                    scarfController.AddToScarf();
                    audioManager.PlayExtend(playerId);

                    if (!yarnLengthenParticles.isPlaying)
                    {
                        yarnLengthenParticles.Play();

                        // Play trail only for baby
                        if (yarnTrail != null)
                        {
                            yarnTrail.emitting = true;
                        }
                    }
                }
                else
                {
                    audioManager.StopExtend(playerId);
                    changeScarfInc = false;
                    if (yarnLengthenParticles.isPlaying)
                    {
                        yarnLengthenParticles.Stop();
                        if (yarnTrail != null)
                        {
                            yarnTrail.emitting = false;
                        }
                    }
                }
                if (player.GetButton("Dec"))
                {
                    changeScarfDec = true;
                    //Debug.Log("dec");
                    scarfController.SubtractFromScarf();
                    audioManager.PlayShorten(playerId);
                    ropeChanging = true;

                    if (!yarnShortenParticles.isPlaying)
                    {
                        yarnShortenParticles.Play();

                        // Play trail only for baby
                        if (yarnTrail != null)
                        {
                            yarnTrail.emitting = true;
                        }
                    }
                }
                else
                {
                    audioManager.StopShorten(playerId);
                    ropeChanging = false;
                    changeScarfDec = false;
                    if (yarnShortenParticles.isPlaying)
                    {
                        yarnShortenParticles.Stop();
                        if (yarnTrail != null)
                        {
                            yarnTrail.emitting = false;
                        }
                    }
                }
            }
        }
        else
        {
            changeScarfInc = false;
            changeScarfDec = false;
            ropeChanging = false;

            audioManager.StopExtend(playerId);
            audioManager.StopShorten(playerId);

            if (yarnLengthenParticles.isPlaying) {
                yarnLengthenParticles.Stop();
                if (yarnTrail != null) {
                    yarnTrail.emitting = false;
                }
            }
            if (yarnShortenParticles.isPlaying) {
                yarnShortenParticles.Stop();
                if (yarnTrail != null) {
                    yarnTrail.emitting = false;
                }
            }
        }
        
        restart = player.GetButtonDown("Restart");
    }


    /// <summary>
    /// Move the player
    /// </summary>
    private void ProcessInput()
    {
        // Process jump, check jump again just in case
        if ((jump || (player.GetButtonDown("Jump") && grounded)) && haveBox == false && !gameManager.tutUnpausing)
        {
            Vector3 yVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.velocity = yVel + Vector3.up * jumpSpeed;
            midJump = true;

            // Play jump sound
            audioManager.PlayJump(playerId);
            if(jumpParticles)
                jumpParticles.Play();
        }

        // Process movement
        if (thrown && canMove)
        {
            // decreased input when in the air
            moveVector = rb.velocity;
            //Debug.Log("Thrown");
            Debug.Log(transform.rotation);
        }

        // Apply velocity to the rigidbody here
        moveVector.y = rb.velocity.y;

        //if(rope.CalculateLength() > rope.RestLength)
        //{
            //Debug.Log("Rope");
        //}

        if((baby.ropeChanging || grandma.ropeChanging || rope.CalculateLength() > rope.RestLength)
            && !grounded && !thrown && !midJump)
        {
            // Do nothing to the velocity, the baby is being pulled in the air
            //Debug.Log("What");
        }
        else
        {
            if (canMove)
            {
                rb.velocity = moveVector;
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }

        if(haveBox && playerId == 1)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // Process rotation
        if ((moveVector.x != 0 || moveVector.z != 0) && allowRotate)
        {
            float z_angle = Mathf.Atan2(movementDir.x , movementDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, z_angle, 0));
        }

        // Process pick up box or baby
        if(player.GetButtonDown("Pick") || (player.GetButtonUp("Pick") && (haveBox || haveBbcat)))
        {
            grab = true;
        }
        else
        {
            grab = false;
        }
        // Check m_TutorialPause state
        // To avoid confliction of two actions
        if (grab && !bePicked && !gameManager.GetTutorialPause())
        {
            pickupCont.PickupPressed();
        }

        // Process drop baby cat, both can drop
        if(player.GetButtonDown("Drop"))
        {
            gmaPickupCont.Drop();
        }

        // Process baby meow
        meow = player.GetButtonDown("Meow");
        if(meow)
        {
            meowSound.Play();
        }

        if (restart)
        {
            gameManager.RestartLevel();
        }
    }


    /// <summary>
    /// Is the player currently on the ground?
    /// </summary>
    /// <returns>True or false depending on if player is grounded</returns>
    private bool IsGrounded()
    {
        RaycastHit hit;
        if(haveBox)
        {
            MoveBoxController mbCont = pickupObject.GetComponent<MoveBoxController>();
            if (playerId == 0 && pickupObject.GetComponent<MoveBoxController>().IsGroundedSmall())
            {
                return true;
            }
            else if (playerId == 1 && pickupObject.GetComponent<MoveBoxController>().IsGrounded())
            {
                return true;
            }
        }
        return Physics.SphereCast(transform.position, transform.localScale.x / 1.5f, Vector3.down, out hit, ground);
    }


    /// <summary>
    /// Stops player from moving or allows them to move again
    /// </summary>
    /// <param name="move">true if player can move, false if cannot</param>
    public void SetCanMove(bool move)
    {
        canMove = move;
    }


    /// <summary>
    /// Use raycast to check if view is blocked, set the block transparent
    /// </summary>
    private void CheckViewBlock()
    {
        Vector3 dir = cam.transform.position - transform.position;

        RaycastHit[] hits;
        // Ignore layer 15 BoxIgnore
        hits = Physics.RaycastAll(transform.position + new Vector3(0, raycastHeadOffset,0), dir, 15);
        if (debug)
        {
            Debug.DrawRay(transform.position + new Vector3(0, raycastHeadOffset,0), dir, Color.red);
        }
        bool blocked = false;
        foreach (RaycastHit h in hits)
        {
            if (h.transform.gameObject && h.transform.tag != "Character")
            {
                // Ignore trigger box collider such as PuzzleCameraPosition
                BoxCollider b = h.transform.gameObject.GetComponent<BoxCollider>();
                if (b && b.isTrigger)
                {
                    continue;
                }
                blocked = true;
                Debug.Log("Block View Obj: " + h.transform.gameObject);
            }
        }
        // TODO: Instead of disable the whole outline effect, shoudld enable and disable OutLine component in all mech renderers
        if (blocked)
        {
            tmp_m_OutlineShowTimeDelay = m_OutlineShowTimeDelay;
            gameManager.BlockSetter(playerId);
        }

        if (tmp_m_OutlineShowTimeDelay > 0)
        {
            foreach (Outline outline in m_Outlines)
            {
                outline.enabled = true;
            }
        }
        else
        {
            foreach (Outline outline in m_Outlines)
            {
                outline.enabled = false;
            }
        }
    }
}
