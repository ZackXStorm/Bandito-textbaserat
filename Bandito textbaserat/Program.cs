using System;
using System.Collections.Generic;
using System.Linq;
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
        static void Main(string[] args)
        {

            // Meny

            bool validMenuInput = false;
            while (!validMenuInput)
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
                    continue;

                }

                if (menuInput.ToUpper() == "S")
                {
                    Console.Clear();
                    Console.WriteLine("Startar spel\n");
                    validMenuInput = true;
                    continue;

                }



                Console.Clear();
                Console.WriteLine("Invalid input\n\n");

            }


            bool validPlayerCountInput = false;
            int playerCount = 0;
            while (!validPlayerCountInput)
            {
                Console.WriteLine("Hur många spelare?");
                string playerCountInput = Console.ReadLine();

                if (playerCountInput == "1" || playerCountInput == "2" || playerCountInput == "3" || playerCountInput == "4")
                {
                    playerCount = int.Parse(playerCountInput);
                    validPlayerCountInput = !validPlayerCountInput;
                    Console.Clear();
                    Console.WriteLine("Sure, vi kör med " + playerCount + " spelare");
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
                    Console.Clear();
                    Console.WriteLine("Invalid input\n\n");
                    continue;
                }
                

            }


            // Create game field


            string[,] gameField = new string[1, 1];


            //SkrivTest(gameField);






            //Create card pile

            cardPile = CreateCardPile();

            //SkrivTestCardPile(cardPile);







            //Create players

            playerQueue = new Queue<Player>();
            Player newPlayer;
            

            for (int i = 1; i < (playerCount + 1); i++)
            {
                Console.WriteLine("Player " + i + " Name:");
                newPlayer = new Player(Console.ReadLine());
                //for (int j = 0; j < 3; j++)
                //{
                //    newPlayer.PlayerCards.Add(cardPile.Pop());
                //}

                playerQueue.Enqueue(newPlayer);
            }

            Console.Clear();

            //Place Super card
            gameField[0, 0] = "2222";



            //gameField[0, 1] = "2212";
            //gameField[1, 0] = "2221";
            //gameField[1, 1] = "1112";
            
            //gameField[2, 0] = "2111";

            //SkrivTest(gameField);


            //Game loop




            List<int> activeFieldCards = new List<int>();
            activeFieldCards.Add(222211); //add super card. 4 first is tunnel id and the rest are the coordinates in gamefield
            bool gameActive = true;
            Console.WriteLine("\tSplet startar");
            playerState = State.AskWhatToDo;
            int selectedCard = 0;
            while (gameActive)
            {
                
                
                activePlayer = playerQueue.Peek();
                

                if (activePlayer.PlayerCards.Count < 3)
                {
                    activePlayer.PlayerCards.Add(cardPile.Pop());
                    Console.WriteLine(activePlayer.Name + " drog ett kort");
                    continue;
                }

                
                DrawRow();
                Console.WriteLine("\tSpelplan:");
                TestDrawGameFeild(gameField);
                DrawRow();

                Console.WriteLine("\tDin hand:");
                SkrivTestPlayerhand(activePlayer);
                DrawRow();

                

                switch (playerState)
                {
                    case State.AskWhatToDo:
                        {
                            AskWhatToDo();
                            continue;
                            
                        }
                    case State.WhichCard:
                        {
                            selectedCard = WhichCard();
                            playerState = State.WhatDoWithCard;
                            continue;
                        }
                    case State.WhatDoWithCard:
                        {
                            WhatDoWithCard();
                            continue;
                        }
                    case State.WhichDirectionRotate:
                        {
                            WhichDirectionRotate(selectedCard);
                            continue;
                        }

                }
                Console.WriteLine();
                








            }

            Console.WriteLine("blaaaa");

            Console.ReadKey();
        }

        static void AskWhatToDo()
        {
            string menuString = "[D]raw cards\n[P]lace Card";
            Console.WriteLine(menuString); 
            string input = Console.ReadLine();
            if (input.ToUpper() == "D")
            {
                
                
                activePlayer.PlayerCards.Clear();

                NextPlayer();
                Console.WriteLine("Nästa spelare");
            }
            if (input.ToUpper() == "P")
            {
                playerState = State.WhichCard;
            }
        }

        static void NextPlayer()
        {
            playerQueue.Enqueue(playerQueue.Dequeue());
            
        }

        static int WhichCard()
        {
            Console.WriteLine("Which card?");
            int input =int.Parse( Console.ReadLine());

            return input;
        }

        static void WhatDoWithCard()
        {
            Console.WriteLine("[R]otate\n[P]lace");
            string input = Console.ReadLine();

            if (input.ToUpper() == "R")
            {
                playerState = State.WhichDirectionRotate;
            }
            if (input.ToUpper() == "P")
            {
                playerState = State.WherePlaceCard;
            }
        }

        static void WhichDirectionRotate(int selectedCard)
        {
            Console.WriteLine("Rotate "+ activePlayer.PlayerCards[selectedCard].TunnelId + "which way?\n[L]eft\n[R]ight");
            string input = Console.ReadLine();

            

            if (input.ToUpper() == "R")
            {
                activePlayer.PlayerCards[selectedCard].TunnelId = activePlayer.PlayerCards[selectedCard].TunnelId.Substring(3, 1) + activePlayer.PlayerCards[selectedCard].TunnelId.Substring(0, 3);
                activePlayer.PlayerCards[selectedCard].TunnelId.Remove(3);
            }
            if (input.ToUpper() == "L")
            {
                activePlayer.PlayerCards[selectedCard].TunnelId += activePlayer.PlayerCards[selectedCard].TunnelId.Substring(0, 1);
                activePlayer.PlayerCards[selectedCard].TunnelId = activePlayer.PlayerCards[selectedCard].TunnelId.Remove(0, 1);
            }
        }

        static string CalculateActiveTunnels(List<int> activeFieldCards)
        {

            return "";
        }

        static void DrawRow()
        {
            int consoleWidth = Console.WindowWidth; // Hämtar konsolens bredd
            Console.WriteLine(new string('-', consoleWidth)); // Skriver en linje över hela bredden

        }
        static void SkrivTestPlayerhand(Player player)
        {
            Console.WriteLine(player.Name + ", Dina kort är; ");
            foreach (PlayCard card in player.PlayerCards)
            {
                Console.WriteLine(card.TunnelId);
            }
            
        }

        static void TestDrawGameFeild(string[,] gameField)
        {
            

            string tmp = "#";
            int numRows = gameField.GetLength(0);
            int numCols = gameField.GetLength(1);

            // En array för att lagra alla rader av konsolutmatningen
            string[] consoleLines = new string[3];

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
                    string cell = gameField[j, i];
                    if (cell == null) continue;


                    // Bygg den horisontella raden för cellen
                    string top = tmp + (cell.Substring(0, 1) == "2" ? " " : tmp) + tmp;
                    string middle = (cell.Substring(3, 1) == "2" ? " " : tmp) + " " + (cell.Substring(1, 1) == "2" ? " " : tmp);
                    string bottom = tmp + (cell.Substring(2, 1) == "2" ? " " : tmp) + tmp;

                    // Lägg till cellens rad till rätt plats i consoleLines
                    consoleLines[0] += top;
                    consoleLines[1] += middle;
                    consoleLines[2] += bottom;
                }

                // Lägg till en ny rad efter att ha bearbetat hela raden
                for (int k = 0; k < 3; k++)
                {
                    Console.WriteLine(consoleLines[k]);
                    consoleLines[k] = "";
                }
            }




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
            Stack<PlayCard> cardPile = new Stack<PlayCard>();

            for (int i =0; i < 69; i++)
            {
                PlayCard card = CreateCard();
                cardPile.Push(card);
            }
            
            cardPile.OrderBy(x => r.Next()).ToList();

            return cardPile;
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
