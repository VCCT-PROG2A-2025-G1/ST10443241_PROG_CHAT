using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using PROG6221ChatBot.Properties;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace PROG6221ChatBot
{
    class Chatbot
    {
        string userName;

        public void Opening()
        {
            string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Test File.wav");

            using (var soundPlayer = new SoundPlayer(soundPath))
            {
                soundPlayer.Play();
            }

            Console.WriteLine("\r\n   ______  __              _   ______          _________  \r\n .' ___  |[  |            / |_|_   _ \\        |  _   _  | \r\n/ .'   \\_| | |--.   ,--. `| |-' | |_) |   .--.|_/ | | \\_| \r\n| |        | .-. | `'_\\ : | |   |  __'. / .'`\\ \\  | |     \r\n\\ `.___.'\\ | | | | // | |,| |, _| |__) || \\__. | _| |_    \r\n `.____ .'[___]|__]\\'-;__/\\__/|_______/  '.__.' |_____|   \r\n                                                          \r\n");
            Console.WriteLine("");
            Console.WriteLine("Chatbot: Hello! May you please enter your UserName?");
            Console.WriteLine("");
            userName = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine($"Chatbot: Hello {userName}! I can help you with some questions you may have!");

            while (true)
            {
                Console.WriteLine();
                Console.Write($"{userName}: ");
                string userInput = Console.ReadLine();
                Console.WriteLine();

                string response = BotLines(userInput);
                TypeWriter(response);

                if (response == $"Chatbot: Goodbye {userName}!")
                {
                    break;
                }
            }
        }

        public string BotLines(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Chatbot: Please enter words for conversation, if you really want to leave, just say goodbye!";
            }

            userInput = userInput.ToLower();

            if (userInput.Contains("hello") || userInput.Contains("hey") || userInput.Contains("hi"))
                return $"Chatbot: Hello Again {userName}! Hopefully, I can help you today?";
            else if (userInput.Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
                return "Chatbot: Goodbye!";
            else if (userInput.Contains("what") && userInput.Contains("purpose"))
                return "Chatbot: I talk about cybersecurity! I will hopefully be able to describe certain keywords. Please refer to the readme file to see what I'm able to respond to!";
            else if (userInput.Contains("what") && userInput.Contains("ask") && userInput.Contains("about"))
                return "Chatbot: I can answer things about cybersecurity, phishing, and safety while browsing the web! If you need a more concrete list, please refer to my readme file.";
            else if (userInput.Contains("what") && userInput.Contains("cybersecurity"))
                return "Chatbot: Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.";
            else if (userInput.Contains("how") && userInput.Contains("cybersecurity"))
                return "Chatbot: Cybersecurity is achieved by implementing a combination of technologies, processes, and practices designed to protect networks, devices, programs, and data from attacks.";
            else if (userInput.Contains("what") && userInput.Contains("phishing"))
                return "Chatbot: Phishing is a type of cyber attack that uses disguised email as a weapon.";
            else if (userInput.Contains("how") && userInput.Contains("phishing"))
                return "Chatbot: Phishing is typically carried out by email spoofing or instant messaging, and it often directs users to enter personal information at a fake website.";
            else if (userInput.Contains("what") && userInput.Contains("firewall"))
                return "Chatbot: A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules.";
            else if (userInput.Contains("how") && userInput.Contains("firewall"))
                return "Chatbot: Firewalls establish a barrier between a trusted internal network and untrusted external networks, such as the internet.";
            else if (userInput.Contains("what") && userInput.Contains("vpn"))
                return "Chatbot: A VPN, or Virtual Private Network, creates a secure connection over the internet between your device and the VPN server.";
            else if (userInput.Contains("how") && userInput.Contains("vpn"))
                return "Chatbot: VPNs work by routing your device's internet connection through the VPN's private server instead of your internet service provider (ISP).";
            else
                return "Chatbot: I'm sorry, I don't have a great understanding currently. Please ask me something else, hopefully more specific to cybersecurity.";
        }


        private void TypeWriter(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);

                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true); // Clear the key
                    Console.Write(text.Substring(text.IndexOf(c) + 1)); // Print rest instantly
                    break;
                }
            }
            Console.WriteLine();
        }

    }

}
