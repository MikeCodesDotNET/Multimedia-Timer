using System;
using MultimediaTimer;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var timer = new Timer(5);
            timer.Elapsed += Timer_Tick;
            timer.Resolution = TimeSpan.FromMilliseconds(2);
            timer.Start();

            while (true)
            {
                string line = Console.ReadLine(); 
                if (line == "exit") 
                {
                    timer.Stop();                   
                    break;
                }  
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"Timer Ticked: {DateTime.Now.ToString("hh:mm:ss.fff")}");
        }
    }
}
