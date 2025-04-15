using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG6221ChatBot
{
    class Chatbot
    {
        string userName;
            public void Opening()
            {
                Console.WriteLine("Hello! May you please enter your UserName?");
                userName = Console.ReadLine();
                Console.WriteLine($"Hello {userName}! I can help you with some questions may have!");

            while (true)
                {
                    string userInput = Console.ReadLine();
                    string response = BotLines(userInput);
                    Console.WriteLine(response);

                    if (userInput.Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
                        break;
                }
            }

            public string BotLines(string userInput)
            {
                userInput = userInput.ToLower();

            if (userInput.Contains("hello") || userInput.Contains("hey") || userInput.Contains("hi"))
                return $"Hello Again {userName}! Hopefully, I can help you today?";
                else if (userInput. Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
                    return "Goodbye!";
                else if (userInput.Contains("what") & userInput.Contains("purpose"))
                return "I talk about cybersecurity! I will hopefuly be able to discribe certain keywords. Please refer to the readme file to see what im able to respond to!";
                else if (userInput.Contains("what") & userInput.Contains("ask") & userInput.Contains("about"))
                return "I can answer things about cybersecurity, phising and safety while browsing the web! If you need a more concrete list, please refer to my readme file.";

            else if (userInput.Contains("what") & userInput.Contains("cybersecurity"))
                return "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.";
            else if (userInput.Contains("what") & userInput.Contains("phishing"))
                return "Phishing is a type of cyber attack that uses disguised email as a weapon.";
            else if (userInput.Contains("what") & userInput.Contains("firewall"))
                return "A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules.";
            else if (userInput.Contains("what") & userInput.Contains("vpn"))
                return "A VPN, or Virtual Private Network, creates a secure connection over the internet between your device and the VPN server.";
            else if (userInput.Contains("what") & userInput.Contains("encryption"))
                return "Encryption is the process of converting information or data into a code to prevent unauthorized access.";
            else if (userInput.Contains("what") & userInput.Contains("password manager"))
                return "A password manager is a software application designed to store and manage your passwords and other credentials securely.";



            else
                    return "I'm sorry, I don't have great understand currently. Please ask me something else, hopefully more specific to cybersecurity.";
            }

    }


}
