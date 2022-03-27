//=========== Written by Lizband_UCC | Bix The Game Dev =======================
//
// Purpose: 
// Applied to: 
// Editor script: 
// Notes: 
//
//=============================================================================
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NUPFirstpersonRespawnHeight : MonoBehaviour
{
    //=-----------------=
    // Public variables
    //=-----------------=
    public List<Rigidbody> physicsEntities = new List<Rigidbody>();
    public List<Rigidbody2D> physicsEntities2D = new List<Rigidbody2D>();
    public Transform respawnPosition;


    //=-----------------=
    // Private variables
    //=-----------------=



    //=-----------------=
    // Reference variables
    //=-----------------=



    //=-----------------=
    // Mono Functions
    //=-----------------=
    private void Start()
    {
        FindEntites();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (physicsEntities.Contains(other.GetComponent<Rigidbody>()) || physicsEntities2D.Contains(other.GetComponent<Rigidbody2D>()))
        {
            other.transform.position = respawnPosition.transform.position;
        }
    }
    
    
    
    //=-----------------=
    // Internal Functions
    //=-----------------=
    
    
    
    //=-----------------=
    // External Functions
    //=-----------------=
    public void FindEntites()
    {
        physicsEntities = FindObjectsOfType<Rigidbody>().ToList();
        physicsEntities2D = FindObjectsOfType<Rigidbody2D>().ToList();
    }
}