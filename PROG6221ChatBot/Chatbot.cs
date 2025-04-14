using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG6221ChatBot
{
    class Chatbot
    {
            public void Opening()
            {
                Console.WriteLine("Hello! How can I help you today?");

                while (true)
                {
                    string userInput = Console.ReadLine();

                    string response = BotLines(userInput);
                    Console.WriteLine(response);

                    if (userInput.ToLower().Contains("bye"))
                        break;
                }
            }

            public string BotLines(string userInput)
            {
                userInput = userInput.ToLower();

            if (userInput == "hello" || userInput == "hey" || userInput == "hi")
                    return "Hello Again! Hopefully, I can I help you today?";
                else if (userInput == "bye" || userInput == "goodbye" || userInput == "bye bye")
                    return "Goodbye!";
                else if (userInput == "What's your purpose")
                return "The weather is sunny today!";
                else
                    return "I'm sorry, I don't understand. Please ask me something else.";
            }

    }


}
