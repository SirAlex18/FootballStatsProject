# 🏈 FootballStatsProject Development Plan

## 📊 Project Overview
A modern football statistics platform featuring a high-performance .NET 9 Web API backend, PostgreSQL database for persistent caching, and a planned React frontend. Designed to minimize external API costs via an async write-behind pattern and optimized for Docker deployment.

## 🏗️ Architecture Snapshot
- **Backend**: .NET 9 Minimal API (`IntegrationToApi`)
- **Database**: PostgreSQL 16 (via EF Core)
- **Caching/Storage Strategy**: Async write-behind (fire-and-forget DB saves) to keep frontend latency <50ms while respecting free-tier API rate limits.
- **Logging**: Serilog with file/console sinks
- **Security**: Environment variables via `.env` + Docker secrets, CORS configured for React (`localhost:5173` / `3000`)
- **Deployment**: Multi-stage Dockerfile & `docker-compose.yml` in `/docker/` directory

## ✅ Completed Milestones
- [x] API Integration & Data Mapping (RestSharp → Internal DTOs)
- [x] Dependency Injection & Configuration (`IConfig`, `Options`)
- [x] EF Core + PostgreSQL integration with unique constraints
- [x] Async write-behind service layer with exponential backoff retries
- [x] Minimal API endpoints (`GET /api/players/{id}/{season}`, `POST /api/players/bulk`)
- [x] CORS, Health/Readiness probes, Serilog logging
- [x] Docker configuration (optimized build context & runtime)
- [x] Upgraded to .NET 9 LTS
- [x] Automated DB password generation & schema initialization via PowerShell & Docker init scripts
- [x] Temporary Blazor Server frontend for API validation

## 🚧 Pending Tasks
1. **Scaffold React Frontend** (Vite + TypeScript + Tailwind CSS)
2. **Local Docker Testing** (Full stack: API, DB, React)
   ```bash
   docker compose -f docker/docker-compose.yml up --build
   ```
3. **Cloud Deployment Prep** (Azure/AWS/GCP, CI/CD pipeline setup)

## 📝 Notes & Best Practices
- Backend is graded **9/10**. Ready for frontend integration.
- All Docker configs live in `/docker/`. Run `docker compose -f docker/docker-compose.yml up` from the project root.
- External API calls are throttled internally (`Task.Delay(100)`) to respect free-tier limits.
- Database indexes on `(PlayerId, Season)` ensure fast lookups.

## 🔜 Next Immediate Step
Run `npm install && npm run dev` in the `FootballStatsReact/` directory to start the React frontend. Let me know when it's running!
