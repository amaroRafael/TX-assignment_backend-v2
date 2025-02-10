# TX-assignment_backend-v2

## Pipeline Setup

This README will guide you through setting up a CI/CD pipeline using GitHub Actions.

### Prerequisites

- A GitHub repository
- Basic knowledge of Git and GitHub
- GitHub Actions enabled for your repository

### Steps to Set Up the Pipeline

1. **Clone your repository:**
   ```sh
   git clone https://github.com/amaroRafael/TX-assignment_backend-v2.git
   ```
2. **Create a new GitHub Actions workflow file:**

Navigate to your repository on GitHub, and create a new file in the .github/workflows/ directory, name it deploy.yml.

3. **Add the following content to deploy.yml:**

  ```sh
  name: Build and deploy .NET Core application to Web App
  on:
    push:
      branches:
      - master
  env:
    AZURE_WEBAPP_NAME: tx-assisgnment-01
    AZURE_WEBAPP_PACKAGE_PATH: TX.RMC.Api/publish
    CONFIGURATION: Release
    DOTNET_CORE_VERSION: 8.0.x
    WORKING_DIRECTORY: TX.RMC.Api
    WORKING_TEST_DIRECTORY: TX.RMC.UnitTests
  jobs:
    build:
      runs-on: ubuntu-latest
      steps:
      - uses: actions/checkout@v4
      - name: Setup .NET SDK
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
      - name: Restore
        run: dotnet restore "${{ env.WORKING_DIRECTORY }}"
      - name: Build
        run: dotnet build "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-restore
      - name: Test
        run: dotnet test "${{ env.WORKING_DIRECTORY }}" --no-build
      - name: Restore UnitTests
        run: dotnet restore "${{ env.WORKING_TEST_DIRECTORY }}"
      - name: Build UnitTests
        run: dotnet build "${{ env.WORKING_TEST_DIRECTORY }}" --configuration Debug --no-restore
      - name: Run UnitTests
        run: dotnet test "${{ env.WORKING_TEST_DIRECTORY }}" --configuration Debug --no-build
      - name: Publish
        run: dotnet publish "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-build --output "${{ env.AZURE_WEBAPP_PACKAGE_PATH }}"
      - name: Publish Artifacts
        uses: actions/upload-artifact@v4
        with:
          name: webapp
          path: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    deploy:
      runs-on: ubuntu-latest
      needs: build
      steps:
      - name: Download artifact from build job
        uses: actions/download-artifact@v4
        with:
          name: webapp
          path: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.tx_assisgnment_01_SPN }}
      - name: Deploy to Azure WebApp
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          package: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}  
  ```

4. **Store credentials as secrets:**
Create a service principal in Azure, then store its credentials as secrets in your GitHub repository, which can then be accessed by the "Azure Login" action within your GitHub Actions workflow to authenticate with Azure.
Copy the credential from the service principal in Azure and store them as secrets within your GitHub repository under "Settings > Secrets and variables > Actions".

### Example: Triggering a Deployment
Every time you push code to the main branch, this GitHub Action will automatically run the pipeline and deploy the project.

1. **Make changes to your project and commit:**

```sh
git add .
git commit -m "Your commit message"
```

2. **Push changes to the main branch:**
```sh
git push origin main
```


## Run & Test Instructions (Local):
On TX.RMC.Api project create a appsettings JSON file for Development environment. 
File name:
  ```sh 
    appsettings.Development.json
  ```

Add the settings:
  ```sh
    "ConnectionStrings": {
      "MongoDBConnection": "<<REPLACE WITH MONGO DB CONNECTION STRING>>"
    },
    "MongoDBDatabase": "<<REPLACE WITH DATABASE NAME>>",
  ``` 


  ## Time spent on task:
     Read documents: 20 mins
     Implement Api (Controllers and Authentication/Authorization): 1 hour
     Implement business logic: 2 hour
     Implement MongoDB data service: 5 hours 
      **I worked 2 hours to create an account at Mongo DB Atlas, read documentation and connect to database using MongoDB.Driver for .NET.
        After that I worked 3 hours trying to make Mongo DB + EF Core working it, but it was throwing exception and I couldn't find the reason.
        It was my first time working with Mongo DB.**
     Make tests: 1 hour
     Refactor: 1 hour
     Documentation: 3 hours