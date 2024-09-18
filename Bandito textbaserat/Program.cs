using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bandito_textbaserat
{

    class Program
    {
        static public Random r = new Random();
        public enum State { AskWhatToDo, WhichCard, WhatDoWithCard, WhichDirectionRotate, WherePlaceCard };
        public static State playerState;
        static Stack<PlayCard> cardPile;
        static Queue<Player> playerQueue;
        static Player activePlayer;
        static List<string> tunnelOpenings = new List<string>();
        static string[,] gameField = new string[129, 139];
        static int GameRound = 1;
        static int selectedCard;
        static void Main(string[] args)
        {

            //int windowWidth = Console.LargestWindowWidth;
            //int windowHeight = Console.LargestWindowHeight;

            //// Ställ in fönstrets storlek
            //Console.SetWindowSize(windowWidth, windowHeight);

            // Meny


            while (true)
            {
                Console.WriteLine("Välommen till Bandito\nVad vill du göra?\n\n");
                string menuText = "[S] tarta nyt spelet:\n\n[I] nstructions\n\n[A] vsluta"; //Meny text
                Console.WriteLine(menuText);
                string menuInput = Console.ReadLine();




                if (menuInput.ToUpper() == "A")
                {
                    Console.WriteLine("Avslutar");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                if (menuInput.ToUpper() == "I")
                {
                    Console.Clear();
                    Console.WriteLine("I är inte tilgänligt\n\n");
                    break;

                }

                if (menuInput.ToUpper() == "S")
                {
                    Console.Clear();
                    Console.WriteLine("Startar spel\n");
                    break;


                }



                Console.Clear();
                WriteInvalid("Invalid input");

            }


            
            int playerCount = 0;
            while (true)
            {
                Console.WriteLine("Hur många spelare?");
                string playerCountInput = Console.ReadLine();

                if (IsValidInput(playerCountInput, false, "1-4"))
                {
                    playerCount = int.Parse(playerCountInput);
                    
                    Console.Clear();
                    Console.WriteLine("Sure, vi kör med " + playerCount + " spelare");
                    break ;
                }
                else if (playerCountInput == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Ha ha vary funny");
                    Console.ReadKey();
                    Console.WriteLine("You think yore so smart and funny don't ya");
                    Console.ReadKey();
                    Console.WriteLine("You know what, screw you");
                    playerCount = int.Parse(playerCountInput);
                    Console.WriteLine("Sure, vi KÖÖÖR med " + playerCount + " spelare");
                    Console.ReadKey();
                    Console.WriteLine("byyyyyye");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                else
                {
                    WriteInvalid("Invalid input");
                    continue;
                }
                

            }


            // Create game field


            


            //SkrivTest(gameField);






            //Create card pile

            cardPile = CreateCardPile();
            
            //SkrivTestCardPile(cardPile);







            //Create players

            playerQueue = new Queue<Player>();
            List<Player> tmpQue = new List<Player>();

            for (int i = 1; i < (playerCount + 1); i++)
            {
                while (true)
                {
                    Console.WriteLine("Player " + i + " Name:");
                    string input = Console.ReadLine();
                    if (IsValidInput(input, true))
                    {
                        
                        tmpQue.Add(new Player(input));
                        break;
                    }
                    WriteInvalid("Invalid input");
                }


                
            }

            tmpQue = tmpQue.OrderBy(x => r.Next()).ToList();
            playerQueue = new Queue<Player>(tmpQue);

            Console.Clear();


            //Place Super card
            string placeSupercard = "2222" + (gameField.GetLength(0) / 2) + (gameField.GetLength(1) / 2);
            PlaceCard(placeSupercard);
            
   

            bool gameActive = true;
            Console.WriteLine("\tSplet startar");
            playerState = State.AskWhatToDo;
            selectedCard = 0;
            
            while (gameActive)
            {

                //Console.Clear();
                Console.WriteLine("Game Round: " + GameRound + "\n");
                activePlayer = playerQueue.Peek();
                

                while (activePlayer.PlayerCards.Count < 3)
                {
                    activePlayer.PlayerCards.Add(cardPile.Pop());
                    Console.WriteLine(activePlayer.Name + " drog ett kort");
                    //activePlayer.PlayerCards.Add(new PlayCard("2111"));

                }

                
                DrawRow();
                Console.WriteLine("\tSpelplan:");
                TestDrawGameFeild(gameField);
                Console.WriteLine(" (Active tunnels: " + tunnelOpenings.Count + ")");
                DrawRow();

                

                if (tunnelOpenings.Count <= 0)
                {
                    break;
                }

                switch (playerState)
                {
                    case State.AskWhatToDo:
                        {
                            Console.WriteLine("\tDin hand:");
                            SkrivTestPlayerhand(activePlayer);
                            DrawRow();
                            AskWhatToDo();
                            break;
                            
                        }
                    case State.WhichCard:
                        {
                            Console.WriteLine("\tDin hand:");
                            SkrivTestPlayerhand(activePlayer);
                            DrawRow();
                            WhichCard();
                            
                            break;
                        }
                    case State.WhatDoWithCard:
                        {
                            Console.WriteLine("\tSecelcted card:");
                            SkrivTestPlayerhand(activePlayer, true);
                            DrawRow();
                            WhatDoWithCard();
                            break;
                        }
                    case State.WhichDirectionRotate:
                        {
                            Console.WriteLine("\tSecelcted card:");
                            SkrivTestPlayerhand(activePlayer, true);
                            DrawRow();
                            WhichDirectionRotate(selectedCard);
                            break;
                        }
                    case State.WherePlaceCard:
                        {
                            Console.WriteLine("\tSecelcted card:");
                            SkrivTestPlayerhand(activePlayer, true);
                            DrawRow();

                            if (WherePlaceCard(selectedCard))
                            {
                                Console.Clear();
                                break;
                            }
                            else
                            {
                                WriteInvalid("Invalid input or rule breake");
                                break;
                            }
                            
                        }

                }

                
                
                
                








            }

            DrawRow();
            Console.WriteLine("Du har vunnit !!!!!!!!!!!!!!!!!!!!!");
            Console.ReadKey();
            Console.WriteLine("Tack för att du spelade");
            Console.ReadKey();
        }

        static void WriteInvalid(string resson)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(resson + "\n");
            Console.ResetColor();
        }
        static void AskWhatToDo()
        {
            string menuString = "[D]raw cards\n[S]elect card to play";
            Console.WriteLine(menuString); 
            string input = Console.ReadLine();
            if (input.ToUpper() == "D")
            {
                Console.Clear();
                
                activePlayer.PlayerCards.Clear();
                Console.Clear();
                NextPlayer();
                Console.WriteLine("Nästa spelare");
                return;
            }
            if (input.ToUpper() == "S")
            {
                Console.Clear();
                playerState = State.WhichCard;
                return;
            }

            else WriteInvalid("Invalid input");
        }

        static void NextPlayer()
        {
            playerQueue.Enqueue(playerQueue.Dequeue());
            playerState = State.AskWhatToDo;
            GameRound++;
        }

        static void WhichCard()
        {
            Console.WriteLine("Which card?\nOr [B]ack");
            string input = Console.ReadLine();
            if (IsValidInput(input, true) && input.ToUpper() == "B")
            {
                Console.Clear();
                playerState = State.AskWhatToDo;
                return;
            }
            if (!IsValidInput(input, false, "1-3"))
            {
                WriteInvalid("Invalid input");
                return;
            }

            Console.Clear();
            playerState = State.WhatDoWithCard;
            selectedCard = int.Parse(input) - 1;

            
        }

        static void WhatDoWithCard()
        {
            Console.WriteLine("[R]otate\n[P]lace\n[B]ack");
            string input = Console.ReadLine();

            if (input.ToUpper() == "R")
            {
                playerState = State.WhichDirectionRotate;
                return;
            }
            if (input.ToUpper() == "P")
            {
                playerState = State.WherePlaceCard;
                return;
            }
            if (input.ToUpper() == "B")
            {
                playerState = State.WhichCard;
                return;
            }

            WriteInvalid("Invalid input");

        }

        static void WhichDirectionRotate(int selectedCard)
        {
            Console.WriteLine("Rotate "+ activePlayer.PlayerCards[selectedCard].TunnelId + " which way?\n[L]eft\n[R]ight\n[P]lace\nOr [B]ack");
            string input = Console.ReadLine();

            

            if (input.ToUpper() == "L")
            {
                activePlayer.PlayerCards[selectedCard].TunnelId = activePlayer.PlayerCards[selectedCard].TunnelId.Substring(3, 1) + activePlayer.PlayerCards[selectedCard].TunnelId.Substring(0, 3);
                activePlayer.PlayerCards[selectedCard].TunnelId.Remove(3);
                return;
            }
            if (input.ToUpper() == "R")
            {
                activePlayer.PlayerCards[selectedCard].TunnelId += activePlayer.PlayerCards[selectedCard].TunnelId.Substring(0, 1);
                activePlayer.PlayerCards[selectedCard].TunnelId = activePlayer.PlayerCards[selectedCard].TunnelId.Remove(0, 1);
                return;
            }
            if (input.ToUpper() == "P")
            {
                playerState = State.WherePlaceCard;
                return;
            }
            if (input.ToUpper() == "B")
            {
                playerState = State.WhatDoWithCard;
                return;
            }

            WriteInvalid("Invalid input");
        }

        static bool WherePlaceCard(int selectedCard)
        {
            Console.WriteLine("1 - " + tunnelOpenings.Count() +"\nOr [B]ack");
            string input = Console.ReadLine();
            if (input.ToUpper() == "B")
            {
                playerState = State.WhatDoWithCard;
                return true;
            }
            if (!IsValidInput(input, false, "1-" + tunnelOpenings.Count()))
            {
                return false;
            }
            else
            {
                int index = int.Parse(input) - 1;
                string xCordinate = (tunnelOpenings[index].Substring(0, 1));
                string yCordinate = (tunnelOpenings[index].Substring(1, 1));
                PlayCard selectedPlaycard = activePlayer.PlayerCards[selectedCard];
                if (PlaceCard(selectedPlaycard.TunnelId + xCordinate + yCordinate))
                {
                    activePlayer.PlayerCards.Remove(selectedPlaycard);

                    NextPlayer();
                    return true;
                }
                
                else return false;
                
            }

            

        }

        static void DrawRow()
        {
            int consoleWidth = Console.WindowWidth; // Hämtar konsolens bredd
            Console.WriteLine(new string('-', consoleWidth)); // Skriver en linje över hela bredden

        }
        static void SkrivTestPlayerhand(Player player, bool onlySelectedCard = false)
        {
            if ( !onlySelectedCard)
            {
                Console.WriteLine(player.Name + ", Dina kort är; ");
            }
            
            string[] consoleLines = new string[3];
            
            for (int i = 0; i < activePlayer.PlayerCards.Count; i++)
            {
                if (onlySelectedCard && i != selectedCard) continue;

                string[] tmpLines = GetCellData(activePlayer.PlayerCards[i].TunnelId, true);
                
                consoleLines[0] += tmpLines[0];
                consoleLines[1] += tmpLines[1];
                consoleLines[2] += tmpLines[2];
            }
            
            DrawCells(consoleLines);


            
        }

        static void TestDrawGameFeild(string[,] gameField)
        {
            

            int nummerTunnlar = 1;
            tunnelOpenings.Clear();
            
            int numRows = gameField.GetLength(0);
            int numCols = gameField.GetLength(1);

            // En array för att lagra alla rader av konsolutmatningen
            string[] consoleLines = new string[3];


            // Skapa en lista som håller reda på om en kolumn är helt null
            bool[] isColumnNull = new bool[numCols];

            for (int i = 0; i < numCols; i++)
            {
                bool columnIsNull = true;
                for (int j = 0; j < numRows; j++)
                {
                    if (gameField[j, i] != null)
                    {
                        columnIsNull = false;
                        break;
                    }
                }
                isColumnNull[i] = columnIsNull;
            }


            for (int lineCheck = 0; lineCheck < 3; lineCheck++)
            {
                // Rensa varje rad
                consoleLines[lineCheck] = "";
            }

            // Bygg hela raden för varje rad i gameField
            for (int j = 0; j < numRows; j++)
            {
                // För varje cell i raden, bygg hela 3x3 rutan
                for (int i = 0; i < numCols; i++)
                {
                    if (isColumnNull[i])
                    {
                        continue; // Hoppa över hela kolumner som är null
                    }


                    string top;
                    string middle;
                    string bottom;
                    string cell = gameField[j, i];
                    //if (cell == null)
                    //{
                    //    continue;
                    //}

                    if (cell == null)
                    {
                        //if (i < numCols - 2 && gameField[j, i + 1] == "O" && gameField[j, i + 2] == null)
                        {
                            top = "   ";
                            middle = "   ";
                            bottom = "   ";
                        }
                        

                    }

                    else if (cell == "O")
                    {
                        // Bygg en kvadrat med '1' i mitten
                        top = "|" + "-" + "|";
                        middle = "|" + nummerTunnlar + "|";
                        bottom = "|" + "-" + "|";
                        tunnelOpenings.Add(j + "" + i);
                        //Console.WriteLine(tunnelOpenings[nummerTunnlar - 1]);
                        nummerTunnlar++;
                        
                        
                        
                    }
                    else
                    {
                        string[] tmpLines = GetCellData(cell);
                        
                        top = tmpLines[0];
                        middle = tmpLines[1];
                        bottom = tmpLines[2];

                    }

                    

                    // Lägg till cellens rad till rätt plats i consoleLines
                    consoleLines[0] += top;
                    consoleLines[1] += middle;
                    consoleLines[2] += bottom;
                }

                // Lägg till en ny rad efter att ha bearbetat hela raden
                DrawCells(consoleLines);
            }


            

        }

        static string[] GetCellData(string cellId, bool devide = false)
        {
            string tmp = "#";
            string[] consoleLines = new string[3];
            // Bygg den horisontella raden för cellen
            consoleLines[0] = tmp + (cellId.Substring(0, 1) == "2" ? " " : tmp) + tmp;
            consoleLines[1] = (cellId.Substring(3, 1) == "2" ? " " : tmp) + " " + (cellId.Substring(1, 1) == "2" ? " " : tmp);
            consoleLines[2] = tmp + (cellId.Substring(2, 1) == "2" ? " " : tmp) + tmp;
            if (devide == true)
            {
                for (int l = 0; l < consoleLines.Length; l++)
                {
                    consoleLines[l] += " | ";
                }
            }
            return consoleLines;

        }
        static void DrawCells(string[] consoleLines)
        {
            for (int k = 0; k < 3; k++)
            {
                if (!string.IsNullOrWhiteSpace(consoleLines[k]))
                {
                    Console.WriteLine(consoleLines[k]);
                }
                consoleLines[k] = "";
            }
        }




        static bool PlaceCard(string placedCard)
        {
            int xCordinate = int.Parse(placedCard.Substring(4, 1));
            int yCordinate = int.Parse(placedCard.Substring(5, 1));
            string tunnelId = placedCard.Substring(0, 4);
            

            



            if (ValidCardPlace(tunnelId, xCordinate, yCordinate))
            {
                
                gameField[xCordinate, yCordinate] = tunnelId;
                UpdateTunnelOpenings(placedCard);
                return true;
            }
            else
            {
                return false;
            }

                
        }

        static bool ValidCardPlace(string tunnelId, int xCordinate, int yCordinate)
        {
            // Kontrollera att koordinaterna ligger inom gränserna för gameField
            if (xCordinate < 0 || xCordinate >= gameField.GetLength(0) ||
                yCordinate < 0 || yCordinate >= gameField.GetLength(1))
            {
                return false; // Koordinaterna är utanför matrisens gränser
            }

            // Hämta cellen på angivna koordinater
            string tmp = gameField[xCordinate, yCordinate];

            // Kontrollera om cellen är null
            if (tmp != null)
            {
                // Kontrollera cellen ovanför
                //if (xCordinate > 0)
                {
                    string cellAbove = gameField[xCordinate - 1, yCordinate];
                    if (cellAbove != null && cellAbove.Substring(2, 1) != tunnelId.Substring(0, 1))
                    {
                        Console.WriteLine("Ovan");
                        return false;
                    }
                }

                // Kontrollera cellen till vänster
                //if (yCordinate > 0)
                {
                    string cellLeft = gameField[xCordinate, yCordinate - 1];
                    if (cellLeft != null && cellLeft.Substring(1, 1) != tunnelId.Substring(3, 1))
                    {
                        Console.WriteLine("Vänster");
                        return false;
                    }
                }

                // Kontrollera cellen nedanför
                //if (xCordinate < gameField.GetLength(0) - 1)
                {
                    string cellBelow = gameField[xCordinate + 1, yCordinate];
                    if (cellBelow != null && cellBelow.Substring(0, 1) != tunnelId.Substring(2, 1))
                    {
                        Console.WriteLine("Nedan");
                        return false;
                    }
                }

                // Kontrollera cellen till höger
                //if (yCordinate < gameField.GetLength(1) - 1)
                {
                    string cellRight = gameField[xCordinate, yCordinate + 1];
                    if (cellRight != null && cellRight.Substring(3, 1) != tunnelId.Substring(1, 1))
                    {
                        Console.WriteLine("Höger");
                        return false;
                    }
                }
            }

            // Om alla kontroller är OK eller cellen är null, returnera true
            return true;
        }


        static void UpdateTunnelOpenings(string placedCard)
        {
            
            
            for (int t = 0; t < 4; t++)
            {

                if (placedCard.Substring(t, 1) == "2")
                {
                    switch (t)
                    {
                        case 0:
                            {
                                gameField[int.Parse(placedCard.Substring(4, 1)) - 1, int.Parse(placedCard.Substring(5, 1))] = gameField[int.Parse(placedCard.Substring(4, 1)) - 1, int.Parse(placedCard.Substring(5, 1))] == null ? "O" : gameField[int.Parse(placedCard.Substring(4, 1)) - 1, int.Parse(placedCard.Substring(5, 1))];

                                //Console.WriteLine(int.Parse((int.Parse(placedCard.Substring(4, 1))) - 1) + "::" + int.Parse(placedCard.Substring(5, 1)));
                                break;
                                
                            }
                        case 1:
                            {
                                gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) + 1] = gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) + 1] == null ? "O" : gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) + 1];

                                //Console.WriteLine(int.Parse(placedCard.Substring(4, 1)) + "::" + int.Parse(placedCard.Substring(5, 1)) + 1);
                                break;
                            }
                        case 2:
                            {
                                gameField[int.Parse(placedCard.Substring(4, 1)) + 1, int.Parse(placedCard.Substring(5, 1))] = gameField[int.Parse(placedCard.Substring(4, 1)) + 1, int.Parse(placedCard.Substring(5, 1))] == null ? "O" : gameField[int.Parse(placedCard.Substring(4, 1)) + 1, int.Parse(placedCard.Substring(5, 1))];

                                //Console.WriteLine(int.Parse(placedCard.Substring(4, 1)) + 1 + "::" + int.Parse(placedCard.Substring(5, 1)));
                                break;
                            }
                        case 3:
                            {
                                gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) - 1] = gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) - 1] == null ? "O" : gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) - 1];

                                //Console.WriteLine(int.Parse(placedCard.Substring(4, 1)) + "::" + int.Parse(placedCard.Substring(5, 1)) - 1);
                                break;
                                
                            }
                    }
                }
            }
            //tunnelOpenings.Add(placedCard);
        }
        static void SkrivTestCardPile(Stack<PlayCard> deck)
        {
            foreach (PlayCard p in deck)
            {
                Console.WriteLine(p.TunnelId);
            }
            Console.ReadKey();
            Environment.Exit(0);
        }

        static Stack<PlayCard> CreateCardPile()
        {
            List<PlayCard> cardPile = new List<PlayCard>();

            for (int i =0; i < 69; i++)
            {
                PlayCard card = CreateCard();
                cardPile.Add(card);
            }
            
            cardPile.OrderBy(x => r.Next()).ToList();

            return new Stack<PlayCard>( cardPile);
        }

        static PlayCard CreateCard()
        {
            
            string tmp = "";
            int numberOfTunelOpenings = 0;
            for (int i = 0; i < 4; i++)
            {
                //if (numberOfTunelOpenings >= 3) // Finns det ett kort med "2222" ??
                //{
                //    break;
                //}
                if (i == 3 &&  numberOfTunelOpenings == 0) // inga kort ska ha "1111"
                {
                    tmp += 2;
                    break;
                }
                if (r.Next(1, 3) == 2)
                {
                    tmp += 2;
                    numberOfTunelOpenings++;
                    continue;
                }
                tmp += 1;
                
            }

            


            PlayCard card = new PlayCard(tmp);
            return card;


        }


        static bool IsValidInput(string input, bool onlyText, string inputInterval = "")
        {
            if (string.IsNullOrEmpty(input))
            {
                return false; // Om input är null eller tom, returnera false
            }
            bool corectType = false;
            bool validInterval = false;
            bool checkValidInput = true;
            if (inputInterval == "")
            {
                checkValidInput = false;
                validInterval = true;
            }

            if (onlyText)
            {
                // Kontrollera om input endast innehåller bokstäver
                corectType = input.All(char.IsLetter);


                if (checkValidInput && corectType)
                {
                    string[] validAlternativs = input.Split(':');
                    foreach (string s in validAlternativs)
                    {
                        if (input.ToUpper() == s)
                        {
                            validInterval = true;
                            break;
                        }
                    }
                    
                }

                return corectType && validInterval;
            }
            else
            {
                // Kontrollera om input endast innehåller siffror
                corectType = input.All(char.IsDigit);

                if (checkValidInput && corectType)
                {
                    string[] validAlternativs = inputInterval.Split('-');
                    
                    if (int.Parse(input) >= int.Parse(validAlternativs[0]) && int.Parse(input) <= int.Parse(validAlternativs[1]))
                    {
                        validInterval = true;
                    }

                }

                return corectType && validInterval;
            }
        }


        public class PlayCard
        {
            string tunnelId;

            public PlayCard(string tunnelId)
            {
                this.tunnelId = tunnelId;
            }

            public string TunnelId
            {
                get { return tunnelId; }
                set { tunnelId = value; }

            }


        }

        public class Player
        {
            string name;
            
            List<PlayCard> playerCards = new List<PlayCard>();


            public Player(string name)
            {
                this.name = name;
                
                //this.playerCards = playerCards;
            }
            

            public string Name
            {
                get { return name; }

            }

            

            public List<PlayCard> PlayerCards
            {
                get { return playerCards; }
                set { playerCards = value; }
            }




        }
    }
}
