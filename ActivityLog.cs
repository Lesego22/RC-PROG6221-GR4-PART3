using System;
using System.Collections.Generic;

namespace InnocentGuardPart3
{
    
    class ActivityLog
    {
        private List<string> log = new List<string>();

        public void Add(string action)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            log.Add($"[{timestamp}] {action}");
        }

        public string GetRecent()
        {
            if (log.Count == 0)
                return "No actions recorded yet. Start chatting, add tasks, or take the quiz!";

            // Show only last 10
            int start = Math.Max(0, log.Count - 10);
            var recent = log.GetRange(start, log.Count - start);

            string result = "Here's a summary of recent actions:\n\n";
            for (int i = 0; i < recent.Count; i++)
                result += $"  {i + 1}. {recent[i]}\n";

            return result;
        }
    }
}