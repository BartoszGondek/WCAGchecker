# Wyniki weryfikacji AccessAudit

- Kompilacja całego rozwiązania: 11 projektów, 0 błędów, 0 ostrzeżeń.
- Testy Domain: 13 zaliczonych, 0 niezaliczonych.
- Testy Application: 10 zaliczonych, 0 niezaliczonych.
- Łącznie: 23 testy jednostkowe.
- Sprawdzono zależności wszystkich 9 projektów produkcyjnych: Domain bez pakietów i referencji,
  Application wskazuje własny Domain, brak referencji pomiędzy kontekstami.
- Sprawdzono 25 prawdziwych żądań HTTP; sprawdzano również dane odpowiedzi, Location,
  typ błędu, zachowanie stanu po odrzuceniu operacji i rozdzielenie danych projektów.

Log RED pochodzi sprzed dodania implementacji. Log GREEN zawiera wyniki testów.
Pliki TRX pozwalają obejrzeć szczegóły wykonania. Nie wykonano zrzutu okna Visual Studio,
ponieważ mechanizm przechwytywania okna nie działał w tej sesji.

## Żądania HTTP

| Metoda | Ścieżka | Oczekiwany status | Wynik |
|---|---|---|---|
| GET | `/` | 200 | OK |
| GET | `/openapi/v1.json` | 200 | OK |
| POST | `/api/projects` | 201 | OK |
| GET | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5` | 200 | OK |
| PUT | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/audit-parameters` | 200 | OK |
| PUT | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/audit-parameters` | 200 | OK |
| PUT | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/audit-parameters` | 422 | OK |
| GET | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5` | 200 | OK |
| POST | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/captures` | 201 | OK |
| GET | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/captures/9cf56a07-082f-43d5-a153-4a49fa6509a7` | 200 | OK |
| POST | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/captures` | 422 | OK |
| GET | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/dashboard` | 200 | OK |
| POST | `/api/projects` | 400 | OK |
| POST | `/api/projects` | 400 | OK |
| POST | `/api/projects` | 400 | OK |
| POST | `/api/projects` | 400 | OK |
| POST | `/api/projects` | 400 | OK |
| PUT | `/api/projects/c94b54a4-1209-4e1d-9408-533a251ca7d5/audit-parameters` | 422 | OK |
| GET | `/api/projects/00000000-0000-0000-0000-000000000001` | 404 | OK |
| PUT | `/api/projects/00000000-0000-0000-0000-000000000001/audit-parameters` | 404 | OK |
| POST | `/api/projects/00000000-0000-0000-0000-000000000001/captures` | 404 | OK |
| GET | `/api/projects/00000000-0000-0000-0000-000000000001/dashboard` | 404 | OK |
| POST | `/api/projects` | 201 | OK |
| GET | `/api/projects/e82bfc15-dd97-4953-8bc4-0e43a7a17f5a/captures/9cf56a07-082f-43d5-a153-4a49fa6509a7` | 404 | OK |
| GET | `/api/projects/e82bfc15-dd97-4953-8bc4-0e43a7a17f5a/dashboard` | 200 | OK |
