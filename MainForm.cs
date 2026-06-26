using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InnocentGuardPart3
{
    public class MainForm : Form
    {
        // Controls
        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;
        private Button quizButton;
        private Button tasksButton;
        private Button logButton;
        private Panel headerPanel;
        private Panel inputPanel;
        private Panel buttonPanel;
        private Label headerLabel;
        private Label logoLabel;

        // Logic
        private ResponseEngine engine = new ResponseEngine();
        private string userName = string.Empty;
        private bool askedForName = false;

        [DllImport("winmm.dll")]
        private static extern bool PlaySound(string szSound, IntPtr hMod, uint fdwSound);

        public MainForm()
        {
            BuildUI();
            PlayVoiceGreeting();
            ShowWelcome();
        }

        private void BuildUI()
        {
            this.Text = "InnocentGuard – Cybersecurity Assistant";
            this.Size = new Size(860, 680);
            this.MinimumSize = new Size(750, 580);
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Consolas", 10f);

            // Header
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(22, 27, 34),
                Padding = new Padding(8)
            };

            logoLabel = new Label
            {
                Text =
                    "  ██╗███╗  ██╗███╗  ██╗ ██████╗  ██████╗███████╗███╗  ██╗████████╗ ██████╗ ██╗   ██╗ █████╗ ██████╗ ██████╗ \n" +
                    "  ██║████╗ ██║████╗ ██║██╔═══██╗██╔════╝██╔════╝████╗ ██║╚══██╔══╝██╔════╝ ██║   ██║██╔══██╗██╔══██╗██╔══██╗\n" +
                    "  ██║██╔██╗██║██╔██╗██║██║   ██║██║     █████╗  ██╔██╗██║   ██║   ██║  ███╗██║   ██║███████║██████╔╝██║  ██║\n" +
                    "  ██║██║╚████║██║╚████║╚██████╔╝╚██████╗███████╗██║╚████║   ██║   ╚██████╔╝╚██████╔╝██║  ██║██║  ██║██████╔╝",
                Font = new Font("Consolas", 4.5f),
                ForeColor = Color.FromArgb(0, 200, 150),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerLabel = new Label
            {
                Text = "🔒  Protecting South Africa Online  🔒",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 220, 180),
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 24,
                BackColor = Color.Transparent
            };

            headerPanel.Controls.Add(logoLabel);
            headerPanel.Controls.Add(headerLabel);

            // Quick action buttons
            buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(22, 27, 34),
                Padding = new Padding(6, 4, 6, 4)
            };

            quizButton = MakeButton("🎮 Start Quiz", Color.FromArgb(80, 50, 140));
            tasksButton = MakeButton("✅ My Tasks", Color.FromArgb(30, 80, 60));
            logButton = MakeButton("📋 Activity Log", Color.FromArgb(60, 60, 30));

            quizButton.Dock = DockStyle.Left;
            tasksButton.Dock = DockStyle.Left;
            logButton.Dock = DockStyle.Left;

            quizButton.Click += (s, e) => SendMessage("start quiz");
            tasksButton.Click += (s, e) => SendMessage("view tasks");
            logButton.Click += (s, e) => SendMessage("show activity log");

            buttonPanel.Controls.Add(logButton);
            buttonPanel.Controls.Add(tasksButton);
            buttonPanel.Controls.Add(quizButton);

            // Chat display
            chatBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(13, 17, 23),
                ForeColor = Color.FromArgb(200, 210, 220),
                Font = new Font("Consolas", 10f),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(10),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Input panel
            inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(22, 27, 34),
                Padding = new Padding(8)
            };

            inputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 37, 46),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Type a message, 'start quiz', 'add task', or 'help'..."
            };

            sendButton = new Button
            {
                Text = "Send",
                Dock = DockStyle.Right,
                Width = 80,
                BackColor = Color.FromArgb(0, 160, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            sendButton.FlatAppearance.BorderSize = 0;

            inputPanel.Controls.Add(inputBox);
            inputPanel.Controls.Add(sendButton);

            sendButton.Click += OnSend;
            inputBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; OnSend(s, e); }
            };

            this.Controls.Add(chatBox);
            this.Controls.Add(inputPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(headerPanel);
        }

        private Button MakeButton(string text, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Width = 140,
                Height = 30,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9f),
                Cursor = Cursors.Hand,
                Margin = new Padding(2)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(path))
                    PlaySound(path, IntPtr.Zero, 0x00020000 | 0x00000001);
            }
            catch { }
        }

        private void ShowWelcome()
        {
            AppendDivider();
            AppendBot("Welcome to InnocentGuard — your Cybersecurity Awareness Assistant.");
            AppendBot("I can chat, run a cybersecurity quiz, manage your tasks, and more!");
            AppendDivider();
            AppendBot("Before we start, what's your name?");
            askedForName = true;
        }

        private void SendMessage(string message)
        {
            inputBox.Text = message;
            OnSend(this, EventArgs.Empty);
        }

        private void OnSend(object sender, EventArgs e)
        {
            string input = inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUser(input);
            inputBox.Clear();

            if (askedForName)
            {
                userName = input;
                engine.SetUserName(userName);
                askedForName = false;
                AppendDivider();
                AppendBot($"Nice to meet you, {userName}! I'm ready to help you stay cyber-safe.");
                AppendBot("Type 'help' to see everything I can do, or just start chatting!");
                AppendDivider();
                return;
            }

            string response = engine.GetResponse(input);
            AppendBot(response);
        }

        private void AppendUser(string message)
        {
            chatBox.SelectionStart = chatBox.TextLength;
            chatBox.SelectionColor = Color.FromArgb(255, 210, 80);
            chatBox.AppendText($"\n  👤 {userName}: {message}\n");
            chatBox.SelectionColor = chatBox.ForeColor;
            chatBox.ScrollToCaret();
        }

        private void AppendBot(string message)
        {
            chatBox.SelectionStart = chatBox.TextLength;
            chatBox.SelectionColor = Color.FromArgb(0, 200, 150);
            chatBox.AppendText($"\n  🤖 InnocentGuard: ");
            chatBox.SelectionColor = Color.FromArgb(200, 210, 220);
            chatBox.AppendText($"{message}\n");
            chatBox.SelectionColor = chatBox.ForeColor;
            chatBox.ScrollToCaret();
        }

        private void AppendDivider()
        {
            chatBox.SelectionStart = chatBox.TextLength;
            chatBox.SelectionColor = Color.FromArgb(50, 80, 70);
            chatBox.AppendText("\n  ────────────────────────────────────────────\n");
            chatBox.SelectionColor = chatBox.ForeColor;
        }
    }
}