using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Range(0.01f, 10.0f)]
    public float lifeTime = 10.0f;
    
    [Range(1.0f, 30.0f)]
    public float speed = 10.0f;
    
    private Rigidbody _rigidbody;
    private float _lifeTime = 0.0f;
    private bool _hit = false;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _lifeTime += Time.deltaTime;

        if (_lifeTime > lifeTime)
        {
            Destroy(gameObject);
        }
        
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
            Hittable hittable = other.GetComponent<Hittable>();
            hittable.Hit();
            Destroy(gameObject);
        }
    }
}
