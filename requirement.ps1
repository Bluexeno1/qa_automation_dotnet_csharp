$env:PATH="C:\Users\account\Documents\portable-dotnet;$env:PATH"
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
dotnet clean
dotnet build

# Run this command .\requirement.ps1 