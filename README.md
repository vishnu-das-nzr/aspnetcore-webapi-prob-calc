📘 Probability Calculation API (.NET 8)

A simple and modular ASP.NET Core Web API for calculating basic probability operations.

🚀 Endpoints

1️⃣ Combined Probability

POST /api/probabilities/combinedwith

Request

{
  "probabilityA": 0.5,
  "probabilityB": 0.5
}

Response

{
  "result": 0.25
}


2️⃣ Either Probability

POST /api/probabilities/either

Request

{
  "probabilityA": 0.5,
  "probabilityB": 0.5
}

Response

{
  "result": 0.75
}

Activity logs.
The activity log file, probability_activity.txt, can be found inside the ProbCalculation.API folder in the source code.

🧱 Project Structure (Essential)

ProbCalculation.API                 # API controllers

Calculation.Core.Service            # Probability calculation logic

Common.Validator.Service            # Input validation

Common.Logging.Service              # Logs requests/responses

Common.Models                       # Request/response DTOs

...Tests                            # Unit test projects


▶️ Run the API

dotnet restore

dotnet run --project ProbCalculation.API

Swagger available at:

https://localhost:7001/swagger

📄 Tech Stack

.NET 8

ASP.NET Core Web API

xUnit
