using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarBot1
{
    internal static class Game
    {
        static Game() { }
        public static string name;
        public static string mapName;
        public static Dictionary<string, User> keyValuePairs = new Dictionary<string, User>();
        public static List<string> terretories = new List<string>();
        public static bool isStarted { get; set; }
        static public void StartGame(string Name, string map)
        {
            isStarted = true;
            name = Name;
            mapName = map;
        }
        public static void StopGame(string Name)
        {
            isStarted = false;
            name = null;
            mapName = null;
        }
        public static int Mine()
        {
            Random rnd = new Random();
            int i = rnd.Next(1, 10);
            return i;


        }
        public static int MinePlus()
        {
            Random rnd = new Random();
            int i = rnd.Next(10, 25);
            return i;


        }
    }

}
