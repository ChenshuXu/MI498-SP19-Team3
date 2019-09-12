using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

/// <summary>
/// The controller for picking up/grabbing boxes or the baby
/// </summary>
public class PickupController : MonoBehaviour
{
    public enum PickupState { Picking, Pick, NotPick }

    [Header("Set Manually")]
    public GameObject fakeBb; // The collider for when holding the baby
    public GameObject pickupPosition; // position of cat when picked up
    public ObiRope rope; // Obi rope object

    public float addWeight = 500f; // Weight to add to box when held
    public float throwForce = 1; // Strength of the throw
    public float upDirStrength = 0.25f; // Upward angle of throw
    public float forDirStrength = 2f; // Strength of throw forward

    public float pickupRange = 1.0f; // how close the cat needs to be to pick it up
    public float pickupCooldown = 2.0f; // How long until the grandma can pick up the baby again
    public GameObject babyPickupArea;

    
    [Header("Set Dynamically")]
    public PickupState pickState = PickupState.NotPick; // is the cat picked up
    private GameObject pickupObject; // The baby cat

    private AudioManager audioManager;
    private GameManager gameManager;
    private InputController inputCont;
    private Rigidbody rb;
    
    private Vector3 throwDirection; // Direction of throw
    private float currCooldownTime = 0f; // Used to keep track of cooldown
    private float origWeight; // Original weight of this player
    private int playerId; // Rewired ID of this player


    private void Start()
    {
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;

        inputCont = GetComponent<InputController>();
        rb = GetComponent<Rigidbody>();
        origWeight = rb.mass;
        playerId = inputCont.playerId;
    }


    private void FixedUpdate()
    {
        // Countdown until grandma is allowed to pick up the baby again
        if (currCooldownTime >= 0)
        {
            currCooldownTime -= Time.deltaTime;
        }
    }


    /// <summary>
    /// Pickup button pressed, process it
    /// </summary>
    public void PickupPressed()
    {
        if (playerId == 1)
        {
            // The baby only checks for boxes
            InputPickUpBox();
        }
        else if (playerId == 0)
        {
            // The grandma checks for the baby then boxes
            DecideGrandma();
        }
    }


    /// <summary>
    /// Grandma can interact with boxes and baby
    /// </summary>
    private void DecideGrandma()
    {
        // Grandma is holding something, drop or throw
        if (inputCont.haveBox)
        {
            DropBox();
            return;
        }
        else if (inputCont.haveBbcat && pickState == PickupState.Pick)
        {
            Throw();
            return;
        }

        // Grandma not holding anything, pick up baby or box if in reach
        //Collider[] hitColliders = Physics.OverlapSphere(pickupPosition.transform.position, pickupRange);
        Collider[] hitColliders = Physics.OverlapBox(pickupPosition.transform.position, babyPickupArea.transform.localScale);
        foreach (Collider c in hitColliders)
        {
            // Check if it's the baby
            if (c.gameObject.tag == "Character" && c.gameObject != this.gameObject)
            {
                if (c.gameObject.GetComponent<InputController>().haveBox)
                {
                    return;
                }
                
                if (pickState == PickupState.NotPick && currCooldownTime <= 0)
                {
                    PickupBaby(c.gameObject);
                    return;
                }
            }
        }

        // No baby, check for boxes
        InputPickUpBox();
    }


    /* --------------------------- Baby cat pickup --------------------------- */
    /// <summary>
    /// Pick up game objects with tag "Character"
    /// <summary>
    private void PickupBaby(GameObject bby)
    {
        SetPickupState(PickupState.Pick);
        gameManager.ChangeCamTargets(true);

        // Set position and parent of baby cat
        pickupObject = bby;
        bby.transform.parent = transform;
        bby.transform.position = pickupPosition.transform.position;
        bby.transform.rotation = transform.rotation;

        // Simulate Bb collider
        fakeBb.transform.position = pickupObject.transform.position;
        fakeBb.transform.localScale = pickupObject.transform.localScale;
        fakeBb.GetComponent<CapsuleCollider>().height = pickupObject.GetComponent<CapsuleCollider>().height;
        fakeBb.GetComponent<CapsuleCollider>().radius = pickupObject.GetComponent<CapsuleCollider>().radius;
        fakeBb.SetActive(true);

        // Turn off rigidbody parts
        Rigidbody pickupRb = pickupObject.GetComponent<Rigidbody>();
        pickupRb.isKinematic = true;
        pickupRb.detectCollisions = false;
        pickupRb.velocity = new Vector3(0, 0, 0);

        // Change input controller bools for conditional checks
        bby.GetComponent<InputController>().bePicked = true;
        GetComponent<InputController>().haveBbcat = true;
    }


    /// <summary>
    /// Drop off baby cat and throw out
    /// <summary>
    private void Throw()
    {
        audioManager.PlayThrow();
        gameManager.ChangeCamTargets(false);
        pickupObject.GetComponent<InputController>().thrown = true;
        SetPickupState(PickupState.NotPick);
        pickupObject.transform.parent = transform.parent;
        currCooldownTime = pickupCooldown;

        // Disable fake baby collider
        fakeBb.SetActive(false);

        // Turn back on rigidbody parts
        Rigidbody pickupRb = pickupObject.GetComponent<Rigidbody>();
        pickupRb.isKinematic = false;
        pickupRb.detectCollisions = true;
        //pickupRb.AddForce(ThrowDir(), ForceMode.Impulse); // impulse might be messing it up
        pickupRb.velocity = ThrowDir();

        // Change input controller bools for conditional checks
        pickupObject.GetComponent<InputController>().bePicked = false;

        GetComponent<InputController>().haveBbcat = false;
        pickupObject.transform.rotation = transform.rotation;
        pickupObject.GetComponent<InputController>().movementDir = GetComponent<InputController>().movementDir;
    }


    /// <summary>
    /// Drop the baby instead of throwing
    /// </summary>
    public void Drop()
    {
        // Drop the baby in front of the grandma
        if (pickState == PickupState.Pick)
        {
            gameManager.ChangeCamTargets(false);
            SetPickupState(PickupState.NotPick);
            pickupObject.transform.parent = transform.parent;
            currCooldownTime = pickupCooldown;

            // Change input controller bools for conditional checks
            pickupObject.GetComponent<InputController>().bePicked = false;
            GetComponent<InputController>().haveBbcat = false;

            // Turn back on rigidbody parts
            Rigidbody pickupRb = pickupObject.GetComponent<Rigidbody>();
            pickupRb.isKinematic = false;
            pickupRb.detectCollisions = true;

            // Disable fake baby collider
            fakeBb.SetActive(false);
        }
    }


    /// <summary>
    /// State machine
    /// <summary>
    void SetPickupState(PickupState state)
    {
        pickState = state;
    }


    /// <summary>
    /// Calculate the throw Vector3
    /// </summary>
    /// <returns>the velocity in a direction</returns>
    public Vector3 ThrowDir()
    {
        // calculate throw direction
        Vector3 dir = transform.forward * forDirStrength + transform.up * upDirStrength;
        throwDirection = dir * rope.RestLength * throwForce;
        return throwDirection;
    }


    /* --------------------------- Box pickup --------------------------- */
    /// <summary>
    /// Figures out if player is picking up or dropping a box
    /// </summary>
    public void InputPickUpBox()
    {
        if (!inputCont.haveBox && inputCont.grounded)
        {
            PickUpBox();
        }
        else if (inputCont.haveBox)
        {
            DropBox();
        }
    }


    /// <summary>
    /// Picking up/grabbing a box
    /// </summary>
    private void PickUpBox()
    {
        // When pick up button is pressed, check if there is a box in reach
        Collider[] hitColliders = Physics.OverlapBox(pickupPosition.transform.position, transform.localScale / 2);
        
        foreach (Collider c in hitColliders)
        {
            // Check if it's a box
            if (c.gameObject.tag == "Grabbable")
            {
                pickupObject = c.gameObject;
                inputCont.pickupObject = pickupObject;
                MoveBoxController boxController = pickupObject.GetComponent<MoveBoxController>();

                // Only pick up a box not grabbed yet
                if (!boxController.bePicked)
                {
                    boxController.bePicked = true;
                    boxController.playerHolding = inputCont;
                    inputCont.haveBox = true;

                    // Rotate player towards box, just want y rotation
                    inputCont.allowRotate = false;
                    float itemRotationY = pickupObject.transform.eulerAngles.y % 90;
                    float change = 0;
                    if (itemRotationY > 45)
                    {
                        // items rotated more than 45 add 90 degrees to much
                        change = -90f;
                    }
                    Vector3 relativePos = pickupObject.transform.position - transform.position;
                    relativePos.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    float yRot = Mathf.Round(rotation.eulerAngles.y / 90.0f) * 90.0f + itemRotationY + change;
                    transform.rotation = Quaternion.Euler(new Vector3(0, yRot, 0));

                    // Different reactions for grandma vs baby
                    if (playerId == 0)
                    {
                        // Grandma can pick up box and move it
                        inputCont.slowed = true;
                    }
                    else if (playerId == 1)
                    {
                        // Baby cat can only grab the box
                        rb.mass = origWeight + 10f;
                        rb.velocity = Vector3.zero;
                        pickupObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        inputCont.canMove = false;
                    }

                    pickupObject.gameObject.layer = 13; //change layer so rope doesnt interfer with box

                    // Turn off physics for this box
                    pickupObject.GetComponent<Rigidbody>().isKinematic = true;
                    pickupObject.transform.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                    pickupObject.transform.parent = transform;
                    pickupObject.transform.GetChild(0).gameObject.SetActive(false);

                    // Turn on a fake collider to simulate the box's collider
                    inputCont.fakeBox.SetActive(true);
                    Vector3 add = new Vector3(0, 0.1f, 0);
                    inputCont.fakeBox.GetComponent<BoxCollider>().center = pickupObject.transform.localPosition + add;
                    inputCont.fakeBox.GetComponent<BoxCollider>().size = pickupObject.transform.localScale;

                    // Only pick up one box at a time
                    break;
                }
            }
        }
    }


    /// <summary>
    /// Dropping/letting go of a box
    /// </summary>
    private void DropBox()
    {
        // Tell the box it's been dropped
        MoveBoxController boxController = pickupObject.GetComponent<MoveBoxController>();
        boxController.bePicked = false;
        boxController.playerHolding = null;
        audioManager.StopMoveBox();

        // Let the player rotate again
        inputCont.allowRotate = true;

        // Different reactions for grandma vs baby
        if (playerId == 0)
        {
            inputCont.slowed = false;
        }
        else if (playerId == 1)
        {
            rb.mass = origWeight;
            inputCont.canMove = true;
        }

        // Turn back on physics for the box
        pickupObject.gameObject.layer = 0;
        pickupObject.GetComponent<Rigidbody>().isKinematic = false;
        pickupObject.GetComponent<Rigidbody>().detectCollisions = true;
        pickupObject.transform.parent = null;
        pickupObject.transform.GetChild(0).gameObject.SetActive(true);

        // Turn off the fake simulated box collider
        inputCont.fakeBox.SetActive(false);
        inputCont.haveBox = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (inputCont.haveBox && collision.gameObject.tag == "Character" && inputCont.playerId == 1 && inputCont.grounded)
        {
            // Increase weight so that the grandma can't push around the baby with the box
            rb.mass = addWeight;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (inputCont.haveBox && collision.gameObject.tag == "Character" && inputCont.playerId == 1 && inputCont.grounded)
        {
            // Decrease weight when grandma leaves
            rb.mass = origWeight + 10;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (inputCont.haveBox && collision.gameObject.tag == "Character" && inputCont.playerId == 1 && inputCont.grounded)
        {
            // Increase weight so that the grandma can't push around the baby with the box
            rb.mass = addWeight;
        }
    }
}
