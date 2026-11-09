# AccessAudit — laboratorium 3

Backend do projektu audytowania dostępności stron, dopasowany do przekazanej tablicy Event Storming.
Środowisko: Visual Studio z obsługą .NET 10, SDK 10.0.403 lub nowszy SDK 10.0.

## Uruchomienie w Visual Studio

1. Otwórz `AccessAudit.sln` (Plik → Otwórz → Projekt/rozwiązanie).
2. Poczekaj na przywrócenie pakietów NuGet.
3. W Eksploratorze rozwiązań kliknij prawym przyciskiem `AccessAudit.ProjectManagement.Presentation` → **Ustaw jako projekt startowy**.
4. Wybierz profil `http`, potem F5. API działa pod `http://localhost:5240`.
5. Otwórz plik `AccessAudit.http` z głównego katalogu rozwiązania (Plik → Otwórz → Plik).
6. Wykonaj pierwsze żądanie, skopiuj `id` z odpowiedzi do zmiennej `projectId`, wykonuj dalsze żądania.
7. Wybierz Test → Eksplorator testów → Uruchom wszystkie testy.

Odpowiedź JSON na stronie głównej jest prawidłowa: to backend API, bez graficznego frontendu.
Definicja OpenAPI jest pod `/openapi/v1.json` w środowisku Development. Nie dodano Swagger UI.
W Postmanie można wykonać te same żądania; port i JSON są w pliku `.http`.

Dodano współdzielone profile `Project Management` i `All three services` w `AccessAudit.slnLaunch`.
Drugi uruchamia trzy API na portach 5240, 5241 i 5242. Jeśli profile nie są dostępne w Twojej
konfiguracji Visual Studio, uruchom sam ProjectManagement opisanym wyżej sposobem.
[Opis profili w dokumentacji Microsoft](https://learn.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects?view=visualstudio).

## Co zostało przygotowane

- Trzy niezależne konteksty z tablicy: Project Management, Audit Engine, Compliance and AI.
- W każdym Domain, Application i Presentation. Dziewięć projektów produkcyjnych oraz dwa testowe.
- W pełni rozwinięty zakres Lab 3 w ProjectManagement: dwa agregaty, wyjątki, serwis, repozytoria, DTO, testy i kontroler.
- Domain nie ma żadnych referencji ani pakietów NuGet. Application odwołuje się tylko do własnego Domain.
- Nie ma referencji pomiędzy kontekstami. Pozostałe dwa konteksty są szkieletami.

## Jak działa pojedyncza operacja

`POST /api/projects` → ProjectsController → ProjectManagementService → konstruktor Project
→ walidacja URL → IProjectRepository.SaveAsync → ProjectDto → HTTP 201.

Niepoprawny URL powoduje wyjątek w Domain. Zapis nie następuje. Obsługa błędów w Presentation
zamienia wyjątek na HTTP 400 z kodem `project.invalid_website_url`.
Kod domenowy nie zna HTTP ani ASP.NET.

`PUT /api/projects/{id}/audit-parameters` pobiera projekt, wywołuje jego metodę
`SelectAuditParameters`, zapisuje wynik i zwraca DTO. Nieobsługiwana wersja kończy się HTTP 422.
Repozytorium zwraca kopie Project, więc modyfikacja bez SaveAsync nie zmienia zapisanego stanu.
Magazyn w pamięci jest demonstracyjny; nie zapewnia trwałości ani rozwiązywania konfliktów równoczesnych edycji.

## Powiązanie z tablicą i decyzje projektowe

| Karteczka | Implementacja |
|---|---|
| Project Management | AccessAudit.ProjectManagement.* |
| Project (żółta) | Agregat Project |
| Audit Screen (żółta) | Osobny agregat AuditScreen, powiązany przez ProjectId |
| Register website for audit (niebieska) | RegisterWebsiteAsync i POST /api/projects |
| Added a project (pomarańczowa) | Utworzenie i zapis Project, sprawdzane testem |
| Selected audit parameters (pomarańczowa) | SelectAuditParameters i PUT /audit-parameters |
| Scraped project data and took screenshots | Import danych przygotowanych przez narzędzie pobierające stronę |
| Interactive Issue Dashboard (zielona) | Wstępny InteractiveIssueDashboardDto: projekt i zebrane ekrany |

**Uzupełnienie tablicy do zatwierdzenia w zespole:** niebieskie polecenie „Select audit parameters”
odpowiada istniejącemu zdarzeniu wyboru parametrów. Drugie polecenie „Record screen capture”
oddziela zapis wyniku od technicznego pobierania strony.

Przyjęte reguły, proponowane jako uzupełnienie tablicy:

1. Adres strony jest bezwzględnym adresem HTTP/HTTPS z hostem.
2. Obsługiwane wersje WCAG to dokładnie 2.1 i 2.2.
3. Nie można zapisać AuditScreen z pustym DOM.
4. AuditScreen musi wskazywać niepusty ProjectId. Istnienie projektu sprawdza serwis.

Wyjątki mają Code i dane kontekstowe: AttemptedUrl, AttemptedVersion, ProjectId lub AttemptedProjectId.
Kody HTTP istnieją wyłącznie w Presentation.

## Dokładny zakres i ograniczenia

To implementacja zadania architektonicznego Lab 3, nie kompletny produkt z całej tablicy.
Endpoint `/captures` **przyjmuje już pobrany DOM i odwołania do zrzutów**.
Nie pobiera stron, nie uruchamia przeglądarki i nie tworzy obrazów. Referencje zrzutów są metadanymi;
aplikacja nie odczytuje wskazanych plików. Przykładowe `screenshots/home.png` nie jest dostarczonym obrazem.
Automatyczny ekstraktor można później podłączyć jako adapter techniczny.

Dashboard zwraca dane wejściowe audytu; nie zawiera jeszcze wykrytych naruszeń WCAG.
Nie wdrożono silnika reguł, LLM, płatności, raportów, eksportu ani automatycznych polityk.
Pomarańczowe zdarzenia są potwierdzane stanem i testami; nie dodano brokera zdarzeń.
Zapis jest w pamięci: po zatrzymaniu procesu dane znikają. Nie dodano bazy ani logowania.

## TDD i nauka kodu

Testy Domain i Application zapisano przed implementacją. Pierwsze uruchomienie zakończyło się
błędami kompilacji z powodu brakujących klas — etap RED, zapisany w `verification/tdd-red.log`.
Potem dodano implementację i uruchomiono testy — wynik w `verification/tdd-green.log`.
To zapis jednej początkowej iteracji; nie udaje historii wielu osobnych commitów RED/GREEN.

Czytaj kod w tej kolejności:

1. ProjectTests.cs — zachowanie i wyjątki.
2. Project.cs — ochrona stanu i SelectAuditParameters.
3. AuditScreenTests.cs i AuditScreen.cs — drugi agregat i ochrona kolekcji.
4. ProjectManagementServiceTests.cs — Moq i sprawdzanie wywołań repozytorium.
5. ProjectManagementService.cs — koordynowanie operacji.
6. ProjectsController.cs, DomainExceptionHandler.cs, Program.cs — HTTP i DI.

Na obronę przećwicz samodzielnie dopisanie testu nieobsługiwanej wersji WCAG,
wyjaśnienie private set, Assert.Throws, Theory, DTO oraz różnicy pomiędzy Domain a Application.
W materiałach prowadzącego jest wymóg rozumienia kodu i zgodności z Waszą tablicą.
Ta pomoc nie zastępuje samodzielnego zrozumienia i weryfikacji reguł z zespołem.

## Oddanie

- Dostosuj nazwę rozwiązania do nazwy zespołowego repozytorium, jeśli jest inna.
- Uzupełnij tablicę o uzgodnione reguły i brakujące polecenia.
- Umieść kod w Waszym repozytorium zgodnie z zasadami zespołu. Nie utworzono zdalnego repozytorium ani niczego nie wypchnięto.
- Zrób własny zrzut zielonych testów w Visual Studio oraz dołącz tablicę.
- Raporty tekstowe i TRX są dodatkowym dowodem uruchomienia, nie zastępują wymaganego zrzutu interfejsu.

## Polecenia pomocnicze

W katalogu rozwiązania:

```powershell
dotnet build AccessAudit.sln
dotnet test AccessAudit.sln
dotnet run --project src/Services/ProjectManagement/AccessAudit.ProjectManagement.Presentation --launch-profile http
```
# lab3-2
