# Entity Relationship Diagram (ERD) - Container Tracking & Port Operations

## Project Name
Container Tracking & Port Operations System

## Date
September 27, 2025

## Core Database Entities

This document outlines the key database entities, their attributes, and relationships for the Container Tracking & Port Operations System. The database is designed for PostgreSQL and focuses on tracking containers throughout their lifecycle at ports and on ships.

### Core Entities

Entities are designed for PostgreSQL with appropriate data types. Each includes key attributes based on the project requirements, with relationships that support container tracking, berth management, and port operations.

### 1. Port Entity
Represents a shipping port with berths and container capacity.

**Attributes:**
- `port_id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `name`: TEXT NOT NULL (Port name, e.g., "Singapore Port")
- `location`: TEXT NOT NULL (Geographic location for mapping and distance calculations)
- `total_container_capacity`: INT NOT NULL (Maximum number of containers the port can handle)

**Relationships:**
- Has many Berths (One Port to Many Berths)
- Indirectly related to Containers via Berths

### 2. Berth Entity
Represents a specific berth within a port where containers can be staged.

**Attributes:**
- `berth_id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `port_id`: INT NOT NULL REFERENCES ports(port_id) (Foreign key to link to parent port)
- `name`: TEXT NOT NULL (Berth identifier, e.g., "Berth A1")
- `capacity`: INT NOT NULL (Number of containers the berth can hold)
- `status`: VARCHAR NOT NULL (Status: "free", "occupied", "maintenance", etc.)

**Relationships:**
- Belongs to one Port (Many Berths to One Port)
- Has many Berth Assignments (One Berth to Many Berth Assignments)
- Indirectly related to Containers via Berth Assignments

### 3. Ship Entity
Represents vessels that transport containers between ports.

**Attributes:**
- `ship_id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `name`: TEXT NOT NULL (Ship name, e.g., "Maersk Sealand")
- `status`: VARCHAR NOT NULL (Ship status: "at sea", "arrived", "loading", "departed", etc.)

**Relationships:**
- Has many Containers assigned (One Ship to Many Containers)
- Has many Ship Container records (One Ship to Many Ship Container records)

### 4. Container Entity
Represents shipping containers being tracked through the system.

**Attributes:**
- `container_id`: VARCHAR PRIMARY KEY (Unique container identifier, using industry standard format)
- `name`: TEXT (Optional container name or description)
- `type`: VARCHAR NOT NULL (Container type: "dry", "refrigerated", "liquid", "hazardous", etc.)
- `status`: VARCHAR NOT NULL (Current status: "transit", "at port", "at ship", "inspected", "loaded", "departed", etc.)
- `current_location`: TEXT (Current geographic or logical location)
- `created_at`: TIMESTAMP DEFAULT CURRENT_TIMESTAMP (When the container record was created)
- `updated_at`: TIMESTAMP DEFAULT CURRENT_TIMESTAMP (Last update to the container record)
- `ship_id`: INT REFERENCES ships(ship_id) (Foreign key to the current ship, NULL if not on a ship)

**Relationships:**
- May be assigned to one Ship (Many Containers to One Ship)
- Has many Berth Assignments (One Container to Many Berth Assignments)
- Has many Ship Container records (One Container to Many Ship Container records)

### 5. Berth Assignment Entity
Represents the assignment of a container to a specific berth.

**Attributes:**
- `id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `container_id`: VARCHAR NOT NULL REFERENCES containers(container_id) (Foreign key to the container)
- `berth_id`: INT NOT NULL REFERENCES berths(berth_id) (Foreign key to the berth)
- `assigned_at`: TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP (When the container was assigned to the berth)
- `released_at`: TIMESTAMP (When the container was released from the berth, NULL if still assigned)

**Relationships:**
- Links one Container to one Berth for a period (joining table for many-to-many with temporal aspect)

### 6. Ship Container Entity
Represents the loading of containers onto ships and tracking their status.

**Attributes:**
- `id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `ship_id`: INT NOT NULL REFERENCES ships(ship_id) (Foreign key to the ship)
- `container_id`: VARCHAR NOT NULL REFERENCES containers(container_id) (Foreign key to the container)
- `loaded_at`: TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP (When the container was loaded onto the ship)

**Relationships:**
- Links one Container to one Ship (joining table for many-to-many with temporal aspect)

## Relationships Overview

- **Primary Foreign Keys**:
  - `port_id` in Berths (links to Ports)
  - `ship_id` in Containers (links to Ships)
  - `container_id` in Berth Assignments (links to Containers)
  - `berth_id` in Berth Assignments (links to Berths)
  - `ship_id` and `container_id` in Ship Containers (links to Ships and Containers)

- **One-to-Many Patterns**:
  - Port → Berths (port has multiple berths)
  - Ship → Containers (ship can carry multiple containers)
  - Container → Berth Assignments (container can have multiple berth assignments over time)
  - Berth → Berth Assignments (berth can have multiple container assignments over time)

- **Indexes**: Should be created on all foreign keys for query optimization

## Key Workflows

1. **Container Arrival**:
   - Container record created/updated → Status set to "arrived" → Assigned to berth → Real-time event published

2. **Container Inspection**:
   - Container status updated to "inspected" → Event published → UI updated via SignalR

3. **Container Loading**:
   - Container assigned to ship → Ship Container record created → Container status updated to "loaded" → Event published

4. **Ship Departure**:
   - Ship status updated to "departed" → Associated containers status updated → Events published

## ERD Diagram

```
[ERD diagram would be placed here - visual representation of the entities and their relationships]
```

## Next Steps

1. Implement database migrations using Entity Framework Core
2. Create data access services for each entity
3. Implement API controllers for CRUD operations
4. Set up event producers/consumers for real-time updates