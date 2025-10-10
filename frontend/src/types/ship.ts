export interface Ship {
  id: number;
  shipId?: number; // Backend compatibility
  name: string;
  status?: string;
  capacity?: number;
  type?: string;
  flag?: string;
  currentLocation?: string;
  arrivalTime?: string;
  departureTime?: string;
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
  status?: string;
  capacity?: number;
  type?: string;
  flag?: string;
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

export interface BerthAssignment {
  id: number;
  shipId: number;
  berthId: number;
  assignedAt: string;
  expectedDeparture?: string;
  actualDeparture?: string;
  status: string;
}
