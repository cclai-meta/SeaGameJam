using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectile;
    private Tower _tower;
    private float _time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _tower = GetComponent<Tower>();
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        if (_time >= _tower.timeInterval)
        {
            _time = 0;
            
            GameObject nearestObject = GetNearestHittableObject();
            if (!nearestObject)
            {
                // Debug.Log("No Nearest Object");
                return;
            }
            
            var newProdectile = Instantiate(projectile, transform.position, transform.rotation);
            Vector3 dir = (nearestObject.transform.position - transform.position).normalized;
            newProdectile.transform.LookAt(transform.position + dir);

            Projectile pc = newProdectile.GetComponent<Projectile>();
            pc.SetTower(_tower);
        }
    }

    GameObject GetNearestHittableObject()
    {
        GameObject res = null;
        float distance = 99999999;
        var enemies = FindObjectsOfType<Hittable>();

        foreach (var e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.gameObject.transform.position);
            if (d < distance)
            {
                distance = d;
                res = e.gameObject;
            }
        }

        return res;
    }
}
