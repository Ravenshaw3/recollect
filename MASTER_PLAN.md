# Recollect Master Plan

## 1) Goals and Success Criteria
- Single source of truth for adventures across Mobile, API, and Admin.
- MVP complete and reliable: track routes, capture media (photo/video/audio), notes, multi-adventure management, upload/sync to server.
- Secure, internet-accessible deployment (Tailscale/Reverse Proxy) with docs.

Success criteria (MVP):
- Create/select/delete multiple adventures on device and in Admin.
- Track route (polyline), add waypoints while moving; pause/resume.
- Capture photo/video, pick from gallery, upload voice notes.
- Upload adventure bundle (adventure, notes, media metadata) to API with visible status.
- Admin shows adventures, notes, media; allows deletes and basic edits.

## 2) Architecture Overview
- Mobile (MAUI net9.0-android): Maps, Media, Notes, Review, new Adventures page; EF Core Sqlite for local state; HttpClient for API.
- API (ASP.NET Core): Adventures, Notes, Media controllers; PostgreSQL via EF Core; static Admin UI.
- Deployment: Docker Compose; Tailscale or Nginx for internet access.

## 3) Workstreams and Deliverables
1. API Contract Hardening
   - Lock DTOs and responses; add/confirm endpoints: create/update/delete adventures; upload audio; list/paginate.
   - Validation and consistent error shape; structured logging.

2. Admin UI Parity
   - CRUD for adventures; details view with waypoints, notes, media.
   - Delete media/notes; upload audio from admin.

3. Mobile Adventures Management
   - New `AdventuresPage`: list/create/select/delete adventures (local DB + sync-ready).
   - Integrate selection across Map/Media/Notes/Review.

4. Tracking & Media Polish
   - Start/Stop/Pause/Resume tracking; throttled waypoint density; polyline refresh.
   - Photo/Video capture; Gallery pick; Voice note upload with lat/lng.

5. Sync & Upload
   - Upload queue with retries/backoff; visible statuses and error recovery.
   - Ensure base URL configuration (Tailscale/prod) is persisted.

6. UX/Theme & Accessibility
   - Darker text/controls; progress toasts; error messages; loading indicators.

7. Build Size/Time
   - Single ABI releases; safe linking (SdkOnly) with preserve list for EF Core and Maps; resource trimming.

8. Observability
   - API structured logs, request correlation; client debug toggle.

9. E2E Validation
   - Manual checklist and curl scripts; emulator/device runs; Admin verification.

## 4) Milestones & Acceptance Criteria
- M1 (Core CRUD): Mobile AdventuresPage + API create/delete; Admin list/create/delete.
- M2 (Tracking/Media): Start/Stop tracking reliable; photo/video/gallery/voice note flows work on Android 13+.
- M3 (Sync): Upload queue; Review “Upload” uploads and shows success in Admin.
- M4 (UX/Perf): Dark theme tweaks; single-ABI release; safe linking with preserve.
- M5 (Ops): Finalized deployment docs; health checks; logs/metrics.

## 5) Workflow
- Branching: `feature/*`, `fix/*`, PRs to `main` with brief descriptions.
- CI (manual now): build Release net9.0-android; API docker compose up; basic api smoke tests.
- Environments: Local (localhost), Tailscale (100.x IP), Production (domain).

## 6) Test Plan (Manual E2E)
1. Adventures: create two; switch; delete one; persist across restart.
2. Tracking: Start; move (emulator or device); see polyline update; Stop.
3. Media: capture photo/video; pick from gallery; upload audio; see items in Review.
4. Upload: Review → Upload; verify in Admin; delete from Admin; refresh mobile.
5. Network: switch to Tailscale IP and re-test.

## 7) Risks & Mitigations
- Android permissions (13+): add READ_MEDIA_* and use FilePicker; runtime permission prompts.
- Trimming: keep EF Core/Maps types with safe linking and preserves; testing before enabling.
- Startup DI: resolve services consistently (constructor or OnAppearing) without reflection.

## 8) Backlog (Post-MVP)
- Offline-first sync; background upload; story generator integration in mobile; iOS target; telemetry dashboard.

## 9) Current Status Snapshot
- Done: Audio upload endpoint; gallery/voice note on mobile; Tailscale/docs; multiple-adventure services; admin scaffold.
- In progress: Adventures UI (mobile); admin CRUD parity; sync robustness; UX contrast; build optimization.

## 10) Next Actions
- Implement `AdventuresPage` with list/create/select/delete and wire into Shell.
- Add Admin CRUD for adventures and details.
- Introduce upload queue with statuses in Review.
