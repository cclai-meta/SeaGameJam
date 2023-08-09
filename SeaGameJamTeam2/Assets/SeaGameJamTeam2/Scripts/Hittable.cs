using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    public int HP = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit()
    {
        HP--;

        if (HP <= 0)
        {
            TDGrid.Get().HandleEntityDeath(true, gameObject);
            Destroy(gameObject);
        }
    }
}
