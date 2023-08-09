using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;
    private Rigidbody _rigidbody;
    private bool _hit = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.MovePosition(_rigidbody.position + transform.forward * speed * Time.deltaTime);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_hit) return;
        var other = collision.collider.gameObject;
        RunCollisionLogic(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hit) return;
        RunCollisionLogic(other.gameObject);
    }
    
    void RunCollisionLogic(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            _hit = true;
            Debug.Log("HIT");
            Destroy(gameObject);
        }
    }
}
