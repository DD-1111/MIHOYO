using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class uneuclid : MonoBehaviour
{
    [Header("Components")]
    public Transform selectedObject; // The selected object for scaling

    [Header("Parameters")]
    public LayerMask objectMask;        // The layer mask for hitting only potential targets with a raycast
    public LayerMask excludeObjectMask; // The layer mask for ignoring the player and target objects while raycasting
    public float positionOffset;        // The offset amount for positioning the object to avoid clipping into walls

    float initialDistance;              // The initial distance between the player camera and the target
    float initialScale;                 // The initial scale of the target objects before resizing
    Vector3 desiredScale;               // The scale we want our object to have each frame
    private Mouse playerMouse;
    private RaycastHit hitInfo;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerMouse = Mouse.current;
    }

    void Update()
    {
        if (playerMouse.rightButton.wasPressedThisFrame)
        {
            if (selectedObject == null)
            {
                Debug.Log("1");
                if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, objectMask))
                {
                    Debug.Log("2");
                    Transform temp = hitInfo.transform;
                    if (temp.tag == "Button")
                    {
                        temp.GetComponent<ConfigurableButton>().DoTrigger();
                    }
           
                }
            }
            else
            {
                selectedObject.GetComponent<Rigidbody>().isKinematic = false;
                selectedObject = null;
            }
        }

        if (playerMouse.leftButton.wasPressedThisFrame)
        {
            if (selectedObject == null)
            {
           
                if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, objectMask))
                {
                    selectedObject = hitInfo.transform;
                    selectedObject.GetComponent<Rigidbody>().isKinematic = true;
                    initialDistance = Vector3.Distance(transform.position, selectedObject.position);
                    initialScale = selectedObject.localScale.x;
                    desiredScale = selectedObject.localScale;
                }
            }
            else
            {
                selectedObject.GetComponent<Rigidbody>().isKinematic = false;
                selectedObject = null;
            }
        }
        if (selectedObject == null)
        {
            return;
        }
        
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, excludeObjectMask))
        {
            selectedObject.position = hitInfo.point - transform.forward * positionOffset * desiredScale.x;
            float currentDistance = Vector3.Distance(transform.position, selectedObject.position);
            float scaleFactor = currentDistance / initialDistance;
            desiredScale.x = desiredScale.y = desiredScale.z = scaleFactor;
            selectedObject.localScale = desiredScale * initialScale;
        }
    }


}

