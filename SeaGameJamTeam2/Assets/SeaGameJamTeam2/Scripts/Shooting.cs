using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectile;
    public float timeInterval = 3;

    private float _time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        if (_time >= timeInterval)
        {
            Instantiate(projectile, transform.position, transform.rotation);
            _time = 0;
        }
    }
}
