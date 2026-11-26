\# National Park Management System



A full-stack web application built using \*\*ASP.NET Core MVC and ASP.NET Core Web API\*\* to manage national parks, trails, appointments, and user interactions.



\## 🚀 Technologies Used



\- \*\*Backend:\*\* C#, ASP.NET Core Web API, ASP.NET Core MVC

\- \*\*Database:\*\* Microsoft SQL Server

\- \*\*ORM:\*\* Entity Framework Core

\- \*\*Authentication:\*\* ASP.NET Core Identity, JWT

\- \*\*Frontend:\*\* HTML, CSS, Bootstrap, JavaScript, jQuery

\- \*\*Integrations:\*\* Stripe, Twilio, Email Services

\- \*\*Tools:\*\* Visual Studio, Git, GitHub



\## ✨ Key Features



\- National park management

\- Trail management

\- Appointment and booking management

\- RESTful API architecture

\- User authentication and authorization

\- Role-based access control

\- Database operations using Entity Framework Core

\- Payment integration using Stripe

\- Email and SMS notification integrations

\- MVC frontend communicating with the Web API



\## 🏗️ Project Structure



```text

Project\_APIs/

│

├── NationalParkWebApp/

│   ├── Controllers/

│   ├── Models/

│   ├── Repository/

│   ├── Views/

│   └── wwwroot/

│

├── Project\_APIs/

│   ├── Controllers/

│   ├── Models/

│   ├── Data/

│   ├── Repository/

│   └── Migrations/

│

└── Project\_APIs.slnx

```



\## ⚙️ Getting Started



\### Prerequisites



\- Visual Studio

\- .NET SDK compatible with the projects

\- Microsoft SQL Server

\- Git



\### Installation



1\. Clone the repository:



```bash

git clone https://github.com/tusharsharma898/NationalPark-Management-System.git

```



2\. Open `Project\_APIs.slnx` in Visual Studio.



3\. Configure the database connection string using .NET User Secrets.



4\. Configure the required credentials for Stripe, JWT, email and Twilio using User Secrets.



5\. Apply Entity Framework Core migrations:



```bash

dotnet ef database update

```



Run this command for the relevant project containing the migrations, with the correct startup project if necessary.



6\. Configure both projects as startup projects in Visual Studio and run the application.



\## 🔐 Configuration



Sensitive information is intentionally excluded from source control.



Use .NET User Secrets for local development to configure:



\- Database connection string

\- JWT signing key

\- Stripe secret key

\- Email credentials

\- Twilio credentials



Never commit passwords, API keys, tokens, or connection strings containing credentials.



\## 👨‍💻 Developer



\*\*Tushar Sharma\*\*



\- GitHub: https://github.com/tusharsharma898

\- LinkedIn: https://www.linkedin.com/in/tushar-tushar-9786203a3/



\---



⭐ If you find this project interesting, feel free to explore the repository!

