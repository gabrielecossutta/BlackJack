using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class IABrainScript : MonoBehaviour
{
    //variable for the value of 3 cards
    private int FirstCard;
    private int SecondCard;
    private int ThirdCard;

    //used to Know where to put cards
    [SerializeField]private GameObject Card1Position;
    [SerializeField]private GameObject Card2Position;
    [SerializeField]private GameObject Card3Position;

    //percent of drawing another card
    private float DrawCardPercent;

    //Total of the 3 cards
    [NonSerialized]public int Total;

    //boolean used to let know the player that the IA have finished their turn
    [NonSerialized]public bool WaitingForPlayer;

    //Boolean used to know went the IA want another card
    private bool WantAnotherCard;

    //boolean to know if the IA is Playing
    private bool isPlaying;

    //boolean to know if the additional card as received
    private bool AdditionalCard;

    //animator for the IA
    private Animator animator;

    //Gameobject used to move card in cardposition
    private GameObject cardToMove;

    //Vector used to assign the position of one of the 3 card position
    private Vector3 targetPosition;

    //boolean to know if the IA is moving a card
    private bool isMovingCard = false;

    //action called by the player to know who wins and who lost
    public Action<int> WinOrLose;


    // Start is called before the first frame update
    void Start()
    {
        //Assign the Action
        WinOrLose = Result;

        //Assign the animator from the Gameobject
        animator = gameObject.GetComponentInParent<Animator>();

        //Clear every variables
        ClearVariables();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if a card is assigned and if the bollean value is true
        if (isMovingCard && cardToMove != null)
        {
            //Interpolate the current Card with the Position were it as to be
            cardToMove.transform.position = Vector3.Lerp(cardToMove.transform.position, targetPosition, 0.1f);

            //Check if it is almost close to the position
            if (Vector3.Distance(cardToMove.transform.position, targetPosition) < 0.02f)
            {
                //Remove the card assignment
                cardToMove = null;
                isMovingCard = false;
            }
        }
    }

    //Ricorsive method
    void Decision(int Total)
    {
        //Check if the IA receved the Third Card
        if (AdditionalCard)
        {
            //Set the Animation trigger to show that the IA receved the Card
            animator.SetTrigger("ThirdCardReceved");

            //Check if the AI ​​total is greater than 21, then it has lost
            if (this.Total > 21)
            {
                //Trigger the animation of Bust
                animator.SetBool("Bust", true);
                WaitingPlayer();
            }
            else
            {
                WaitingPlayer();
            }
        }
        else if (this.Total == 21) //If the Total is 21 the IA stay 
        {
            //Trigger animation of Stay
            animator.SetTrigger("Stay");
            WaitingPlayer();
        }
        else if (this.Total > 15) //If greater than 15 the player has a change of drawing another card, greater the number less the changes
        {
            //We subtract the total from 21 and divide by 10, we obtein a float
            DrawCardPercent = (21 - this.Total) / 10;

            //Using Random.value we obtain a float between 0 and 1, so we can have 60 to 10 % of drawing a card
            if (UnityEngine.Random.value >= DrawCardPercent)
            {
                //If below the random value we trigger the Stay animation
                animator.SetTrigger("Stay");
                WaitingPlayer();
            }
            else
            {
                //if greater than the random value we trigger the "Give me another Card" animation
                animator.SetTrigger("AnotherCard");
                //bool used to know if the IA want another card
                WantAnotherCard = true;
            }
        }
        else
        {
            //if greater than the random value we trigger the "Give me another Card" animation
            animator.SetTrigger("AnotherCard");
            //bool used to know if the IA want another card
            WantAnotherCard = true;
        }
    }

    private void WaitingPlayer()
    {
        WaitingForPlayer = true;
    }

    //OnTriggerEnter used to check cards
    private void OnTriggerEnter(Collider other)
    {
        if (isPlaying)
        {
            if (FirstCard == 0)
            {
                //we try to convert to int the firsts 2 letter of the name of the card to assign the value
                int.TryParse(other.gameObject.name.Substring(0, 2), out FirstCard);

                //we reset the velocity of the card to stop them and start moving it to the correct position
                other.attachedRigidbody.velocity = Vector3.zero;

                StartMovingCard(other.gameObject, Card1Position.gameObject.transform.position);

                //If the first card is a 1 it is converted to a 11
                if (FirstCard == 1)
                {
                    FirstCard = 11;
                }
            }
            else if (SecondCard == 0)
            {
                //we try to convert to int the firsts 2 letter of the name of the card to assign the value
                int.TryParse(other.gameObject.name.Substring(0, 2), out SecondCard);

                //we reset the velocity of the card to stop them and start moving it to the correct position
                other.attachedRigidbody.velocity = Vector3.zero;

                StartMovingCard(other.gameObject, Card2Position.gameObject.transform.position);

                //if the second card is a one and the first is not, it converted to 11
                if (SecondCard == 1 && FirstCard != 11)
                {
                    SecondCard = 11;
                }

                Total = FirstCard + SecondCard;

                Decision(Total);
            }
            else if (WantAnotherCard && ThirdCard == 0) //if WantAnotherCard is true the trigger can accept another card
            {
                //we try to convert to int the firsts 2 letter of the name of the card to assign the value
                int.TryParse(other.gameObject.name.Substring(0, 2), out ThirdCard);

                //we reset the velocity of the card to stop them and start moving it to the correct position
                other.attachedRigidbody.velocity = Vector3.zero;

                StartMovingCard(other.gameObject, Card3Position.gameObject.transform.position);

                Total = Total + ThirdCard;

                //if the third card is 1 and the total il below 21 it converted to 11
                if (ThirdCard == 1 && Total <= 21)
                {
                    ThirdCard = 11;
                }

                //but if the total is greater than 21 we check if there is another one to convert back to one and not eleven
                if (Total > 21)
                {
                    if (FirstCard == 11)
                    {
                        FirstCard = 1;
                    }
                    else if (SecondCard == 11)
                    {
                        SecondCard = 1;
                    }
                    Total = FirstCard + SecondCard + ThirdCard;

                }

                AdditionalCard = true;

                Decision(Total);
            }
            else
            {
                animator.SetTrigger("GetDamage");
            }
        }
    }

    //method to start the lerp in the update
    private void StartMovingCard(GameObject Card, Vector3 Position)
    {
        cardToMove = Card;
        targetPosition = Position;
        isMovingCard = true;
    }

    //clear all the variables assigned
    private void ClearVariables()
    {
        AdditionalCard = false;
        FirstCard = 0;
        SecondCard = 0;
        ThirdCard = 0;
        Total = 0;
        isPlaying = true;
        WantAnotherCard = false;
        animator.SetBool("Win", false);
        animator.SetBool("Bust", false);
    }

    //Action invoked from the Dealer to all the IA, this method trigger various Action
    private void Result(int Total)
    {
        if (Total > 21)
        {
            animator.SetBool("Win", true);
        }
        else if (Total == 21) //if the Dealer as 21 everyone losee
        {
            animator.SetBool("Bust", true);
        }
        else if (this.Total > Total)
        {
            animator.SetBool("Win", true);
        }
        else if (this.Total <= Total)
        {
            animator.SetBool("Bust", true);
        }

        WaitingForPlayer = false;

        //start a coroutine to allow the animation to finish
        StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(2f);
        ClearVariables();
    }
}
