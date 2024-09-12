using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bandito_textbaserat
{
    class Program
    {
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


            //Spel loop

            bool gameAktive = true;
            List<PlayCard> cardpile = CreateCardPile();
            while (gameAktive)
            {
                Console.WriteLine(cardpile[0].TunnelId);
                Console.ReadKey();
            }



            Console.ReadKey();
        }







        
        static List<PlayCard> CreateCardPile()
        {
            List<PlayCard> cardPile = new List<PlayCard>();

            Random r = new Random();
            string tmp = "";
            for (int i = 0; i < 4; i++)
            {
                tmp += r.Next(0, 2).ToString();
            }



            PlayCard ettKort = new PlayCard(tmp);


            cardPile.Add(ettKort);
            return cardPile;
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

            }


        }
    }
}
