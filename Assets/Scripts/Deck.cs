using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    private CardHand playerHand;
    private CardHand dealerHand;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {
        InitCardValues();        

    }

    private void Start()
    {
        playerHand = player.GetComponent<CardHand>();
        dealerHand = dealer.GetComponent<CardHand>();
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        int i = 0;
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int paso = 1;
        for(i = 0; i < 52; i++)
        {
            if (i % 13 == 0) {
                values[i] = 11;
            } else if (i == 10 ||i == 11 || i == 12||i == 23 || i == 24 || i == 25 || i == 36 || i== 37 
                || i == 38 ||i == 49 || i == 50 ||i == 51)
            {
                values[i] = 10;
            }
            else
            {
                values[i] = paso;
            }

            paso++;
            if (paso > 14) paso = 2;
        }
        
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
       
        int index;
        List<int> usedIndexes = new List<int>();
        Sprite[] newSpritesOrder = new Sprite[52];
        int[] newDeck = new int[52];
        for (int i = 0; i < 52; i++)
        {
            index = UnityEngine.Random.Range(0, 52);
            while (usedIndexes.Contains(index))
            {
                index = UnityEngine.Random.Range(0, 52);
                Debug.Log("Cambio el valor del index");
            }
            usedIndexes.Add(index);
            newDeck[i] = values[index];
            newSpritesOrder[i] = faces[index];
        }
        values = newDeck;
        faces = newSpritesOrder;
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            
        }
        CheckVictory();

    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealerHand.Push(faces[cardIndex], values[cardIndex]);
        Debug.Log("Dealer: " + values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        playerHand.Push(faces[cardIndex], values[cardIndex]);
        Debug.Log("Player: " + values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         * 
         */
        CheckVictory();

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        dealerHand.InitialToggle();

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
         if(dealerHand.points <= 16) dealerHand.Push(faces[cardIndex], values[cardIndex]);

        cardIndex++;
         
         
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

    void CheckVictory()
    {
        if (playerHand.points == 21 && dealerHand.points == 21)
        {
            finalMessage.text = "Aquí ganamos todos";
            return;
        }
        if (playerHand.points == 21 || dealerHand.points > 21)
        {
            finalMessage.text = "Ganaste champion";
            return;
        }
        if (dealerHand.points == 21 || playerHand.points > 21)
        {
            finalMessage.text = "Pulsen F en sus teclados";
            return;
        }

    }
    
}
