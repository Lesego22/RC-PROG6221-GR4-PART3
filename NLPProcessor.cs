using System;
using System.Collections.Generic;

namespace InnocentGuardPart3
{
    class NLPProcessor
    {
        private Dictionary<string, List<string>> intentMap = new Dictionary<string, List<string>>()
        {
            { "add_task",     new List<string> { "add task", "create task", "new task", "add a task", "create a task", "i want to add", "remind me to", "set a task", "make a task" } },
            { "view_tasks",   new List<string> { "view tasks", "show tasks", "my tasks", "list tasks", "see tasks", "what are my tasks", "show my tasks", "display tasks", "tasks" } },
            { "complete_task",new List<string> { "complete task", "mark done", "mark as done", "finish task", "i completed", "done with task", "mark complete" } },
            { "delete_task",  new List<string> { "delete task", "remove task", "cancel task", "get rid of task" } },
            { "start_quiz",   new List<string> { "start quiz", "take quiz", "begin quiz", "quiz me", "test me", "i want to take a quiz", "cybersecurity quiz", "play quiz", "quiz" } },
            { "activity_log", new List<string> { "show activity log", "activity log", "what have you done", "what have you done for me", "show log", "recent actions", "history", "show history", "log" } },
            { "set_reminder", new List<string> { "set reminder", "remind me", "set a reminder", "add reminder", "i need a reminder" } }
        };

        public string DetectIntent(string input)
        {
            string lower = input.ToLower().Trim();
            foreach (var intent in intentMap)
                foreach (string phrase in intent.Value)
                    if (lower.Contains(phrase))
                        return intent.Key;
            return "unknown";
        }

        public string ExtractTaskTitle(string input)
        {
            string lower = input.ToLower();
            string[] removePhrases = { "add task", "create task", "new task", "add a task", "create a task", "set a task", "make a task", "remind me to", "i want to add" };

            string result = input;
            foreach (string phrase in removePhrases)
            {
                int idx = lower.IndexOf(phrase);
                if (idx >= 0)
                {
                    result = input.Substring(idx + phrase.Length).Trim();
                    break;
                }
            }

            if (result.Length > 0)
                result = char.ToUpper(result[0]) + result.Substring(1);

            return string.IsNullOrWhiteSpace(result) ? "New Task" : result;
        }

        public int ExtractTaskId(string input)
        {
            string[] words = input.Split(' ');
            foreach (string word in words)
                if (int.TryParse(word, out int id))
                    return id;
            return -1;
        }
    }
}