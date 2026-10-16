# Gateway (`src/Services/Gateway/WebApplication1`, project `Yarp.Api`)
**Responsibility:** single entry point; YARP reverse proxy with JWT auth enabled in pipeline (`Program.cs:19-56`).
**Routes** (`appsettings.json`): `/auth/**`->auth-cluster (localhost:5279/7067), `/tickets/**`->localhost:5000/5050, `/baskets/**`->localhost:5001/5051, `/orders/**`->https localhost:5053. No payments route.
**Dependencies:** none (config only). Not in docker-compose.
**Run:** `dotnet run` in project dir (addresses assume local launchSettings, not compose ports 6000/6003/6066/6080).
**Test:** none.
**Unknowns:** JWT settings section name used (`Program.cs:22` unread); whether routes carry auth policies.
