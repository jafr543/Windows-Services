# 🗂️ File Monitoring Windows Service

A C# Windows Service that monitors a source folder and automatically processes newly created files. ⚙️

## ✨ Features

- 📁 Monitor a folder using FileSystemWatcher
- ⏳ Check file readiness before processing
- 🔄 Retry mechanism for files that are still being created
- 🆔 Rename files using a unique GUID
- 📦 Move processed files to a destination folder
- 📝 Logging service events
- ⚙️ Dynamic configuration using App.config
- 🪟 Install and run as a Windows Service

## 🛠️ Technologies

- C#
- .NET Framework
- Windows Services
- FileSystemWatcher
- File I/O
- App.config

## 🔄 Workflow

Source Folder → 🔍 Detect File → ⏳ Check Readiness → 🆔 Generate GUID → 📦 Move File → Destination Folder

---

🎯 A practical project built to learn and apply Windows Services with C#.

## Author
- Jafr Jaber
- GitHub(https://github.com/jafr543)
