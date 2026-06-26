using System;
using System.Collections.Generic;

namespace InnocentGuardPart3
{
    
    class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }  
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public bool IsTrueFalse { get; set; }

        public QuizQuestion(string question, List<string> options, string correct, string explanation, bool isTrueFalse = false)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correct.ToUpper();
            Explanation = explanation;
            IsTrueFalse = isTrueFalse;
        }
    }

    class QuizEngine
    {
        private List<QuizQuestion> questions = new List<QuizQuestion>();
        private int currentIndex = 0;
        private int score = 0;
        private bool isActive = false;
        private bool waitingForAnswer = false;

        public bool IsActive => isActive;
        public bool WaitingForAnswer => waitingForAnswer;

        public QuizEngine()
        {
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            // Multiple choice questions
            questions.Add(new QuizQuestion(
                "What should you do if you receive an email asking for your password?",
                new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
                "C", "Reporting phishing emails helps protect others and stops the scammers."));

            questions.Add(new QuizQuestion(
                "What is the minimum recommended length for a strong password?",
                new List<string> { "A) 6 characters", "B) 8 characters", "C) 12 characters", "D) 4 characters" },
                "C", "Passwords should be at least 12 characters to be considered strong."));

            questions.Add(new QuizQuestion(
                "What does HTTPS in a website URL mean?",
                new List<string> { "A) The site is fast", "B) The site is encrypted and secure", "C) The site is free", "D) The site is government owned" },
                "B", "HTTPS means the connection between your browser and the site is encrypted."));

            questions.Add(new QuizQuestion(
                "Which of these is the safest password?",
                new List<string> { "A) password123", "B) John1990", "C) Tr!9kL#2mP@", "D) 123456" },
                "C", "A strong password uses random uppercase, lowercase, numbers, and symbols."));

            questions.Add(new QuizQuestion(
                "What is phishing?",
                new List<string> { "A) A type of malware", "B) A fake email trick to steal info", "C) A firewall tool", "D) A type of VPN" },
                "B", "Phishing is when scammers send fake emails pretending to be trusted companies."));

            questions.Add(new QuizQuestion(
                "What does 2FA stand for?",
                new List<string> { "A) Two File Access", "B) Two-Factor Authentication", "C) Twice Fast Access", "D) Two Form Application" },
                "B", "2FA adds a second layer of security so even a stolen password isn't enough to log in."));

            questions.Add(new QuizQuestion(
                "What is social engineering?",
                new List<string> { "A) Building social media apps", "B) Manipulating people to reveal info", "C) Fixing software bugs", "D) Creating strong passwords" },
                "B", "Social engineering tricks people rather than hacking systems — always verify who you're talking to."));

            questions.Add(new QuizQuestion(
                "Which of these is a sign of a phishing email?",
                new List<string> { "A) Comes from your boss", "B) Has a PDF attachment", "C) Creates urgent panic and asks you to click a link", "D) Has a company logo" },
                "C", "Phishing emails create urgency to make you act without thinking — always pause and verify."));

            // True/False questions
            questions.Add(new QuizQuestion(
                "TRUE or FALSE: You should use the same password for all your accounts to make it easier to remember.",
                new List<string> { "A) True", "B) False" },
                "B", "Never reuse passwords! If one account is hacked, all your accounts become vulnerable.",
                true));

            questions.Add(new QuizQuestion(
                "TRUE or FALSE: Public Wi-Fi networks are safe to use for online banking.",
                new List<string> { "A) True", "B) False" },
                "B", "Public Wi-Fi is not secure. Always use a VPN or mobile data for banking.",
                true));

            questions.Add(new QuizQuestion(
                "TRUE or FALSE: A VPN helps protect your privacy by hiding your internet traffic.",
                new List<string> { "A) True", "B) False" },
                "A", "Correct! A VPN encrypts your traffic so hackers and your ISP can't easily see what you're doing.",
                true));

            questions.Add(new QuizQuestion(
                "TRUE or FALSE: Malware can only enter your device through email attachments.",
                new List<string> { "A) True", "B) False" },
                "B", "Malware can enter through websites, USB drives, downloads, and many other ways — not just emails.",
                true));

            // Shuffle questions so they appear in random order
            var rng = new Random();
            for (int i = questions.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var temp = questions[i];
                questions[i] = questions[j];
                questions[j] = temp;
            }
        }

        // Start the quiz
        public string Start()
        {
            isActive = true;
            currentIndex = 0;
            score = 0;
            return "🎮 Starting the Cybersecurity Quiz!\n\n" +
                   $"There are {questions.Count} questions. Type the letter of your answer (A, B, C, or D).\n\n" +
                   GetCurrentQuestion();
        }

        // Get current questions as a formatted string
        public string GetCurrentQuestion()
        {
            if (currentIndex >= questions.Count)
                return GetFinalScore();

            var q = questions[currentIndex];
            waitingForAnswer = true;

            string result = $"Question {currentIndex + 1} of {questions.Count}:\n\n";
            result += $"  {q.Question}\n\n";
            foreach (var option in q.Options)
                result += $"  {option}\n";

            return result;
        }

        // Process the user's answer
        public string SubmitAnswer(string input)
        {
            if (!isActive || !waitingForAnswer)
                return "No quiz is running. Type 'start quiz' to begin!";

            string answer = input.Trim().ToUpper();

            if (answer.Length > 1) answer = answer.Substring(0, 1);

            var q = questions[currentIndex];
            waitingForAnswer = false;
            currentIndex++;

            string feedback;
            if (answer == q.CorrectAnswer)
            {
                score++;
                feedback = $"✅ Correct! {q.Explanation}";
            }
            else
            {
                feedback = $"❌ Not quite. The correct answer was {q.CorrectAnswer}. {q.Explanation}";
            }

            if (currentIndex >= questions.Count)
                return feedback + "\n\n" + GetFinalScore();

            return feedback + "\n\n" + GetCurrentQuestion();
        }

        //Final score message
        private string GetFinalScore()
        {
            isActive = false;
            waitingForAnswer = false;

            string grade;
            if (score == questions.Count)
                grade = "🏆 Perfect score! You're a cybersecurity pro!";
            else if (score >= questions.Count * 0.8)
                grade = "🌟 Great job! You know your cybersecurity well!";
            else if (score >= questions.Count * 0.5)
                grade = "👍 Good effort! Keep learning to stay safe online.";
            else
                grade = "📚 Keep learning — cybersecurity knowledge keeps you safe online!";

            return $"Quiz complete!\n\nYour score: {score} out of {questions.Count}\n\n{grade}";
        }
    }
}