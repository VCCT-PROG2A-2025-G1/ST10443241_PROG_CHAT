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
            Console.WriteLine($"Hello {userName}! I can help you with some questions you may have!");

            while (true)
            {
                string userInput = Console.ReadLine();

                string response = BotLines(userInput);
                Console.WriteLine(response);

                if (response == "Goodbye!")
                    break;
            }
        }

        public string BotLines(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Please enter words for conversation, if you really want to leave, just say goodbye!";
            }

            userInput = userInput.ToLower();

            if (userInput.Contains("hello") || userInput.Contains("hey") || userInput.Contains("hi"))
                return $"Hello Again {userName}! Hopefully, I can help you today?";
            else if (userInput.Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
                return "Goodbye!";
            else if (userInput.Contains("what") && userInput.Contains("purpose"))
                return "I talk about cybersecurity! I will hopefully be able to describe certain keywords. Please refer to the readme file to see what I'm able to respond to!";
            else if (userInput.Contains("what") && userInput.Contains("ask") && userInput.Contains("about"))
                return "I can answer things about cybersecurity, phishing, and safety while browsing the web! If you need a more concrete list, please refer to my readme file.";
            else if (userInput.Contains("what") && userInput.Contains("cybersecurity"))
                return "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.";
            else if (userInput.Contains("how") && userInput.Contains("cybersecurity"))
                return "Cybersecurity is achieved by implementing a combination of technologies, processes, and practices designed to protect networks, devices, programs, and data from attacks.";
            else if (userInput.Contains("what") && userInput.Contains("phishing"))
                return "Phishing is a type of cyber attack that uses disguised email as a weapon.";
            else if (userInput.Contains("how") && userInput.Contains("phishing"))
                return "Phishing is typically carried out by email spoofing or instant messaging, and it often directs users to enter personal information at a fake website.";
            else if (userInput.Contains("what") && userInput.Contains("firewall"))
                return "A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules.";
            else if (userInput.Contains("how") && userInput.Contains("firewall"))
                return "Firewalls establish a barrier between a trusted internal network and untrusted external networks, such as the internet.";
            else if (userInput.Contains("what") && userInput.Contains("vpn"))
                return "A VPN, or Virtual Private Network, creates a secure connection over the internet between your device and the VPN server.";
            else if (userInput.Contains("how") && userInput.Contains("vpn"))
                return "VPNs work by routing your device's internet connection through the VPN's private server instead of your internet service provider (ISP).";
            else
                return "I'm sorry, I don't have a great understanding currently. Please ask me something else, hopefully more specific to cybersecurity.";
        }
    }


}
