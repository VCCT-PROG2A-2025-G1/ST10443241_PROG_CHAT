using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Media;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

/*video Link:
https://youtu.be/1naiY_njy6o
*/
namespace Part3
{            
//------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public partial class MainWindow : Window
    {
        // Application state
        public List<string> activityLog = new List<string>();
        public const int MaxLogEntries = 10;
        public List<UserTask> tasks = new List<UserTask>();
        public int nextId = 1;
        public int quizIndex = 0;
        public int score = 0;
        public bool isInQuizMode = false;
        private int _logPage = 0;
        private bool _showingActivityLog = false;

        // Chatbot-related fields
        private SoundPlayer _soundPlayer;
        private string userName;
        private string userInterest;
        private string userTopic;
        private bool isFirstInteraction = true;

        // Quiz data
        public readonly string[] quizQuestions =
        {
            "1. What is the BEST way to protect against phishing?",
            "2. What does a VPN primarily protect?",
            "3. Which is the STRONGEST password?",
            "4. What is a firewall used for?",
            "5. How can you avoid identity theft?",
            "6. What is a common sign of a scam email?",
            "7. Why should you enable two-factor authentication (2FA)?",
            "8. What is 'shoulder surfing'?",
            "9. What should you do if you fall for a scam?",
            "10. What does HTTPS ensure?",
            "11. Using public Wi-Fi without VPN is always safe. (True/False)",
            "12. Password managers are less secure than remembering passwords yourself. (True/False)"
        };

        public readonly string[][] quizOptions =
        {
            new[] {"A) Clicking links in emails to verify them", "B) Never opening emails from unknown senders", "C) Hovering over links to check URLs before clicking", "D) Using the same password everywhere"},
            new[] {"A) Your computer from viruses", "B) Your home Wi-Fi speed", "C) Your internet privacy and data encryption", "D) Your email storage space"},
            new[] {"A) CorrectHorseBatteryStaple", "B) Password123!", "C) 12345678", "D) admin"},
            new[] {"A) Speeding up internet connections", "B) Storing passwords securely", "C) Blocking spam emails", "D) Monitoring and blocking suspicious network traffic"},
            new[] {"A) Sharing your Social Security Number online", "B) Freezing your credit and monitoring accounts", "C) Using public Wi-Fi for banking", "D) Reusing passwords for convenience"},
            new[] {"A) Professional grammar and spelling", "B) A trusted company logo", "C) Urgent demands (e.g., 'Act now!')", "D) A legitimate-looking sender address"},
            new[] {"A) It adds an extra layer of security beyond passwords", "B) It makes logging in faster", "C) It reduces the need for strong passwords", "D) It prevents all hacking attempts"},
            new[] {"A) A type of hacking using Wi-Fi signals", "B) A method to speed up internet browsing", "C) A way to recover lost passwords", "D) Spying on someone as they enter sensitive info"},
            new[] {"A) Ignore it and hope for the best", "B) Report it and change compromised passwords", "C) Delete all evidence", "D) Share the scam link with friends"},
            new[] {"A) Faster website loading", "B) Better website design", "C) Encrypted data transmission", "D) Free access to paid content"},
            new[] {"True", "False"},
            new[] {"True", "False"}
        };

        public readonly int[] correctAnswers = { 3, 3, 1, 4, 2, 3, 1, 4, 2, 3, 2, 2 };
        public readonly string[] quizExplanations =
        {
            "Hovering over links lets you see the actual URL before clicking, helping you spot fraudulent links.",
            "VPNs encrypt your internet traffic, protecting your privacy and data from eavesdroppers.",
            "Long, random passphrases are more secure than short, complex passwords or common words.",
            "Firewalls monitor and control network traffic based on security rules.",
            "Credit freezes and account monitoring help prevent and detect identity theft early.",
            "Scammers often create urgency to bypass your rational thinking.",
            "2FA requires a second verification step even if your password is compromised.",
            "Shoulder surfing is when someone watches you enter sensitive information.",
            "Reporting scams helps authorities track scammers and prevents further damage.",
            "HTTPS encrypts data between your browser and the website.",
            "Public Wi-Fi is often unsecured, making VPN essential for protection.",
            "Password managers generate and store strong, unique passwords securely."
        };

        // Task creation
        public bool isCreatingTask = false;
        public UserTask pendingTask = new UserTask();
        private bool _inTaskCompletionMode = false;
        private bool _inTaskDeletionMode = false;

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // Initialize sound player
            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "notification.wav");//purposely not added to add error to log, was only required in part 1 anyways
                _soundPlayer = new SoundPlayer(soundPath);
                _soundPlayer.Load();
                _soundPlayer.Play();
            }
            catch (Exception ex)
            {
                Log($"Could not initialize sound: {ex.Message}");
            }

            Log("Program started");
            PrintLogo();
            PrintWelcomeMessage();
        }

        private void PrintWelcomeMessage()
        {
            PrintCSAB("Welcome to CyberChat! You can:");
            PrintCSAB("- Say things like 'Remind me to update my password tomorrow at 3pm'");
            PrintCSAB("- Type 'start quiz' to begin the cybersecurity quiz");
            PrintCSAB("- Type 'show tasks' to view your current tasks");
            PrintCSAB("- Type 'activity log' to see recent activities");
            PrintCSAB("- Type 'more' when viewing logs to see additional entries");
            PrintCSAB("Please enter your username to begin:");
        }

        private void PrintLogo()
        {
            PrintCSAB(@"
                          _  ____      ____  _       _______     ____  _____  ________   ______    ______    ______      ___    _________  
                         / \|_  _|    |_  _|/ \     |_   __ \   |_   \|_   _||_   __  |.' ____ \ .' ____ \  |_   _ \   .'   `. |  _   _  | 
                        / _ \ \ \  /\  / / / _ \      | |__) |    |   \ | |    | |_ \_|| (___ \_|| (___ \_|   | |_) | /  .-.  \|_/ | | \_| 
                       / ___ \ \ \/  \/ / / ___ \     |  __ /     | |\ \| |    |  _| _  _.____`.  _.____`.    |  __'. | |   | |    | |     
                     _/ /   \ \_\  /\  /_/ /   \ \_  _| |  \ \_  _| |_\   |_  _| |__/ || \____) || \____) |  _| |__) |\  `-'  /   _| |_    
                    |____| |____|\/  \/|____| |____||____| |___||_____|\____||________| \______.' \______.' |_______/  `.___.'   |_____|   
                                                                                                                       
                    ");
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Input processing
        public void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
            }
        }

        public void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        public void ProcessInput()
        {
            string userInput = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            PrintUser(userInput);
            InputBox.Clear();

            if (_showingActivityLog && userInput.Equals("more", StringComparison.OrdinalIgnoreCase))
            {
                ShowActivityLog(showMore: true);
                return;
            }
            _showingActivityLog = false;

            if (isFirstInteraction)//only does this first time in main window
            {
                userName = userInput;
                isFirstInteraction = false;
                PrintCSAB($"Hello {userName}! I can help you with cybersecurity questions, tasks, and quizzes.");
                PrintCSAB("You can ask me about things like phishing, passwords, VPNs, or type 'start quiz' for a cybersecurity quiz.");
                return;
            }

            if (isInQuizMode)//insures that the input doesnt affect the other modes
            {
                QuizAnswer(userInput);
                return;
            }

            if (_inTaskCompletionMode)//insures that the input doesnt affect the other modes
            {
                if (userInput.Equals("back", StringComparison.OrdinalIgnoreCase))
                {
                    _inTaskCompletionMode = false;
                    PrintCSAB("Returned to main menu.");
                    return;
                }
                HandleTaskCompletion(userInput);
                return;
            }

            if (_inTaskDeletionMode)//insures that the input doesnt affect the other modes
            {
                if (userInput.Equals("back", StringComparison.OrdinalIgnoreCase))
                {
                    _inTaskDeletionMode = false;
                    PrintCSAB("Returned to main menu.");
                    return;
                }
                HandleTaskDeletion(userInput);
                return;
            }

            //normalize inputs
            string input = userInput.ToLower();

            if (input == "start quiz" || input == "quiz" || input == "start game")//has to be exact
            {
                StartQuiz();
            }
            else if (ContainsAny(input, "log", "activity", "history", "what did i do"))
            {
                ShowActivityLog();
            }
            else if (input == "exit" || input == "quit" || input == "leave" || input == "bye" || input == "goodbye")
            {
                Close();
            }
            else if (!TryProcessNaturalLanguage(input))
            {
                ProcessChatbotInput(input);
            }
        }
   

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------


        public bool TryProcessNaturalLanguage(string input)
        {
            if (isCreatingTask)
            {
                TaskContinue(input);
                return true;
            }

            if (DetectTaskCreation(input))
            {
                TaskStart();
                return true;
            }

            if ((ContainsAny(input, "show", "list", "view", "check", "display", "what are my")) &&
                (ContainsAny(input, "task", "reminder", "todos", "to-do", "list")))//contains any two words
            {
                Log("Viewed task list");
                ListTasks();
                return true;
            }

            if ((ContainsAny(input, "complete", "finish", "done", "mark") &&
                 ContainsAny(input, "task", "reminder")) ||
                input.Contains("i've done") || input.Contains("ive done") || input.Contains("i finished") || input.Contains("i did")) 
            {
                TaskCompleted(input);
                return true;
            }

            if ((ContainsAny(input, "delete", "remove", "clear", "cancel") &&
                 ContainsAny(input, "task", "reminder")))
            {
                TaskDeleted(input);
                return true;
            }

            return false;
        }

        private bool DetectTaskCreation(string input)
        {
            var patterns = new[]
            {
                "remind me to",
                "add task",
                "create task",
                "new task",
                "set reminder",
                "i need to",
                "don't forget to",
                "add reminder",
                "create reminder"
            };

            return patterns.Any(p => input.Contains(p)) ||
                   (ContainsAny(input, "remind", "add", "create", "new", "set") &&
                    ContainsAny(input, "task", "reminder", "todo", "note"));
        }//if any pattern or word is detected, task creation is started

        public void TaskStart()
        {
            isCreatingTask = true;
            pendingTask = new UserTask
            {
                Id = nextId++,//unique ID for each task
                IsCompleted = false
            };

            PrintCSAB("Sure! What's the title of the task?");
        }

        public void TaskContinue(string input)
        {
            if (!isCreatingTask) return;

            if (string.IsNullOrWhiteSpace(pendingTask.Title))
            {
                pendingTask.Title = input.Trim();
                PrintCSAB("Got it. Any description you'd like to add? (or 'cancel' to abort)");
                return;
            }

            if (string.IsNullOrWhiteSpace(pendingTask.Description))
            {
                if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                {
                    isCreatingTask = false;
                    PrintCSAB("Task creation cancelled.");
                    return;
                }
                pendingTask.Description = input.Trim();
                PrintCSAB("When would you like to be reminded? (e.g., 'today at 3pm', 'tomorrow 9am', 'next week 2:30pm' or 'cancel')");
                return;
            }

            if (pendingTask.ReminderDate == null)
            {
                if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                {
                    isCreatingTask = false;
                    PrintCSAB("Task creation cancelled.");
                    return;
                }

                var (date, time) = ParseDateTime(input);
                if (date == null)
                {
                    PrintCSAB("Sorry, I didn't understand the date/time. Please try something like 'tomorrow at 3pm' or 'today 14:30' (or 'cancel')");
                    return;
                }

                pendingTask.ReminderDate = date;
                pendingTask.ReminderTime = time;
                tasks.Add(pendingTask);
                Log($"Added task: {pendingTask.Title} (Reminder: {pendingTask.ReminderDate.Value:yyyy-MM-dd} at {pendingTask.ReminderTime})");
                PrintCSAB($"All set! I'll remind you to \"{pendingTask.Title}\" on {pendingTask.ReminderDate:yyyy-MM-dd} at {pendingTask.ReminderTime:h\\:mm tt}.");//displays all added info
                isCreatingTask = false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private (DateTime? date, TimeSpan? time) ParseDateTime(string input)//extract the proper dates and times from the input
        {
            input = input.ToLower();
            DateTime? date = null;
            TimeSpan? time = null;

            // Try to parse time first
            if (TryParseTime(input, out var parsedTime))
            {
                time = parsedTime;
                input = RemoveTimePhrases(input);
            }

            // Parse date
            if (input.Contains("today") || input.Contains("now"))
                date = DateTime.Today;
            else if (input.Contains("tomorrow"))
                date = DateTime.Today.AddDays(1);
            else if (input.Contains("next week") || input.Contains("in a week"))
                date = DateTime.Today.AddDays(7);
            else if (input.Contains("next month") || input.Contains("in a month"))
                date = DateTime.Today.AddDays(30);
            else if (DateTime.TryParse(input, out DateTime parsedDate))
                date = parsedDate.Date;

            // Default to today if only time was specified
            if (time.HasValue && !date.HasValue)
                date = DateTime.Today;

            return (date, time);
        }

        private bool TryParseTime(string input, out TimeSpan time)
        {
            time = TimeSpan.Zero;

            // Try common time formats
            if (DateTime.TryParseExact(input, "h tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                time = dt.TimeOfDay;
                return true;
            }
            if (DateTime.TryParseExact(input, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                time = dt.TimeOfDay;
                return true;
            }
            if (DateTime.TryParseExact(input, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                time = dt.TimeOfDay;
                return true;
            }

            return false;
        }

        private string RemoveTimePhrases(string input)//leaves only the important info
        {
            var phrases = new[] { "at", "by", "around", "about", "approximately" };
            foreach (var phrase in phrases)
            {
                input = input.Replace(phrase, "");
            }
            return input;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void TaskCompleted(string input)
        {
            _inTaskCompletionMode = true;
            HandleTaskCompletion(input);
        }


        public void TaskDeleted(string input)
        {
            _inTaskDeletionMode = true;
            HandleTaskDeletion(input);
        }

        private void HandleTaskCompletion(string input)//searches task by id
        {
            if (ExtractId(input, out int taskId))
            {
                CompleteTaskById(taskId);
            }
            else
            {
                bool foundTask = false;
                foreach (var task in tasks.Where(t => !t.IsCompleted))
                {
                    if (input.Contains(task.Title.ToLower()) ||
                        task.Title.ToLower().Split(' ').Any(word => input.Contains(word)))
                    {
                        CompleteTaskById(task.Id);
                        foundTask = true;
                        break;
                    }
                }

                if (!foundTask)
                {
                    PrintCSAB("No matching task found.");
                }
            }

            ListTasks();
            PrintCSAB("Enter another task to complete or type 'back' to return");
        }

        private void HandleTaskDeletion(string input)//delets by id
        {
            if (ExtractId(input, out int taskId))
            {
                DeleteTaskById(taskId);
            }
            else
            {
                bool foundTask = false;
                foreach (var task in tasks)
                {
                    if (input.Contains(task.Title.ToLower()) ||
                        task.Title.ToLower().Split(' ').Any(word => input.Contains(word)))
                    {
                        DeleteTaskById(task.Id);
                        foundTask = true;
                        break;
                    }
                }

                if (!foundTask)
                {
                    PrintCSAB("No matching task found.");
                }
            }

            ListTasks();
            PrintCSAB("Enter another task to delete or type 'back' to return");
        }
        
        public void CompleteTaskById(int taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = true;
                Log($"Marked task as complete: {task.Title}");
                PrintCSAB($"Great job completing: \"{task.Title}\"!");

                if (task.Title.Contains("password") || task.Title.Contains("2fa") ||
                    task.Title.Contains("privacy") || task.Title.Contains("backup"))
                {
                    PrintCSAB("You're taking important steps to improve your cybersecurity!");
                }
            }
            else
            {
                PrintCSAB("I couldn't find that task. Here's your current list:");
                ListTasks();
            }
        }

 
        public void DeleteTaskById(int taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                tasks.Remove(task);
                Log($"Deleted task: {task.Title}");
                PrintCSAB($"I've removed the task: \"{task.Title}\"");
            }
            else
            {
                PrintCSAB("I couldn't find that task. Here's your current list:");
                ListTasks();
            }
        }

        public void ListTasks()//dipslays all tasks in list and info
        {
            if (!tasks.Any())
            {
                PrintCSAB("No tasks found.");
                return;
            }

            PrintCSAB("\nYour Tasks:");
            foreach (var task in tasks)
            {
                string status = task.IsCompleted ? "[✓]" : "[ ]";
                string reminder = "";
                if (task.ReminderDate.HasValue)
                {
                    reminder = $" (Reminder: {task.ReminderDate.Value:yyyy-MM-dd}";
                    if (task.ReminderTime.HasValue)
                        reminder += $" at {task.ReminderTime.Value:h\\:mm tt}";
                    reminder += ")";
                }
                PrintCSAB($"{task.Id}. {status} {task.Title} - {task.Description}{reminder}");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void StartQuiz()
        {
            quizIndex = 0;
            score = 0;
            isInQuizMode = true;
            Log("Started cybersecurity quiz");
            PrintCSAB("Welcome to the Cybersecurity Quiz!");
            PrintCSAB("Answer with A, B, C, or D for multiple-choice. For true/false, answer True or False.");
            PrintCSAB("Good luck!\n");
            AskQuizQuestion();
        }

        public void AskQuizQuestion()
        {
            if (quizIndex < quizQuestions.Length)
            {
                PrintCSAB(quizQuestions[quizIndex]);//insures quiz length
                foreach (var option in quizOptions[quizIndex])
                {
                    PrintCSAB(option);
                }
                PrintCSAB("\nYour answer?");
            }
        }

        public void QuizAnswer(string input)
        {
            int userAnswer = 0;

            if (quizIndex >= 10)
            {
                if (input.Equals("true", StringComparison.OrdinalIgnoreCase)) userAnswer = 1;//any case allowed
                else if (input.Equals("false", StringComparison.OrdinalIgnoreCase)) userAnswer = 2;//however must by spelt correctly
            }
            else // Handle multiple choice
            {
                userAnswer = input.ToUpper() //any case allowed
                switch
                {
                    "A" => 1,
                    "B" => 2,
                    "C" => 3,
                    "D" => 4,
                    _ => 0
                };
            }

            if (userAnswer == 0)
            {
                PrintCSAB("Invalid input. Use A, B, C, or D for multiple-choice, or True/False for true/false questions.");
                return;
            }

            if (userAnswer == correctAnswers[quizIndex])
            {
                PrintCSAB("Correct!\n");
                score++;
            }
            else
            {
                PrintCSAB($"Incorrect! {quizExplanations[quizIndex]}\n");
            }

            quizIndex++;

            if (quizIndex < quizQuestions.Length)
            {
                AskQuizQuestion();
            }
            else
            {
                isInQuizMode = false;
                Log($"Completed quiz with score: {score}/{quizQuestions.Length}");
                PrintCSAB($"Quiz complete! Your score: {score}/{quizQuestions.Length}");//displayes output based on score out of total

                if (score == quizQuestions.Length)
                    PrintCSAB("Perfect! You're a cybersecurity expert!");
                else if (score >= quizQuestions.Length * 0.8)
                    PrintCSAB("Excellent! You have strong cybersecurity knowledge.");
                else if (score >= quizQuestions.Length / 2)
                    PrintCSAB("Good job! You know the basics but could learn more.");
                else
                    PrintCSAB("Keep learning! Cybersecurity is important for everyone.");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        
        
        //Part 1 & 2 code, tolded not to expain, however there is still class at the end



        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void ProcessChatbotInput(string input)
        {
            string emotion = CheckUserEmotions(input);
            string emotionalResponse = EmotionalResponse(emotion);
            if (!string.IsNullOrEmpty(emotionalResponse))
            {
                PrintCSAB(emotionalResponse);
            }

            string response = GetChatbotResponse(input);
            PrintCSAB(response);
        }

        private string CheckUserEmotions(string input)
        {
            input = input.ToLower();
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious"))
                return "worried";
            if (input.Contains("angry") || input.Contains("frustrated") || input.Contains("annoyed"))
                return "frustrated";
            if (input.Contains("happy") || input.Contains("excited") || input.Contains("great") || input.Contains("amazing"))
                return "positive";
            if (input.Contains("confused") || input.Contains("unsure") || input.Contains("dont know") || input.Contains("don't know"))
                return "confused";
            return "neutral";
        }

        private string EmotionalResponse(string emotion)
        {
            switch (emotion)
            {
                case "worried":
                    return "I understand this might be concerning. Cybersecurity can feel overwhelming at times.";
                case "frustrated":
                    return "I hear your frustration. These issues can be annoying to deal with.";
                case "confused":
                    return "It's okay to feel unsure. I'm here to help explain things clearly.";
                case "positive":
                    return "I'm glad you're feeling positive! Cybersecurity awareness is important.";
                default:
                    return string.Empty;
            }
        }

        private string GetChatbotResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Please enter words for conversation, if you really want to leave, just say goodbye!";
            }

            userInput = userInput.ToLower();

            // Respond to greetings
            if (userInput.Contains("hello") || userInput.Contains("hey") || userInput.Contains("hi"))
                return $"Hello Again {userName}! Hopefully, I can help you today?";

            // Respond to farewells
            else if (userInput.Contains("bye") || userInput.Contains("goodbye") || userInput.Contains("byebye"))
            {
                Close();
                return $"Goodbye {userName}!";
            }

            // Responses to what the bot does
            else if (userInput.Contains("what") && userInput.Contains("purpose"))
                return "I talk about cybersecurity! I will hopefully be able to describe certain keywords. Please refer to the readme file to see what I'm able to respond to!";
            else if (userInput.Contains("what") && userInput.Contains("ask") && userInput.Contains("about"))
                return "I can answer things about cybersecurity, phishing, and safety while browsing the web! If you need a more concrete list, please refer to my readme file.";

            // Tip response
            else if (userInput.Contains("tip"))
                return GiveRandomTip(userInput);

            // Check if user is expressing interest in a topic
            else if (userInput.Contains("care about") || userInput.Contains("i like") || userInput.Contains("interested in"))
            {
                if (userInput.Contains("privacy"))
                {
                    userInterest = "privacy";
                    userTopic = "privacy";
                    return "Great! I'll remember that you're interested in privacy. It's a crucial part of staying safe online.";
                }
                else if (userInput.Contains("phishing"))
                {
                    userInterest = "phishing";
                    userTopic = "phishing";
                    return "Phishing is an important topic! I'll focus on that for you.";
                }
                else if (userInput.Contains("password"))
                {
                    userInterest = "password";
                    userTopic = "password";
                    return "Password security is fundamental! I'll keep that in mind for our conversation.";
                }
                else if (userInput.Contains("cybersecurity") || userInput.Contains("cyber security"))
                {
                    userInterest = "cybersecurity";
                    userTopic = "cybersecurity";
                    return "Cybersecurity is a broad and essential field! I'll focus on general cybersecurity topics for you.";
                }
                else if (userInput.Contains("firewall"))
                {
                    userInterest = "firewall";
                    userTopic = "firewall";
                    return "Firewalls are critical for network security! I'll remember your interest in this topic.";
                }
                else if (userInput.Contains("vpn"))
                {
                    userInterest = "vpn";
                    userTopic = "vpn";
                    return "VPNs are great for privacy and security! I'll focus on virtual private networks for you.";
                }
                else if (userInput.Contains("scam"))
                {
                    userInterest = "scam";
                    userTopic = "scam";
                    return "Understanding scams is key to avoiding them! I'll focus on scam prevention for you.";
                }
                else if (userInput.Contains("identity theft") || userInput.Contains("identity fraud"))
                {
                    userInterest = "identity theft";
                    userTopic = "identity theft";
                    return "Identity theft protection is crucial in today's digital world! I'll focus on this important topic.";
                }
            }

            //"what" questions are answered with a definition and "how" with a brief explanation
            else if (userInput.Contains("how"))
            {
                if (userInput.Contains("cybersecurity"))
                {
                    userTopic = "cybersecurity";
                    return "Cybersecurity is achieved by implementing a combination of technologies, processes, and practices designed to protect networks, devices, programs, and data from attacks.";
                }
                else if (userInput.Contains("phishing"))
                {
                    userTopic = "phishing";
                    return "Phishing is typically carried out by email spoofing or instant messaging, and it often directs users to enter personal information at a fake website.";
                }
                else if (userInput.Contains("firewall"))
                {
                    userTopic = "firewall";
                    return "Firewalls establish a barrier between a trusted internal network and untrusted external networks, such as the internet.";
                }
                else if (userInput.Contains("vpn"))
                {
                    userTopic = "vpn";
                    return "VPNs work by routing your device's internet connection through the VPN's private server instead of your internet service provider (ISP).";
                }
                else if (userInput.Contains("password"))
                {
                    userTopic = "password";
                    return "Passwords work by comparing user input against a stored value, often using cryptographic hashing for security.";
                }
                else if (userInput.Contains("scam"))
                {
                    userTopic = "scam";
                    return "Scams typically work by exploiting human psychology, using urgency, fear, or greed to bypass rational judgment.";
                }
                else if (userInput.Contains("privacy"))
                {
                    userTopic = "privacy";
                    return "Privacy can be protected through technical measures (encryption), policy controls, and careful management of personal data sharing.";
                }
                else if (userInput.Contains("identity theft"))
                {
                    userTopic = "identity theft";
                    return "Identity theft works by criminals gathering personal data (like SSNs or credit card numbers) through phishing, hacking, or data breaches, then impersonating the victim.";
                }
            }

            // Respond to specific "what" questions about cybersecurity topics
            else if (userInput.Contains("what"))
            {
                if (userInput.Contains("cybersecurity"))
                {
                    userTopic = "cybersecurity";
                    return "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.";
                }
                else if (userInput.Contains("phishing"))
                {
                    userTopic = "phishing";
                    return "Phishing is a type of cyber attack that uses disguised email as a weapon.";
                }
                else if (userInput.Contains("firewall"))
                {
                    userTopic = "firewall";
                    return "A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules.";
                }
                else if (userInput.Contains("vpn"))
                {
                    userTopic = "vpn";
                    return "A VPN, or Virtual Private Network, creates a secure connection over the internet between your device and the VPN server.";
                }
                else if (userInput.Contains("password"))
                {
                    userTopic = "password";
                    return "A password is a secret combination of characters used to authenticate a user's identity and grant access to systems or data.";
                }
                else if (userInput.Contains("scam"))
                {
                    userTopic = "scam";
                    return "A scam is a deceptive scheme designed to trick individuals into giving away money, personal information, or access to systems.";
                }
                else if (userInput.Contains("privacy"))
                {
                    userTopic = "privacy";
                    return "Privacy refers to an individual's right to control how their personal information is collected, used, and shared.";
                }
                else if (userInput.Contains("identity theft"))
                {
                    userTopic = "identity theft";
                    return "Identity theft occurs when someone uses another person's personal information without permission, typically for financial gain.";
                }
            }

            // Continue conversation on a specific topic
            else if (!string.IsNullOrEmpty(userTopic) && (userInput.Contains("more") || userInput.Contains("explain") || userInput.Contains("tell me")))
            {
                return GetTopicContinuation(userTopic);
            }

            // Default response for unrecognized input
            else if (!string.IsNullOrEmpty(userInterest))
            {
                return $"As someone interested in {userInterest}, you might want to ask about related topics instead.";
            }

            return "I'm sorry, I don't have a great understanding currently. Please ask me something else, hopefully more specific to cybersecurity.";
        }

        private string GetTopicContinuation(string topic)
        {
            switch (topic.ToLower())
            {
                case "privacy":
                    return "Continuing about privacy - did you know you can review your privacy settings on most social media platforms? " +
                           "You can control who sees your information and what data is collected about you.";

                case "phishing":
                    return "More about phishing - attackers often use current events or urgent scenarios to trick people. " +
                           "Always verify unexpected requests, even if they seem to come from trusted sources.";

                case "password":
                    return "Expanding on passwords - a good password should be long (12+ characters) and unique for each account. " +
                           "Consider using a password manager to keep track of them all securely.";

                case "cybersecurity":
                    return "More on cybersecurity - it's not just about technology, but also about people and processes. " +
                           "Regular software updates and employee training are just as important as firewalls and antivirus software.";

                case "firewall":
                    return "Going deeper on firewalls - there are both hardware and software firewalls. " +
                           "A good practice is to use both for layered protection, especially for business networks.";

                case "vpn":
                    return "More about VPNs - when you use a VPN, your internet traffic is encrypted, making it much harder for others to intercept your data. " +
                           "This is especially important when using public Wi-Fi networks.";

                case "scam":
                    return "Further on scams - modern scams can be very sophisticated, using psychological manipulation. " +
                           "Remember: if an offer seems too good to be true, it probably is.";

                case "identity theft":
                    return "More about identity theft - victims often don't realize their identity has been stolen until significant damage is done. " +
                           "Regularly checking your credit reports can help detect early signs of identity theft.";

                default:
                    return "";
            }
        }

        private string GiveRandomTip(string userInput)
        {
            List<string> phishingTips = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Check the sender's email address carefully - phishing emails often use addresses that look similar to legitimate ones.",
                "Never click on suspicious links. Hover over them first to see the actual URL.",
                "Look for poor grammar and spelling - these are common in phishing attempts.",
                "If an email creates a sense of urgency, be extra careful - this is a common phishing tactic."
            };

            List<string> passwordTips = new List<string>
            {
                "Use a mix of uppercase, lowercase, numbers, and special characters in your passwords.",
                "Consider using a passphrase instead of a password - they're easier to remember and harder to crack.",
                "Never reuse passwords across different accounts.",
                "Enable two-factor authentication wherever possible for extra security.",
                "Use a password manager to securely store and generate strong passwords."
            };

            List<string> scamTips = new List<string>
            {
                "If an offer seems too good to be true, it probably is. Always verify before responding.",
                "Legitimate organizations will never ask for sensitive information via email or text.",
                "Be wary of unexpected calls claiming to be from tech support - this is a common scam tactic.",
                "Research any company or individual before sending money or personal details.",
                "Scammers often pressure you to act quickly. Take your time to verify any request."
            };

            Random random = new Random();

            if (userInput.Contains("phishing"))
            {
                return phishingTips[random.Next(phishingTips.Count)];
            }
            else if (userInput.Contains("password"))
            {
                return passwordTips[random.Next(passwordTips.Count)];
            }
            else if (userInput.Contains("scam"))
            {
                return scamTips[random.Next(scamTips.Count)];
            }

            List<string> allTips = new List<string>();
            allTips.AddRange(phishingTips);
            allTips.AddRange(passwordTips);
            allTips.AddRange(scamTips);
            return allTips[random.Next(allTips.Count)];
        }

//------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void ShowActivityLog(bool showMore = false)
        {
            if (showMore) _logPage++;
            else _logPage = 0;//displays all marked activities

            var entries = activityLog.Skip(_logPage * 5).Take(5).ToList();

            PrintCSAB("\n=== Activity Log ===");
            if (entries.Count == 0)//starts at the newsest log , descending to older
            {
                PrintCSAB("No more activities to show.");
                _showingActivityLog = false;
            }
            else
            {
                foreach (var entry in entries)
                {
                    PrintCSAB(entry);
                }
                _showingActivityLog = true;
                PrintCSAB("\nType 'more' to see additional entries or ask another question to exit log view.");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public bool ExtractId(string input, out int id)// used to find the id stataed by user
        {
            id = 0;

            if (int.TryParse(input.Trim(), out id))
            {
                return true;
            }

            var matches = Regex.Matches(input, @"\d+");
            foreach (Match match in matches)
            {
                if (int.TryParse(match.Value, out id))
                {
                    return true;
                }
            }

            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public bool ContainsAny(string input, params string[] terms)//used to find any keyword stataed by user
        {
            return terms.Any(term => input.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void Log(string message)//how the log addeds and delets logs
        {
            var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            activityLog.Insert(0, entry);
            if (activityLog.Count > MaxLogEntries * 3) 
                activityLog.RemoveAt(activityLog.Count - 1);
        }

        public void PrintCSAB(string message)//how thje bot displays responses
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = $"CSAB: {message}",
                Foreground = System.Windows.Media.Brushes.LightGreen,
                Margin = new Thickness(0, 4, 0, 4),
                TextWrapping = TextWrapping.Wrap
            });
            Scroll.ScrollToEnd();
        }

        public void PrintUser(string message)//How user responsess are displayed
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = $"{userName}: {message}",
                Foreground = System.Windows.Media.Brushes.LightBlue,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 4, 0, 4),
                TextWrapping = TextWrapping.Wrap
            });
            Scroll.ScrollToEnd();
        }

    }

    public class UserTask//class contaning task information
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public TimeSpan? ReminderTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}