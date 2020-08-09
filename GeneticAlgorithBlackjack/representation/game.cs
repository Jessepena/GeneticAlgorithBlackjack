using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticAlgorithBlackjack.Utils;
using GeneticAlgorithBlackjack.Engine;

namespace GeneticAlgorithBlackjack.Representation
{
    public enum GameState
    {
        PlayerBlackjack,
        PlayerDrawing,
        PlayerBusted,
        DealerDrawing,
        DealerBusted,
        HandComparison,
        RestartPlayerHand
    }

    class Game
    {
        private StrategyBase strategy;
        private TestConditions testConditions;

        public int GenerateFitness(int handsToPlay)
        {
            return 22;
            int playerChips = 0;
            var deck = new Deck(testConditions.NumDecks);
            var randomizer = new Randomizer();

            Hand dealerHand = new Hand();
            Hand playerHand = new Hand();
            List<Hand> playerHands = new List<Hand>();
            List<int> betAmountPerHand = new List<int>();

            for (int hands = 0; hands < handsToPlay; hands++)
            {
                // Primero limpiamos todos los datos para jugar una mano nueva.
                dealerHand.Cards.Clear();
                playerHand.Cards.Clear();
                betAmountPerHand.Clear();

                // Agregamos cartas a las manos del dealer como a la del jugador
                playerHands.Add(playerHand);
                playerHands.Add(playerHand);
                dealerHand.AddCard(deck.DealCard());
                dealerHand.AddCard(deck.DealCard());

                // Agregando las Apuestas
                betAmountPerHand.Add(testConditions.BetSize);
                playerChips -= testConditions.BetSize;

                ////////////////      Decisiones de Juego  ///////////////
                // 1) Blackjack
                if (playerHand.HandValue() == 21)
                {
                    // si el dealer tiene 21 tambien es un empate
                    if (dealerHand.HandValue() == 21)
                    {
                        playerChips += betAmountPerHand[0];
                    }
                    else
                    {
                        playerChips += testConditions.BlackjackPayoffSize;
                    }
                    betAmountPerHand[0] = 0;
                    continue;
                }
                // 2) BlackJack del dealer, se continua solamente porque ya la apuesta se disminuyo.
                if (dealerHand.HandValue() == 21) continue;

                // 3) Si un jugador tiene una mano jugable, tomar la decision de una estrategia y jugar hasta que haga Standing o se pase.
                for (var handIndex = 0; handIndex < playerHands.Count; handIndex++)
                {
                    playerHand = playerHands[handIndex];

                    var gameState = GameState.PlayerDrawing;
                    while (gameState == GameState.PlayerDrawing)
                    {
                        // Si el jugador hizo Split y resulta en BlackJack, se paga y se sigue.
                        if (playerHand.HandValue() == 21)
                        {
                            if (playerHand.Cards.Count == 2)    // Blackjack
                            {
                                int blackjackPay = testConditions.BlackjackPayoffSize * betAmountPerHand[handIndex] / testConditions.BetSize;
                                playerChips += blackjackPay;
                                betAmountPerHand[handIndex] = 0;
                            }
                            gameState = GameState.DealerDrawing;
                            break;
                        }

                        // Se busca en la estrategia cual fuese el movimiento a hacer
                        var action = strategy.GetActionForHand(playerHand, dealerHand.Cards[0]);

                        // Si se puede hacer un Double-Down con mas de dos cartas, hacemos un Hit
                        if (action == ActionToTake.Double && playerHand.Cards.Count > 2)
                            action = ActionToTake.Hit;

                        switch (action)
                        {
                            /////// HIT ///////
                            case ActionToTake.Hit:
                                playerHand.AddCard(deck.DealCard());

                                // Si sumamos 21, hacemos Stand
                                if (playerHand.HandValue() == 21)
                                    gameState = GameState.DealerDrawing;

                                // Si nos pasamos
                                if (playerHand.HandValue() > 21)
                                {
                                    betAmountPerHand[handIndex] = 0;
                                    gameState = GameState.PlayerBusted;
                                }
                                break;

                            /////// STAND ///////
                            case ActionToTake.Stand:
                                gameState = GameState.DealerDrawing;
                                break;

                            /////// DOUBLE-DOWN ///////
                            case ActionToTake.Double:
                                // Como las reglas estipulan, Double-Down se apuesta otro Chip y se hace el ultimo Hit.
                                playerChips -= testConditions.BetSize;
                                betAmountPerHand[handIndex] += testConditions.BetSize;

                                playerHand.AddCard(deck.DealCard());

                                // Si el jugador se Pasa de 21.
                                if (playerHand.HandValue() > 21)
                                {
                                    betAmountPerHand[handIndex] = 0;
                                    gameState = GameState.PlayerBusted;
                                }
                                else
                                    gameState = GameState.DealerDrawing;
                                break;

                            /////// SPLIT ///////
                            case ActionToTake.Split:
                                // Se añade otra mano a la lista.
                                var newHand = new Hand();
                                newHand.AddCard(playerHand.Cards[1]);
                                playerHand.Cards[1] = deck.DealCard();
                                newHand.AddCard(deck.DealCard());
                                playerHands.Add(newHand);

                                // Se apuesta por esa mano nueva.
                                playerChips -= testConditions.BetSize;
                                betAmountPerHand.Add(testConditions.BetSize);

                                break;
                        }

                        // 4.  Si el jugador tiene manos disponibles, buscar la jugada del dealer
                        bool playerHandsAvailable = betAmountPerHand.Sum() > 0;

                        if (playerHandsAvailable)
                        {
                            var gameState = GameState.DealerDrawing;

                            // El dealer debe hacer Hit hasta tener 17.
                            while (dealerHand.HandValue() < 17)
                            {
                                dealerHand.AddCard(deck.DealCard());
                                
                                //Si el dealer se pasa,
                                if (dealerHand.HandValue() > 21)
                                {
                                    // Se debe pagar cada mano que siga valida, Si es un Bust o un Blackjack se consideran como 0 para el bet.
                                    for (int handIndex = 0; handIndex < playerHands.Count; handIndex++)
                                        playerChips += betAmountPerHand[handIndex] * 2;  // la apuesta original y su respectiva cantidad
                                    gameState = GameState.DealerBusted;
                                    break;
                                }
                            }

                        // 5. and then compare the dealer hand to each player hand
                        if (gameState != GameState.DealerBusted)
                        {
                            int dealerHandValue = dealerHand.HandValue();
                            for (int handIndex = 0; handIndex < playerHands.Count; handIndex++)
                            {
                                var playerHandValue = playerHands[handIndex].HandValue();

                                // if it's a tie, give the player his bet back
                                if (playerHandValue == dealerHandValue)
                                {
                                    playerChips += betAmountPerHand[handIndex];
                                }
                                else
                                {
                                        if (playerHandValue > dealerHandValue)
                                        {
                                            // player won
                                            playerChips += betAmountPerHand[handIndex] * 2;  // the original bet and a matching amount
                                        }
                                        else
                                        {
                                            // player lost, nothing to do since the chips have already been decremented
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return playerChips;
                }
            }
        }
    }
}