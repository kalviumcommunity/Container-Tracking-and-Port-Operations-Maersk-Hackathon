# Berth Management Components

This folder contains all the frontend components for managing berth-related data in the Container Tracking and Port Operations system.

## Components Overview

### 🏗️ Core Components

#### `BerthModal.vue`
A comprehensive modal component for creating and editing berth information.

**Features:**
- Full-screen modal with form validation
- Supports both create and edit modes
- Real-time capacity utilization display for existing berths
- Special features selection with checkboxes
- Physical specifications (length, depth, draft)
- Equipment information (cranes, capacity)
- Operating hours and maintenance windows

**Usage:**
```vue
<BerthModal
  :is-editing="false"
  :berth="selectedBerth"
  :status-options="berthStatuses"
  :port-options="availablePorts"
  :is-submitting="saving"
  @submit="handleBerthSubmit"
  @cancel="closeBerthModal"
/>
```

#### `BerthList.vue`
A comprehensive table view for displaying and managing multiple berths.

**Features:**
- Sortable columns (name, status, capacity, etc.)
- Bulk selection with checkboxes
- Pagination controls
- Utilization progress bars
- Action buttons (view, edit, assignments, delete)
- Responsive design with mobile support
- Empty state handling

**Usage:**
```vue
<BerthList
  :berths="berths"
  :pagination="paginationData"
  :filters="currentFilters"
  :selected-berths="selectedBerths"
  :current-sort="sortConfig"
  :can-manage-berths="userCanManage"
  @edit="editBerth"
  @view="viewBerth"
  @delete="deleteBerth"
/>
```

#### `BerthCard.vue`
Individual berth display component with rich information visualization.

**Features:**
- Compact and detailed views
- Visual utilization indicators
- Feature badges
- Equipment information display
- Actions dropdown menu
- Responsive design

**Usage:**
```vue
<BerthCard
  :berth="berth"
  :compact="false"
  :show-actions="true"
  @view="viewBerth"
  @edit="editBerth"
  @assignments="manageBerthAssignments"
/>
```

#### `BerthForm.vue`
Standalone form component for berth data entry and editing.

**Features:**
- Section-based organization
- Form validation with error display
- Feature selection with visual icons
- Current status display for existing berths
- Customizable action buttons
- Progress indicators

**Usage:**
```vue
<BerthForm
  :is-editing="true"
  :berth="currentBerth"
  :status-options="statusOptions"
  :port-options="portOptions"
  title="Update Berth Information"
  :show-reset-button="true"
  @submit="saveBerth"
  @cancel="cancelEdit"
/>
```

### 🔍 Utility Components

#### `BerthFilters.vue`
Advanced filtering component with multiple filter types.

**Features:**
- Quick filter buttons for common scenarios
- Status and port dropdowns
- Capacity and utilization range sliders
- Feature checkboxes
- Search functionality
- Filter count display

**Usage:**
```vue
<BerthFilters
  :status-options="statusOptions"
  :port-options="portOptions"
  :initial-filters="currentFilters"
  @filters-changed="updateFilters"
/>
```

#### `BerthStats.vue`
Comprehensive statistics and analytics component.

**Features:**
- Key metrics cards (total, available, capacity, utilization)
- Status breakdown charts
- Port distribution
- Utilization range visualization
- Feature usage statistics

**Usage:**
```vue
<BerthStats
  :stats="berthStatistics"
  :loading="loadingStats"
/>
```

## Data Structures

### Berth Interface
```typescript
interface Berth {
  berthId: number;
  name: string;
  portName: string;
  portId: number;
  status: string;
  berthType?: string;
  capacity: number;
  currentOccupancy?: number;
  length?: number;
  waterDepth?: number;
  maxDraft?: number;
  craneCount?: number;
  craneCapacity?: number;
  features?: {
    refrigerated?: boolean;
    dangerous?: boolean;
    oversized?: boolean;
    heavyLift?: boolean;
    railConnection?: boolean;
    roadAccess?: boolean;
  };
  operatingHours?: string;
  maintenanceWindow?: string;
  notes?: string;
  activeAssignmentCount?: number;
  createdAt?: string;
  updatedAt?: string;
}
```

## Styling & Design

### Design System
- **Primary Colors:** Blue (#3B82F6) for primary actions
- **Status Colors:** Green (available), Yellow (occupied), Red (maintenance)
- **Typography:** Tailwind CSS utility classes
- **Spacing:** Consistent 4px grid system
- **Shadows:** Subtle elevation with `shadow-md` and `shadow-lg`

### Responsive Breakpoints
- **Mobile:** < 640px (stacked layouts)
- **Tablet:** 640px - 1024px (2-column grids)
- **Desktop:** > 1024px (3-4 column grids)

## Component Architecture

### State Management
- Components are designed to be stateless where possible
- Parent components manage data and pass props down
- Events are emitted up for data mutations
- Form validation is handled locally within form components

### Accessibility
- Proper ARIA labels and roles
- Keyboard navigation support
- Screen reader friendly content
- Color contrast compliance

### Performance
- Computed properties for expensive calculations
- Lazy loading for large datasets
- Virtualized scrolling considerations for large lists
- Optimized re-rendering with proper key usage

## Integration with Backend

### Expected API Endpoints
```
GET    /api/berths              - List berths with filtering/pagination
POST   /api/berths              - Create new berth
GET    /api/berths/{id}         - Get specific berth details
PUT    /api/berths/{id}         - Update berth
DELETE /api/berths/{id}         - Delete berth
GET    /api/berths/stats        - Get berth statistics
GET    /api/ports               - Get available ports for assignment
```

### Expected Data Format
Components expect data in the format defined by the backend DTOs (`BerthDto`, `BerthCreateUpdateDto`, `BerthDetailDto`).

## Usage Examples

### Basic Berth Management Page
```vue
<template>
  <div class="container mx-auto px-4 py-6">
    <!-- Stats Overview -->
    <BerthStats :stats="statistics" class="mb-6" />
    
    <!-- Filters -->
    <BerthFilters 
      :status-options="statusOptions"
      :port-options="ports"
      @filters-changed="updateFilters"
      class="mb-6"
    />
    
    <!-- Berth List -->
    <BerthList
      :berths="berths"
      :pagination="pagination"
      :filters="filters"
      :selected-berths="selectedBerths"
      :current-sort="sortConfig"
      :can-manage-berths="canManage"
      @edit="openEditModal"
      @view="viewBerth"
      @delete="confirmDelete"
    />
    
    <!-- Edit Modal -->
    <BerthModal
      v-if="showEditModal"
      :is-editing="true"
      :berth="editingBerth"
      :status-options="statusOptions"
      :port-options="ports"
      :is-submitting="saving"
      @submit="saveBerth"
      @cancel="closeEditModal"
    />
  </div>
</template>
```

## Future Enhancements

1. **Real-time Updates:** WebSocket integration for live berth status updates
2. **Drag & Drop:** Visual berth assignment interface
3. **3D Visualization:** Interactive port layout with berth positions
4. **Advanced Analytics:** Predictive utilization analysis
5. **Mobile App:** Dedicated mobile components for field operations
6. **Offline Support:** Progressive Web App capabilities

## Contributing

When adding new components or modifying existing ones:

1. Follow the established naming conventions
2. Maintain consistent prop interfaces
3. Add proper TypeScript types
4. Include comprehensive documentation
5. Test across different screen sizes
6. Ensure accessibility compliance
7. Add to the index.ts exports