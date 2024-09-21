using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerScript : MonoBehaviour
{
    //variable used to assign multiple card values
    private int Card;

    //used to Know where to put cards
    [SerializeField]public GameObject CardPosition;

    //copy of where to put cards
    private GameObject CardPositionCopy;

    //Reference to Pòayers
    [SerializeField]private GameObject Player1;
    [SerializeField]private GameObject Player2;
    [SerializeField]private GameObject Player3;

    //number of one in the dealerZone
    private int NumberOfOnes;

    //sum of all the dealer cards
    private int Total;

    //Variable to know when we can reset the deck
    private bool ResetDeck = false;

    //Text_TMP to know whose turn it is
    [SerializeField]private TMP_Text Turn;

    //boolean to know if the TMP_Text can be changed
    bool TurnChanged = false;

    //Gameobject used to move card in cardposition
    private GameObject cardToMove;

    //bool to know if Dealer has to move cards
    private bool isMovingCard = false;

    //bool to know if all player have finisched their turn
    [NonSerialized] public bool AllEnemyReady = false;

    [SerializeField]private DeckScript deckScript;

    // Start is called before the first frame update
    void Start()
    {
        ClearVaribles();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if every IA has WaitingForPlayer true
        if (Player1.gameObject.GetComponent<IABrainScript>().WaitingForPlayer == true && Player2.gameObject.GetComponent<IABrainScript>().WaitingForPlayer == true && Player3.gameObject.GetComponent<IABrainScript>().WaitingForPlayer == true)
        {
            if (!AllEnemyReady)
            {
                ChangeTurn();
            }
            AllEnemyReady = true;
        }

        //Check if a card is assigned and if the bollean value is true
        if (isMovingCard && cardToMove != null)
        {
            //Interpolate the current Card with the Position were it as to be
            cardToMove.transform.position = Vector3.Lerp(cardToMove.transform.position, CardPositionCopy.transform.position, 0.3f);

            //Check if it is almost close to the position
            if (Vector3.Distance(cardToMove.transform.position, CardPositionCopy.transform.position) < 0.5f)
            {
                //we reset the velocity of the card to stop them and start moving it to the correct position
                cardToMove.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //we move the card in the correct position
                cardToMove.transform.position = CardPositionCopy.transform.position;
                //because the player can receve more than 3 cards implemented only one position that is updated every time the dealer receve another card
                CardPositionCopy.transform.position += new Vector3(0.2f, 0.05f, 0);
                //Remove the card assignment
                cardToMove = null;
                isMovingCard = false;
            }
        }
    }

    //Just an example of operator ternary to change the Text on the table
    void ChangeTurn()
    {
        //if Turn.text = "Turn: Players" then Turn: Dealer otherwise Turn: Players
        Turn.text = (Turn.text == "Turn: Players") ? "Turn: Dealer" : "Turn: Players";
    }

    private void OnTriggerEnter(Collider other)
    {
        //this if active the OnTriggerEnter only if all the Ia have finished their turn
        if (AllEnemyReady)
        {
            //we try to convert to int the firsts 2 letter of the name of the card to assign the value
            int.TryParse(other.gameObject.name.Substring(0, 2), out Card);

            StartMovingCard(other.gameObject);

            //because the player can receive up to 4 ace, i implemented a value that track how many 1 the player have so that if the total is greater than 21 an the player has one o more one to convert back in to one the total doesnt exceed 21
            if (Card == 1)
            {
                if (Total + Card <= 21)
                {
                    Card = 11;
                    NumberOfOnes = NumberOfOnes + 1;
                }
            }
            if (Card + Total > 21)
            {
                if (NumberOfOnes > 0)
                {
                    Total = Total - 10;
                    NumberOfOnes--;
                }
            }
            Total = Total + Card;
        }
    }

    //call the action in alls the IAs and pass the Dealer total to know who wins and who lost
    public void SendTotal()
    {
        //only if all the player are ready the button "Stay" can be pressed
        if (AllEnemyReady)
        {
            IABrainScript[] allBrains = FindObjectsOfType<IABrainScript>();

            //for each IA call the Action Win or Lose
            foreach (IABrainScript brain in allBrains)
            {
                brain.WinOrLose.Invoke(Total);
                ResetDeck = true;
            }
            if (ResetDeck)
            {
                //variable used to block the "shuffle" button
                deckScript.UnableDeck = false;

                //create a new deck with the new order of card
                deckScript.SpawnDeck();

                //set AllEnemtReady to falls so the button Stay cant be pressed again
                AllEnemyReady = false;

                ClearVaribles();

            }

        }
    }

    //method to start the lerp in the update
    private void StartMovingCard(GameObject Card)
    {
        cardToMove = Card;
        isMovingCard = true;
    }

    //clear all the variables
    private void ClearVaribles()
    {
        CardPositionCopy = CardPosition;
        ChangeTurn();
        Card = 0;
        NumberOfOnes = 0;
        Total = 0;
        ResetDeck = false;
        isMovingCard = false;
        AllEnemyReady = false;
    }
}
