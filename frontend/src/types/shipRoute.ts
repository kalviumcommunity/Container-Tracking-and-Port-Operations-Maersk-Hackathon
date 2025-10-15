/**
 * ShipRoute type definitions aligned with backend DTOs
 */

export interface ShipRoute {
  id: number
  shipId: number
  shipName: string
  routeNumber: string
  originPortId: number
  originPortName: string
  destinationPortId: number
  destinationPortName: string
  scheduledDeparture: string
  scheduledArrival: string
  actualDeparture: string | null
  actualArrival: string | null
  routeStatus: string
  weatherDelay: number
  portDelay: number
  fuelConsumption: number | null
}

export interface ShipRouteCreateUpdate {
  shipId: number
  routeNumber: string
  originPortId: number
  destinationPortId: number
  scheduledDeparture: string
  scheduledArrival: string
  actualDeparture: string | null
  actualArrival: string | null
  routeStatus: string
  weatherDelay: number
  portDelay: number
  fuelConsumption: number | null
}

export type RouteStatus = 'Scheduled' | 'In Transit' | 'Delayed' | 'Completed' | 'Cancelled'
