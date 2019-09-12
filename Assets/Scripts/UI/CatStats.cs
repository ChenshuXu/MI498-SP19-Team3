using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatStats : MonoBehaviour
{
    public GameObject Cat;
    //public ScarfController scarf;
    public int health=5;
    public int damageDealtToCat = 1;
    public int maxHealth = 5;

    public float damageCooldown=2;
    private bool coolDown;
    private float coolDownCounter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        coolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown)
        {
            coolDownCounter += Time.deltaTime;
            if (coolDownCounter > damageCooldown)
            {
                coolDown = false;
                coolDownCounter = 0;
            }
        }
        else
        {
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(health);
        if (other.tag == "Damage")
        {
            if (!coolDown)
            {
                health -= damageDealtToCat;
            }
            coolDown = true;

        }
        else if (other.tag == "Health")
        {
            if (health < maxHealth)
            {
                health += 1;
                Destroy(other.gameObject);
            }       
        }
        else if (other.tag == "Yarn")
        {
            //scarf.IncreaseMaxScarf();
            Destroy(other.gameObject);
        }
        /*else if (other.tag == "Yarn")
        {
            scarf.AddToScarf();
            Destroy(other.gameObject);
        }
        else if (other.tag == "Test")
        {
            scarf.SubtractFromScarf();
            Destroy(other.gameObject);
        }*/
    }
}
