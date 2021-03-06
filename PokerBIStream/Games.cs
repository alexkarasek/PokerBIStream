﻿using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace testJson


{
    public class Games
    {
        [JsonProperty(PropertyName = "id")]
        public string gameid;
        public string timestamp;
        public string sitename;
        public string tablename;
        public string limits;
        public int buttonseat;
        public List<Seats> seats = new List<Seats>();
        public List<Actions> actions = new List<Actions>();
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

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
        public string position;
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
        public decimal amount;
    }

    public class Bets
    {
        public string playername { get; set; }
        public decimal betsum { get; set; }
    }

    public class ga_payload
    {
        //[JsonProperty(PropertyName = "id")]
        public string gameid;
        public string timestamp;
        public string sitename;
        public string tablename;
        public string limits;
        public string streetname;
        public int gameactionid;
        public string action;
        public string player;
        public decimal amount;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

  