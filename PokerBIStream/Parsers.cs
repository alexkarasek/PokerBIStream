using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using PokerBIStream;
using System.Dynamic;

namespace testJson
{
    class FileParser
    {

        public FileParser(string filename_in, string url, string key, string archivepath)
        {
            Games g = new Games();
            List<Bets> bets = new List<Bets>();

            string line_in = "";  //seed for first game in file
            string streetname = "Pre-Flop";  //seed for first game in file
            int gameActionCtr = 0; //seed action ctr (used for GameActionId for sorting later)

            using (StreamReader sr = new StreamReader(filename_in))

            while ((line_in = sr.ReadLine()) != null)
            {
                GameParser gp = new GameParser(line_in, g, streetname, gameActionCtr, url, key, archivepath, bets); //, lines);
                streetname = gp.Streetname;  //persist streetname
                gameActionCtr = gp.GameActionCtr;  //persist action ctr  

                using (StreamWriter sw = new StreamWriter(archivepath + "Verbose\\" + g.gameid + ".txt", true))
                {
                    sw.WriteLine(line_in);
                }
            }
           
            {
                ////CosmosDb cosmosDb = new CosmosDb(url, key, g);
                //// cosmosDb.GetStartedDemo(url, key, g).Wait();
                using (StreamWriter sw2 = new StreamWriter(archivepath + "JSON\\" + g.gameid + ".json"))
                {
                    sw2.WriteLine(g);
                }
            }

        }
    }

    class LineParser
    {
        public string GameId { get; set; }
        public int NewGame { get; set; }
        public int NewStreet { get; set; }
        public string TimeStamp { get; set; }
        public string SiteName { get; set; }
        public string TableName { get; set; }
        public string Limits { get; set; }
        public string Streetname { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
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
            //patterns.Add(@"\scollected\s[0-9]");  //Win
            patterns.Add(@"\scollected\s\$[0-9]");  //Win
            patterns.Add(@"\schecks\s");  //Check
            patterns.Add(@"\sposts\sthe\sante");  //Ante
            patterns.Add(@"\*\*\*\s.+\s\*\*\*");  //New Street
            //patterns.Add(@".+\:\sraises+.+[0-9]+\sto\s[0-9]+");  //Raise
            patterns.Add(@".+\:\sraises+.+[0-9]+\sto\s\$[0-9]+");  //Raise
            patterns.Add(@"Seat+\s\d\:\s.+\sin\schips\)");  //Seats
            patterns.Add(@"Uncalled bet"); //Returned Bet
            patterns.Add(@"\sRake\s\$"); //Rake
            patterns.Add(@"Table\s\'.+\'"); //Table Name

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
                            SiteName = line_in.Substring(0, line_in.IndexOf(" "));
                            rgxStage = new Regex(@"\$.+\)");
                            Stage = rgxStage.Matches(line_in);
                            Limits = Stage[0].Value.Substring(0, Stage[0].Value.Length - 1);
                            break;

                        case 2:  //BET
                            //rgxStage = new Regex(@"bets\s\d+");
                            rgxStage = new Regex(@"bets\s\$.+");
                            Stage = rgxStage.Matches(line_in.Replace(" and is all-in", ""));
                            //Amount = Convert.ToInt32(Stage[0].Value.Substring(5, Stage[0].Length - 5));
                            Amount = decimal.Parse(Stage[0].Value.Substring(6, Stage[0].Length - 6)) * -1;
                            Action = "bet";
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 3:  //BLIND  
                            //rgxStage = new Regex(@"\s\d+");
                            rgxStage = new Regex(@"\s\$.+");
                            Stage = rgxStage.Matches(line_in.Replace(" and is all-in", ""));
                            Action = "blind";
                            //Amount = Convert.ToInt32(Stage[0].Value.Substring(1, Stage[0].Length - 1));
                            Amount = decimal.Parse(Stage[0].Value.Substring(2, Stage[0].Length - 2)) * -1;
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
                            //rgxStage = new Regex(@"\s\d+");
                            rgxStage = new Regex(@"\s\$.+");
                            Stage = rgxStage.Matches(line_in.Replace(" and is all-in", ""));
                            Action = "call";
                            //Amount = Convert.ToInt32(Stage[0].Value.Substring(1, Stage[0].Length - 1));
                            Amount = decimal.Parse(Stage[0].Value.Substring(2, Stage[0].Length - 2)) * -1;
                            Player = line_in.Substring(0, line_in.IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 6:  //WIN
                            //rgxStage = new Regex(@"\d+\sfrom\s");
                            rgxStage = new Regex(@"\$.+\sfrom\s");
                            Stage = rgxStage.Matches(line_in);
                            Action = "win";
                            //Amount = Convert.ToInt32(Stage[0].Value.Substring(0, Stage[0].Length - 6));
                            Amount = decimal.Parse(Stage[0].Value.Substring(1, Stage[0].Length - 6));
                            rgxStage = new Regex(@".+\scollect");
                            Player = line_in.Substring(0, line_in.Replace(": collect", " collect").IndexOf(" collect"));
                            GameActionCtr++;
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
                            Streetname = line_in.Replace("HOLE CARDS", "Pre-flop").Substring(4, line_in.Replace("HOLE CARDS", "Pre-flop").IndexOf("***", 4) - 5);
                            }
                            break;

                        case 10:  //RAISE
                            //rgxStage = new Regex(@"\s\d+\s");
                            rgxStage = new Regex(@"\s\$.+\sto");
                            Stage = rgxStage.Matches(line_in.Replace(" and is all-in", ""));
                            Action = "raise";
                            //Amount = Convert.ToInt32(Stage[0].Value.Substring(1, Stage[0].Length - 2));
                            //Amount = decimal.Parse(Stage[0].Value.Substring(2, Stage[0].Length - 5));
                            Amount = decimal.Parse(line_in.Substring(line_in.Replace(" and is all-in", "").IndexOf("to $") + 4, line_in.Replace(" and is all-in", "").Length - line_in.Replace(" and is all-in", "").IndexOf("to $") - 4)) * -1;
                            Player = line_in.Substring(0, line_in.Replace(" and is all-in", "").IndexOf(":"));
                            GameActionCtr++;
                            break;

                        case 11:  //SEATS
                            Action = "SeatInfo";
                            //rgxStage = new Regex(@"\:\s.+\s\(");
                            //Stage = rgxStage.Matches(line_in);

                            Seatnumber = Convert.ToInt32(line_in.Substring(5, 1));
                            Player = line_in.Substring(8, line_in.Length - 8 - (line_in.Length - line_in.IndexOf(" (")));
                            break;

                        case 12:  //Return
                            //rgxStage = new Regex(@"\d+\sfrom\s");
                            rgxStage = new Regex(@"\$\d.+\)");
                            rgxStage =  rgxStage.Matches(line_in).Count == 0 ? new Regex(@"\$\d\)") : rgxStage;
                            Stage = rgxStage.Matches(line_in);
                            Action = "bet_returned";
                            //Amount = Convert.ToInt32(Stage[0].Value.Substring(0, Stage[0].Length - 6));
                            Amount = decimal.Parse(Stage[0].Value.Substring(1,Stage[0].Length - 2));
                            rgxStage = new Regex(@"returned to\s.+");
                            Stage = rgxStage.Matches(line_in);
                            Player = Stage[0].Value.Replace("returned to ","");
                            GameActionCtr++;
                            break;

                        case 13:  //RAKE
                            Action = "rake";
                            int chk1 = line_in.IndexOf("Rake") + 6;
                            int chk2 = line_in.Length - line_in.IndexOf("Rake") - 7;
                            string tst = line_in.Substring(chk1, chk2);
                            Amount = decimal.Parse(line_in.Substring(line_in.IndexOf("Rake") + 6, line_in.Length - line_in.IndexOf("Rake") - 7));
                            Player = "house";
                            GameActionCtr++;
                            break;

                        case 14: //TableName
                            rgxStage = new Regex(@"\'.+\'");
                            Stage = rgxStage.Matches(line_in);
                            TableName = Stage[0].Value.Substring(1, Stage[0].Value.Length - 2);
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
        
        public GameParser(string line_in, Games g, string streetname, int gameActionCtr, string url, string key, string archivepath, List<Bets> bets) 
        {

            LineParser lp = new LineParser(line_in, streetname, gameActionCtr);
            if (streetname != lp.Streetname)
            {

                bets.Select(u => { u.betsum = 0; return u; }).ToList();
            }
            Streetname = lp.Streetname;
            int actionFlag = GameActionCtr == lp.GameActionCtr ? 0 : 1;
            GameActionCtr = lp.GameActionCtr;
            Bets b = new Bets();

            if (lp.NewGame == 1)  //start of new game. print Games object from previous game
            {

                if (g.gameid != null)
                {
                    ////CosmosDb cosmosDb = new CosmosDb(url, key, g);
                    ////cosmosDb.GetStartedDemo(url, key, g).Wait();
                    using (StreamWriter sw2 = new StreamWriter(archivepath + "JSON\\" + g.gameid + ".json"))
                    {
                        sw2.WriteLine(g);
                    }

                    //MemoryStream ms = new MemoryStream();
                    //DataContractJsonSerializer j = new DataContractJsonSerializer(typeof(Games));
                    //j.WriteObject(ms, g);

                    //ms.Position = 0;
                    //StreamReader str = new StreamReader(ms);
                    //Console.Write("JSON form of Games object: ");
                    //Console.WriteLine(str.ReadToEnd());

                }

                //clear action level vars
                g.actions.Clear();
                g.seats.Clear();
                bets.Clear();

                Streetname = "Pre-flop";
                GameActionCtr = 0;

                //assign game level vars
                g.gameid = lp.GameId;
                g.timestamp = lp.TimeStamp;
                g.limits = lp.Limits;
                g.sitename = lp.SiteName;

                //prep output file
                using (StreamWriter sw3 = new StreamWriter(archivepath + "Text\\" + g.gameid + ".txt"))
                {
                    sw3.WriteLine("GameId" + "|" + "Timestamp" + "|" + "SiteName" + "|" + "TableName" + "|" + "Limits" + "|" + "StreetName" + "|" + "GameActionId" + "|" + "Action" + "|" + "Player" + "|" + "Amount");//);

                }

                ////delete file if it already exists for this game
                //using (StreamWriter sw = new StreamWriter(archivepath + "Verbose\\" + g.gameid + ".txt"))
                //{
                //    sw.WriteLine(line_in);
                //}
            }
            else if (lp.TableName != null)
            {
                g.tablename = lp.TableName;
            }
            else if (lp.Streetname != "" && lp.Action != null)  //assign Actions values
            {
                if (lp.Action == "SeatInfo")
                {
                    g.Addseats(new Seats { playername = lp.Player, seatno = lp.Seatnumber });
                    b.playername = lp.Player;
                    b.betsum = 0;
                    bets.Add(b);
                }
                else
                {
                    if (actionFlag == 1)
                    {

                        //write action to output file
                        using (StreamWriter sw3 = new StreamWriter(archivepath + "Text\\" + g.gameid + ".txt", true))
                        {

                            if (lp.Action == "bet" || lp.Action == "blind" || lp.Action == "call" || lp.Action == "raise")
                            {

                                if (lp.Action == "raise")
                                {
                                    decimal raiseamt = lp.Amount - bets.Where(p => p.playername == lp.Player).First().betsum;
                                    bets.Where(p => p.playername == lp.Player).Select(u => { u.betsum = (u.betsum + raiseamt); return u; }).ToList();
                                    lp.Amount = raiseamt;
                                }
                                else
                                {
                                    bets.Where(p => p.playername == lp.Player).Select(u => { u.betsum = (u.betsum + lp.Amount); return u; }).ToList();
                                }

                               
                            }

                            sw3.WriteLine(g.gameid.ToString() + "|" + g.timestamp.ToString() + "|" + g.sitename + "|" + g.tablename + "|" + g.limits + "|" + lp.Streetname + "|" + lp.GameActionCtr.ToString() + "|" + lp.Action + "|" + lp.Player + "|" + lp.Amount.ToString());//);

                        }
                    }

                    g.Addactions(new Actions { GameActionId = lp.GameActionCtr, actor = lp.Player, action = lp.Action, amount = lp.Amount, streetname = lp.Streetname });
                }
            }

            Console.WriteLine(line_in);

        }

    }
    public class Lines
    {
        public List<string> L { get; set; }  //aggregate lines for each game to write later

        public Lines()
        {
            L = new List<string>();
        }

        public void AddLines(string linetoadd)
        {
            L.Add(linetoadd);
        }
        public void ClearLines ()
        {
            L.Clear();
        }

    }


}

