using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bandito_textbaserat
{

    class Program
    {
        public static Random r = new Random();

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

                if (playerCountInput.Length > 1 || !char.IsDigit(playerCountInput[0]) || int.Parse(playerCountInput) > 4 || int.Parse(playerCountInput) <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input\n\n");
                    continue;
                }
                playerCount = int.Parse(playerCountInput);
                validPlayerCountInput = !validPlayerCountInput;
                Console.Clear();
                Console.WriteLine("Sure, vi kör med " + playerCount + " spelare");

            }


            // Create game field


            string[,] gameField = new string[1, 2];


            //SkrivTest(gameField);






            //Create card pile

            Stack<PlayCard> cardPile = CreateCardPile();

            //SkrivTestCardPile(cardPile);







            //Create players

            Queue<Player> playerQueue = new Queue<Player>();
            Player newPlayer;
            Player activePlayer;

            for (int i = 1; i < (playerCount + 1); i++)
            {
                Console.WriteLine("Player " + i + " Name:");
                newPlayer = new Player(Console.ReadLine());
                for (int j = 0; j < 3; j++)
                {
                    newPlayer.PlayerCards.Add(cardPile.Pop());
                }

                playerQueue.Enqueue(newPlayer);
            }

            Console.Clear();

            //Place Super card
            gameField[0, 0] = "2222";
            gameField[0, 1] = "2212";

            //SkrivTest(gameField);


            //Game loop




            List<int> activeFieldCards = new List<int>();
            activeFieldCards.Add(222211); //add super card. 4 first is tunnel id and the rest are the coordinates in gamefield
            bool gameActive = true;
            Console.WriteLine("Splet startar\n");

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
                Console.WriteLine("Spelplan:\n");
                TestDrawGameFeild(gameField);
                DrawRow();

                Console.WriteLine("Din hand:\n");
                SkrivTestPlayerhand(activePlayer);
                DrawRow();

                Console.ReadKey();
                Console.WriteLine();
                








            }

            Console.WriteLine("blaaaa");

            Console.ReadKey();
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
            //for (int i = 0; i < gameField.GetLength(0); i++)
            //{
            //    for (int j = 0; j < gameField.GetLength(1); j++)
            //    {
            //        gameField[i, j] = CreateCard().TunnelId; // Spara koordinaterna som en sträng
            //    }
            //}

            // Skriv ut arrayen i konsolen
            string tmp = "X";
            string[] consoleLine = new string[3];
            
            for (int i = 0; i < gameField.GetLength(1); i++)
            {
                

                for (int j = 0; j < gameField.GetLength(0); j++)
                {
                    
                    for (int lineCheck = 0; lineCheck < 3; lineCheck++)
                    {
                        if (lineCheck == 0 || lineCheck == 2)
                        {
                            consoleLine[lineCheck] += tmp;
                            if (lineCheck == 0)
                            {
                                consoleLine[lineCheck] += (gameField[j, i].Substring(0, 1) == "2") ? " " : tmp;

                            }
                            else
                            {
                                consoleLine[lineCheck] += (gameField[j, i].Substring(2, 1) == "2") ? " " : tmp;
                            }
                            consoleLine[lineCheck] += tmp;

                        }
                        else
                        {
                            consoleLine[lineCheck] += (gameField[j, i].Substring(3, 1) == "2") ? " " : tmp;
                            consoleLine[lineCheck] += " ";
                            consoleLine[lineCheck] += (gameField[j, i].Substring(1, 1) == "2") ? " " : tmp;
                        }
                        
                    }
                    for (int p = 0; p <3; p++)
                    {
                        Console.WriteLine(consoleLine[p]);
                        consoleLine[p] = "";
                    }
                    
                    
                }
                
            }

            
        }
        static void TestDrawGameFeild2(int[,] gameField)
        {
            

            // Skriv ut arrayen i konsolen
            for (int i = 0; i < gameField.GetLength(0); i++)
            {
                for (int j = 0; j < gameField.GetLength(1); j++)
                {
                    Console.Write(gameField[i, j] + " ");
                }
                Console.WriteLine(); // Ny rad efter varje rad i arrayen
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

            


            PlayCard card = new PlayCard(int.Parse(tmp));
            return card;


        }

        public class PlayCard
        {
            int tunnelId;

            public PlayCard(int tunnelId)
            {
                this.tunnelId = tunnelId;
            }

            public int TunnelId
            {
                get { return tunnelId; }

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
