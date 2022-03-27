//=========== Written by Arthur W. Sheldon AKA Lizband_UCC ====================
//
// Purpose: Make camera follow players view (Fixes jitter problem when rotating and moving)
// Applied to: Camera Operator (An empty GameObject with a camera as it's child)
// Editor script: 
// Notes: This script was created by following Plai's FPS Movement Tutorial
//  series. (https://www.youtube.com/c/PlaiDev)
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NUPFirstpersonCameraFollow : MonoBehaviour
{
    //=-----------------=
    // Public variables
    //=-----------------=
    [SerializeField] Transform cameraPosition;


    //=-----------------=
    // Private variables
    //=-----------------=
    


    //=-----------------=
    // Reference variables
    //=-----------------=



    //=-----------------=
    // Mono Functions
    //=-----------------=
    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
