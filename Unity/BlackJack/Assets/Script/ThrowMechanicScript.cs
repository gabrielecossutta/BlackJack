using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ThrowMechanicScript : MonoBehaviour
{
    //strength of the throws 
    [SerializeField, Range(1, 10)]private float throwForce;

    //String used to search the card in the array of Card
    [NonSerialized]public new string name;

    //Gameobject to know have to be moved
    private GameObject objectToDrag;

    //LineRender used to visually see where the player is throwing the card
    [SerializeField] private LineRenderer lineRenderer;

    //starting position for the Line Rendere
    private Vector3 startMousePosition;

    //starting position of the card
    private Vector3 startObjectPosition;

    //bool to know if something is dragged
    private bool isDragging = false;

    //Rigidbody to apply  the throw force
    private Rigidbody rigidbody;

    //Used to know information about what hit the raycast
    private RaycastHit hit;

    [SerializeField] private DeckScript deckScript;
    void Start()
    {

    }

    void Update()
    {
        //Manage the mouse click
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }

        if (isDragging)
        {
            UpdateLine();
        }
    }

    //If the player click with the mouse this method is called
    //This method allow to drag card in the level
    void StartDrag()
    {
        //Cast a ray from the camera to the mousePosition
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the ray hit something, it check if it is a card, otherwise it does nothing
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //Assign the hit gameobject
            objectToDrag = hit.transform.gameObject;

            //Check if it as the "Card" tag
            if (objectToDrag.tag == "Card")
            {
                //Assign the position to use it for draw the Line Render and to calculate the throw direction
                startObjectPosition = objectToDrag.transform.position;

                //Assign the rigidbody for applying the force later
                rigidbody = objectToDrag.GetComponent<Rigidbody>();

                //Assign the name of the object and remove the last 7 char "(Clone)", used to check if the card throw is put below the deck
                name = hit.transform.gameObject.name;
                deckScript.PutUnderDeck(name.Substring(0, name.Length - 7));

                isDragging = true;
                lineRenderer.enabled = true;
            }
            else //If it does not have the "Card" tag, we have to unassign
            {
                objectToDrag = null;
            }
        }
    }

    //Method used to Update the Line Rendrer to draw the throw direction
    void UpdateLine()
    {
        //If the object to throw is null exit from this method
        if (objectToDrag == null)
        {
            return;
        }

        //We save the mouseposition
        Vector3 mousePosition = Input.mousePosition;

        //We convert the object Drag position to Screen Points to respect the camera deep
        mousePosition.z = Camera.main.WorldToScreenPoint(objectToDrag.transform.position).z;

        //Then we convert everything in WorldPoint to draw the ray
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Draw StartPosition
        lineRenderer.SetPosition(0, startObjectPosition);
        //Draw EndPosition
        lineRenderer.SetPosition(1, worldMousePosition);
    }

    void EndDrag()
    {
        //If the object to throw is null exit from this method
        if (objectToDrag == null)
        {
            return;
        }

        isDragging = false;
        lineRenderer.enabled = false;

        //We save the mouseposition
        Vector3 mousePosition = Input.mousePosition;

        //We convert the object Drag position to Screen Points to respect the camera deep
        mousePosition.z = Camera.main.WorldToScreenPoint(objectToDrag.transform.position).z;

        //Then we convert everything in WorldPoint to draw the ray
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //We calculate and Normalize to obtein the vector Direction
        Vector3 direction = (worldMousePosition - startObjectPosition).normalized;

        //we calculate the distance, more distance = more force
        float distance = Vector3.Distance(startObjectPosition, worldMousePosition);

        //We add the force to the RigidBody to throw the object
        rigidbody.AddForce(direction * throwForce * distance, ForceMode.Impulse);

        objectToDrag = null;
        rigidbody = null;
    }
}