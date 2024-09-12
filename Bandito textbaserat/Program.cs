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

                if ( playerCountInput.Length > 1 || !char.IsDigit(playerCountInput[0]) || int.Parse(playerCountInput) > 4 || int.Parse(playerCountInput) <= 0)
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
            
            
            int[,] gameField = new int[1, 1];

            // Fyll arrayen med koordinater
            

            //Game loop

            bool gameAktive = true;
            List<PlayCard> cardPile = CreateCardPile();
            while (gameAktive)
            {
                Console.WriteLine(cardPile[0].TunnelId);
                Console.ReadKey();
            }



            Console.ReadKey();
        }






        static void SkrivTest()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    gameField[i, j] = CreateCard().TunnelId; // Spara koordinaterna som en sträng
                }
            }

            // Skriv ut arrayen i konsolen
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(gameField[i, j] + " ");
                }
                Console.WriteLine(); // Ny rad efter varje rad i arrayen
            }

            Console.ReadKey();
            Environment.Exit(0);
        }
        
        static List<PlayCard> CreateCardPile()
        {
            List<PlayCard> cardPile = new List<PlayCard>();

            for (int i =0; i < 68; i++)
            {
                PlayCard card = CreateCard();
                cardPile.Add(card);
            }
            
            return cardPile;
        }

        static PlayCard CreateCard()
        {
            
            string tmp = "";
            for (int i = 0; i < 4; i++)
            {
                tmp += r.Next(1, 3).ToString();
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
            int playTurnNumber;
            List<PlayCard> playerCards;


            public Player(string name, int playTurnNumber)
            {
                this.name = name;
                this.playTurnNumber = playTurnNumber;
                //this.playerCards = playerCards;
            }
            

            public string Name
            {
                get { return name; }

            }

            public int PlayTurnNumber
            {
                get { return playTurnNumber; }
            }

            public List<PlayCard> PlayerCards
            {
                get { return playerCards; }
                set { playerCards = value; }
            }




        }
    }
}
