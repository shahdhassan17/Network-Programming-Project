# Multi-threaded Compression Server

## Project Description

This project is a TCP-based Multi-threaded Compression Server developed using C# Windows Forms and Socket Programming.

The system consists of:

- Server Application
- Client Application with GUI

The client sends a file to the server.
The server receives the file, compresses it using GZip compression, then sends the compressed file back to the client.

The server supports multiple clients simultaneously using Multi-threading.

---

# Technologies Used

- C#
- Windows Forms
- TCP Socket Programming
- Multi-threading
- GZip Compression
- File Handling

---

# Features

## Server
- Accepts multiple clients simultaneously
- Receives files from clients
- Compresses files using GZipStream
- Sends compressed files back to clients
- Displays logs using RichTextBox

## Client
- Connects to the server
- Allows user to browse and select files
- Sends files to the server
- Receives compressed files
- Saves compressed files locally
- Displays logs using RichTextBox

---

# How It Works

## Client Side
1. User selects a file.
2. Client sends:
   - File Size
   - File Data
3. Client waits for compressed file from server.
4. Client receives:
   - Compressed File Size
   - Compressed File Data
5. Client saves compressed file.

---

## Server Side
1. Server waits for client connections.
2. Each client is handled in a separate thread.
3. Server receives:
   - File Size
   - File Data
4. Server compresses the file using GZipStream.
5. Server sends:
   - Compressed File Size
   - Compressed File Data

---

# Project Structure

```txt
Server Project
│
├── Form1.cs
├── Program.cs
└── Form1.Designer.cs

Client Project
│
├── Form1.cs
├── Program.cs
└── Form1.Designer.cs
