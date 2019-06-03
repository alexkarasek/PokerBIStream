using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;

namespace testJson
    

{
    public class Games
    {
        public string gameid;
        public string timestamp;
        public List<Seats> seats = new List<Seats>();
        public List<Actions> actions = new List<Actions>();

        public void Addactions(Actions a)
        {
            actions.Add(a);
        }

        public void Clearactions()
        {
            actions.Clear();
        }
        public void Addseats(Seats s)
        {
            seats.Add(s);
        }

        public void Clearseats()
        {
            seats.Clear();
        }
    }

    public class Seats
    {
        public string playername;
        public int seatno;
        //public int startingchips;
        //public int isButton;
        //public int isBB;
        //public int isSB;
    }

    public class Actions
    {
        public int GameActionId;
        public string streetname;
        public string action;
        public string actor;
        public int amount;
    }
}
