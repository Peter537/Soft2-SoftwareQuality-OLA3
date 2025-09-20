# Software Review – TodoApi

Denne review dækker funktionalitet, ikke-funktionelle egenskaber og best practices for vores Task Service.

## Funktionel korrekthed

- Controlleren [TodoApi.Controllers.TodoController](TodoApi/Controllers/TodoController.cs) håndterer CRUD for todo-items via repository-abstraktionen [TodoApi.Repositories.ITodoRepository](TodoApi/Repositories/ITodoRepository.cs). 
- Logikken er enkel og korrekt: 
  - Returnerer Ok med liste for GetAll.
  - Returnerer NotFound, hvis et id ikke findes i Get.
  - Returnerer BadRequest ved ugyldig model (ModelState) i Create/Update.
  - Returnerer BadRequest, hvis status er "Invalid" (forretningsregel).
  - Returnerer NoContent ved Delete.
- Modellen [TodoApi.Models.TodoItem](TodoApi/Models/TodoItem.cs) bruger data-annoteringer (fx Required), så validering fanger tomme felter. 
- Unit tests i [TodoApi.Tests/Controllers/TodoControllerTests.cs](TodoApi.Tests/Controllers/TodoControllerTests.cs) dækker de vigtigste grene. Coverage-rapporten ligger her: [documentation/TestResults/CoverageReport/index.html](documentation/TestResults/CoverageReport/index.html).

## Ikke-funktionelle forhold

- Performance: Den in-memory-implementering [TodoApi.Repositories.InMemoryTodoRepository](TodoApi/Repositories/InMemoryTodoRepository.cs) er hurtig for små datasæt. Til produktion foreslås database og evt. caching. Bemærk at trådsikkerhed kan forbedres (fx ConcurrentDictionary).
- Maintainability: Koden er overskueligt struktureret (Controllers, Models, Repositories). Dependency Injection via interface gør det nemt at bytte implementation. Navngivning og mapper er konsistente.
- Testbarhed: Høj. Repository er mockbart, og controlleren kan testes uden webhost/HTTP.

## Adherence to best practices

- God separations of concerns: Controller er tynd, repository kapsler adgang.
- Async/await bruges korrekt.
- Style-regler styres via [TodoApi/stylecop.json](TodoApi/stylecop.json) og SonarCloud-opsætning i dokumentationen.
- Forbedringsmuligheder:
  - Flere XML-kommentarer og konsistente svarobjekter (problem-details).
  - Returnere 201 Created med Location ved Create.
  - Centraliseret modelvalidering/filter.
  - CancellationToken på asynkrone metoder.
  - Trådsikkerhed i repository.

## Forbedringer efter review

- Rettede analyserapporterede problemer (null-dereference og ubrugte felter). Se "ingen fejl"-rapport: [documentation/static-analysis-report_no_errors.txt](documentation/static-analysis-report_no_errors.txt).
- Strammet tests, så cases for NotFound/BadRequest er dækket.
- Dokumenteret kørsel af tests og coverage i [README.md](README.md).

Samlet vurdering: Koden gør det, den skal, er let at vedligeholde og testvenlig. Med små forbedringer omkring HTTP-kontrakter, trådsikkerhed og dokumentation står løsningen stærkt.
