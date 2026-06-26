## Part 3 added
# InnocentGuard Part 3 - Final POE
Cybersecurity Awareness Chatbot built in C# using windows Forms.

Description:
InnocentGuard is a Windows Forms chatbot application built in C# that helps
the users to learn about cybersecurity in a conversational way.
The bot covers topics like phishing, passwords, malware, VPNs, and safe
browsing. It also includes a cybersecurity quiz, a task assistant, an NLP
simulation, and an activity log.


Software Required:
Windows 10 or 11
Visual Studio 2022
.NET 8.0 (Windows)
MySQL Community Server 9.x (optional — app works without it)
MySql.Data NuGet package (installed automatically via .csproj)

How to Open and Run the Project:
Download or clone this repository
Open Visual Studio 2022
Click File → Open → Project/Solution
Navigate to the InnocentGuardPart3 folder
Select InnocentGuardPart3.csproj and click Open
Wait for Visual Studio to restore the NuGet packages automatically
Press F5 to build and run the application

The Database Setup Instructions (MySQL):
I used the SQL Server Database Project to create the database and the table. I also used a connection string in the TaskItem.cs to connect the methods: add, update, delete and completed to the database.

How to Use the Task Assistant:
The Task Assistant lets you manage cybersecurity-related tasks by:
Adding a task:
Type add task or remind me to enable 2FA
The bot will ask for a title and an optional reminder timeframe.

Viewing all tasks:
Type view tasks or click the My Tasks button at the top.

Completing a task:
Type complete task 1 (replace 1 with the task number)

Deleting a task:
Type delete task 1 (replace 1 with the task number)


How to Access the Quiz / Mini-Game:
Type start quiz in the chat or click the Start Quiz button at the top.
The quiz has 12 questions covering phishing, passwords, malware,
2FA, VPNs, and social engineering
Questions are a mix of multiple choice and true/false
After each answer the bot tells you if you were right or wrong
and explains why
Your final score is shown at the end with personalised feedback

How to Test the NLP Simulation:
The NLP simulation recognises different ways to phrase the same request.
Try these examples to see it in action:

Instead of "add task" try:
create a task
new task
remind me to update my password
I want to add a task

Instead of "start quiz" try:
quiz me
test me
I want to take a quiz


Instead of "view tasks" try:
show my tasks
what are my tasks
list tasks


How to View the Activity Log

Type show activity log in the chat or click the Activity Log button.

The log shows the last 10 actions taken during the session, including:
Tasks added, completed, or deleted
Quiz started or completed
NLP commands recognised


Login Details / Important Notes

No login is required to use the chatbot
The greeting.wav file must be in the same folder as the .exe
for the voice greeting to play
The app targets .NET 8.0 Windows — make sure it is installed


Video Presentation Link
https://youtu.be/4OhToo9Efto
