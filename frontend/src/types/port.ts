/**
 * Port interface matching the backend PortDto
 */
export interface Port {
  portId: number;
  name: string;
  code?: string; // International port code (e.g., USNYC, USLA)
  country: string;
  location: string;
  coordinates?: string; // GPS coordinates (latitude,longitude)
  totalContainerCapacity: number;
  currentContainerCount?: number;
  maxShipCapacity?: number;
  currentShipCount?: number;
  operatingHours?: string; // e.g., "24/7", "06:00-22:00"
  timeZone?: string; // e.g., "UTC+5", "EST"
  contactInfo?: string;
  services?: string; // Available services (Container, Bulk, Tanker, Passenger)
  status?: string; // Active, Maintenance, Closed
  berthCount?: number; // Number of berths within this port
  createdAt?: string;
  updatedAt?: string;
}

/**
 * Interface for creating or updating a port
 */
export interface PortCreateUpdate {
  name: string;
  code?: string;
  country: string;
  location: string;
  coordinates?: string;
  totalContainerCapacity: number;
  maxShipCapacity?: number;
  operatingHours?: string;
  timeZone?: string;
  contactInfo?: string;
  services?: string;
  status?: string;
}

/**
 * Port with detailed information including berths
 */
export interface PortDetail extends Port {
  berths: import('./berth').Berth[];
}

/**
 * Port statistics for dashboard display
 */
export interface PortStats {
  totalPorts: number;
  activePorts: number;
  totalCapacity: number;
  currentUtilization: number;
  averageOccupancyRate: number; // percentage
}

/**
 * Form validation errors for port forms
 */
export interface PortFormErrors {
  name?: string;
  code?: string;
  country?: string;
  location?: string;
  coordinates?: string;
  totalContainerCapacity?: string;
  maxShipCapacity?: string;
  operatingHours?: string;
  timeZone?: string;
  contactInfo?: string;
  services?: string;
  status?: string;
}