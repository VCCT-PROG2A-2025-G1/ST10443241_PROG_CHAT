using PROG6221ChatBot;
using System;
using System.Windows;
using Part3;

/*

https://youtu.be/zsjznbC-M5E New video on Part 2 Code Explanation
Refrences:
W3School - Basic help with all code and structure
OpenAI - logic and syntax errors, also the sound problems
StackOverflow(Programming reddit) - Any additionaly thing i need peoples input from
*/
namespace PROG6221ChatBot
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Show the WPF window first
            var mainWindow = new MainWindow();
            mainWindow.ShowDialog();

            Chatbot bot = new Chatbot(); // Create an instance of the Chatbot class
            bot.EntireChat(); // Start the chatbot interaction

        }
    }
}
