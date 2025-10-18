/**
 * Enhanced Ship interface matching the updated backend ShipDto
 * Includes all rich fields from the backend Model
 */
export interface Ship {
  // Core identification
  shipId: number;
  id?: number; // Frontend compatibility
  name: string;
  
  // Ship specifications
  imoNumber?: string; // International Maritime Organization number
  flag?: string; // Flag state/country of registration
  type?: string; // Container Ship, Bulk Carrier, Tanker, etc.
  capacity?: number; // Container capacity in TEU
  
  // Physical dimensions
  length?: number; // Length in meters
  beam?: number; // Beam (width) in meters
  draft?: number; // Draft (depth below waterline) in meters
  grossTonnage?: number; // Gross tonnage
  yearBuilt?: number; // Year the ship was built
  
  // Operational status
  status: string; // At Sea, Docked, Loading, etc.
  
  // Location and movement
  coordinates?: string; // GPS coordinates (latitude, longitude)
  speed?: number; // Current speed in knots
  heading?: number; // Current heading/direction in degrees
  
  // Route information
  nextPort?: string; // Next port destination
  estimatedArrival?: string; // ETA at next port
  currentPortId?: number; // Current port if docked
  
  // Container information
  containerCount?: number; // Current number of containers
  
  // Legacy compatibility fields
  currentLocation?: string; // Maps to coordinates or current port
  arrivalTime?: string; // Maps to estimatedArrival
  departureTime?: string; // For scheduled departure
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateShipRequest {
  name: string;
  status?: string;
  capacity?: number;
  type?: string;
  flag?: string;
  currentLocation?: string;
}

export interface UpdateShipRequest {
  name?: string;
  imoNumber?: string;
  flag?: string;
  type?: string;
  capacity?: number;
  status?: string;
  length?: number;
  beam?: number;
  draft?: number;
  grossTonnage?: number;
  yearBuilt?: number;
  coordinates?: string;
  speed?: number;
  heading?: number;
  nextPort?: string;
  estimatedArrival?: string;
  currentPortId?: number;
  
  // Legacy compatibility
  currentLocation?: string;
}

export interface ShipContainer {
  id: number;
  shipId: number;
  containerId: string;
  loadedAt: string;
  unloadedAt?: string;
  position?: string;
  status: string;
}

/**
 * Ship route information for optimization and tracking
 */
export interface ShipRoute {
  id: number;
  shipId: number;
  routeNumber?: string;
  originPortId: number;
  destinationPortId: number;
  scheduledDeparture: string;
  scheduledArrival: string;
  actualDeparture?: string;
  actualArrival?: string;
  routeStatus: 'Scheduled' | 'InProgress' | 'Completed' | 'Delayed';
  weatherDelay: number; // hours
  portDelay: number; // hours
  fuelConsumption?: number; // liters
  createdAt: string;
  updatedAt: string;
}

/**
 * Real-time ship tracking data
 */
export interface ShipRealtimeStatus {
  shipId: number;
  status: 'at-sea' | 'docked' | 'loading' | 'arriving' | 'departing';
  coordinates?: string;
  speed?: number;
  heading?: number;
  lastUpdated: string;
  estimatedArrival?: string;
}

/**
 * Ship statistics for dashboard display
 */
export interface ShipStats {
  totalShips: number;
  dockedShips: number;
  atSeaShips: number;
  averageSpeed: number;
  totalContainersTransported: number;
}

export interface BerthAssignment {
  id: number;
  shipId: number;
  berthId: number;
  assignedAt: string;
  expectedDeparture?: string;
  actualDeparture?: string;
  status: string;
}
