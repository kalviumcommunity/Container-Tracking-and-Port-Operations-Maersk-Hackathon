# Main Components

This folder contains the core/main components of the Port Operations application. These are the primary components that represent major application sections and workflows.

## Components

### 🏠 **Home.vue & Home_new.vue**
- Main landing/welcome pages
- Entry points for users
- Application overview and navigation

### 📊 **Dashboard.vue** 
- Main operational dashboard
- Real-time metrics and KPIs
- Central hub for port operations monitoring

### 📦 **ContainerManagement.vue**
- Complete container lifecycle management
- Container CRUD operations
- Advanced filtering, searching, and bulk operations
- Container statistics and analytics

### 🚢 **PortOperationManagement.vue**
- Port-wide operations management
- Ship scheduling and arrivals
- Port capacity and resource management

### ⚓ **BerthOperationsMain.vue**
- Berth assignment and management
- Berth scheduling and availability
- Ship-to-berth allocation

### 📡 **EventStreaming.vue**
- Real-time event monitoring
- Live data feeds and notifications
- Event analytics and streaming dashboard

## Usage

Import components individually:
```typescript
import Dashboard from '@/components/main/Dashboard.vue'
import ContainerManagement from '@/components/main/ContainerManagement.vue'
```

Or use the index file for bulk imports:
```typescript
import { Dashboard, ContainerManagement, EventStreaming } from '@/components/main'
```

## Dependencies

These components depend on:
- `../dashboard/` - Dashboard sub-components
- `../containers/` - Container-specific UI components  
- `../berths/` - Berth-specific UI components
- `../../services/` - API services and business logic
- `../../types/` - TypeScript type definitions

## Notes

- All main components are designed to be standalone and reusable
- Each component handles its own state management and API calls
- Components follow the composition API pattern with TypeScript
- Error handling and loading states are built into each component