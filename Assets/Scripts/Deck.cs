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
    private int[] givenValues = new int[52];
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
            PushDealer();
            PushPlayer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            
        }
        CheckVictory();

    }

    private void CalculateProbabilities()
    {
        float dealerConMasChance = 0f;
        float pasarDe21 = 0f;
        float entre17y21 = 0f;
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        int visiblePoints = dealerHand.points - dealerHand.cards[0].GetComponent<CardModel>().value;
        int diferenciaPuntos = visiblePoints - playerHand.points;
        if (diferenciaPuntos >= 0)
        {
            dealerConMasChance = 100f;
        }
        else
        {
            int necesario = playerHand.points - visiblePoints + 1;
            int valido = necesario; 
            while(valido <= 11)
            {
                dealerConMasChance += probabilidadDeValor(valido);
                valido++;
            }
            
        }

        //Probabilidad de más de 21
        int diferenciaA21 = 22 - playerHand.points;
        if(diferenciaA21 > 11)
        {
            pasarDe21 = 0;
        } else
        {
            while (diferenciaA21 <= 11)
            {
                pasarDe21 += probabilidadDeValor(diferenciaA21);
                diferenciaA21++;
            }
        }


        //Probabilidad de entre 17 y 21
        int diferenciaA17 = 17 - playerHand.points;
        int diferenciaHasta21 = 21 - playerHand.points;
        if(diferenciaA17 > 0)
        {
            if (diferenciaA17 > 11)
            {
                entre17y21 = 0f;
            }
            else
            {
                while (diferenciaA17 <= 11)
                {
                    entre17y21 += probabilidadDeValor(diferenciaA17);
                    diferenciaA17++;
                }

                entre17y21 -= pasarDe21;
            }
        }
        

        actualizarProbText(dealerConMasChance, pasarDe21, entre17y21);

    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealerHand.Push(faces[cardIndex], values[cardIndex]);
        Debug.Log("Dealer: " + values[cardIndex]);
        givenValues[cardIndex] = values[cardIndex];
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        playerHand.Push(faces[cardIndex], values[cardIndex]);
        Debug.Log("Player: " + values[cardIndex]);
        givenValues[cardIndex] = values[cardIndex];
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
         while(dealerHand.points < 17)
        {
            PushDealer();
        }


        if(playerHand.points > dealerHand.points && playerHand.points < 22 || dealerHand.points > 21)
        {
            finalMessage.text = "Enhorabuena";
            return;
        } 

        if(dealerHand.points > playerHand.points && dealerHand.points < 22)
        {
            finalMessage.text = "Lástima";
            return;
        }

        finalMessage.text = "Empate";

         
         
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
            finalMessage.text = "Empate";
            dealerHand.InitialToggle();
            return;
        }
        if (playerHand.points == 21 || dealerHand.points > 21)
        {
            finalMessage.text = "Enhorabuena";
            dealerHand.InitialToggle();
            return;
        }
        if (dealerHand.points == 21 || playerHand.points > 21)
        {
            finalMessage.text = "Lástima";
            dealerHand.InitialToggle();
            return;
        }

    }

    private void actualizarProbText(float uno, float dos, float tres)
    {
        probMessage.text = "Probabilidad que  el dealer tenga más: " + uno + Environment.NewLine +
            "Probabilidad de pasar de 21: " + dos + Environment.NewLine + 
            "Probabilidad de estar entre 17 y 21: "  + tres;
    }

    private float probabilidadDeValor(int value)
    {
        int posibles = 0;
        if(value == 10)
        {
            posibles = 12;
        } else
        {
            posibles = 4;
        }
        int visibleCardsThatAreTheSame = 0;
        if(value == 11 || value == 1)
        {
            for (int i = 1; i < cardIndex; i++)
            {
                if(givenValues[i] == 11 || givenValues[i] == 1)
                {
                    if (i != 1) visibleCardsThatAreTheSame++;
                }
            }
        }

        int favorable = posibles - visibleCardsThatAreTheSame;
        int potencial = 52 - (cardIndex - 1);
        return (float) ((float)favorable / (float)potencial) * 100;
    }
    
}
