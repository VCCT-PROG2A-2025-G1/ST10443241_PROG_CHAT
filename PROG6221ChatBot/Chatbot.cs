using System;
using System.IO;
using System.Media;
using System.Threading;

namespace PROG6221ChatBot
{


    namespace PROG6221ChatBot
    {
        class Chatbot
        {
            private readonly SoundPlayer _soundPlayer; // Handles playing sound files
            private string userName; // Stores the username of the user interacting with the chatbot

            public Chatbot()
            {
                // Initialize the sound player with the path to the sound file
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Best you can get out of me.wav");
                _soundPlayer = new SoundPlayer(soundPath);

                _soundPlayer.Load();
            }



            public void EntireChat()
            {
                Logo(); // Display the logo
                GreetUser(); // Greet the user
                MainConversation(); // Start the main conversation loop
            }



            public void Logo()
            {
                Console.WriteLine("\r\n      _  ____      ____  _       _______     ____  _____  ________   ______    ______    ______      ___    _________  \r\n     / \\|_  _|    |_  _|/ \\     |_   __ \\   |_   \\|_   _||_   __  |.' ____ \\ .' ____ \\  |_   _ \\   .'   `. |  _   _  | \r\n    / _ \\ \\ \\  /\\  / / / _ \\      | |__) |    |   \\ | |    | |_ \\_|| (___ \\_|| (___ \\_|   | |_) | /  .-.  \\|_/ | | \\_| \r\n   / ___ \\ \\ \\/  \\/ / / ___ \\     |  __ /     | |\\ \\| |    |  _| _  _.____`.  _.____`.    |  __'. | |   | |    | |     \r\n _/ /   \\ \\_\\  /\\  /_/ /   \\ \\_  _| |  \\ \\_  _| |_\\   |_  _| |__/ || \\____) || \\____) |  _| |__) |\\  `-'  /   _| |_    \r\n|____| |____|\\/  \\/|____| |____||____| |___||_____|\\____||________| \\______.' \\______.' |_______/  `.___.'   |_____|   \r\n                                                                                                                       \r\n");
                //Left the ASCII art as a text string rather than image, there was nothing in the rubric directly stating that the ASCII had to be an image
                Console.WriteLine("");
            }



            public void GreetUser()
            {
                Console.WriteLine("CSAB: Hello! May you please enter your username?");
                Console.WriteLine("");
                PlaySound(); // Play the opening sound

                while (true)
                {

                    userName = Console.ReadLine(); // Capture the username from the user
                    Console.WriteLine("");

                    if (string.IsNullOrWhiteSpace(userName))
                    {
                        Console.WriteLine("CSAB: I didn't catch that. Could you please enter a valid username?"); 
                    }

                    break;
                }
                Console.WriteLine($"CSAB: Hello {userName}! I can help you with some questions you may have!");
            }



            public void MainConversation()
            {
                while (true)
                {
                    Console.WriteLine();
                    Console.Write($"{userName}: ");
                    string userInput = Console.ReadLine(); // Capture user input
                    Console.WriteLine();

                    //CheckUserEmotions(userInput); // Check for user emotions
                    string response = BotResponse(userInput); // Generate a response based on user input
                    TypeWriter(response); // Display the response with a typewriter effect

                    // Exit the loop if the user says goodbye
                    if (userInput.Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
                    {
                        break;
                    }
                }
            }

            /*public void CheckUserEmotions(string userInput)
            {
            
            }
            */


            public void PlaySound()
            {
                try
                {
                    _soundPlayer.PlaySync(); // Play the sound synchronously
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during sound playback
                    Console.WriteLine($"Could not play sound: {ex.Message}");
                }
            }

            public string BotResponse(string userInput)
            {
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    // Prompt the user to enter valid input
                    return "CSAB: Please enter words for conversation, if you really want to leave, just say goodbye!";
                }

                userInput = userInput.ToLower(); // Normalize user input to lowercase for easier comparison

                // Respond to greetings
                if (userInput.Contains("hello") || userInput.Contains("hey") || userInput.Contains("hi"))
                    return $"CSAB: Hello Again {userName}! Hopefully, I can help you today?";

                // Respond to farewells
                else if (userInput.Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
                    return $"CSAB: Goodbye {userName}!";


                // Responses to what the bot does
                else if (userInput.Contains("what") && userInput.Contains("purpose"))
                    return "CSAB: I talk about cybersecurity! I will hopefully be able to describe certain keywords. Please refer to the readme file to see what I'm able to respond to!";
                else if (userInput.Contains("what") && userInput.Contains("ask") && userInput.Contains("about"))
                    return "CSAB: I can answer things about cybersecurity, phishing, and safety while browsing the web! If you need a more concrete list, please refer to my readme file.";


                //"what" questions are answered with a definition and "how" with a brief explination
                else if (userInput.Contains("what") && userInput.Contains("cybersecurity"))
                    return "CSAB: Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.";

                else if (userInput.Contains("how") && userInput.Contains("cybersecurity"))
                    return "CSAB: Cybersecurity is achieved by implementing a combination of technologies, processes, and practices designed to protect networks, devices, programs, and data from attacks.";

                else if (userInput.Contains("what") && userInput.Contains("phishing"))
                    return "CSAB: Phishing is a type of cyber attack that uses disguised email as a weapon.";

                else if (userInput.Contains("how") && userInput.Contains("phishing"))
                    return "CSAB: Phishing is typically carried out by email spoofing or instant messaging, and it often directs users to enter personal information at a fake website.";

                else if (userInput.Contains("what") && userInput.Contains("firewall"))
                    return "CSAB: A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules.";

                else if (userInput.Contains("how") && userInput.Contains("firewall"))
                    return "CSAB: Firewalls establish a barrier between a trusted internal network and untrusted external networks, such as the internet.";

                else if (userInput.Contains("what") && userInput.Contains("vpn"))
                    return "CSAB: A VPN, or Virtual Private Network, creates a secure connection over the internet between your device and the VPN server.";

                else if (userInput.Contains("how") && userInput.Contains("vpn"))
                    return "CSAB: VPNs work by routing your device's internet connection through the VPN's private server instead of your internet service provider (ISP).";

                else if (userInput.Contains("what") && userInput.Contains("password"))
                    return "CSAB: A password is a secret combination of characters used to authenticate a user's identity and grant access to systems or data.";

                else if (userInput.Contains("how") && userInput.Contains("password"))
                    return "CSAB: Passwords work by comparing user input against a stored value, often using cryptographic hashing for security.";

                else if (userInput.Contains("what") && userInput.Contains("scam"))
                    return "CSAB: A scam is a deceptive scheme designed to trick individuals into giving away money, personal information, or access to systems.";

                else if (userInput.Contains("how") && userInput.Contains("scam"))
                    return "CSAB: Scams typically work by exploiting human psychology, using urgency, fear, or greed to bypass rational judgment.";

                else if (userInput.Contains("what") && userInput.Contains("privacy"))
                    return "CSAB: Privacy refers to an individual's right to control how their personal information is collected, used, and shared.";

                else if (userInput.Contains("how") && userInput.Contains("privacy"))
                    return "CSAB: Privacy can be protected through technical measures (encryption), policy controls, and careful management of personal data sharing.";

                else if (userInput.Contains("what") && userInput.Contains("identity theft"))
                    return "CSAB: Identity theft occurs when someone uses another person's personal information without permission, typically for financial gain.";

                else if (userInput.Contains("how") && userInput.Contains("identity theft"))
                    return "CSAB: Identity theft works by criminals gathering personal data (like SSNs or credit card numbers) through phishing, hacking, or data breaches, then impersonating the victim.";


                // Default response for unrecognized input
                else
                    // Inform the user that the bot doesn't understand the input
                    return "CSAB: I'm sorry, I don't have a great understanding currently. Please ask me something else, hopefully more specific to cybersecurity.";
            }

            //typewriter effect for bot text
            public void TypeWriter(string text, int delay = 30)
            {
                foreach (char c in text)
                {
                    // Print output with a delay
                    Console.Write(c);
                    Thread.Sleep(delay);

                    //  skip the typewriter effect
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        Console.Write(text.Substring(text.IndexOf(c) + 1));
                        break;
                    }
                }
                // Move to the next line after the text is displayed
                Console.WriteLine();
            }
        }
    }
}
