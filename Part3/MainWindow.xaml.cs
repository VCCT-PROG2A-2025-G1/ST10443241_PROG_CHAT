using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Part3
{
    public partial class MainWindow : Window
    {
        public List<string> activityLog = new List<string>();
        public const int MaxLogEntries = 10;
        public List<UserTask> tasks = new List<UserTask>();
        public int nextId = 1;
        public int quizIndex = 0;
        public int score = 0;
        public bool isInQuizMode = false;

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
            "10. What does HTTPS ensure?"
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
            new[] {"A) Faster website loading", "B) Better website design", "C) Encrypted data transmission", "D) Free access to paid content"}
        };

        public readonly int[] correctAnswers = { 3, 3, 1, 4, 2, 3, 1, 4, 2, 3 };

        public bool isCreatingTask = false;
        public UserTask pendingTask = new UserTask();

        public MainWindow()
        {
            InitializeComponent();
            Log("Program started");
            PrintCSAB("Welcome to CyberChat! You can:");
            PrintCSAB("- Say things like 'Remind me to update my password'");
            PrintCSAB("- Type 'start quiz' to begin the cybersecurity quiz");
            PrintCSAB("- Type 'show tasks' to view your current tasks");
            PrintCSAB("- Type 'activity log' to see recent activities");
        }

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

            if (isInQuizMode)
            {
                QuizAnswer(userInput);
                return;
            }

            string input = userInput.ToLower();

            if (input == "start quiz")
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
            else if (!TryProcessNaturalLanguage(input)) // This will handle all task-related commands
            {
                PrintCSAB("I didn't understand that. Try saying things like:");
                PrintCSAB("- 'Remind me to update my password'");
                PrintCSAB("- 'Add task to enable 2FA'");
                PrintCSAB("- 'Show my tasks'");
                PrintCSAB("- 'Start quiz'");
            }
        }

        public bool TryProcessNaturalLanguage(string input)
        {
            if (isCreatingTask)
            {
                TaskContinue(input);
                return true;
            }

            if (input.StartsWith("add task") ||
                (ContainsAny(input, "remind", "add", "create", "new", "set") &&
                 ContainsAny(input, "task", "reminder", "todo", "note")) ||
                input.Contains("i need to") || input.Contains("don't forget to"))
            {
                TaskStart();
                return true;
            }

            // Task viewing with various phrasings
            if (ContainsAny(input, "show", "list", "view", "see", "display", "what are my") &&
                ContainsAny(input, "task", "reminder", "todos", "to-do"))
            {
                Log("Viewed task list");
                ListTasks();
                return true;
            }

            // Task completion with various phrasings
            if ((ContainsAny(input, "complete", "finish", "done", "mark") &&
                 ContainsAny(input, "task", "reminder")) ||
                input.Contains("i've done") || input.Contains("i finished"))
            {
                TaskCompleted(input);
                return true;
            }

            // Task deletion with various phrasings
            if ((ContainsAny(input, "delete", "remove", "clear", "cancel") &&
                 ContainsAny(input, "task", "reminder")) ||
                input.Contains("i don't need to"))
            {
                TaskDeleted(input);
                return true;
            }

            return false;
        }

        public void TaskStart()
        {
            isCreatingTask = true;
            pendingTask = new UserTask
            {
                Id = nextId++,
                IsCompleted = false
            };

            PrintCSAB("Sure! What's the title of the task?");
        }

        public void TaskContinue(string input)
        {
            if (string.IsNullOrWhiteSpace(pendingTask.Title))
            {
                pendingTask.Title = input.Trim();
                PrintCSAB("Got it. Any description you'd like to add?");
                return;
            }

            if (string.IsNullOrWhiteSpace(pendingTask.Description))
            {
                pendingTask.Description = input.Trim();
                PrintCSAB("When would you like to be reminded? (e.g., today, tomorrow, next week)");
                return;
            }

            if (pendingTask.ReminderDate == null)
            {
                pendingTask.ReminderDate = DateFromInput(input);
                if (pendingTask.ReminderDate == null)
                {
                    PrintCSAB("Sorry, I didn't understand the date. Please try something like 'tomorrow' or 'today'");
                    return;
                }

                tasks.Add(pendingTask);
                Log($"Added task: {pendingTask.Title}");
                PrintCSAB($"All set! I'll remind you to \"{pendingTask.Title}\" on {pendingTask.ReminderDate:yyyy-MM-dd}.");

                isCreatingTask = false;
                pendingTask = null;
                return;
            }
        }

        public DateTime? DateFromInput(string input)
        {
            input = input.ToLower();
            if (input.Contains("today") || input.Contains("now")) return DateTime.Today;
            if (input.Contains("tomorrow")) return DateTime.Today.AddDays(1);
            if (input.Contains("next week") || input.Contains("in a week")) return DateTime.Today.AddDays(7);
            if (input.Contains("next month") || input.Contains("in a month")) return DateTime.Today.AddDays(30);

            // Try to parse a date directly
            if (DateTime.TryParse(input, out DateTime parsed)) return parsed;

            return null;
        }


        public void TaskCompleted(string input)
        {
            if (ExtractId(input, out int taskId))
            {
                CompleteTaskById(taskId);
                return;
            }

            // Try to find task by title if no ID was provided
            foreach (var task in tasks.Where(t => !t.IsCompleted))
            {
                if (input.Contains(task.Title.ToLower()) ||
                    task.Title.ToLower().Split(' ').Any(word => input.Contains(word)))
                {
                    CompleteTaskById(task.Id);
                    return;
                }
            }

            // If no task was identified, show the list and ask for ID
            ListTasks();
            if (tasks.Any())
            {
                PrintCSAB("\nWhich task would you like to complete? (Enter the ID or part of the title)");
            }
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

        public void TaskDeleted(string input)
        {
            if (ExtractId(input, out int taskId))
            {
                DeleteTaskById(taskId);
                return;
            }

            // Try to find task by title if no ID was provided
            foreach (var task in tasks)
            {
                if (input.Contains(task.Title.ToLower()) ||
                    task.Title.ToLower().Split(' ').Any(word => input.Contains(word)))
                {
                    DeleteTaskById(task.Id);
                    return;
                }
            }

            // If no task was identified, show the list and ask for ID
            ListTasks();
            if (tasks.Any())
            {
                PrintCSAB("\nWhich task would you like to remove? (Enter the ID or part of the title)");
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

        public void StartQuiz()
        {
            quizIndex = 0;
            score = 0;
            isInQuizMode = true;
            Log("Started cybersecurity quiz");
            PrintCSAB("Welcome to the Cybersecurity Quiz!");
            PrintCSAB("Answer with A, B, C, or D. Good luck!\n");
            AskQuizQuestion();
        }

        public void AskQuizQuestion()
        {
            if (quizIndex < quizQuestions.Length)
            {
                PrintCSAB(quizQuestions[quizIndex]);
                foreach (var option in quizOptions[quizIndex])
                {
                    PrintCSAB(option);
                }
                PrintCSAB("\nYour answer (A/B/C/D)?");
            }
        }

        public void QuizAnswer(string input)
        {
            int userAnswer = input.ToUpper() switch
            {
                "A" => 1,
                "B" => 2,
                "C" => 3,
                "D" => 4,
                _ => 0
            };

            if (userAnswer == 0)
            {
                PrintCSAB("Invalid input. Use A, B, C, or D.");
                return;
            }

            if (userAnswer == correctAnswers[quizIndex])
            {
                PrintCSAB("✅ Correct!\n");
                score++;
            }
            else
            {
                PrintCSAB($"❌ Incorrect! The correct answer was {(char)(correctAnswers[quizIndex] + 64)}.\n");
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
                PrintCSAB($"Quiz complete! Your score: {score}/{quizQuestions.Length}");
                if (score == quizQuestions.Length) PrintCSAB("Perfect! You're a cybersecurity expert!");
                else if (score >= quizQuestions.Length / 2) PrintCSAB("Good job! You know the basics.");
                else PrintCSAB("Keep learning! Cybersecurity is important.");
            }
        }

        public bool ExtractId(string input, out int id)
        {
            id = 0;
            var words = input.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int number))
                {
                    id = number;
                    return true;
                }
            }
            return false;
        }

        public bool ContainsAny(string input, params string[] terms)
        {
            return terms.Any(term => input.Contains(term));
        }

        public void ListTasks()
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
                string reminder = task.ReminderDate.HasValue
                    ? $" (Reminder: {task.ReminderDate.Value:yyyy-MM-dd})"
                    : "";
                PrintCSAB($"{task.Id}. {status} {task.Title} - {task.Description}{reminder}");
            }
        }

        public void ShowActivityLog()
        {
            PrintCSAB("\n=== Activity Log (Recent First) ===");
            if (activityLog.Count == 0)
            {
                PrintCSAB("No activities logged yet.");
            }
            else
            {
                foreach (var entry in activityLog)
                {
                    PrintCSAB(entry);
                }
            }
        }

        public void PrintCSAB(string message)
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

        public void PrintUser(string message)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = $"You: {message}",
                Foreground = System.Windows.Media.Brushes.LightBlue,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 4, 0, 4),
                TextWrapping = TextWrapping.Wrap
            });
            Scroll.ScrollToEnd();

        }

        public void Log(string message)
        {
            var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            activityLog.Insert(0, entry);
            if (activityLog.Count > MaxLogEntries)
                activityLog.RemoveAt(MaxLogEntries);
        }
    }

    public class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}