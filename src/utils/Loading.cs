using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerminHack
{
    public static partial class Utils
    {
        public static class Loading {
            public static void Start(int ms) {
            string[] animation = { "|", "/", "-", "\\" };
            int duration = ms / 100;
            int animationPos = 0;
            int posY = Console.GetCursorPosition().Top;
            Console.WriteLine();
            for(int i = 0; i <= 100; i++) {
                int percentage = i/2;
                string[] loadingbarArr = { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                string loadingbar = "";
                if (animationPos < 3) { animationPos++; } else { animationPos=0; }
                
                if (percentage > 0) {
                    if (i%2==0) {
                        loadingbarArr[0] = "\u001b[47m ";
                        loadingbarArr[percentage-1] += "\u001b[0m";
                    } else {
                        loadingbarArr[0] = "\u001b[47m ";
                        loadingbarArr[percentage-1] += "\u001b[0m";
                    }
                }
                foreach (string x in loadingbarArr) loadingbar += x;
                Console.SetCursorPosition(0, posY);
                Console.Write($"[{animation[animationPos]}] [{loadingbar}] {i}%");
                
                if (i==100) {
                    Console.SetCursorPosition(0, posY);
                    Console.Write($"[\u001b[32m✔\u001b[0m] [{loadingbar}] {i}%");
                    Thread.Sleep(1000);
                }

                Thread.Sleep(duration);
            }
            Console.WriteLine();
        }
        }
    }
}
