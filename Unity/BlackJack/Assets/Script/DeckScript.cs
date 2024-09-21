using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckScript : MonoBehaviour
{
    //array containing all the card in order
    [SerializeField]private GameObject[] Deck;

    //index of the Card
    private int Index;

    //position of the spawned card
    private float x, y, z;
    private float SpawnPoint;

    //Distance to apply from each card spawned
    [SerializeField]private float distance;

    //bool used to deactivate the shuffle button
    [NonSerialized]public bool UnableDeck = false;

    //copy of the previous card name put under the deck
    private String previousName;

    void Start()
    {
        //assign x y z
        x = gameObject.transform.position.x;
        y = gameObject.transform.position.y;
        z = gameObject.transform.position.z;

        SpawnDeck(Deck);
    }

    //SpawnDeck callable from outside the script
    public void SpawnDeck()
    {
        SpawnDeck(Deck);
    }

    private void SpawnDeck(GameObject[] DeckToSpawn)
    {
        //Find all the gameobject with teh tag "Card"
        GameObject[] DeleteDeck = GameObject.FindGameObjectsWithTag("Card");

        //Destroy the previous deck
        foreach (GameObject Card in DeleteDeck)
        {
            Destroy(Card);
        }

        SpawnPoint = y;
        foreach (GameObject Card in DeckToSpawn)
        {
            //increase the Spawn Point do the card spawned doesn't collide with other cards
            SpawnPoint += distance;
            
            //create a new Card
            GameObject newCard = Instantiate(Card);

            //Spawn the card with correct position and rotation, transform unity use quaternion so we convert a Vector3 in to a Quaternion
            newCard.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            newCard.transform.position = new Vector3(x, SpawnPoint, z);
        }
        UnableDeck = false;
    }


    //Generate a deck with all the Card Shuffled
    public void ShuffleDeck()
    {
        //if the bool id false the deck cant be shuffled because a round is started
        if (!UnableDeck)
        {
            //create a new Array with 52 slots
            GameObject[] shuffledDeck = new GameObject[52];

            //we copy the Deck in the new Deck
            Deck.CopyTo(shuffledDeck, 0);

            //we iterate the Array, to each card we assign a random position and we swap the cards, then we swap another card 
            for (int i = 0; i < shuffledDeck.Length; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, shuffledDeck.Length);
                GameObject temp = shuffledDeck[i];
                shuffledDeck[i] = shuffledDeck[randomIndex];
                shuffledDeck[randomIndex] = temp;
            }

            //we copy the shuffled deck in the Deck
            shuffledDeck.CopyTo(Deck, 0);
            SpawnDeck(Deck);
        }
    }

    //method used to put the card that we throw under the deck
    public void PutUnderDeck(String Card)
    {
        //check if we click on the same card twice in a row
        if (previousName == Card)
        {
            return;
        }
        //if the throw a card we cant no longer shuffle the deck
        UnableDeck = true;

        //we search the card in the Deck and find the index to swap after
        for (int i = 0; i < Deck.Length; i++)
        {
            if (Deck[i].name == Card)
            {
                Index = i;
            }
        }

        //create a new Array with 52 slots
        GameObject[] PutUnderDeck = new GameObject[52];

        //we copy the Deck in the new Deck
        Deck.CopyTo(PutUnderDeck, 0);

        //we move by one every card in the deck
        for (int i = PutUnderDeck.Length - 2; i >= 0; i--)
        {
            PutUnderDeck[i + 1] = PutUnderDeck[i];
        }
        //move the Clicked card on the bottom
        PutUnderDeck[0] = Deck[Index];

        //we copy the shuffled deck in the Deck
        PutUnderDeck.CopyTo(Deck, 0);
        previousName = Card;

    }
}
