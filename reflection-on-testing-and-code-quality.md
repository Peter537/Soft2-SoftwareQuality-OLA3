# Refleksion over test og kodekvalitet

Statisk analyse og coverage har haft stor betydning for vores arbejde. Vi brugte StyleCop og SonarCloud i stedet for PMD, og Coverlet/ReportGenerator i stedet for JaCoCo. Pointen er den samme: værktøjerne finder fejl og inkonsistenser tidligt. Vi fangede bl.a. navngivning, ubrugte felter og en mulig null-dereference i controlleren. Det gjorde koden mere ensartet og let at læse, og vi sparede tid ved ikke at opdage det sent i forløbet.

Mocking har været nøglen til gode unit tests. Vi mocker [TodoApi.Repositories.ITodoRepository](TodoApi/Repositories/ITodoRepository.cs), så vi kan teste controller-logik isoleret i [TodoApi.Controllers.TodoController](TodoApi/Controllers/TodoController.cs) uden webhost/HTTP eller rigtig storage. Det gør testene hurtige, stabile og nemme at skrive. Samtidig kan vi nemt skabe kanttilfælde (fx "id findes ikke" eller "status er Invalid") uden at bygge tung opsætning.

Både code reviews og software reviews giver værdi, men på forskellige niveauer. Code review fanger de små ting: læsbarhed, navngivning, simple bugs. Software review ser på helheden: gør API'et det rigtige, er performance ok, er arkitekturen enkel at udvide, og følger vi best practices. Kombinationen løfter kvaliteten, fordi vi både ser detaljerne og det store billede.

Equivalence Partitioning og Boundary Value Analysis påvirkede vores testdesign. Vi delte input i meningsfulde grupper: gyldig vs. ugyldig model (ModelState), fundet vs. ikke fundet id, samt status "gyldig" vs. "Invalid". Grænserne handler om at sikre tests for begge sider af en betingelse: fx tomt/udfyldt felt (Required), og "første gang vi rammer NotFound" vs. "fundet og Ok". Selvom vi ikke har hårde længdegrænser i modellen, tænker vi stadig i grænser, når vi vælger eksempler.

Alt i alt fandt vi og rettede problemer tidligere, forbedrede læsbarhed og gjorde controlleren mere robust. Coverage-rapporten [documentation/TestResults/CoverageReport/index.html](documentation/TestResults/CoverageReport/index.html) gav os indsigt i, hvor vi manglede tests, og statisk analyse holdt os på sporet med ens regler på tværs af filer.
