/**
 * Pagination response interface
 */
export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

/**
 * Berth filters interface
 */
export interface BerthFilters {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  searchTerm?: string;
  status?: string;
  type?: string;
  portId?: string;
  minCapacity?: string;
  maxCapacity?: string;
}

/**
 * Enhanced Berth interface matching the updated backend BerthDto
 * Includes all rich fields from the backend Model
 */
export interface Berth {
  // Core identification
  berthId: number;
  name: string;
  identifier?: string; // Unique berth code (e.g., "B001", "WHARF-A1")
  
  // Berth specifications
  type?: string; // Container, Bulk, RoRo, Cruise, etc.
  capacity: number;
  currentLoad?: number;
  maxShipLength?: number; // in meters
  maxDraft?: number; // in meters
  
  // Operational status
  status: string; // Available, Occupied, Maintenance, etc.
  priority?: number; // Priority level (1-9)
  
  // Services and equipment
  availableServices?: string; // Comma-separated services
  craneCount?: number;
  
  // Financial
  hourlyRate?: number;
  
  // Port relationship
  portId: number;
  portName?: string;
  
  // Metrics
  activeAssignmentCount?: number;
  
  // Additional information
  notes?: string;
}

/**
 * Interface for creating or updating a berth
 */
export interface BerthCreateUpdate {
  name: string;
  identifier?: string;
  type?: string;
  capacity: number;
  status: string;
  portId: number;
  maxShipLength?: number;
  maxDraft?: number;
  availableServices?: string;
  craneCount?: number;
  hourlyRate?: number;
  priority?: number;
  notes?: string;
}

/**
 * Enhanced berth with performance analytics (future enhancement)
 */
export interface BerthWithAnalytics extends Berth {
  // Performance metrics
  averageOccupancyTime?: number; // in hours
  totalShipsServed: number;
  totalContainersProcessed: number;
  efficiencyRating?: number; // 0.00-5.00
  
  // Maintenance tracking
  lastMaintenanceDate?: string;
  nextMaintenanceDate?: string;
}

/**
 * Berth assignment interface
 */
export interface BerthAssignment {
  id: number;
  berthId: number;
  berthName?: string;
  shipId?: number;
  containerId?: string;
  assignmentType: string;
  priority?: string;
  status: string;
  scheduledArrival?: string;
  scheduledDeparture?: string;
  actualArrival?: string;
  actualDeparture?: string;
  assignedAt: string;
  releasedAt?: string;
  containerCount?: number;
  estimatedProcessingTime?: number;
  actualProcessingTime?: number;
  cost?: number;
  notes?: string;
}

/**
 * Berth usage charges for financial tracking
 */
export interface BerthUsageCharge {
  id: number;
  berthAssignmentId: number;
  hourlyRate: number;
  totalHours: number;
  baseCharges: number;
  serviceCharges: number;
  totalCharges: number;
  chargedAt: string;
  paymentStatus: 'Pending' | 'Paid' | 'Overdue';
  createdAt: string;
  updatedAt: string;
}

/**
 * Berth statistics for dashboard display
 */
export interface BerthStats {
  totalBerths: number;
  availableBerths: number;
  occupiedBerths: number;
  maintenanceBerths: number;
  averageOccupancyRate: number; // percentage
  totalRevenue: number;
  activeBerths: number;
  totalCapacity: number;
  currentOccupancy: number;
  statusCounts: Record<string, number>;
  averageUtilization: number;
  berthsByStatus: Record<string, number>;
  berthsByType: Record<string, number>;
  berthsByPort: Record<string, number>;
  portCounts: Record<string, number>;
  utilizationRanges: Record<string, number>;
  featureCounts: Record<string, number>;
}

/**
 * Real-time berth status for live updates
 */
export interface BerthRealtimeStatus {
  berthId: number;
  status: 'available' | 'occupied' | 'maintenance' | 'reserved';
  lastUpdated: string;
  currentShipId?: number;
  estimatedAvailable?: string;
}

/**
 * Form validation errors for berth forms
 */
export interface BerthFormErrors {
  name?: string;
  identifier?: string;
  type?: string;
  capacity?: string;
  currentLoad?: string;
  maxShipLength?: string;
  maxDraft?: string;
  status?: string;
  priority?: string;
  availableServices?: string;
  craneCount?: string;
  hourlyRate?: string;
  portId?: string;
  notes?: string;
}

// Type aliases for backward compatibility
export type BerthCreateRequest = BerthCreateUpdate;
export type BerthUpdateRequest = BerthCreateUpdate;