# Entity Relationship Diagram (ERD) - Container Tracking & Port Operations

## Project Name
Container Tracking & Port Operations System

## Date
September 29, 2025 (Updated)

## Overview

This document outlines the key database entities, their attributes, and relationships for the Container Tracking & Port Operations System. The database is implemented using PostgreSQL with Entity Framework Core and follows C# naming conventions. The system supports comprehensive container tracking, berth management, and port operations with full environment variable configuration and extensive test data seeding.

## Core Database Entities

### 1. Port Entity
Represents a shipping port with berths and container capacity.

**Table Name:** `Ports`

**Attributes:**
- `PortId`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `Name`: TEXT NOT NULL (Port name, e.g., "Port of Copenhagen")
- `Location`: TEXT NOT NULL (Geographic location for mapping and distance calculations)
- `TotalContainerCapacity`: INT NOT NULL (Maximum number of containers the port can handle)

**C# Model Properties:**
```csharp
public int PortId { get; set; }
public string Name { get; set; }
public string Location { get; set; }
public int TotalContainerCapacity { get; set; }
public ICollection<Berth> Berths { get; set; }
```

**Sample Data:**
- Port of Copenhagen (Denmark) - 10,000 capacity
- Port of Rotterdam (Netherlands) - 15,000 capacity
- Port of Hamburg (Germany) - 12,000 capacity

**Relationships:**
- Has many Berths (One Port to Many Berths)
- Indirectly related to Containers via Berths

### 2. Berth Entity
Represents a specific berth within a port where containers can be staged.

**Table Name:** `Berths`

**Attributes:**
- `BerthId`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `PortId`: INT NOT NULL REFERENCES Ports(PortId) (Foreign key to link to parent port)
- `Name`: TEXT NOT NULL (Berth identifier, e.g., "CPH-A1", "RTM-2")
- `Capacity`: INT NOT NULL (Number of containers the berth can hold)
- `Status`: VARCHAR NOT NULL (Status: "Available", "Occupied", "Maintenance")

**C# Model Properties:**
```csharp
public int BerthId { get; set; }
public int PortId { get; set; }
public string Name { get; set; }
public int Capacity { get; set; }
public string Status { get; set; }
public Port Port { get; set; }
public ICollection<BerthAssignment> BerthAssignments { get; set; }
```

**Sample Data:**
- CPH-A1, CPH-A2 (Copenhagen berths)
- RTM-1, RTM-2 (Rotterdam berths)  
- HAM-North-1, HAM-South-1 (Hamburg berths)

**Relationships:**
- Belongs to one Port (Many Berths to One Port)
- Has many Berth Assignments (One Berth to Many Berth Assignments)

### 3. Ship Entity
Represents vessels that transport containers between ports.

**Table Name:** `Ships`

**Attributes:**
- `ShipId`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `Name`: TEXT NOT NULL (Ship name, e.g., "Maersk Edinburgh")
- `Status`: VARCHAR NOT NULL (Ship status: "Docked", "At Sea", "Loading", "Maintenance")

**C# Model Properties:**
```csharp
public int ShipId { get; set; }
public string Name { get; set; }
public string Status { get; set; }
public ICollection<Container> Containers { get; set; }
public ICollection<ShipContainer> ShipContainers { get; set; }
```

**Sample Data:**
- Maersk Edinburgh, MSC Gulsun, Ever Given
- COSCO Shipping Universe, HMM Algeciras
- Real shipping vessel names with various statuses

**Relationships:**
- Has many Containers assigned (One Ship to Many Containers)
- Has many Ship Container records (One Ship to Many Ship Container records)

### 4. Container Entity
Represents shipping containers being tracked through the system.

**Table Name:** `Containers`

**Attributes:**
- `ContainerId`: VARCHAR PRIMARY KEY (Unique container identifier, using industry standard format like "MSKU1000001")
- `Name`: TEXT (Container name or description, e.g., "Electronics Container", "Automotive Parts")
- `Type`: VARCHAR NOT NULL (Container type: "Dry", "Refrigerated", "Hazardous", "Liquid", "Open Top", "Flat Rack")
- `Status`: VARCHAR NOT NULL (Current status: "Empty", "Loaded", "In Transit", "Loading", "Unloading", "Inspected")
- `CurrentLocation`: TEXT (Current geographic or logical location)
- `CreatedAt`: TIMESTAMP DEFAULT CURRENT_TIMESTAMP (When the container record was created)
- `UpdatedAt`: TIMESTAMP DEFAULT CURRENT_TIMESTAMP (Last update to the container record)
- `ShipId`: INT REFERENCES Ships(ShipId) (Foreign key to the current ship, NULL if not on a ship)

**C# Model Properties:**
```csharp
public string ContainerId { get; set; }
public string Name { get; set; }
public string Type { get; set; }
public string Status { get; set; }
public string CurrentLocation { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime UpdatedAt { get; set; }
public int? ShipId { get; set; }
public Ship? Ship { get; set; }
public ICollection<BerthAssignment> BerthAssignments { get; set; }
public ICollection<ShipContainer> ShipContainers { get; set; }
```

**Sample Data:**
- 50 containers with realistic IDs (MSKU1000001, MSCU1000002, etc.)
- Various types: Electronics, Automotive Parts, Refrigerated Goods
- Multiple statuses and locations across different ports

**Relationships:**
- May be assigned to one Ship (Many Containers to One Ship)
- Has many Berth Assignments (One Container to Many Berth Assignments)
- Has many Ship Container records (One Container to Many Ship Container records)

### 5. Berth Assignment Entity
Represents the assignment of a container to a specific berth with temporal tracking.

**Table Name:** `BerthAssignments`

**Attributes:**
- `Id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `ContainerId`: VARCHAR NOT NULL REFERENCES Containers(ContainerId) (Foreign key to the container)
- `BerthId`: INT NOT NULL REFERENCES Berths(BerthId) (Foreign key to the berth)
- `AssignedAt`: TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP (When the container was assigned to the berth)
- `ReleasedAt`: TIMESTAMP (When the container was released from the berth, NULL if still assigned)

**C# Model Properties:**
```csharp
public int Id { get; set; }
public string ContainerId { get; set; }
public int BerthId { get; set; }
public DateTime AssignedAt { get; set; }
public DateTime? ReleasedAt { get; set; }
public Container Container { get; set; }
public Berth Berth { get; set; }
```

**Sample Data:**
- 20 berth assignments with mix of current and historical assignments
- Realistic time ranges for container staging at berths

**Relationships:**
- Links one Container to one Berth for a time period (temporal many-to-many relationship)

### 6. Ship Container Entity
Represents the loading of containers onto ships with loading timestamps.

**Table Name:** `ShipContainers`

**Attributes:**
- `Id`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `ShipId`: INT NOT NULL REFERENCES Ships(ShipId) (Foreign key to the ship)
- `ContainerId`: VARCHAR NOT NULL REFERENCES Containers(ContainerId) (Foreign key to the container)
- `LoadedAt`: TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP (When the container was loaded onto the ship)

**C# Model Properties:**
```csharp
public int Id { get; set; }
public int ShipId { get; set; }
public string ContainerId { get; set; }
public DateTime LoadedAt { get; set; }
public Ship Ship { get; set; }
public Container Container { get; set; }
```

**Sample Data:**
- 15 ship container operations linking containers to ships
- Various loading times over past weeks

**Relationships:**
- Links one Container to one Ship with temporal data (joining table for many-to-many with timestamps)

## Relationships Overview

- **Primary Foreign Keys**:
  - `PortId` in Berths (links to Ports)
  - `ShipId` in Containers (links to Ships)
  - `ContainerId` in BerthAssignments (links to Containers)
  - `BerthId` in BerthAssignments (links to Berths)
  - `ShipId` and `ContainerId` in ShipContainers (links to Ships and Containers)

- **One-to-Many Patterns**:
  - Port → Berths (port has multiple berths)
  - Ship → Containers (ship can carry multiple containers)
  - Container → BerthAssignments (container can have multiple berth assignments over time)
  - Berth → BerthAssignments (berth can have multiple container assignments over time)

- **Indexes**: Created on all foreign keys for query optimization

## Sample Data Overview (Current Implementation)

### Ports (6 entries)
- New York Port (Port ID: 1)
- Long Beach Port (Port ID: 2)
- Hamburg Port (Port ID: 3)
- Singapore Port (Port ID: 4)
- Rotterdam Port (Port ID: 5)
- Hong Kong Port (Port ID: 6)

### Berths (26+ entries)
- 2-6 berths per port with varied capacities
- Depths: 10m, 15m, 20m, 25m, 30m
- Capacities: 5,000 to 25,000 TEU

### Ships (12 entries)
- Container ships with realistic names (MV Atlantic Express, MSC Barcelona, etc.)
- Capacities: 8,000 to 24,000 TEU
- Distributed across different ports

### Containers (50 entries)
- Industry-standard IDs: MSKU1000001, MSCU1000002, etc.
- Types: Electronics, Automotive Parts, Refrigerated Goods, Textiles, Machinery
- Statuses: Empty, Loaded, In Transit, Loading, Unloading, Inspected
- Realistic locations: "Port of New York", "Warehouse B-12", etc.

### BerthAssignments (20 entries)
- Mix of current assignments (ReleasedAt = NULL) and historical records
- Realistic time ranges for container staging

### ShipContainer Operations (15 entries)
- Containers loaded onto ships with timestamps
- Various loading dates over recent weeks

## Environment Variable Configuration

The system now uses environment variables for secure configuration:

### Required Environment Variables (.env file)
```
DB_HOST=localhost
DB_PORT=5433
DB_NAME=ContainerTrackingDB
DB_USERNAME=postgres
DB_PASSWORD=your_password_here
```

### Configuration Files
- `appsettings.json`: Base configuration without sensitive data
- `appsettings.Development.json`: Development-specific settings with detailed logging
- `appsettings.Production.json`: Production settings with optimized logging

## Sample Queries (PostgreSQL)

**Note:** PostgreSQL is case-sensitive. Use quoted identifiers when querying through pgAdmin.

Basic port information:
```sql
SELECT "PortId", "Name", "Location" FROM "Ports";
```

Containers at a specific port:
```sql
SELECT c."ContainerId", c."Name", c."Type", c."Status"
FROM "Containers" c
JOIN "BerthAssignments" ba ON c."ContainerId" = ba."ContainerId"
JOIN "Berths" b ON ba."BerthId" = b."BerthId"
JOIN "Ports" p ON b."PortId" = p."PortId"
WHERE p."Name" = 'New York Port' AND ba."ReleasedAt" IS NULL;
```

Ship details with current port:
```sql
SELECT s."ShipId", s."Name", s."Capacity", p."Name" as "CurrentPort"
FROM "Ships" s
JOIN "Ports" p ON s."CurrentPortId" = p."PortId";
```

Active berth assignments:
```sql
SELECT ba."Id", c."ContainerId", c."Name", b."BerthNumber", p."Name" as "PortName"
FROM "BerthAssignments" ba
JOIN "Containers" c ON ba."ContainerId" = c."ContainerId"
JOIN "Berths" b ON ba."BerthId" = b."BerthId"
JOIN "Ports" p ON b."PortId" = p."PortId"
WHERE ba."ReleasedAt" IS NULL
ORDER BY ba."AssignedAt" DESC;
```

Containers currently on ships:
```sql
SELECT sc."Id", s."Name" as "ShipName", c."ContainerId", c."Name" as "ContainerName", sc."LoadedAt"
FROM "ShipContainers" sc
JOIN "Ships" s ON sc."ShipId" = s."ShipId"
JOIN "Containers" c ON sc."ContainerId" = c."ContainerId"
ORDER BY sc."LoadedAt" DESC;
```

## Key Workflows

1. **Container Arrival**:
   - Container record created/updated → Status set to "Empty" or "Loaded" → Assigned to berth via BerthAssignment → Real-time event published

2. **Container Inspection**:
   - Container status updated to "Inspected" → Event published → UI updated via SignalR

3. **Container Loading**:
   - Container assigned to ship → ShipContainer record created → Container status updated to "Loading" → Event published

4. **Ship Departure**:
   - Ship status updated to "Departed" → Associated containers status updated via ShipContainer records → Events published

## Implementation Status (Updated September 29, 2025)

✅ **Completed:**
- Database schema aligned with ER diagram using C# PascalCase conventions
- Entity Framework Core models with proper relationships
- Comprehensive data seeding with 50+ containers, 26+ berths, 12 ships
- Environment variable configuration for secure database connection
- PostgreSQL database setup with pgAdmin management (port 5433)
- Sample query examples with proper case-sensitive syntax

✅ **Database Migration Applied:**
- Migration "UpdateSchemaToMatchDiagram" successfully applied
- All tables created with proper foreign key constraints
- Data seeding completed with extensive test data

✅ **Security Implementation:**
- Environment variables (.env files) for sensitive configuration
- Separate appsettings files for Development/Production
- CORS policies configured for different environments

## Authentication & Authorization Entities

### 7. User Entity
Represents user accounts with role-based access control.

**Table Name:** `Users`

**Attributes:**
- `UserId`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `Username`: VARCHAR(50) NOT NULL UNIQUE (Unique username for login)
- `Email`: VARCHAR(100) NOT NULL UNIQUE (Email address for notifications)
- `PasswordHash`: TEXT NOT NULL (SHA256 hashed password)
- `FullName`: VARCHAR(100) NOT NULL (Display name)
- `PhoneNumber`: VARCHAR(20) NULLABLE (Contact number)
- `Department`: VARCHAR(100) NULLABLE (User's department)
- `PortId`: INT NULLABLE (Optional port assignment)
- `IsActive`: BOOLEAN NOT NULL (Account status)
- `CreatedAt`: TIMESTAMPTZ NOT NULL (Account creation date)
- `UpdatedAt`: TIMESTAMPTZ NOT NULL (Last modification date)
- `LastLoginAt`: TIMESTAMPTZ NULLABLE (Last login timestamp)

**C# Model Properties:**
```csharp
public int UserId { get; set; }
public string Username { get; set; }
public string Email { get; set; }
public string PasswordHash { get; set; }
public string FullName { get; set; }
public string? PhoneNumber { get; set; }
public string? Department { get; set; }
public int? PortId { get; set; }
public bool IsActive { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime UpdatedAt { get; set; }
public DateTime? LastLoginAt { get; set; }

// Navigation Properties
public Port? Port { get; set; }
public ICollection<UserRole> UserRoles { get; set; }
```

**Relationships:**
- Optional relationship with Port (Many Users to One Port)
- Many-to-Many relationship with Roles via UserRoles

### 8. Role Entity
Represents system roles with different permission levels.

**Table Name:** `Roles`

**Attributes:**
- `RoleId`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `Name`: VARCHAR(50) NOT NULL UNIQUE (Role name: Admin, PortManager, Operator, Viewer)
- `Description`: VARCHAR(200) NOT NULL (Role description)
- `IsSystemRole`: BOOLEAN NOT NULL (Cannot be deleted if true)
- `CreatedAt`: TIMESTAMPTZ NOT NULL (Role creation date)

**C# Model Properties:**
```csharp
public int RoleId { get; set; }
public string Name { get; set; }
public string Description { get; set; }
public bool IsSystemRole { get; set; }
public DateTime CreatedAt { get; set; }

// Navigation Properties
public ICollection<UserRole> UserRoles { get; set; }
public ICollection<RolePermission> RolePermissions { get; set; }
```

**Default Roles:**
- **Admin**: Full system access, user management
- **PortManager**: Manage specific port operations
- **Operator**: Day-to-day container and ship operations
- **Viewer**: Read-only access to data

### 9. Permission Entity
Represents granular permissions for system operations.

**Table Name:** `Permissions`

**Attributes:**
- `PermissionId`: INT PRIMARY KEY (Auto-incrementing unique identifier)
- `Name`: VARCHAR(100) NOT NULL UNIQUE (Permission name)
- `Description`: VARCHAR(200) NOT NULL (Permission description)
- `Category`: VARCHAR(50) NOT NULL (Permission category: System, Port, Container, Ship, etc.)
- `IsSystemPermission`: BOOLEAN NOT NULL (Cannot be deleted if true)
- `CreatedAt`: TIMESTAMPTZ NOT NULL (Permission creation date)

**C# Model Properties:**
```csharp
public int PermissionId { get; set; }
public string Name { get; set; }
public string Description { get; set; }
public string Category { get; set; }
public bool IsSystemPermission { get; set; }
public DateTime CreatedAt { get; set; }

// Navigation Properties
public ICollection<RolePermission> RolePermissions { get; set; }
```

**Permission Categories:**
- **System**: GlobalPortAccess, ManageAllPorts, ManageUsers, ManageRoles
- **Port**: ManagePortDetails, ViewPortDetails, ViewPortReports
- **Container**: ManageContainers, ViewContainers, TrackContainers
- **Ship**: ManageShips, ViewShips, ScheduleShips
- **Cargo**: ManageCargo, ViewCargo
- **Berth**: ManageBerths, ViewBerths, AllocateBerths
- **Equipment**: ManageEquipment, ViewEquipment

### 10. UserRole Entity (Junction Table)
Links users to their assigned roles.

**Table Name:** `UserRoles`

**Attributes:**
- `UserId`: INT NOT NULL (Foreign Key to Users)
- `RoleId`: INT NOT NULL (Foreign Key to Roles)
- `AssignedAt`: TIMESTAMPTZ NOT NULL (Role assignment date)
- `AssignedByUserId`: INT NULLABLE (User who assigned the role)
- `IsActive`: BOOLEAN NOT NULL (Role assignment status)

**C# Model Properties:**
```csharp
public int UserId { get; set; }
public int RoleId { get; set; }
public DateTime AssignedAt { get; set; }
public int? AssignedByUserId { get; set; }
public bool IsActive { get; set; }

// Navigation Properties
public User User { get; set; }
public Role Role { get; set; }
public User? AssignedBy { get; set; }
```

**Composite Primary Key:** (`UserId`, `RoleId`)

### 11. RolePermission Entity (Junction Table)
Links roles to their granted permissions.

**Table Name:** `RolePermissions`

**Attributes:**
- `RoleId`: INT NOT NULL (Foreign Key to Roles)
- `PermissionId`: INT NOT NULL (Foreign Key to Permissions)
- `GrantedAt`: TIMESTAMPTZ NOT NULL (Permission grant date)
- `GrantedByUserId`: INT NULLABLE (User who granted the permission)

**C# Model Properties:**
```csharp
public int RoleId { get; set; }
public int PermissionId { get; set; }
public DateTime GrantedAt { get; set; }
public int? GrantedByUserId { get; set; }

// Navigation Properties
public Role Role { get; set; }
public Permission Permission { get; set; }
public User? GrantedBy { get; set; }
```

**Composite Primary Key:** (`RoleId`, `PermissionId`)

## Updated Security Model

### Authentication Flow
1. User logs in with username/password
2. System validates credentials and generates JWT token
3. Token contains user ID, roles, and permissions as claims
4. Subsequent API requests include JWT token in Authorization header
5. Custom authorization attributes validate permissions/roles

### Authorization Levels
1. **Public**: No authentication required
2. **Authenticated**: Valid JWT token required
3. **Permission-Based**: Specific permissions required (e.g., `ManageContainers`)
4. **Role-Based**: Specific roles required (e.g., `Admin`, `PortManager`)
5. **Port-Specific**: Access limited to assigned port
6. **Resource Ownership**: User can only access their own resources

### Default Admin Account
- **Username**: `admin`
- **Password**: `Admin123!` (should be changed on first login)
- **Role**: Admin with all permissions
- **Created during**: Database seeding process

## ERD Diagram

![ER & Workflow Diagram](https://i.postimg.cc/mZ592xMT/Screenshot-2025-09-27-152440.png)

## Next Steps

1. ✅ ~~Implement database migrations using Entity Framework Core~~ **COMPLETED**
2. Create API controllers for CRUD operations
3. Implement real-time tracking with SignalR
4. Set up event producers/consumers for real-time updates
5. Build frontend integration for container tracking dashboard