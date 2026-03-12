# TimeClockSystem – Architecture Guide for Claude Code



## Project Purpose

A full-stack web attendance system allowing employees to Clock In and Clock Out of shifts.



This project is built as a home assignment for a Junior Full Stack Developer position.



The system should remain simple, clean, and easy to explain during a technical interview.



Avoid unnecessary complexity or over-engineering.



---



# Working Instructions for Claude



## 1. Explain Before Taking Any Action



Before writing code, creating files, editing folders, or modifying existing code:



Explain clearly and in detail:



- what you are going to do

- why it is needed

- which files will be created or modified

- what each file will contain

- how the change affects the architecture



Then stop and wait for explicit approval before continuing.



---



## 2. Wait for Explicit Approval



After explaining the plan, do not immediately write code.



Wait for approval such as:

"yes", "continue", "go ahead", or "approved".



Only after approval should you start implementing.



---



## 3. Describe Implementation Plans Clearly



Before writing any significant amount of code, describe the implementation plan in detail.



Explain:



- the order of implementation

- the files involved

- the classes or interfaces to be created

- the main methods

- how components interact

- which layer each component belongs to



Then wait for approval.



---



## 4. Explain Architectural Decisions in Detail



Whenever making an architectural decision, explain:



- what was chosen

- why it belongs in that layer

- why other layers were not chosen

- how it affects the rest of the system

- what dependencies are involved

- any trade-offs or risks



The explanation should be clear enough to help understand the architecture in an interview.



---



## 5. Keep Changes Focused



When implementing changes:



- Do not modify unrelated files.

- Prefer minimal, focused changes.

- Avoid large refactors unless explicitly requested.



If a broader refactor seems necessary, explain it first.



---



## 6. Use Explicit DTOs



Use explicit request and response DTOs between API and Core.



Controllers should not expose domain entities directly unless explicitly approved.



DTOs should contain only the fields needed for TimeClockSystem/
├── server/
│ ├── TimeClockSystem.API
│ ├── TimeClockSystem.Core
│ └── TimeClockSystem.Infrastructure
│
├── client/
│ └── time-clock-client
│
├── CLAUDE.md
├── README.md
└── TimeClockSystem.sln

---



# Dependency Direction



Dependencies must flow only in this direction:



API → Core → Infrastructure



Core must never depend on Infrastructure or API.



---



# Critical Rule – Time Source



Clock In and Clock Out operations must use external time only.



Approved external API (currently in use):



https://timeapi.io/api/v1/time/current/zone?timezone=Europe/Zurich



The response field used for parsing is `date_time` (snake_case).

The value includes the Zurich UTC offset (e.g. +01:00 or +02:00 depending on DST),

so DateTimeOffset.Parse is sufficient — no manual timezone resolution needed.



Rules:



- Never use DateTime.Now

- Never use DateTime.UtcNow

- Never use browser time

- If the API fails, the operation must fail



There is no fallback to local time.



---



# Business Rule



A user cannot have more than one open shift.



This must be enforced by:



1. Business logic in ShiftService

2. Database constraint (Filtered Unique Index)



The database constraint is the final protection against race conditions.



---



# Development Priority Order



Follow this implementation order:



1. Solution structure

2. Entities and Enums

3. Interfaces

4. DTOs

5. DbContext and EF configuration

6. Migrations

7. External time provider

8. Password hashing and JWT

9. Repositories

10. Services

11. Controllers

12. Middleware

13. Program configuration

14. Frontend integration
