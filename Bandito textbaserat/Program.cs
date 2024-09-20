using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bandito_textbaserat
{

    class Program // Btw: "...." betyder att kommenteren fortsätter i nästa kommentar
    {
        static public Random r = new Random();
        public enum State { AwaitingNextPlayer, AskWhatToDo, WhichCard, WhatDoWithCard, WhichDirectionRotate, WherePlaceCard }; // De olika States som en splare kan vara i under spel loopen
        public static State playerState; // Det aktiva statet
        static Stack<PlayCard> cardPile;
        static Queue<Player> playerQueue;
        static Player activePlayer; //Spelaren som för tillfälet spelar 
        static string[,] gameField = new string[129, 139]; //gameFild celler kan vara tre saker. 1: null om det är inget i den. 2: ett placerat spel korts tunnelid vilket är ett fyrsiffrigt nummer som beskriver hur tunlarna ser ut åt de fyra vädersträcken. Den går med klockan och börjar norr. ex som första sifran är 1 så har kortet en vägg uppåt, men om det är en 2 så har den en tunnel uppåt. 3: "O", vilket inte markerar ett faktiskt kort utom celler tunnlarna går till
        static List<string> tunnelOpenings = new List<string>(); //Denna list håller koll på kordinaterna på cellerna i gamefield som är "O"
        static int GameRound = 1; // Rundan
        static int selectedCard; // Vilket kort i spelarens hand dem valt
        static int playerCount; //Antal spelare
        static bool useAwaitingNextPlayer; //Denna är till för om du vill skippa hela "awating next palyer" gräjen
        static void Main(string[] args)
        {

            // Meny
            while (true)
            {
                Console.WriteLine("Välommen till Bandito\nVad vill du göra?\n\n");
                string menuText = "[S]tarta nyt spelet:\n\n[I]nstructions\n\n[A]vsluta"; //Meny text
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

            //frågar om spelaren vill ha random kort eller förbesämda 
            bool useRandomCards; // Om spelarna vill använda random kort
            while (true)
            {
                Console.WriteLine("Do you want to use random cards? \n[Y]es\n[N]o");
                string input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                {
                    useRandomCards = true;
                    break;
                }

                if (input.ToUpper() == "N")
                {
                    useRandomCards = false;
                    break;
                }
                Console.Clear();
                WriteInvalid("Invalid input");
            }
            Console.Clear();


            //Create card pile
            cardPile = CreateCardPile(useRandomCards);


            // Ask input of player count
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
            Console.Clear() ;


            // Frågar om hela "Awating next player" grejen ska hända
            while (playerCount > 1)
            {
                Console.WriteLine("Do you want to use 'Awating next player'\n[Y]es\n[N]o");
                string input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                {
                    useAwaitingNextPlayer = true;
                    break;
                }

                if (input.ToUpper() == "N")
                {
                    useAwaitingNextPlayer = false;
                    break;
                }
                Console.Clear();
                WriteInvalid("Invalid input");
            }
            Console.Clear();


            //Create players and player queue
            playerQueue = new Queue<Player>();
            List<Player> tmpQue = new List<Player>();

            for (int i = 1; i < (playerCount + 1); i++)
            {
                while (true)
                {
                    Console.WriteLine("Player " + i + " Name:");
                    string input = Console.ReadLine();
                    bool nameAlredyExist = false;
                    foreach (Player p in tmpQue)
                    {
                        if (input.ToUpper() == p.Name.ToUpper())
                        {
                            nameAlredyExist = true;
                            break;
                        }

                    }
                    if (IsValidInput(input, true) && !nameAlredyExist)
                    {
                        
                        tmpQue.Add(new Player(input));
                        break;
                    }
                    WriteInvalid("Name already taken");
                }


                
            }

            tmpQue = tmpQue.OrderBy(x => r.Next()).ToList(); // Detta är varför jag behövde använda en tmp lista, för att orderBy inte fungerar på en Stack. Men eftersom en Stack är väldigt bra som en player queue ville jag fortfrande använda det
            playerQueue = new Queue<Player>(tmpQue);

            Console.Clear();



            //Place Super card
            string placeSupercard = "2222" + (gameField.GetLength(0) / 2) + (gameField.GetLength(1) / 2); //Plaserar den i mitten av gameField
            PlaceCard(placeSupercard);
            
   

            // Game Loop
            Console.WriteLine("\tSplet startar");
            playerState = useAwaitingNextPlayer ? State.AwaitingNextPlayer : State.AskWhatToDo; //Om vi ska använda AwaitingNextPlayer eller inte
            selectedCard = 1; // Detta är bara för att det ska vara något så att den aldrig kan vara null (aka felhantering)
            
            while (true)
            {

                Console.WriteLine("Game Round: " + GameRound + "\n");
                activePlayer = playerQueue.Peek(); //Nästa spelare i queue blir aktiv spelare
                

                while (activePlayer.PlayerCards.Count < 3) // Om aktiv spelare har mindre en tre kort.....
                {
                    activePlayer.PlayerCards.Add(cardPile.Pop()); //..... då dra ett kort till du har tre
                    Console.WriteLine(activePlayer.Name + " drog ett kort");
                    

                }

                DrawRow(); // En metod för att bara rita en rad
                Console.WriteLine("\tSpelplan:");
                DrawGameFeild(gameField); //Ritar hur spelpanen ser ut genom att konvertera en cells tunnel id till en 3x3 ruta så att man kan visualesera för spelaren hur korten och spelplanen ser ut.
                Console.WriteLine(" (Active tunnels: " + tunnelOpenings.Count + ")");
                DrawRow();


                if (tunnelOpenings.Count <= 0) // Om antalet öppna tunnlar = 0 är seplet vunet, men eftersom vi redan har placerat superkortet och gjort alla beräknigar för öpnna tunnlar så spelar det ingen roll att den ligger så här tidigt i game loopen, den kommer ändå inte triggas än  
                {
                    break; // Går ut ur while loopen vilket visar vinnar texten
                }

                switch (playerState)
                {
                    case State.AwaitingNextPlayer: //Denna är för att invänta nästa seplare utan att visa någon information om spelet. Detta för att regelerna är tydlga med att spelare inte ska veta om hur varandras kort ser ut. Så denna State ger spelare tid att byta plats så att de inte ser varandras kort
                        {
                            Console.Clear();
                            Console.WriteLine("\n\nAwaiting next player: " + activePlayer.Name + "\nPress [N] when ready");
                            AwaitingNextPlayer();
                            Console.Clear();
                            break;
                        }

                    case State.AskWhatToDo:
                        {
                            Console.WriteLine("\tDin hand:");
                            DrawPlayerHand(activePlayer); //Skriver ut spelarens HELA hand. anlednigen varför den inte ligger 1 gång nedanför gamefild är för att om man valt ett kort i en State så ska bara det kortet skrivas ut
                            DrawRow();
                            AskWhatToDo();
                            break;
                            
                        }
                    case State.WhichCard:
                        {
                            Console.WriteLine("\tDin hand:");
                            DrawPlayerHand(activePlayer);
                            DrawRow();
                            WhichCard();
                            
                            break;
                        }
                    case State.WhatDoWithCard:
                        {
                            Console.WriteLine("\tSecelcted card:");
                            DrawPlayerHand(activePlayer, true); // true betyder att endast det valda kortet i playerHand ska visas
                            DrawRow();
                            WhatDoWithCard();
                            break;
                        }
                    case State.WhichDirectionRotate:
                        {
                            Console.WriteLine("\tSecelcted card:");
                            DrawPlayerHand(activePlayer, true);
                            DrawRow();
                            WhichDirectionRotate(selectedCard);
                            break;
                        }
                    case State.WherePlaceCard:
                        {
                            Console.WriteLine("\tSecelcted card:");
                            DrawPlayerHand(activePlayer, true);
                            DrawRow();

                            if (WherePlaceCard(selectedCard)) // om det retunerar true så var placeringen av kortet lagligt 
                            {
                                Console.Clear();
                                break;
                            }
                            else // annars inte
                            {
                                WriteInvalid("Invalid input or rule break");
                                break;
                            }
                            
                        }

                }

                
                
                
                








            }

            // Om antalet öppna tunlar är = 0 så kommer koden hit 
            DrawRow();
            Console.WriteLine("Du har vunnit !!!!!!!!!!!!!!!!!!!!!");
            Console.ReadKey();
            Console.WriteLine("Tack för att du spelade");
            Console.ReadKey(); //Spelet är nu slut
        }

        static void AwaitingNextPlayer()
        {
            string input = Console.ReadLine();
            if (input.ToUpper() == "N") //Väntar på nästa spelare
            {
                playerState = State.AskWhatToDo;
            }
        }
        static void WriteInvalid(string reason) // Denna metod skriver bara ut argument stringen med färgen röd
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(reason + "\n");
            Console.ResetColor();
        }
        static void AskWhatToDo()
        {
            string menuString = "[D]raw cards\n[S]elect card to play";
            Console.WriteLine(menuString); 
            string input = Console.ReadLine();
            if (input.ToUpper() == "D") //"Draw cards" är för om spelaren inte kan lägga, vilket då gör att....
            {
                Console.Clear();
                
                activePlayer.PlayerCards.Clear(); //...seplaren kastar sin hand och....
                Console.Clear();
                NextPlayer(); // ..... det blir nästa spelares tur. Men spelaren behöver inte dra nya kort här efetrsom det sker i while loopen i början av sin nästa tur
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
            playerQueue.Enqueue(playerQueue.Dequeue()); //Spelaren tas bort i början av kön och läggs till sist
            playerState = useAwaitingNextPlayer ? State.AwaitingNextPlayer : State.AskWhatToDo; // Denna är samma som den innan while loopen för game loop
            GameRound++;
        }

        static void WhichCard()
        {
            Console.WriteLine("Which card?\nOr [B]ack");
            string input = Console.ReadLine();
            if (IsValidInput(input, true) && input.ToUpper() == "B") 
            {
                Console.Clear();
                playerState = State.AskWhatToDo; //Backar till föra Satet 
                return;
            }
            if (!IsValidInput(input, false, "1-3")) //Om föra isatsen inte triggats, och inte denna heller då.....
            {
                WriteInvalid("Invalid input"); //... är det en invalid input
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

            

            if (input.ToUpper() == "R")
            {
                //Rotering åt höger sker genom att att sista siffran blir första 
                activePlayer.PlayerCards[selectedCard].TunnelId = activePlayer.PlayerCards[selectedCard].TunnelId.Substring(3, 1) + activePlayer.PlayerCards[selectedCard].TunnelId.Substring(0, 3);
                activePlayer.PlayerCards[selectedCard].TunnelId.Remove(3);
                return;
            }
            if (input.ToUpper() == "L")
            {
                //Vänster är tvärt om 
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
            Console.WriteLine("1 - " + tunnelOpenings.Count() +"\nOr [B]ack"); //Spelarn ska bara kunan välja 1 till max antal tunnelöpnigar 
            string input = Console.ReadLine();
            if (input.ToUpper() == "B")
            {
                playerState = State.WhatDoWithCard;
                return true;
            }
            if (!IsValidInput(input, false, "1-" + tunnelOpenings.Count())) // intervallen i detta fal är 1 till max antal tunnelöpnigar 
            {
                return false;
            }
            else
            {
                int index = int.Parse(input) - 1; // Den tunnelöpnigen du önskar att placera ditt kort
                //Kordinaterna för den cellen
                string xCordinate = tunnelOpenings[index].Substring(0, 1); 
                string yCordinate = tunnelOpenings[index].Substring(1, 1);
                
                PlayCard selectedPlaycard = activePlayer.PlayerCards[selectedCard]; //Dett kortet du önskar att spela
                if (PlaceCard(selectedPlaycard.TunnelId + xCordinate + yCordinate)) 
                {
                    activePlayer.PlayerCards.Remove(selectedPlaycard); //Om kortet kan placeras kommer kortet att tas bort från spelarens hand

                    NextPlayer(); //Sedan blir det nästa spelares tur 
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
        static void DrawPlayerHand(Player player, bool onlySelectedCard = false)
        {
            if (!onlySelectedCard)
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

        static void DrawGameFeild(string[,] gameField)
        {
            

            int numberOfTunnelOpening = 1; //Både antalet av öpnna tunnlar, men också för att numrera tunnel öpningarna för spelaren
            tunnelOpenings.Clear(); // Tömer listan för öppna tunlar för att sedan fylla på på nytt
            
            int numRows = gameField.GetLength(0);
            int numCols = gameField.GetLength(1);

            // En array för att lagra alla rader av konsolutmatningen
            string[] consoleLines = new string[3];


            // Skapa en lista som håller reda på om en kolumn är helt null. Detta så att den inte skriver ut en hel columb i onödan om hela är tom 
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



            // Rensa varje rad bara så att det inte finns något gamalt kvar.
            for (int lineCheck = 0; lineCheck < 3; lineCheck++)
            {
                
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


                    string top; //Topen på kortet,...
                    string middle; //...., Mitten...
                    string bottom; //.... och båten
                    string cell = gameField[j, i]; //Cellen som ska kollas

                    if (cell == null) // Hela tomma culumber har redan hoppats över, denna är till för de tomma cellerna i en culumb/rad som inte är helt tom, bara så att alting ligger på rätt ställa
                    {

                        top = "   ";
                        middle = "   ";
                        bottom = "   ";

                    }

                    else if (cell == "O") // Om en cell är "O" har den markerats som en tunnelöpning, inte ett kort faktiskt spelkort utom de cellerna tunnlarna går till
                    {
                        // Bygg en kvadrat med numret av tunnelöpningen i mitten
                        top = "|" + "-" + "|";
                        middle = "|" + numberOfTunnelOpening + "|";
                        bottom = "|" + "-" + "|";
                        tunnelOpenings.Add(j + "" + i); //Sparar ner kordinatrena för "O" celler
                        numberOfTunnelOpening++;
                        
                        
                        
                    }
                    else
                    {
                        string[] tmpLines = GetCellData(cell); // Hämtar hur cellen ser ut över tre rader
                        
                        // tilder dem
                        top = tmpLines[0];
                        middle = tmpLines[1];
                        bottom = tmpLines[2];

                    }

                    

                    // Lägg till cellens rad till rätt plats i consoleLines
                    consoleLines[0] += top;
                    consoleLines[1] += middle;
                    consoleLines[2] += bottom;
                }

                // Skriv ut de tre raderna för förta raden i gameFild
                DrawCells(consoleLines);
            }


            

        }

        static string[] GetCellData(string cellId, bool devide = false) //Anelding att jag tog ut det ur DrawGameFeild() och gjore det till en metod var att jag ville använda den för DrawPlayerHand() med 
        {
            string tmp = "#"; // vad "väggar" ska markeras som
            string[] consoleLines = new string[3];
            // Bygg den horisontella raden för cellen
                //Hörnen på ett kort är alltid väggar så dem är bara tmp. Mitten är allid öppen. Men det är de fyra vädersträcken som ska kollas om det är "2" vilket gör dem till en öppning, annars är det "1" vilket betyder vägg
            consoleLines[0] = tmp + (cellId.Substring(0, 1) == "2" ? " " : tmp) + tmp; //Toppen på kortet
            consoleLines[1] = (cellId.Substring(3, 1) == "2" ? " " : tmp) + " " + (cellId.Substring(1, 1) == "2" ? " " : tmp); // mitten
            consoleLines[2] = tmp + (cellId.Substring(2, 1) == "2" ? " " : tmp) + tmp; // båten
            if (devide == true) //Denna används när metoden aktiveras vid utskrivningen av playerHand. det ser bättre ut där om man sepererar korten lite genom....
            {
                for (int l = 0; l < consoleLines.Length; l++)
                {
                    // .... att lägga till en vertikal linje mellan varje kort, och på sidorna längt till vänster och höger
                    consoleLines[l] += " | ";
                    consoleLines[l] = consoleLines[l].Insert(0, " | ");
                }
            }
            return consoleLines;

        }
        static void DrawCells(string[] consoleLines) // Denna togs ut ur drawGameFild() av samma anledning som tidigare
        {
            for (int k = 0; k < 3; k++)
            {
                if (!string.IsNullOrWhiteSpace(consoleLines[k])) //Den hoppar över alla helt tomma rader
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
            

            



            if (ValidCardPlace(tunnelId, xCordinate, yCordinate)) // Kollar om det är lagligt att placera kortet där. Om det är det så placeras kortet, om inte så får spelaren försöka igen
            {
                
                gameField[xCordinate, yCordinate] = tunnelId; //Om placeringen är valid kommer kortet att placeras .....
                UpdateTunnelOpenings(placedCard); //... och nya beräkna nya tunnelöpningar
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
            string cell = gameField[xCordinate, yCordinate];

            // Kontrollera om cellen är null
            if (cell != null)
            {
                //  Denna är lite kompliserad. Den jämnför tunel IDt på det kort du önskar att placera i förhållande till de närliggade cellernas "motsata" tunnel IDn ########. ex så gämförst tunelidÖver[2] mot tunnelidUnder[0], och om dem inte är samma 

                // Kontrollera cellen ovanför genom att jämnföra sifran som pekar ner på det kortet över [2] mot sifran som pekar uppåt på kortet under [0]
                string cellAbove = gameField[xCordinate - 1, yCordinate];
                if (cellAbove != null && cellAbove.Substring(2, 1) != tunnelId.Substring(0, 1))
                {
                    
                    return false;
                }

                // Kontrollera cellen till vänster
                string cellLeft = gameField[xCordinate, yCordinate - 1];
                if (cellLeft != null && cellLeft.Substring(1, 1) != tunnelId.Substring(3, 1))
                {
                    
                    return false;
                }

                // Kontrollera cellen nedanför
                string cellBelow = gameField[xCordinate + 1, yCordinate];
                if (cellBelow != null && cellBelow.Substring(0, 1) != tunnelId.Substring(2, 1))
                {
                    
                    return false;
                }

                // Kontrollera cellen till höger
                string cellRight = gameField[xCordinate, yCordinate + 1];
                if (cellRight != null && cellRight.Substring(3, 1) != tunnelId.Substring(1, 1))
                {
                   
                    return false;
                }
            }

            // Om alla kontroller är OK eller cellen är null, returnera true. Om inte så kommer placeringen att inte vara valid och då kommer kortet inte att läggas
            return true;
        }


        static void UpdateTunnelOpenings(string placedCard) // Denna metod är vad som placerar alla "O" i gameFild. och om du hoppade hit utan att läsa på vad det är, så är det en markering i en cell för att berätta att en öppen tunnel går dit
        {
            
            
            for (int t = 0; t < 4; t++) //Kolla alla fyra siffror i tunnel IDt för kortet du precis laggt
            {

                if (placedCard.Substring(t, 1) == "2") //Om sifran är 2, aka en tunnelöpning
                {
                    switch (t)// Denna är lite komplicerad, men den kollar om den ska/vart placera "O"n runt det placerade kortet genom att kolla om cellerna runt är null, vilket då den ska bli "O", men om de redan har ett väre så finns redan ett kort där eller "O" så då ska det behålla sitt värde
                    {
                        case 0:
                            {
                                //Kollar cellen åvan
                                gameField[int.Parse(placedCard.Substring(4, 1)) - 1, int.Parse(placedCard.Substring(5, 1))] = gameField[int.Parse(placedCard.Substring(4, 1)) - 1, int.Parse(placedCard.Substring(5, 1))] ?? "O";

                                
                                break;
                                
                            }
                        case 1:
                            {
                                //Cellen till höger
                                gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) + 1] = gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) + 1] ?? "O";

                                
                                break;
                            }
                        case 2:
                            {
                                //Cellen nedanför
                                gameField[int.Parse(placedCard.Substring(4, 1)) + 1, int.Parse(placedCard.Substring(5, 1))] = gameField[int.Parse(placedCard.Substring(4, 1)) + 1, int.Parse(placedCard.Substring(5, 1))] ?? "O";

                                
                                break;
                            }
                        case 3:
                            {
                                //Cellen till höger
                                gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) - 1] = gameField[int.Parse(placedCard.Substring(4, 1)), int.Parse(placedCard.Substring(5, 1)) - 1] ?? "O";

                                
                                break;
                                
                            }
                    }
                }
            }
            //tunnelOpenings.Add(placedCard);
        }
        static void SkrivTestCardPile(Stack<PlayCard> deck) //Bara en test metod som inte längre används, men om vi ska fortsätta med spelet kan den vara bra att ha senare. Den bara skriver ut alla IDn i kort högen
        {
            foreach (PlayCard p in deck)
            {
                Console.WriteLine(p.TunnelId);
            }
            Console.ReadKey();
            Environment.Exit(0);
        }

        static Stack<PlayCard> CreateCardPile(bool useRandomcards)
        {
            List<PlayCard> cardPile = new List<PlayCard>(); //Det är samma anledning här som med playerQueue: OrderBy fungerar bara på en list men jag vill använda en Stack för spelhögen då det passar bäst som det

            // I vilket fall kommer cardpile att ha mer en 69 kort i sig som spelreglerna säger, men som du kanske märker är det dubbla antalet här. det är för att jag delat up alla rektanglar i kvadrater, så tekniskt sätt är det fortfarande lika många tunlar
            if (useRandomcards) // Alla kort är random 
            {
                for (int i = 0; i < 138; i++)
                {
                    PlayCard card = CreateRandomCard();
                    cardPile.Add(card);
                }
            }
            else //Förbesämda kort
            {
                for (int i = 0;i < 38 ; i++)
                {
                    cardPile.Add(new PlayCard("2211")); //Skarp sväng
                }
                for (int i = 0; i < 21; i++)
                {
                    cardPile.Add(new PlayCard("2111")); //End card
                }
                for (int i = 0; i < 51; i++)
                {
                    cardPile.Add(new PlayCard("1222")); // T korsning
                }
                for (int i = 0; i < 19; i++)
                {
                    cardPile.Add(new PlayCard("1212")); //Rak väg
                }
                for (int i = 0; i < 9; i++)
                {
                    cardPile.Add(new PlayCard("2222")); // + korsning
                }
                

            }
            
            cardPile = cardPile.OrderBy(x => r.Next()).ToList(); //Random ordning

            return new Stack<PlayCard>(cardPile);
        }

        static PlayCard CreateRandomCard() // ###### Läg till korten
        {
            
            string tmp = "";
            int numberOfTunelOpenings = 0;
            for (int i = 0; i < 4; i++)
            {
                
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


        static bool IsValidInput(string input, bool onlyText, string inputInterval = "") //Detta är en metod jag använder för att kolla spelar input. 1: string input är spelar input. 2: bool onlyText är om metoden ska kolla om det endast är text i inputen, om den är false kollar den istället om det endast är siffror. string inputInterval är ett frivilig argument och är typ bara för siffer koll, den är en intervall som gör att sifran endast får vara mellan lägsta till högsta sifran
        {
            if (string.IsNullOrEmpty(input) || input.Length > 20) // om inputen är tom är det invalid. Men om den är längre en 20 karaktärer är det också det
            {
                return false; // Om input är null eller tom, returnera false
            }
            bool corectType = false; // Är det endast siffror/bokstäver ?
            bool validInterval = false; // Är det inom intervallen ?
            bool checkValidInterval = true; // ska inetrval kollas ?.....
            if (inputInterval == "") //.... om den är tom, då nej
            {
                checkValidInterval = false;
                validInterval = true; // om det inte finns någon inervall att kolla, så vist, då är den inanför "intervallen"
            }

            if (onlyText) // Kolla om bara bokstäver
            {
                // Kontrollera om input endast innehåller bokstäver
                corectType = input.All(char.IsLetter);

                // För tillfälet tror jag att intervaller inte används för bokstäver då det kollas med en if satser direkt, men den kanske blir användbar i ett annat projekt
                if (checkValidInterval && corectType) // Om den är inanför inetervallen är irelevent om corectType = false, för då är det invalid input
                {
                    string[] validAlternativs = input.Split(':'); //Sakpar en "lista" med alla alternavit som är valid
                    foreach (string s in validAlternativs) 
                    {
                        if (input.ToUpper() == s) //Kollar om inputen machar några av de "valid" alternativen
                        {
                            validInterval = true;
                            break;
                        }
                    }
                    
                }

                
            }
            else
            {
                // Kontrollera om input endast innehåller siffror
                corectType = input.All(char.IsDigit);

                if (checkValidInterval && corectType)
                {
                    string[] validAlternativs = inputInterval.Split('-');
                    
                    if (int.Parse(input) >= int.Parse(validAlternativs[0]) && int.Parse(input) <= int.Parse(validAlternativs[1])) // Kollar om värdet ligger inom intervallen
                    {
                        validInterval = true;
                    }

                }

                
            }
            return corectType && validInterval;
        }


        public class PlayCard
        {
            string tunnelId; // Fyrsifrigt väre som beskriver tunlarna på kortet medurs börjande upifrån: 1 = vägg, och 2 = tunnel öppning

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


            public Player(string name) // Jag vill inte tildela Player en lista i kontruktorn när de skaps, men vill fortfrande att de ska kunan kommas åt.....
            {
                this.name = name;
                
                
            }
            

            public string Name 
            {
                get { return name; }
                //Eftersom namnet på en spelaer inte förändras under spelets gång behövs ingen set

            }



            public List<PlayCard> PlayerCards // .... gemom get set
            {
                get { return playerCards; }
                set { playerCards = value; }
            }




        }
    }
}
