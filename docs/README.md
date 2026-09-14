JR.AussieQualy

A clean, structured .NET project for analysing Formula 1 qualifying laps from JSON data.The system loads lap‑by‑lap information, determines best laps per qualifying segment (Q1/Q2/Q3), evaluates sector performance, and exposes the results through a minimal API.

The project is organised into three layers:

Domain — Core models representing drivers, laps, sectors, tyres, and conditions.

Infrastructure — JSON loading and repository logic for lap data.

Application — Qualifying service, DTO mapping, sector status logic, and final classification.

API — Minimal API exposing /qualifying/{driverCode}.

Tests — Unit tests (service logic).

Console App - Console application to act as client to api 

🧱 Architecture Overview

Domain Layer

Contains pure C# models:

Driver

Lap

SectorTimes

MiniSectorStatus

Conditions

TireInfo

QualifyingSegment

These models have no dependencies on other layers.

Infrastructure Layer

Responsible for loading and parsing lap data:

LapTimesJsonModel — Raw JSON structure.

LapTimesJsonLoader — Deserialises JSON.

LapTimesRepository — Converts JSON into domain models.

Application Layer

Contains the main business logic:

IQualifyingService

QualifyingService

Responsibilities include:

Selecting best laps per qualifying segment.

Determining global best sector times.

Assigning sector status (purple/green/yellow).

Mapping domain models to DTOs.

Calculating final qualifying position.

API Layer

Minimal API exposing:

GET /qualifying/{driverCode}

Returns a DriverQualifyingResultDto containing:

Driver info

Final qualifying position

Best Q1/Q2/Q3 laps

Sector breakdowns

Tyre and condition data

📁 Folder Structure

JR.AussieQualy/
│
├── JR.AussieQualy.Domain/
│   └── Models/
│
├── JR.AussieQualy.Infrastructure/
│   └── LapTimes/
│
├── JR.AussieQualy.Application/
│   ├── Dtos/
│   └── Qualifying/
│
├── JR.AussieQualy.Api/
│   └── Program.cs
│
└── JR.AussieQualy.Tests/
    ├── Unit/
    └── Integration/

▶️ Running the API

From the solution root:

dotnet run --project JR.AussieQualy.Api

The API will start on the default Kestrel port (usually http://localhost:5000).

🌐 Example API Request

Request

GET http://localhost:5000/qualifying/HAM

Example Response

{
  "driver": {
    "name": "Lewis Hamilton",
    "driverNumber": 44,
    "team": "Mercedes"
  },
  "finalQualifyingPosition": 3,
  "q1": {
    "lapTime": 90.123,
    "lapStartTime": "00:00:10",
    "tire": { "compound": "Soft", "life": 3 },
    "conditions": {
      "airTemperature": 25,
      "humidity": 40,
      "pressure": 1010,
      "rainfall": 0,
      "trackTemperature": 32,
      "windDirectionBearing": 180,
      "windSpeedInMs": 3
    },
    "segmentPosition": 1,
    "sector1": { "sectorTime": 30.0, "miniSectors": ["green"], "sectorStatus": "green" },
    "sector2": { "sectorTime": 30.0, "miniSectors": ["yellow"], "sectorStatus": "yellow" },
    "sector3": { "sectorTime": 30.123, "miniSectors": ["purple"], "sectorStatus": "purple" }
  },
  "q2": null,
  "q3": null
}

🧪 Running Tests

Unit Tests

dotnet test JR.AussieQualy.Tests/Unit

All tests use:

NUnit

Moq

WebApplicationFactory for API integration tests

📌 Notes

Sector status uses tolerance‑based comparison to determine purple sectors.

Global best sectors are calculated across all drivers, not just the selected one.

Missing laps or segments return null in the DTO.

The API returns:

400 for invalid driver codes

404 for unknown drivers

200 for valid results

📞 Support

If you want to extend the project (e.g., add race sessions, tyre degradation modelling, or multi‑lap stint analysis), feel free to ask — happy to help you evolve it.