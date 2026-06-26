using System;

namespace InnocentGuardPart3
{
    class SentimentDetector
    {
        private string[] worriedWords = { "worried", "scared", "afraid", "nervous", "anxious", "unsafe", "danger", "hacked", "stolen", "victim" };
        private string[] curiousWords = { "curious", "wondering", "how does", "what is", "can you explain", "tell me", "want to know", "interested" };
        private string[] frustratedWords = { "frustrated", "annoyed", "confused", "don't understand", "makes no sense", "difficult", "complicated", "lost" };

        public string Detect(string input)
        {
            string lower = input.ToLower();
            foreach (string word in worriedWords) if (lower.Contains(word)) return "worried";
            foreach (string word in frustratedWords) if (lower.Contains(word)) return "frustrated";
            foreach (string word in curiousWords) if (lower.Contains(word)) return "curious";
            return "neutral";
        }
    }
}