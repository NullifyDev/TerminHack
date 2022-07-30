using System.Threading;
using System;
using System.Net.Mime;
using Microsoft.VisualBasic.CompilerServices;
using TerminHack;
using static TerminHack.Utils;
namespace TerminHack {
    public class Program {

        string currDir = "/home/Ramdahl";
        string username = "Ramdahl";
        string devicename = "Ramdahl";
        string permission = "$";

        static void Main(string[] args) {
            Program app = new Program();
            Console.Clear();
            Utils.Terminal.Output("Launching RamdahlOS...", 1);

            Console.Clear();
            Utils.Terminal.Output("Welcome to RamdahlOS v1.0", 2);
            string userprompt = $"{app.username}@{app.devicename} {app.currDir}{app.permission} ";

            while(true) {
                Console.Write(userprompt);
                string[] input = Console.ReadLine().ToLower().Split(" ");
                var arguments = input.Length > 1 ? input[1..^0] : Array.Empty<string>();
                CommandHandler.HandleCommand(input[0].ToLower(), arguments);
            }
        }

        public static void Shutdown(Int32 ms) {
            Utils.Terminal.Output("Shuttng Down...", 1);
            Thread.Sleep(ms);
            Environment.Exit(0);
        }
    }
}