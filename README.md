# Interject Dotnet Data Api

### A Dotnet version of the Data Api used to connect Interject to your data sources. This can be deployed within a client's network behind a firewall. Or it can be made public as client needs vary.

---

- ### **[How to setup the API for development](#setup-for-dev)**
- ### **[How to add a new controller](#new-controller)**
- ### **[Working with the request pipeline](#request-pipeline)**

# <a name="setup-for-dev">How to setup the API for development</a>

Steps: _using Visual Studio Code_

1. Clone the repo.
2. You will need dotnet sdk installed.
3. You will also need the dotnet C# extension listed in the .vscode extensions.json file.
4. Navigate to the interject-dotnet-api directory and execute the restore command.
   > dotnet restore
5. You should now be able to now run the application:

   - For debugging: F5 or...
     <img src="./ReadmeSrc/VSCodeDebug.png">

   - Without ability to hit breakpoints
     > dotnet run

6. This should use Kestrel serve the application locally at http://localhost:5000
7. You can test by sending a request to the status controlelr http://localhost:5000/api/vi/status

# <a name="new-controller">How to add a new controller</a>

1. Create and new file in the Controllers directory using the naming convention {Name}Controller.cs.
2. You can use code snippets to scaffold the controller. The prefix is 'contr'.
   > contr > TAB

# <a name="request-pipeline">Working with the request pipeline</a>
