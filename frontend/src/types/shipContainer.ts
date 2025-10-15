/**
 * ShipContainer type definitions aligned with backend DTOs
 * Represents cargo (containers) loaded on ships
 */

export interface ShipContainer {
  id: number
  shipId: number
  shipName: string
  containerId: string
  containerName: string
  loadedAt: string
}

export interface ShipContainerCreateUpdate {
  shipId: number
  containerId: string
  loadedAt: string | null
}
