using System.Collections.Generic;

namespace InnocentGuardPart3
{
    class ChatMemory
    {
        private Dictionary<string, string> memory = new Dictionary<string, string>();
        public void Remember(string key, string value) => memory[key] = value;
        public string Recall(string key) => memory.ContainsKey(key) ? memory[key] : string.Empty;
        public bool Knows(string key) => memory.ContainsKey(key) && !string.IsNullOrEmpty(memory[key]);
    }
}