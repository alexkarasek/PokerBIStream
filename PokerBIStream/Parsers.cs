﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

namespace testJson
{
    class FileParser   
    {
        public FileParser(string filename_in)
        {
            Games g = new Games();
            string line_in = "";  //seed for first game in file
            string streetname = "Pre-Flop";  //seed for first game in file
            int gameActionCtr = 0; //seed action ctr (used for GameActionId for sorting later)

            using (StreamReader sr = new StreamReader(filename_in))

                while ((line_in = sr.ReadLine()) != null)
                {
                    GameParser gp = new GameParser(line_in, g, streetname, gameActionCtr);
                    streetname = gp.Streetname;  //persist streetname
                    gameActionCtr = gp.GameActionCtr;  //persist action ctr
                }
           
                {
                    //allows output after last hand

                    MemoryStream ms = new MemoryStream();
                    DataContractJsonSerializer j = new DataContractJsonSerializer(typeof(Games));
                    j.WriteObject(ms, g);

                    ms.Position = 0;
                    StreamReader str = new StreamReader(ms);
                    Console.Write("JSON form of Games object: ");
                    Console.WriteLine(str.ReadToEnd());
                }
            Console.ReadLine();
        }
    }

    class LineParser
    {
        public string GameId { get; set; }
        public int NewGame { get; set; }
        public int NewStreet { get; set; }
        public string TimeStamp { get; set; }
        public string Streetname { get; set; }
        public string Action { get; set; }
        public int Amount { get; set; }
        public string Player { get; set; }
        public int GameActionCtr { get; set; }
        public int Seatnumber { get; set; }

        public LineParser(string line_in, string streetname ,int gameactionctr)
        {
            //Define regex patters -> TODO: move this to config file
            List<string> patterns = new List<string>();
            patterns.Add(@"Dealt to\[.\D\s.\D\]");   //Deal
            patterns.Add(@"^PokerStars\s");  //New Game
            patterns.Add(@"\sbets\s");  //Bet
            patterns.Add(@"\sposts\s.+\sblind");  //Blind
            patterns.Add(@"\sfolds\s");   //Fold
            patterns.Add(@"\scalls\s");  //Call
            patterns.Add(@"\scollected\s[0-9]");  //Win
            patterns.Add(@"\schecks\s");  //Check
            patterns.Add(@"\sposts\sthe\sante");  //Ante
            patterns.Add(@"\*\*\*\s.+\s\*\*\*");  //New Street
            patterns.Add(@".+\:\sraises+.+[0-9]+\sto\s[0-9]+");  //Raise
            patterns.Add(@"Seat+\s\d\:\s");  //Seats

            Streetname = streetname;
            GameActionCtr = gameactionctr;

            for (int i = 0; i < patterns.Count; i++)
            {

                Regex rgx = new Regex(patterns[i], RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(line_in);
                if (matches.Count > 0)
                {
                    Regex rgxStage;// = new Regex(@"");
                    MatchCollection Stage; //= rgxStage.Matches(line_in);

                    switch (i)
                    {
                        //case 0:  //DEAL
                        //    if (line.IndexOf("SHOWS") == -1 && line.IndexOf("shows") == -1 && line.IndexOf(" showed ") == -1 && line.IndexOf(" mucked ") == -1)
                        //    {

                        //        cards = line.Substring(line.IndexOf("["), 7);
                        //        player = line.Substring(9, line.IndexOf("[") - 10);
                        //        Street_out = "PRE-FLOP";
                        //    }
                        //    break;

                        case 1:  //NEW GAME
                            GameId = line_in.Substring(17, line_in.IndexOf(":") - 17);
                            NewGame = 1;
                            rgxStage = new Regex(@"\d{4}/\d{2}/\d{2}.+");
                            Stage = rgxStage.Matches(line_in);
                            TimeStamp = Stage[0].Value;
                            break;

                        case 2:  //BET
                            rgxStage = new Regex(@"bets\s\d+");
                            Stage = rgxStage.Matches(line_in);
                            Amount = Convert.ToInt32(Stage[0].Value.Substring(5, Stage[0].Length - 5));
                            Action = "bet";
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 3:  //BLIND  
                            rgxStage = new Regex(@"\s\d+");
                            Stage = rgxStage.Matches(line_in);
                            Action = "blind";
                            Amount = Convert.ToInt32(Stage[0].Value.Substring(1, Stage[0].Length - 1));
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr ++;
                            break;

                        case 4:   //FOLD
                            Action = "fold";
                            Amount = 0;
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 5:  //CALL
                            rgxStage = new Regex(@"\s\d+");
                            Stage = rgxStage.Matches(line_in);
                            Action = "call";
                            Amount = Convert.ToInt32(Stage[0].Value.Substring(1, Stage[0].Length - 1));
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 6:  //WIN
                            rgxStage = new Regex(@"\d+\sfrom\s");
                            Stage = rgxStage.Matches(line_in);
                            Action = "win";
                            Amount = Convert.ToInt32(Stage[0].Value.Substring(0, Stage[0].Length - 6));
                            rgxStage = new Regex(@".+\scollect");
                            Player = line_in.Substring(0, line_in.Replace(": collect", " collect").IndexOf(" collect"));
                            gameactionctr++;
                            break;

                        case 7:  //CHECK
                            Action = "check";
                            Amount = 0;
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        //case 8:  //ANTE
                        //    rgxStage = new Regex(@"ante\s\d+");
                        //    Stage = rgxStage.Matches(line);
                        //    action = "ante";
                        //    amount = Stage[0].Value.Substring(5, Stage[0].Length - 5);
                        //    player = line.Substring(0, line.IndexOf(@":"));
                        //    break;

                        case 9:  //NEW STREET
                            if (line_in.Replace("HOLE CARDS", "Pre-flop").Substring(4, line_in.IndexOf("***", 4) - 5) != "SUMMARY"
                                        && line_in.Replace("HOLE CARDS", "Pre-flop").Substring(4, line_in.IndexOf("***", 4) - 5) != "SHOW DOWN")
                            { 
                            Streetname = line_in.Replace("HOLE CARDS", "Pre-flop").Substring(4, line_in.IndexOf("***", 4) - 5);
                            }
                            break;

                        case 10:  //RAISE
                            rgxStage = new Regex(@"\s\d+\s");
                            Stage = rgxStage.Matches(line_in);
                            Action = "raise";
                            Amount = Convert.ToInt32(Stage[0].Value.Substring(1, Stage[0].Length - 2));
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 11:  //SEATS
                            Action = "SeatInfo";
                            //rgxStage = new Regex(@"\:\s.+\s\(");
                            //Stage = rgxStage.Matches(line_in);

                            Seatnumber = Convert.ToInt32(line_in.Substring(5, 1));
                            Player = line_in.Substring(8, line_in.Length - 8 - (line_in.Length - line_in.IndexOf(" (")));
                            break;


                    }
                }
            }


                    
        }
    }

    class GameParser
    {
        public string Streetname { get; set; }  //allows street to be persisted
        public int GameActionCtr { get; set; } //allows action counter to be persisted
       
        public GameParser(string line_in, Games g, string streetname, int gameActionCtr) 
        {

            LineParser lp = new LineParser(line_in, streetname, gameActionCtr);
            Streetname = lp.Streetname;
            GameActionCtr = lp.GameActionCtr;
            

            if (lp.NewGame == 1)  //start of new game. print Games object from previous game
            {
                if (g.gameid != null)
                {
                    MemoryStream ms = new MemoryStream();
                    DataContractJsonSerializer j = new DataContractJsonSerializer(typeof(Games));
                    j.WriteObject(ms, g);

                    ms.Position = 0;
                    StreamReader str = new StreamReader(ms);
                    Console.Write("JSON form of Games object: ");
                    Console.WriteLine(str.ReadToEnd());

                    Console.ReadLine();
                }

                //clear action level vars
                g.actions.Clear();
                g.seats.Clear();
                Streetname = "Pre-flop";
                GameActionCtr = 0;

                //assign game level vars
                g.gameid = lp.GameId; 
                g.timestamp = lp.TimeStamp;
             }
            
            else if (lp.Streetname != "" && lp.Action != null)  //assign Actions values
            {
                if (lp.Action == "SeatInfo")
                {
                    g.Addseats(new Seats { playername = lp.Player, seatno = lp.Seatnumber });
                }
                else
                {
                    g.Addactions(new Actions { GameActionId = lp.GameActionCtr, actor = lp.Player, action = lp.Action, amount = lp.Amount, streetname = lp.Streetname });
                }
            }


            Console.WriteLine(line_in);

        }
        
    }
}
