# 📚 Documentation Index

## Overview
Complete documentation for the Container Tracking & Port Operations System - a production-ready ASP.NET Core 8.0 API with Azure PostgreSQL, JWT authentication, and comprehensive business data.

## 🚀 Quick Start

### For New Developers
1. **[Development Setup Guide](development-setup-guide.md)** - Get started in 5 minutes
2. **[JWT Testing Guide](../JWT-TESTING-GUIDE.md)** - Test authentication immediately
3. **[README](../README.md)** - Project overview and quick reference

### For API Testing
1. **[API Specification](api-specification.md)** - Complete endpoint reference
2. **[Authentication Guide](authentication-guide.md)** - RBAC and JWT details
3. **Postman Collection** - Import `Container-Tracking-API-Auth.postman_collection.json`

## 📖 Documentation Categories

### 🏗 Architecture & Design

#### [Architecture Overview](architecture-overview.md)
**Purpose:** High-level system architecture and component interactions  
**Key Topics:**
- System architecture diagram with Mermaid
- Component breakdown (Frontend, Backend, Data, Events)
- Data flow diagrams (Container tracking, Status updates, Berth assignments)
- Azure deployment architecture
- Real-time communication with SignalR

**Visual Diagrams:**
- ✅ System Architecture (Mermaid)
- ✅ Container Creation Flow (Sequence Diagram)
- ✅ Container Status Update Flow (Sequence Diagram)
- ✅ Berth Assignment Flow (Sequence Diagram)
- ✅ Azure Deployment Architecture (Mermaid)

---

#### [Database Entity Relationships](database-entity-relationships.md)
**Purpose:** Complete database schema and entity relationships  
**Key Topics:**
- Entity Relationship Diagram (ERD) with Mermaid
- Core entities: Port, Berth, Container, Ship, User, Role, Permission
- Relationships and foreign keys
- Sample data (25 ports, 60+ ships, 300 containers)
- Authentication entities (RBAC structure)

**Visual Diagrams:**
- ✅ Complete ERD (Mermaid)
- Entity attribute details
- Relationship cardinality

---

### 🔐 Authentication & Security

#### [Authentication Guide](authentication-guide.md)
**Purpose:** Comprehensive JWT authentication and RBAC documentation  
**Key Topics:**
- JWT authentication flow with sequence diagram
- Authorization flow with decision flowchart
- 4 user roles: Admin, Port Manager, Operator, Viewer
- 21 granular permissions
- Role-permission matrix with visual diagram
- Authorization attributes and usage

**Visual Diagrams:**
- ✅ JWT Authentication Flow (Sequence Diagram)
- ✅ Authorization Decision Flow (Flowchart)
- ✅ Role-Permission Hierarchy (Mermaid Graph)

**Default Admin Account:**
- Username: `admin`
- Password: `Admin123!`

---

#### [JWT Testing Guide](../JWT-TESTING-GUIDE.md)
**Purpose:** Quick reference for testing authentication  
**Key Topics:**
- Postman collection usage
- PowerShell/Bash test scripts
- Manual cURL examples
- Common troubleshooting

---

### 🛠 Development

#### [Development Setup Guide](development-setup-guide.md)
**Purpose:** Complete setup instructions for local and Azure development  
**Key Topics:**
- Prerequisites and required software
- Quick start (5-minute setup)
- Local PostgreSQL setup (Option 1)
- Docker PostgreSQL setup (Option 2)
- Azure PostgreSQL configuration (Option 3 - Production)
- Project structure diagram with Mermaid
- Database migrations and seeding
- Testing the setup

**Visual Diagrams:**
- ✅ Project Structure (Mermaid Graph)
- Folder structure overview

**Azure Configuration:**
- Server: `container-tracking-db-server.postgres.database.azure.com`
- SSL Mode: Required
- Enhanced seeding available

---

#### [API Specification](api-specification.md)
**Purpose:** Complete REST API endpoint reference  
**Key Topics:**
- API request flow diagram with Mermaid
- Authentication endpoints
- Container management endpoints
- Ship operations endpoints
- Port management endpoints
- Berth assignment endpoints
- Request/response examples
- Error handling

**Visual Diagrams:**
- ✅ API Request Flow (Mermaid Graph)
- Middleware pipeline visualization

**Base URL:** `http://localhost:5221`

---

### 📊 Data Management

#### [Enhanced Seeding Guide](../backend/docs/ENHANCED-SEEDING-GUIDE.md)
**Purpose:** Comprehensive business data seeding documentation  
**Key Topics:**
- 25 major world ports (6 continents)
- 60+ ships from major shipping lines
- 300 diverse containers with realistic data
- 120+ berth assignments
- 80+ ship-container operations
- PowerShell automation script
- API seeding endpoints

**Quick Start:**
```powershell
cd backend
.\seed-enhanced-data.ps1
```

---

### 🧪 Testing

#### [Testing Guide](testing_guide.md)
**Purpose:** API testing strategies and examples  
**Key Topics:**
- Postman collection usage
- Integration testing
- Unit testing approaches
- Common test scenarios

---

#### [Auth Quick Start](auth-quick-start.md)
**Purpose:** Rapid authentication testing guide  
**Key Topics:**
- Quick login examples
- Token usage patterns
- Common authentication scenarios

---

### 📝 Project Management

#### [Project Overview](project-overview.md)
**Purpose:** High-level project description and goals  
**Key Topics:**
- Project objectives
- Technology stack
- Key features
- Roadmap

---

#### [CHANGELOG](CHANGELOG.md)
**Purpose:** Version history and release notes  
**Key Topics:**
- Feature additions
- Bug fixes
- Breaking changes
- Migration notes

---

## 🎯 Common Use Cases

### I want to...

#### Start developing immediately
1. **[Development Setup Guide](development-setup-guide.md)** - Setup instructions
2. **[JWT Testing Guide](../JWT-TESTING-GUIDE.md)** - Test authentication
3. **[API Specification](api-specification.md)** - Endpoint reference

#### Understand the system architecture
1. **[Architecture Overview](architecture-overview.md)** - Visual diagrams and flows
2. **[Database Entity Relationships](database-entity-relationships.md)** - ERD and schema
3. **[API Specification](api-specification.md)** - API flow diagram

#### Test the API
1. **[JWT Testing Guide](../JWT-TESTING-GUIDE.md)** - Quick authentication testing
2. **[API Specification](api-specification.md)** - Complete endpoint reference
3. **Postman Collection** - Import and test all endpoints
4. **[Testing Guide](testing_guide.md)** - Comprehensive testing strategies

#### Understand authentication
1. **[Authentication Guide](authentication-guide.md)** - Complete RBAC documentation with diagrams
2. **[JWT Testing Guide](../JWT-TESTING-GUIDE.md)** - Quick testing reference
3. **[Auth Quick Start](auth-quick-start.md)** - Rapid authentication setup

#### Seed business data
1. **[Enhanced Seeding Guide](../backend/docs/ENHANCED-SEEDING-GUIDE.md)** - Comprehensive seeding documentation
2. Run: `.\seed-enhanced-data.ps1` in backend directory

#### Deploy to Azure
1. **[Architecture Overview](architecture-overview.md)** - Azure deployment architecture
2. **[Development Setup Guide](development-setup-guide.md)** - Azure PostgreSQL configuration
3. Check `.env` file for Azure connection settings

## 📊 Visual Documentation Assets

### Mermaid Diagrams Added (2025)
- ✅ System Architecture (architecture-overview.md)
- ✅ Container Creation Flow Sequence Diagram (architecture-overview.md)
- ✅ Container Status Update Flow Sequence Diagram (architecture-overview.md)
- ✅ Berth Assignment Flow Sequence Diagram (architecture-overview.md)
- ✅ Azure Deployment Architecture (architecture-overview.md)
- ✅ JWT Authentication Flow Sequence Diagram (authentication-guide.md)
- ✅ Authorization Decision Flowchart (authentication-guide.md)
- ✅ Role-Permission Hierarchy (authentication-guide.md)
- ✅ Database ERD (database-entity-relationships.md)
- ✅ API Request Flow (api-specification.md)
- ✅ Project Structure Diagram (development-setup-guide.md)
- ✅ Frontend-Backend Architecture (README.md)

## 🔄 Recently Updated (2025)

### Documentation Enhancements
- **All Mermaid diagrams added** for visual clarity
- **Enhanced seeding documentation** (25 ports, 60+ ships, 300 containers)
- **Azure PostgreSQL configuration** updated throughout
- **JWT authentication flows** visualized with sequence diagrams
- **RBAC hierarchy** shown with visual diagrams

### Technical Updates
- Azure PostgreSQL Flexible Server deployment
- Enhanced business data seeding system
- 3 database migrations applied
- JWT authentication fully implemented
- 4 roles and 21 permissions configured

## 📞 Support & Resources

### Key Files
- **[README.md](../README.md)** - Project overview
- **[JWT-TESTING-GUIDE.md](../JWT-TESTING-GUIDE.md)** - Quick auth testing
- **[CHANGELOG.md](CHANGELOG.md)** - Version history
- **Postman Collection** - `Container-Tracking-API-Auth.postman_collection.json`

### Test Scripts
- **PowerShell:** `scripts/test-auth.ps1`
- **Bash:** `scripts/test-auth.sh`
- **Enhanced Seeding:** `backend/seed-enhanced-data.ps1`

### Default Credentials
- **Admin Username:** `admin`
- **Admin Password:** `Admin123!`

---

**Last Updated:** January 2025  
**Documentation Status:** ✅ Complete with Mermaid Diagrams  
**Production Status:** ✅ Deployed to Azure PostgreSQL
