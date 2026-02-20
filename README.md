# Real-Time Chat API (Backend)

A scalable, secure real-time chat backend built with **ASP.NET Core** and **SignalR**, designed for production-grade messaging with modern cloud integrations.

This project demonstrates:
- Real-time bidirectional communication
- Secure user authentication with email verification
- Horizontal scaling via Redis backplane
- Asynchronous processing with Azure Service Bus
- Persistent storage & file handling with MongoDB + Azure Blob

**Backend-only** — no frontend included. Test with SignalR .NET client, Postman (WebSocket), or custom clients.

## Features

- **User Authentication**
  - Sign-up with email + password
  - Email verification via one-time code (6-digit)
  - Login with JWT bearer token
  - Secure password hashing (BCrypt)

- **Email Processing**
  - Asynchronous email sending using **Azure Service Bus** queues
  - Decoupled sender → worker pattern for reliability & scalability

- **Real-Time Messaging (SignalR)**
  - Private 1:1 chats
  - Group/room chats
  - Instant message delivery with WebSockets
  - Typing indicators (optional extension point)
  - User presence / online status

- **Message Persistence**
  - All messages stored in **MongoDB** (NoSQL)
  - Paged message history retrieval (private & group)
  - Indexed for fast queries by chatroom/user + timestamp

- **File Uploads**
  - Support for attachments/images/files
  - Stored securely in **Azure Blob Storage**
  - References saved in MongoDB documents

- **Scaling & Infrastructure**
  - **Redis Pub/Sub** as SignalR backplane → supports multiple server instances
  - Dockerized services (app + Redis + MongoDB)
  - Ready for Azure deployment (App Service / Container Apps + Service Bus + Blob + Cosmos DB/Mongo)

## Tech Stack

| Category              | Technology                          | Purpose                              |
|-----------------------|-------------------------------------|--------------------------------------|
| Framework             | ASP.NET Core (.NET 10 / latest)     | High-performance backend API         |
| Real-time             | SignalR                             | WebSockets + fallback transports     |
| Scaling Backplane     | Redis (Pub/Sub)                     | Multi-instance message synchronization|
| Database              | MongoDB                             | Flexible message & user storage      |
| Messaging Queue       | Azure Service Bus                   | Async email verification processing  |
| File Storage          | Azure Blob Storage                  | Secure, scalable file attachments    |
| Authentication        | JWT Bearer                          | Secure token-based auth              |
| Containerization      | Docker                              | Consistent dev/prod environments     |
| Other                 | BCrypt, MailKit, Serilog            | Security, mail sending, logging      |

## Architecture Overview

[Clients (Console / Mobile / Test Tools)]
│
│ (HTTPS + WebSockets)
▼
[ASP.NET Core API + SignalR Hub]
├── JWT Auth
├── Azure Service Bus → Email Worker (verification codes)
├── MongoDB (users, messages, chat metadata)
├── Azure Blob Storage (file uploads)
└── Redis Pub/Sub Backplane (for SignalR scaling)
