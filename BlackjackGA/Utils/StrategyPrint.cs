using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BlackjackGA.Representation;
using BlackjackGA.Engine;

namespace BlackjackGA.Utils
{
    class StrategyPrint
    {
        public static void ShowPlayableHands(StrategyBase strategy, string savedImageName, string displayText)
        {
            // Limpiamos el canvas
            Canvas canvas = new Canvas();
            canvas.Children.Clear();

            Color hitColor      = Colors.AliceBlue,
                  standColor    = Colors.MediumTurquoise,
                  doubleColor = Colors.PaleVioletRed,
                  splitColor = Colors.Teal;

            AddInformationalText(displayText, canvas);

            // Esta fuese la matriz de las Soft Hands
            // Fila para las cartas del jugador y columna para la carta del dealer

            AddColorBox(Colors.White, "", 0, 0, canvas);
            int x = 1, y = 0;
            foreach (var upcardRank in Card.ListOfRanks)
            {
                // La estrategia para 10, J, Q, y K son la misma asi que la saltamos.
                if (upcardRank == Card.Ranks.Jack || upcardRank == Card.Ranks.Queen || upcardRank == Card.Ranks.King)
                    continue;

                Card dealerUpcard = new Card(upcardRank, Card.Suits.Diamonds);

                string upcardRankName = Card.RankString(upcardRank);
                AddColorBox(Colors.White, upcardRankName, x, 0, canvas);
                y = 1;

                for (int hardTotal = 20; hardTotal > 4; hardTotal--)
                {
                    // agregar una celda blanca con el total
                    AddColorBox(Colors.White, hardTotal.ToString(), 0, y, canvas);

                    
                    Hand playerHand = new Hand();

                    // dividimos entre dos y sumamos uno cuando sea necesario para realizar la divicion.
                    int firstCardRank = ((hardTotal % 2) != 0) ? (hardTotal + 1) / 2 : hardTotal / 2;
                    int secondCardRank = hardTotal - firstCardRank;

                    if (firstCardRank == secondCardRank)
                    {
                        firstCardRank++;
                        secondCardRank--;

                        if (firstCardRank == 11)
                        {
                            firstCardRank = 4;
                            playerHand.AddCard(new Card(Card.Ranks.Seven, Card.Suits.Clubs));
                        }
                    }

                    playerHand.AddCard(new Card((Card.Ranks)firstCardRank, Card.Suits.Diamonds));
                    playerHand.AddCard(new Card((Card.Ranks)secondCardRank, Card.Suits.Hearts));

                    // conseguir la estrategia
                    var action = strategy.GetActionForHand(playerHand, dealerUpcard);
                    switch (action)
                    {
                        case ActionToTake.Hit:
                            AddColorBox(hitColor, "H", x, y, canvas);
                            break;

                        case ActionToTake.Stand:
                            AddColorBox(standColor, "S", x, y, canvas);
                            break;

                        case ActionToTake.Double:
                            AddColorBox(doubleColor, "D", x, y, canvas);
                            break;
                    }
                    y++;
                }
                x++;
            }

            // Esta fuese otra matriz para manos sin Ace, Hard Hands

            const int leftColumnForAces = 12;
            AddColorBox(Colors.White, "", leftColumnForAces, 0, canvas);
            x = leftColumnForAces + 1;
            foreach (var upcardRank in Card.ListOfRanks)
            {
                // Aqui tambien las cartas de 10 son iguales asi que seguimos.
                if (upcardRank == Card.Ranks.Jack || upcardRank == Card.Ranks.Queen || upcardRank == Card.Ranks.King)
                    continue;

                string upcardRankName = Card.RankString(upcardRank);
                Card dealerUpcard = new Card(upcardRank, Card.Suits.Diamonds);

                AddColorBox(Colors.White, upcardRankName, x, 0, canvas);
                y = 1;

                // Aqui no empezamos con las A's ya que eso fuese AA, lo cual es un par. tampoco empezariamos
                // con 10 ya que eso fuese blackjack, Asi que empezamos con 9.

                for (var otherCard = Card.Ranks.Nine; otherCard >= Card.Ranks.Two; otherCard--)
                {
                  
                    // si es "A-x" se agrega una celda blanca con la mano del jugador
                    string otherCardRank = Card.RankString(otherCard);
                    AddColorBox(Colors.White, "A-" + otherCardRank, leftColumnForAces, y, canvas);

                   
                    Hand playerHand = new Hand();

                    // primera carta aqui es la A y probamos con todas las otras
                    playerHand.AddCard(new Card(Card.Ranks.Ace, Card.Suits.Hearts)); 
                    playerHand.AddCard(new Card(otherCard, Card.Suits.Spades));

                    // mostramos la estrategia
                    var action = strategy.GetActionForHand(playerHand, dealerUpcard);
                    switch (action)
                    {
                        case ActionToTake.Hit:
                            AddColorBox(hitColor, "H", x, y, canvas);
                            break;

                        case ActionToTake.Stand:
                            AddColorBox(standColor, "S", x, y, canvas);
                            break;

                        case ActionToTake.Double:
                            AddColorBox(doubleColor, "D", x, y, canvas);
                            break;
                    }
                    y++;
                }
                x++;
            }

            // La matriz para los pares!

            int startY = y + 1;
            AddColorBox(Colors.White, "", leftColumnForAces, startY, canvas);
            x = leftColumnForAces + 1;
            foreach (var upcardRank in Card.ListOfRanks)
            {
                // lo mismo con los 10.
                if (upcardRank == Card.Ranks.Jack || upcardRank == Card.Ranks.Queen || upcardRank == Card.Ranks.King)
                    continue;

                string upcardRankName = Card.RankString(upcardRank);
                Card dealerUpcard = new Card(upcardRank, Card.Suits.Diamonds);

                AddColorBox(Colors.White, upcardRankName, x, startY, canvas);
                y = startY + 1;

                for (var pairedRank = Card.Ranks.Ace; pairedRank >= Card.Ranks.Two; pairedRank--)
                {
                     
                    if (pairedRank == Card.Ranks.Jack || pairedRank == Card.Ranks.Queen || pairedRank == Card.Ranks.King)
                        continue;

                   
                    string pairedCardRank = Card.RankString(pairedRank);
                    AddColorBox(Colors.White, pairedCardRank + "-" + pairedCardRank, leftColumnForAces, y, canvas);
                    

                    Hand playerHand = new Hand();
                    playerHand.AddCard(new Card(pairedRank, Card.Suits.Hearts)); 
                    playerHand.AddCard(new Card(pairedRank, Card.Suits.Spades)); 

                    
                    var action = strategy.GetActionForHand(playerHand, dealerUpcard);
                    switch (action)
                    {
                        case ActionToTake.Hit:
                            AddColorBox(hitColor, "H", x, y, canvas);
                            break;

                        case ActionToTake.Stand:
                            AddColorBox(standColor, "S", x, y, canvas);
                            break;

                        case ActionToTake.Double:
                            AddColorBox(doubleColor, "D", x, y, canvas);
                            break;

                        case ActionToTake.Split:
                            AddColorBox(splitColor, "P", x, y, canvas);
                            break;
                    }
                    y++;
                }
                x++;
            }

            // ahora que esta dibujado, guardamos la imagen
            if (!string.IsNullOrEmpty(savedImageName))
                SaveCanvasToPng(canvas, savedImageName);
        }

        private static void AddColorBox(Color color, string label, int x, int y, Canvas canvas)
        {
            int columnWidth = (int)canvas.ActualWidth / 25;
            int rowHeight = (columnWidth * 4) / 5;
            int startX = columnWidth;
            int startY = columnWidth;

            // el elemento es un borde
            var box = new Border();
            box.BorderBrush = Brushes.Black;
            box.BorderThickness = new System.Windows.Thickness(1);
            box.Background = new SolidColorBrush(color);
            box.Width = columnWidth;
            box.Height = rowHeight;

         
            var itemText = new TextBlock();
            itemText.HorizontalAlignment = HorizontalAlignment.Center;
            itemText.VerticalAlignment = VerticalAlignment.Center;
            itemText.Text = label;
            box.Child = itemText;

            canvas.Children.Add(box);
            Canvas.SetTop(box, startY + y * rowHeight);
            Canvas.SetLeft(box, startX + x * columnWidth);
        }

        private static void AddInformationalText(string message, Canvas canvas)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var itemText = new TextBlock();
            itemText.HorizontalAlignment = HorizontalAlignment.Left;
            itemText.VerticalAlignment = VerticalAlignment.Center;
            itemText.Inlines.Add(new Bold(new Run(message)));
            itemText.FontSize = 20;

            int spacing = (int)canvas.ActualWidth / 25;
            int bottomY = (int)canvas.ActualHeight;
            canvas.Children.Add(itemText);
            Canvas.SetTop(itemText, bottomY - spacing * 1.5);
            Canvas.SetLeft(itemText, spacing);
        }

        private static void SaveCanvasToPng(Canvas canvas, string savedImageName)
        {
            Size size = canvas.RenderSize;
            Rect rect = new Rect(size);

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)rect.Right,
                (int)rect.Bottom,
                96d, 96d,
                PixelFormats.Default);

            
            canvas.Measure(size);
            canvas.Arrange(rect);
            rtb.Render(canvas);

            // encode to pgb
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            pngEncoder.Save(ms);
            ms.Close();
            System.IO.File.WriteAllBytes(savedImageName + ".png", ms.ToArray());
        }
    }
}
