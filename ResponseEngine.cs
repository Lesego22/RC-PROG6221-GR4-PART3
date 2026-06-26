using System;
using System.Collections.Generic;

namespace InnocentGuardPart3
{
    public delegate string ResponseDelegate(string input);
    public delegate string SentimentDelegate(string input);

    class ResponseEngine
    {
        private SentimentDetector sentimentDetector = new SentimentDetector();
        private ChatMemory memory = new ChatMemory();
        private NLPProcessor nlp = new NLPProcessor();
        private TaskManager taskManager = new TaskManager();
        private QuizEngine quiz = new QuizEngine();
        private ActivityLog activityLog = new ActivityLog();
        private Random random = new Random();
        private string lastTopic = string.Empty;

        // Delegates
        private ResponseDelegate getKeywordResponse;
        private ResponseDelegate getRandomResponse;
        private SentimentDelegate detectSentiment;

        // State for multi-step task creation
        private bool awaitingTaskTitle = false;
        private bool awaitingTaskReminder = false;
        private string pendingTaskTitle = string.Empty;

        private Dictionary<string, string> keywordResponses = new Dictionary<string, string>()
        {
            { "password",           "Passwords should be at least 12 characters and include uppercase, lowercase, numbers, and symbols. Never reuse the same password on different sites!" },
            { "safe browsing",      "To browse safely: look for HTTPS in the URL, avoid public Wi-Fi for banking, keep your browser updated, and be careful what you download." },
            { "malware",            "Malware is harmful software designed to steal your data. Install a trusted antivirus and keep everything updated." },
            { "two-factor",         "Two-factor authentication (2FA) adds a second layer of security. Even if someone steals your password, they still need the second code." },
            { "2fa",                "Enable 2FA on all your important accounts — email, banking, and social media especially." },
            { "social engineering", "Social engineering tricks people rather than systems. Never give out passwords over the phone — legitimate companies won't ask for them." },
            { "vpn",                "A VPN hides your internet traffic. It's especially useful on public Wi-Fi. Try ProtonVPN or NordVPN." },
            { "scam",               "If something sounds too good to be true, it almost always is. Never send money to someone you haven't verified." },
            { "privacy",            "Review your app permissions, use strong passwords, enable 2FA, and be careful what you share on social media." },
            { "how are you",        "I'm doing great and always on guard! How can I help you today?" },
            { "what is your purpose","I'm InnocentGuard — your cybersecurity assistant. I can chat about cyber safety, give you a quiz, help manage your tasks, and more!" },
            { "help",               "Here's what I can do:\n\n  💬 Chat: password, phishing, malware, vpn, 2fa, scam, privacy\n  🎮 Quiz: type 'start quiz'\n  ✅ Tasks: type 'add task', 'view tasks', 'complete task 1', 'delete task 1'\n  📋 Log: type 'show activity log'\n\nJust ask away!" },
        };

        private Dictionary<string, List<string>> randomResponses = new Dictionary<string, List<string>>()
        {
            {
                "phishing", new List<string>
                {
                    "Phishing emails often create urgency — 'Your account will be closed!' Always pause and verify before clicking.",
                    "Check sender addresses carefully — 'support@paypa1.com' is NOT the same as 'support@paypal.com'.",
                    "If an email asks you to log in, open a new tab and go directly to the website instead of clicking the link.",
                    "Legitimate banks will NEVER ask for your password via email or SMS — it's almost certainly a phishing attempt."
                }
            }
        };

        public ResponseEngine()
        {
            getKeywordResponse = LookupKeywordResponse;
            getRandomResponse = LookupRandomResponse;
            detectSentiment = sentimentDetector.Detect;
        }

        public void SetUserName(string name)
        {
            memory.Remember("name", name);
            activityLog.Add($"User '{name}' started a session");
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't catch that — could you type something?";

            string lower = input.ToLower().Trim();
            string userName = memory.Recall("name");
            string prefix = string.IsNullOrEmpty(userName) ? "" : $"{userName}, ";

            if (quiz.IsActive && quiz.WaitingForAnswer)
            {
                string quizResult = quiz.SubmitAnswer(input);
                activityLog.Add($"Quiz answer submitted: '{input}'");
                return quizResult;
            }

            if (awaitingTaskReminder)
            {
                awaitingTaskReminder = false;
                string reminder = lower == "no" || lower == "skip" ? "" : input.Trim();
                var task = taskManager.AddTask(pendingTaskTitle, $"Cybersecurity task: {pendingTaskTitle}", reminder);
                activityLog.Add($"Task added: '{pendingTaskTitle}'" + (reminder != "" ? $" with reminder: {reminder}" : ""));
                pendingTaskTitle = string.Empty;
                return $"✅ Got it! Task '{task.Title}' has been saved." +
                       (reminder != "" ? $" I'll remind you: {reminder}." : "") +
                       "\n\nType 'view tasks' to see all your tasks.";
            }

            if (awaitingTaskTitle)
            {
                awaitingTaskTitle = false;
                awaitingTaskReminder = true;
                pendingTaskTitle = input.Trim();
                return $"Task title set to: '{pendingTaskTitle}'\n\nWould you like to set a reminder? " +
                       "Type a date or timeframe (e.g. 'in 3 days', 'tomorrow') or type 'skip' to skip.";
            }

            
            string intent = nlp.DetectIntent(lower);

            switch (intent)
            {
                case "add_task":
                    string extracted = nlp.ExtractTaskTitle(input);
                    if (extracted == "New Task" || extracted.Length < 3)
                    {
                        awaitingTaskTitle = true;
                        return "Sure! What would you like to call this task?";
                    }
                    awaitingTaskReminder = true;
                    pendingTaskTitle = extracted;
                    return $"Task title set to: '{extracted}'\n\nWould you like a reminder? " +
                           "Type a timeframe (e.g. 'in 3 days') or type 'skip'.";

                case "view_tasks":
                    activityLog.Add("User viewed task list");
                    return taskManager.GetAllTasks();

                case "complete_task":
                    int completeId = nlp.ExtractTaskId(input);
                    if (completeId == -1) return "Please specify the task number. Example: 'complete task 2'";
                    bool completed = taskManager.CompleteTask(completeId);
                    if (completed) activityLog.Add($"Task {completeId} marked as completed");
                    return completed ? $"✅ Task {completeId} marked as completed!" : $"I couldn't find task {completeId}.";

                case "delete_task":
                    int deleteId = nlp.ExtractTaskId(input);
                    if (deleteId == -1) return "Please specify the task number. Example: 'delete task 2'";
                    bool deleted = taskManager.DeleteTask(deleteId);
                    if (deleted) activityLog.Add($"Task {deleteId} deleted");
                    return deleted ? $"🗑️ Task {deleteId} has been deleted." : $"I couldn't find task {deleteId}.";

                case "start_quiz":
                    activityLog.Add("Quiz started");
                    return quiz.Start();

                case "activity_log":
                    activityLog.Add("User viewed activity log");
                    return activityLog.GetRecent();

                case "set_reminder":
                    awaitingTaskTitle = true;
                    return "Sure! What task would you like to set a reminder for?";
            }

            
            if (lower.Contains("more") || lower.Contains("explain") || lower.Contains("another tip"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                    return prefix + LookupKeywordResponse(lastTopic);
                return "Which topic would you like more info on? Try: password, phishing, malware, or VPN.";
            }

            
            string emotion = detectSentiment(lower);
            string empathy = GetEmpathyPrefix(emotion, prefix);

            string kwResult = getKeywordResponse(lower);
            if (!kwResult.StartsWith("Sorry"))
                return empathy + kwResult;

            string rndResult = getRandomResponse(lower);
            if (!rndResult.StartsWith("Sorry"))
                return empathy + rndResult;

            return $"{prefix}I didn't quite understand that. Type 'help' to see everything I can do!";
        }

        private string LookupKeywordResponse(string input)
        {
            foreach (var entry in keywordResponses)
            {
                if (input.Contains(entry.Key))
                {
                    lastTopic = entry.Key;
                    if (entry.Key != "how are you" && entry.Key != "help" && entry.Key != "what is your purpose")
                        memory.Remember("topic", entry.Key);
                    return entry.Value;
                }
            }
            return "Sorry, no match.";
        }

        private string LookupRandomResponse(string input)
        {
            foreach (var entry in randomResponses)
            {
                if (input.Contains(entry.Key))
                {
                    lastTopic = entry.Key;
                    return entry.Value[random.Next(entry.Value.Count)];
                }
            }
            return "Sorry, no match.";
        }

        private string GetEmpathyPrefix(string emotion, string prefix)
        {
            switch (emotion)
            {
                case "worried": return $"{prefix}It's understandable to feel that way — cyber threats are real. Let me help. ";
                case "frustrated": return $"{prefix}I understand this can feel overwhelming. Let me explain clearly. ";
                case "curious": return $"{prefix}Great question! I love the curiosity. ";
                default: return string.Empty;
            }
        }
    }
}