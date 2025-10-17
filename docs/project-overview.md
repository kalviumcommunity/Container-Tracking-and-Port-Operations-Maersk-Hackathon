# Container Tracking & Port Operations System - Project Overview

## Project Name 
Container Tracking & Port Operations System

## Date
September 27, 2025

## Business Domain
This system manages container tracking and port/berth operations for maritime logistics, enabling real-time monitoring and efficient management of container lifecycles. It simulates Maersk-like operations with real-time updates via a Kafka-compatible streaming system (Azure Event Hubs). Key modules include Container Management (CRUD), Container Event Tracking (lifecycle events), Berth Management (assignments), Port Operations, Ship Management, and Event Streaming (integration for events like "container arrived" or "container loaded").

## Tech Stack

- **Backend**: ASP.NET Core Web API (C#), .NET 7/8
- **ORM**: Entity Framework Core with Npgsql (PostgreSQL)
- **Streaming**: Confluent.Kafka for .NET (configured to use Azure Event Hubs Kafka endpoint)
- **Real-time Communication**: ASP.NET Core SignalR
- **Frontend**: Vue 3 (Composition API) + Pinia for state management, Vite
- **Development**: Docker (docker-compose for PostgreSQL+Kafka dev), GitHub Actions for CI/CD
- **Deployment**: Azure (App Service / Static Web Apps / Event Hubs / Azure DB)

## Key Modules & Features

1. **Container Management**
   - Create, update, and track container information
   - View container history and current status
   - Filter and search containers by various attributes

2. **Event Streaming & Processing**
   - Stream container events (Arrived/Inspection/Loaded/Departed)
   - Real-time notifications for status changes
   - Operator acknowledgment of events

3. **Berth Management**
   - Manual assignment of containers to berths
   - Berth capacity management
   - Berth status tracking (occupied/free)

4. **Ship Operations**
   - Track ships entering and leaving port
   - Load/unload containers to/from ships
   - Manage ship capacity and contents

5. **Port Operations Dashboard**
   - Real-time overview of port activities
   - Container location and status visualization
   - Performance metrics and capacity utilization

## Development Workflow

1. Feature branches with naming convention: `feat/xxx` or `fix/xxx`
2. PR reviews and approvals before merging
3. Unit testing for backend (xUnit) and frontend (Vitest)
4. CI/CD pipeline via GitHub Actions

## Constraints & Guidelines

1. Input validation using DataAnnotations + Fluent API
2. EF Core migrations for schema management
3. Business logic in Services (keep controllers thin)
4. Dependency Injection for services and Kafka producers/consumers
5. Extensible design for future authentication (Azure AD)