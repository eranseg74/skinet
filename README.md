`dotnet --info` -> Get information about ASP.Net installed versions in our computer

`dotnet -h` -> Displays all ASP.Net possible command + explanations

`dotnet new list` -> Displays a list of all project types that can be created in ASP.Net. In this project I'm using the webapi in order to create a RestAPI controllers with endpoints that will be responsible for getting requests from the client, get the data from a database and give it to the client

CREATING an ASP.Net Core WebAPI:
First we need to create a solution file which is quite like a container that will contain all the libraries and projects in our solution

`dotnet new sln` -> Creates a new solution. The name of the solution is derived from the folder from which the creation command was issued

`dotnet new webapi -o <ProjectName> -controllers` -> Creates a webapi project. The -o will create a folder named by the text in the [ProjectName] in which the new project will be hosted. The -controllers is a switch that tells the ASP.Net to create a framework that is based on controllers endpoints

`dotnet new classlib -o <libraryName>` -> Creates a new folder named by the text in the <ProjectName>. We will use the [Core] folder/library as a folder that will hold all the core entities of the solution. We will also create an [Infrastructure] library that will contain all the controllers in the solution. The idea is that when the client sends a request it will be redirected to the [Infrastructure] level. This level will translate the request to HTTP requests that will be sent to the [Core] level. The Core level will be responsible for the interaction with the database

`dotnet sln add <Project / Library name>` -> Adding the projects inside the created solution.

`dotnet sln list` -> Displays all the libraries and projects in the solution

DEFINING THE DEPENDENCIES BETWEEN PROJECTS IN THE SOLUTION
Defining a dependency for a project to be dependent on another project means that the dependent project will have access to the functionality that the other project has. For example - The Infrastructure project relies on the Core project so the Infrastructure code will be able to use the functionality defined in the code inside the Core library

1) Need to go the the folder that is dependent - For example, The API is dependent on the Infrastructure so:
2) `cd <Dependent project>` -> In our project when defining the dependency from API on the Infrastructure - `cd API`
3) `dotnet add reference <project/library location>` (In our project - `dotnet add reference ../Infrastructure`)

BUILDING THE APP - SERVER SIDE
// CREATING THE ENTITIES

This is done in the Core folder. Creating the Entities folder in the Core folder. In the Entities folder - creating the Products class which contains all the required properties for a product.

CREATING THE ENTITY FRAMEWORK CLASSES (Infrastructure folder)
In this phase few additional packages are required from the NUGET package manager:
Important! Always make sure that the version of the EntityFramework matches the major version of the .Net runtime youare using!!!
- `Microsoft.EntityFrameworkCore.SqlServer` -> Install it in the Infrastructure project
- `Microsoft.EntityFrameworkCore.Design` -> Install it in the API project
  
CREATING A DATABASE SERVER WITH SQL SERVER
We run the SQL SERVER through Docker. The docker-compose.yml file defines the container properties. Also in the docker settings we define [2] CPUs, [8GB] memory, [256GB] storage on hard disk, and [1GB] memory for swap files. SQL Server requires a complex password containing capital and non capital letters, numbers and special characters. Not all special characters are excepted ([@] is Ok) and **minimum of 6 (or 8) letters**.
Docker comes with its own networking so we need to specify the external port that is used as the access point to docker, and the second port is used internaly between containers

Running Docker Compose - use the following command (must be in the folder level that contains the docker-compose.yml file):
docker compose up -d -> runs docker
docker compose down -> stop running docker

CREATING MIGRATIONS
To execute migrations we must install the dotnet-ef Nuget Package. Going to the Nuget.org site and finding the dotnet-ef package and installing it globaly (in case it is not installed yet). The version must match the main version that is installed and used in the application (see the targetversion attribute in the csproj file):
`dotnet tool install --global dotnet-ef --version 10.0.0`

Adding a new migration:
`dotnet ef migrations add <migrationName> -s <starterProject> -p <DbContextLocation>`
In our case:
dotnet ef migrations add InitialCreate -s API -p Infrastructure

To remove a migration use the [remove] keyword with the exact parameters:
dotnet ef migrations remove InitialCreate -s API -p Infrastructure

After the migrations we can create / update the database. If the database is not yet created it will be created following this command. It will use the last migration to create / update the database:
dotnet ef database update -s API -p Infrastructure (from the skinet folder)

**[POSTMAN]**
Working with variables in postman.
Clicking on the collection name displays the collection tabs. On the Variables tab it is possible to define a name for the variable and its value and then use it in all the collection's requests
It is also possible to create scripts that will run on every request (can be done also in the request level) such as saving tokens as variables and then using them in other requests.
In the Console view we can see the actual request with all the values of the variables

LOADING TO GITHUB

1) Create a new repository
2) Create a gitignore file using the dotnet command to exclude all the unnesaccary dotnet files using the command: `dotnet new gitignore`
3) To add files to the gitignore, right-click on the file and select the `add to .gitignore` option
4) After creating a new repository on github synchronize between the local files and github by running the following command on the root folder (skinet in this case): `git remote add origin <gitUrl>/<RepoName>.git`. In our case it will be git remote add `origin https://github.com/eranseg74/Skinet.git`
5) Add changes
6) Commit
7) Push
