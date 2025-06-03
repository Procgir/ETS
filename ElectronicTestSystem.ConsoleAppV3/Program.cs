using ElectronicTestSystem.ConsoleApi.Clients.Abstractions.Tests;
using ElectronicTestSystem.ConsoleApi.Clients.Groups;
using ElectronicTestSystem.ConsoleApi.Clients.Marks;
using ElectronicTestSystem.ConsoleAppV3;

var baseApiUrl = "http://localhost:5100";

new CalculateMarkApp(
    new MarksHttpClientApi(baseApiUrl, new HttpClient()),
    new TestsHttpClientApi(baseApiUrl, new HttpClient()),
    new GroupsHttpClientApi(baseApiUrl, new HttpClient())).Run(args);