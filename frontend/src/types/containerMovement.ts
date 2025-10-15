/**
 * ContainerMovement type definitions aligned with backend DTOs
 */

export interface ContainerMovement {
  id: number
  containerId: string
  movementType: string
  fromLocation: string
  toLocation: string
  movementTimestamp: string
  movedAt: string  // Alias for movementTimestamp
  status: string
  coordinates: string | null
  notes: string | null
  portId: number | null
  portName: string | null
  berthId: number | null
  berthName: string | null
  shipId: number | null
  shipName: string | null
  temperature: number | null
  humidity: number | null
  estimatedCompletion: string | null
  actualCompletion: string | null
  recordedByUserId: number
  recordedByUserName: string
  createdAt: string
}

export interface ContainerMovementCreate {
  containerId: string
  movementType: string
  fromLocation: string
  toLocation: string
  movementTimestamp?: string
  status: string
  coordinates?: string
  notes?: string
  portId?: number
  berthId?: number
  shipId?: number
  temperature?: number
  humidity?: number
  estimatedCompletion?: string
}

export interface ContainerMovementUpdate {
  status: string
  coordinates?: string
  notes?: string
  temperature?: number
  humidity?: number
  actualCompletion?: string
}

export type MovementType = 'Load' | 'Unload' | 'Transfer' | 'Arrival' | 'Departure' | 'Inspection'
export type MovementStatus = 'Pending' | 'In Progress' | 'Completed' | 'Cancelled'
