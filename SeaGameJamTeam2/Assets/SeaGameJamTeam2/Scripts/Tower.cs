using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public enum TowerType
    {
        Venus = 0, 
        Eyeball = 1, 
        Monster = 2,
    }

    [SerializeField] private GameObject venus;
    [SerializeField] private GameObject eyeball;
    [SerializeField] private GameObject monster;
    
    public TowerType towerType;

    private int exp = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SetTowerType(towerType);
    }

    // Update is called once per frame
    void Update()
    {
        //just for prototype:
    }

    public void AddExp()
    {
        exp++;
        
        if (exp < 5)
        {
            SetTowerType(TowerType.Venus);
        }
        else if (exp < 10)
        {
            SetTowerType(TowerType.Eyeball);
        }
        else if (exp < 15)
        {
            SetTowerType(TowerType.Monster);
        }
    }
    
    public void SetTowerType(TowerType type)
    {
        venus.SetActive(type == TowerType.Venus);
        eyeball.SetActive(type == TowerType.Eyeball);
        monster.SetActive(type == TowerType.Monster);
    }
    
}
