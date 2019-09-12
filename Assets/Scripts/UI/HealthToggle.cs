using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthToggle : MonoBehaviour
{
    public CatStats cat;
    public GameObject heart;
    private GameObject newHeart;
    public bool isBB;
    private float instPos;
    private float lasthealth;
    // Start is called before the first frame update
    void Start()
    {
        lasthealth = cat.health;
        for (int i = 0; i < cat.health-1; i++)
        {
            if (isBB)
            {
                instPos -= heart.GetComponent<RectTransform>().sizeDelta.x;
            }
            else
            {
                instPos += heart.GetComponent<RectTransform>().sizeDelta.x;
            }
            
            newHeart = Instantiate(heart, new Vector3(heart.transform.position.x+instPos, heart.transform.position.y, heart.transform.position.z), Quaternion.identity);
            newHeart.transform.SetParent(gameObject.transform); //make instantiated piece child heart UI
            //heart = newHeart;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (cat.health != lasthealth)
        {
            if (cat.health < lasthealth)
            {
                int numChildren = gameObject.transform.childCount-1; //pos of last heart
                Destroy(this.transform.GetChild(numChildren).gameObject);
                if (isBB)
                {
                    instPos += heart.GetComponent<RectTransform>().sizeDelta.x;
                }
                else
                {
                    instPos -= heart.GetComponent<RectTransform>().sizeDelta.x;
                }
            }
            else
            {
                if (isBB)
                {
                    instPos -= heart.GetComponent<RectTransform>().sizeDelta.x;
                }
                else
                {
                    instPos += heart.GetComponent<RectTransform>().sizeDelta.x;
                }
                newHeart = Instantiate(heart, new Vector3(heart.transform.position.x + instPos, heart.transform.position.y, heart.transform.position.z), Quaternion.identity);
                newHeart.transform.SetParent(gameObject.transform);//make instantiated piece child heart UI
            }
            lasthealth = cat.health;
        }
        
    }
}
