# CodeBro - Collaborative Code Editor

🚀 **CodeBro** is a collaborative programming environment built on .NET and SignalR, allowing multiple users to edit code together in real time. It's an ideal tool for development teams, pair programming sessions, or interactive coding lessons.

<p align="center">
  <img src="https://github.com/user-attachments/assets/79c59469-c696-43fe-badb-eff2ddf90dd7" width="400">
</p>

---

## 📌 Key Features

✅ **Real-time collaborative editing** - Multiple users can edit the same code simultaneously.  
✅ **Session-based architecture** - Each user connects to a unique session.  
✅ **SignalR integration** - Fast transmission of changes between users.  
✅ **Dark/Light theme** - Customizable UI for an optimal coding experience.  
✅ **Built with .NET 6+** - Robust and scalable server.

---

## 🚀 **How to Run the Project?**

### 1️⃣ **Set up and start the server**
Make sure you have **.NET 6+** installed, then run:

```sh
dotnet run
```

The server will start by default at `http://localhost:5000/`.

### 2️⃣ **Launch the client**
The WPF client will automatically connect to the SignalR server.

---

## ⚙️ **Technologies Used**
- **Backend:** .NET 6+, ASP.NET Core, SignalR
- **Frontend:** WPF (RichTextBox for code editing)
- **Communication:** WebSockets via SignalR

---

## 🔧 **Project Architecture**

```
📂 CodeBro
 ├── 📁 CodeBro.Server (Server .NET)
 │   ├── CodeHub.cs (SignalR Hub for collaboration)
 │   ├── Program.cs (Server startup)
 │   └── appsettings.json
 ├── 📁 CodeBro.Client (WPF Client)
 │         ├── MainWindow.xaml (Editor UI)
 │         ├── MainWindow.xaml.cs (UI Logic)
 │         ├── 📁 CodeBro.Client (WPF Client)
 │              ├── Token.cs
 │              ├── Lexer.cs (Syntax highlighting)
 │              └── Parser.cs
 │   └── Settings.config
 └── README.md
```

---

## 🛠 **Future Improvements**
🔹 **Authentication and private sessions** - Allow users to create password-protected sessions.  
🔹 **Auto-save feature** - Preserve version history for each session.  
🔹 **Support for multiple languages** - Advanced lexer for Python, JavaScript, C#, etc.  
🔹 **Integrated chat between users** - Live communication channels for teams.  
🔹 **AI-powered code suggestions** - Intelligent code generation and completion.  
🔹 **Cloud hosting** - Deploy directly on Azure/AWS for global access.  

---

⚡ **Code together with CodeBro!** ⚡

