# API Specification - Container Tracking & Port Operations

## Project Name
Container Tracking & Port Operations System

## Date
September 30, 2025

## Overview

This document provides the complete API specification for the Container Tracking & Port Operations System. The API is fully implemented and ready for production use.

## Base URL
```
http://localhost:5221
```

## Authentication

The API uses **JWT (JSON Web Tokens)** for authentication and **Role-Based Access Control (RBAC)** for authorization.

### Authentication Required
All endpoints except `/api/auth/login` and `/api/auth/register` require authentication.

### Authorization Header
```
Authorization: Bearer <jwt_token>
```

### Default Admin Account
- **Username:** `admin`
- **Password:** `Admin123!`
- **Roles:** Admin (full access)

### Quick Start Authentication
1. **Login** to get a JWT token:
   ```bash
   curl -X POST http://localhost:5221/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"Admin123!"}'
   ```

2. **Use the token** in subsequent requests:
   ```bash
   curl -X GET http://localhost:5221/api/containers \
     -H "Authorization: Bearer YOUR_TOKEN_HERE"
   ```

📖 **Complete Authentication Guide:** See [authentication-guide.md](authentication-guide.md) for detailed RBAC documentation.

## Response Format

All API responses follow a consistent structure:

### Success Response
```json
{
  "success": true,
  "data": {
    // Response data here
  },
  "message": "Operation completed successfully",
  "timestamp": "2025-09-30T10:00:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Error description",
  "errors": ["Detailed error messages"],
  "timestamp": "2025-09-30T10:00:00Z"
}
```

## API Endpoints

### 0. Authentication API 🔐

Base URL: `/api/auth`

#### POST /api/auth/login
Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response Example:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2025-09-30T12:00:00Z",
  "user": {
    "userId": 1,
    "username": "admin",
    "email": "admin@containertrack.com",
    "fullName": "System Administrator",
    "roles": ["Admin"],
    "permissions": ["GlobalPortAccess", "ManageAllPorts", "ManageUsers"],
    "assignedPort": null,
    "isActive": true
  }
}
```

#### POST /api/auth/register
Register a new user account.

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe",
  "phoneNumber": "+1234567890",
  "department": "Operations",
  "portId": 1,
  "roles": ["Operator"]
}
```

#### GET /api/auth/profile
Get current user profile information.

**Headers Required:**
```
Authorization: Bearer <jwt_token>
```

**Response Example:**
```json
{
  "success": true,
  "data": {
    "userId": 1,
    "username": "admin",
    "email": "admin@containertrack.com",
    "fullName": "System Administrator",
    "roles": ["Admin"],
    "permissions": ["GlobalPortAccess", "ManageAllPorts", "..."],
    "assignedPort": null,
    "isActive": true,
    "lastLoginAt": "2025-09-30T09:00:00Z",
    "createdAt": "2025-09-30T08:00:00Z"
  }
}
```

#### POST /api/auth/change-password
Change user password.

**Headers Required:**
```
Authorization: Bearer <jwt_token>
```

**Request Body:**
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!"
}
```

#### POST /api/auth/users/{userId}/roles
Assign roles to a user (Admin only).

**Headers Required:**
```
Authorization: Bearer <jwt_token>
```

**Request Body:**
```json
{
  "roleNames": ["Operator", "Viewer"]
}
```

---

### 1. Containers API

Base URL: `/api/containers`

#### GET /api/containers
Get all containers with optional filtering.

**Query Parameters:**
- `status` (optional): Filter by container status
- `location` (optional): Filter by current location
- `page` (optional): Page number for pagination
- `limit` (optional): Number of items per page

**Response Example:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "containerId": "MAEU1234567",
      "name": "Container Alpha",
      "type": "Dry",
      "status": "Available",
      "currentLocation": "Port of Copenhagen",
      "size": "40ft",
      "weight": 24500.0,
      "shipId": 1,
      "shipName": "Maersk Milano",
      "createdAt": "2024-01-10T08:00:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
  ],
  "message": "Containers retrieved successfully"
}
```

#### GET /api/containers/{id}
Get a specific container by ID.

**Parameters:**
- `id`: Container ID (integer)

**Response Example:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "containerId": "MAEU1234567",
    "name": "Container Alpha",
    "type": "Dry",
    "status": "Available",
    "currentLocation": "Port of Copenhagen",
    "size": "40ft",
    "weight": 24500.0,
    "shipId": 1,
    "shipName": "Maersk Milano",
    "berthAssignments": [
      {
        "id": 1,
        "berthId": 1,
        "berthName": "CPH-A1",
        "assignedAt": "2024-01-10T08:00:00Z",
        "status": "Active"
      }
    ],
    "shipContainers": [
      {
        "id": 1,
        "shipId": 1,
        "shipName": "Maersk Milano",
        "loadedAt": "2024-01-10T08:00:00Z",
        "status": "Loaded"
      }
    ]
  }
}
```

#### GET /api/containers/status/{status}
Get containers filtered by status.

**Parameters:**
- `status`: Container status (Available, In Transit, Loading, Loaded, Unloading, Maintenance)

#### GET /api/containers/location/{location}
Get containers filtered by current location.

**Parameters:**
- `location`: Current location string

#### POST /api/containers
Create a new container.

**Request Body:**
```json
{
  "containerId": "MAEU9876543",
  "name": "New Container",
  "type": "Dry",
  "status": "Available",
  "currentLocation": "Port of Hamburg",
  "size": "20ft",
  "weight": 15000.0,
  "shipId": 2
}
```

**Response:** Returns the created container with assigned ID.

#### PUT /api/containers/{id}
Update an existing container.

**Parameters:**
- `id`: Container ID

**Request Body:**
```json
{
  "name": "Updated Container Name",
  "status": "In Transit",
  "currentLocation": "Port of Rotterdam"
}
```

#### DELETE /api/containers/{id}
Delete a container.

**Parameters:**
- `id`: Container ID

### 2. Ships API

Base URL: `/api/ships`

#### GET /api/ships
Get all ships.

**Response Example:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Maersk Milano",
      "status": "Docked",
      "capacity": 18000,
      "currentLocation": "Port of Copenhagen",
      "arrivalTime": "2024-01-10T06:00:00Z",
      "departureTime": null,
      "containers": [
        {
          "id": 1,
          "containerId": "MAEU1234567",
          "name": "Container Alpha",
          "status": "Loaded"
        }
      ]
    }
  ]
}
```

#### GET /api/ships/{id}
Get a specific ship by ID with detailed container information.

#### GET /api/ships/status/{status}
Get ships filtered by status.

**Parameters:**
- `status`: Ship status (At Sea, Docked, Loading, Unloading, Maintenance)

#### POST /api/ships
Create a new ship.

**Request Body:**
```json
{
  "name": "New Ship Name",
  "status": "At Sea",
  "capacity": 15000,
  "currentLocation": "North Sea"
}
```

#### PUT /api/ships/{id}
Update an existing ship.

#### DELETE /api/ships/{id}
Delete a ship.

### 3. Ports API

Base URL: `/api/ports`

#### GET /api/ports
Get all ports.

**Response Example:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Port of Copenhagen",
      "location": "Copenhagen, Denmark",
      "totalContainerCapacity": 10000,
      "currentOccupancy": 2500,
      "berths": [
        {
          "id": 1,
          "name": "CPH-A1",
          "capacity": 500,
          "status": "Occupied"
        }
      ]
    }
  ]
}
```

#### GET /api/ports/{id}
Get a specific port by ID with berth details.

#### POST /api/ports
Create a new port.

**Request Body:**
```json
{
  "name": "Port of New Harbor",
  "location": "New Harbor City",
  "totalContainerCapacity": 8000
}
```

#### PUT /api/ports/{id}
Update an existing port.

#### DELETE /api/ports/{id}
Delete a port.

### 4. Berths API

Base URL: `/api/berths`

#### GET /api/berths
Get all berths.

**Response Example:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "CPH-A1",
      "portId": 1,
      "portName": "Port of Copenhagen",
      "capacity": 500,
      "currentOccupancy": 250,
      "status": "Occupied",
      "length": 400.0,
      "depth": 16.0,
      "berthAssignments": [
        {
          "id": 1,
          "containerId": "MAEU1234567",
          "containerName": "Container Alpha",
          "assignedAt": "2024-01-10T08:00:00Z",
          "status": "Active"
        }
      ]
    }
  ]
}
```

#### GET /api/berths/{id}
Get a specific berth by ID.

#### GET /api/berths/port/{portId}
Get all berths for a specific port.

#### GET /api/berths/status/{status}
Get berths filtered by status.

**Parameters:**
- `status`: Berth status (Available, Occupied, Maintenance)

#### POST /api/berths
Create a new berth.

**Request Body:**
```json
{
  "name": "HAM-T-03",
  "portId": 3,
  "capacity": 600,
  "status": "Available",
  "length": 350.0,
  "depth": 14.0
}
```

#### PUT /api/berths/{id}
Update an existing berth.

#### DELETE /api/berths/{id}
Delete a berth.

### 5. Berth Assignments API

Base URL: `/api/berth-assignments`

#### GET /api/berth-assignments
Get all berth assignments.

**Response Example:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "containerId": "MAEU1234567",
      "containerName": "Container Alpha",
      "berthId": 1,
      "berthName": "CPH-A1",
      "assignedAt": "2024-01-10T08:00:00Z",
      "expectedDeparture": "2024-01-15T14:00:00Z",
      "actualDeparture": null,
      "status": "Active",
      "notes": "Priority container for Maersk Milano"
    }
  ]
}
```

#### GET /api/berth-assignments/{id}
Get a specific berth assignment by ID.

#### GET /api/berth-assignments/container/{containerId}
Get berth assignments for a specific container.

#### GET /api/berth-assignments/berth/{berthId}
Get berth assignments for a specific berth.

#### POST /api/berth-assignments
Create a new berth assignment.

**Request Body:**
```json
{
  "containerId": "MAEU1234567",
  "berthId": 1,
  "expectedDeparture": "2024-01-20T14:00:00Z",
  "status": "Active",
  "notes": "Urgent assignment"
}
```

#### PUT /api/berth-assignments/{id}
Update an existing berth assignment.

#### DELETE /api/berth-assignments/{id}
Delete a berth assignment.

### 6. Ship Containers API

Base URL: `/api/ship-containers`

#### GET /api/ship-containers
Get all ship-container relationships.

**Response Example:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "shipId": 1,
      "shipName": "Maersk Milano",
      "containerId": "MAEU1234567",
      "containerName": "Container Alpha",
      "loadedAt": "2024-01-10T08:00:00Z",
      "unloadedAt": null,
      "position": "Deck A-01",
      "status": "Loaded"
    }
  ]
}
```

#### GET /api/ship-containers/{id}
Get a specific ship-container relationship by ID.

#### GET /api/ship-containers/ship/{shipId}
Get all container assignments for a specific ship.

#### POST /api/ship-containers
Create a new ship-container assignment.

**Request Body:**
```json
{
  "shipId": 1,
  "containerId": "MAEU1234567",
  "position": "Deck B-05",
  "status": "Loaded"
}
```

#### PUT /api/ship-containers/{id}
Update an existing ship-container assignment.

#### DELETE /api/ship-containers/{id}
Delete a ship-container assignment.

## Status Codes

- `200 OK`: Request successful
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Data Models

### Container Statuses
- `Available`: Container is available for assignment
- `In Transit`: Container is being moved
- `Loading`: Container is being loaded onto a ship
- `Loaded`: Container is loaded on a ship
- `Unloading`: Container is being unloaded from a ship
- `Maintenance`: Container is under maintenance

### Ship Statuses
- `At Sea`: Ship is traveling between ports
- `Docked`: Ship is docked at a port
- `Loading`: Ship is loading containers
- `Unloading`: Ship is unloading containers
- `Maintenance`: Ship is under maintenance

### Berth Statuses
- `Available`: Berth is available for container assignment
- `Occupied`: Berth has containers assigned
- `Maintenance`: Berth is under maintenance

### Assignment Statuses
- `Active`: Assignment is currently active
- `Completed`: Assignment has been completed
- `Cancelled`: Assignment was cancelled

## Testing

### Swagger UI
Access interactive API documentation at: http://localhost:5221/swagger

### Postman Collection
Import the provided Postman collection: `Container-Tracking-API.postman_collection.json`

### Sample Test Scenarios

1. **Container Lifecycle**:
   - Create container → Assign to berth → Load onto ship → Update status → Unload

2. **Port Operations**:
   - List all ports → Get specific port with berths → Create new berth → Assign containers

3. **Ship Management**:
   - Create ship → Load containers → Update ship status → Track container positions

For detailed testing scenarios, see the [Testing Guide](testing_guide.md).