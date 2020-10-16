﻿using System;

namespace BlackjackGA.Utils
{
    public class TestConditions
    {
            public int NumDecks { get; set; } = 4;

            public int NumHandsToPlay { get; set; } = 10000;

            public int BetSize { get; set; } = 2;

            public int BlackjackPayoffSize { get; set; } = 3;   // Pago en caso de tener blackjack, la mayoría de los casinos pagan 3:2

            public int NumFinalTests { get; set; } = 100;

            public bool StackTheDeck { get; set; } = false;
    }  
}