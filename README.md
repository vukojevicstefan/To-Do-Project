To-Do Project

A simple To-Do application built with React, Vite, and Azure Data Studio. 
The backend uses an ORM to connect to a database defined in appsettings.json. 
Users can have multiple To-Do lists, and each To-Do list can have multiple task items.


Table of Contents:
-Getting Started
-Architecture Overview
-Key Decisions & Compromises
-Screenshots

Getting Started
These instructions will get the project running locally.

Prerequisites:

Make sure you have the following installed:
Node.js and npm
Download: https://nodejs.org/
Verify installation:
node -v
npm -v

Vite
Install globally (optional):
npm install -g create-vite

Azure Data Studio (for database management)
Download: https://learn.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio

.NET SDK
Download: https://dotnet.microsoft.com/download

Installing & Running Locally

Clone the repository
git clone https://github.com/YOUR-USERNAME/To-Do-Project.git
cd To-Do-Project

Install frontend dependencies
npm install


Run the React app
npm run dev


Configure database
Update the appsettings.json with your connection string.

Open Azure Data Studio to verify your database.
Run migrations if applicable (depends on your ORM).

Architecture Overview

The project follows a simple relational data structure:

User
Each user can have many To-Do lists.

ToDoList
Each To-Do list can have many TaskItems.

TaskItem
Represents individual tasks inside a To-Do list.

The frontend is built with React using Vite for fast bundling.
The backend uses an ORM (like Entity Framework or similar) and connects to the database via appsettings.json.

Diagram:
User
 └── ToDoLists
       └── TaskItems

       
Key Decisions & Compromises

Default Azure Data Studio
Chosen for lightweight database management and easy integration with SQL databases.
Compromise: lacks some advanced features of full SQL Server Management Studio.

ORM for database access
Simplifies database queries and migrations.
Compromise: less control over raw SQL optimizations.

Vite + React
Faster build times than Create React App.
Lightweight and modern dev tooling.
Compromise: requires slightly more manual setup for some React features (like environment variables).

No Docker

Simplifies setup for local development.
Compromise: deployment is manual instead of containerized.




