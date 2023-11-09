using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarBot1
{
    internal class User
    {
        public int money;
        public string? name;
        public int rocks = 0;
        public int soldiers = 0;
        public int rating = 0;
        public bool Kirka = false;
        static int id = 0;
        public int userId = 0;
        public bool isOnWar = false;
        public User() { }
        public User(string? s)
        {
            name = s;
            money = 1000;
            userId += id;
            id++;

        }
    }
}
