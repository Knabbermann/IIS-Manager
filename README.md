# IIS Manager 
Welcome to the README file for the IIS Manager project. This file contains important information to help developers get started with the project.

## Project Overview
The IIS Manager project is an web application for managing Internet Information Services (IIS) on Windows servers. The application allows users to manage their app pools and monitoring one or more IIS Servers, and it also features a built-in user management system, enabling users to not only manage app pools but also create and manage user accounts.

## Technologies
- Programming Language: C#, CSS, HTML
- Framework: .NET 7.0 & EntityFrameworkCore 7.0
- Web: Razor Pages, Bootstrap
## Prerequisites
To run and further develop the project, you'll need:

- Visual Studio (recommended: Visual Studio 2022)
- .NET 7.0 SDK
- Basic knowledge of C# and web development
- MSSQL Server
## Installation
- Clone this repository to your local workspace.
- Open the project in Visual Studio.
## Configuration
- Create a configuration file named `appsettings.json` in the root directory of the `IIS-Manager.Web` Project. The Connectionstring should be named `WebDbContextConnection`.
- It should look like this:
- ```{
  "ConnectionStrings": {
    "WebDbContextConnection": "Server=myServerAddress;Database=myDatabase;User Id=myUsername;Password=myPassword;Trusted_Connection=False;Encrypt=False;"
  },
- Open a command prompt or terminal and navigate to the root directory of the `IIS-Manager.Web` project.
- Run the following command to apply the database migrations and create the database:
- `dotnet ef database update`
## Running the Application
- Make sure you have the required dependencies installed.
- Start the application in Visual Studio.
## Contributors
We welcome contributions to improve this project. If you have suggestions or want to report issues, please open an issue or create a pull request.

## License
This project is licensed under the CC-BY-SA 4.0 License.

