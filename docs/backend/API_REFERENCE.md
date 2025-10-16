# 🔌 API Reference Guide

## Overview
Comprehensive REST API documentation for Maersk Container Tracking & Port Operations System. This API provides endpoints for container management, ship operations, berth assignments, and user authentication.

**Base URL**: `http://localhost:5221/api`  
**Authentication**: JWT Bearer Token  
**Content Type**: `application/json`

## 🔐 Authentication Endpoints

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "1",
      "username": "admin",
      "email": "admin@maersk.com",
      "role": "Admin"
    }
  },
  "message": "Login successful"
}
```

### Register New User (Admin Only)
```http
POST /api/auth/register
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "newuser",
  "email": "user@maersk.com",
  "password": "Password123!",
  "role": "Operator"
}
```

## 📦 Container Management

### Get All Containers
```http
GET /api/containers
Authorization: Bearer {token}
```

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `size` (int): Items per page (default: 10)
- `status` (string): Filter by status
- `type` (string): Filter by container type

### Create Container
```http
POST /api/containers
Authorization: Bearer {token}
Content-Type: application/json

{
  "containerNumber": "MAEU7654321",
  "type": "Standard",
  "size": "20ft",
  "weight": 15000.00,
  "cargo": "Electronics",
  "status": "Available",
  "location": "Port of Copenhagen",
  "portId": 1
}
```

### Update Container
```http
PUT /api/containers/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "In Transit",
  "location": "Port of Shanghai"
}
```

### Delete Container
```http
DELETE /api/containers/{id}
Authorization: Bearer {token}
```

## 🚢 Ship Management

### Get All Ships
```http
GET /api/ships
Authorization: Bearer {token}
```

### Create Ship
```http
POST /api/ships
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Maersk Sealand",
  "imoNumber": "IMO1234567",
  "flag": "Denmark",
  "type": "Container Ship",
  "capacity": 20000,
  "length": 400.5,
  "width": 59.0,
  "status": "At Port"
}
```

## 🏗️ Berth Management

### Get All Berths
```http
GET /api/berths
Authorization: Bearer {token}
```

### Create Berth Assignment
```http
POST /api/berth-assignments
Authorization: Bearer {token}
Content-Type: application/json

{
  "shipId": 1,
  "berthId": 1,
  "scheduledArrival": "2025-10-17T08:00:00Z",
  "scheduledDeparture": "2025-10-18T16:00:00Z",
  "status": "Scheduled"
}
```

## 🏢 Port Operations

### Get All Ports
```http
GET /api/ports
Authorization: Bearer {token}
```

### Create Port
```http
POST /api/ports
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Port of Copenhagen",
  "code": "DKCPH",
  "country": "Denmark",
  "city": "Copenhagen",
  "coordinates": {
    "latitude": 55.6761,
    "longitude": 12.5683
  }
}
```

## 📊 Analytics Endpoints

### Dashboard Statistics
```http
GET /api/analytics/dashboard-stats
Authorization: Bearer {token}
```

### Container Throughput
```http
GET /api/analytics/throughput?period=monthly&year=2025
Authorization: Bearer {token}
```

### Berth Utilization
```http
GET /api/analytics/berth-utilization?portId=1
Authorization: Bearer {token}
```

## 👥 User Management (Admin Only)

### Get All Users
```http
GET /api/users
Authorization: Bearer {token}
```

### Update User Role
```http
PUT /api/users/{id}/role
Authorization: Bearer {token}
Content-Type: application/json

{
  "role": "Port Manager"
}
```

## 🔍 Search & Filtering

### Advanced Container Search
```http
GET /api/containers/search?q=MAEU&type=Standard&status=Available
Authorization: Bearer {token}
```

### Ship Container Assignments
```http
GET /api/ship-containers?shipId=1
Authorization: Bearer {token}
```

## ❌ Error Responses

All endpoints return standardized error responses:

```json
{
  "success": false,
  "message": "Error description",
  "errors": {
    "field": ["Validation error message"]
  }
}
```

**Common HTTP Status Codes:**
- `200 OK` - Success
- `201 Created` - Resource created
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Invalid/missing token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## 🧪 Testing with Postman

1. Import collection: `docs/backend/Container-Tracking-API.postman_collection.json`
2. Set environment variables:
   - `baseUrl`: `http://localhost:5221`
   - `token`: (automatically set after login)
3. Run "Login (Admin)" request first
4. Use other requests with auto-populated token

## 🔗 API Documentation

- **Swagger UI**: `http://localhost:5221/swagger`
- **OpenAPI Spec**: `http://localhost:5221/swagger/v1/swagger.json`

## 🎯 Rate Limiting

- **Rate Limit**: 1000 requests per hour per IP
- **Burst Limit**: 100 requests per minute
- **Headers**: 
  - `X-RateLimit-Limit`
  - `X-RateLimit-Remaining`
  - `X-RateLimit-Reset`

## 🔒 Security Features

- JWT token expiration: 24 hours
- Refresh token rotation
- CORS enabled for frontend domain
- Input validation and sanitization
- SQL injection protection
- XSS prevention headers