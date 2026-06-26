using System;

namespace InnocentGuardPart3
{
    
    class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool IsCompleted { get; set; }

        public TaskItem(int id, string title, string description, string reminder = "")
        {
            Id = id;
            Title = title;
            Description = description;
            Reminder = reminder;
            IsCompleted = false;
        }

        public override string ToString()
        {
            string status = IsCompleted ? "✅" : "🔲";
            string reminderText = string.IsNullOrEmpty(Reminder) ? "" : $" | Reminder: {Reminder}";
            return $"{status} [{Id}] {Title}{reminderText}";
        }
    }
}