/*
Создать калькулятор оценок на основе пройденных тестов пользователями
Оценка - число от 2 до 5, которая рассчитывается как: все правильные ответы в тесте - 100%, тогда 2 - < 61% 3 - >=61%, 4 - >= 75%, 5 - >= 91%
- вход: тест, ответы пользователя
- выход: оценка
*/

using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleAppV1;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Infrastructure;
using ElectronicTestSystem.Infrastructure.Groups.Converters;
using ElectronicTestSystem.Infrastructure.Groups.Repositories;
using ElectronicTestSystem.Infrastructure.Tests.Converters;
using ElectronicTestSystem.Infrastructure.Tests.Repositories;

new CalculateMarkApp(
 new MarkCalculatorFacade(
  new GroupService(new GroupsFileRepository(FileRepositoriesFileNames.Groups, new GroupConverter())), 
  new TestService(new TestsFileRepository(FileRepositoriesFileNames.Tests, new TestConverter()), null), //TODO: add UoW 
  new MarkCalculator())).Run(args);
