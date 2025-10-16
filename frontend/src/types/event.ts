/**
 * Event System Type Definitions
 * Based on backend Event model and DTOs
 */

// Event entity (matches EventDto from backend)
export interface Event {
  id: number
  eventId: number
  eventType: string
  category: string
  priority: string
  status: string
  title: string
  description: string
  source: string
  severity: string
  eventTimestamp: string
  eventTime: string
  
  // Foreign keys
  containerId?: number
  shipId?: number
  berthId?: number
  portId?: number
  assignedToUserId?: number
  acknowledgedByUserId?: number
  
  // Navigation properties (names)
  containerNumber?: string
  shipName?: string
  berthName?: string
  portName?: string
  assignedToUserName?: string
  acknowledgedByUserName?: string
  
  // Additional fields
  eventData?: Record<string, any>
  metadata?: Record<string, any>
  coordinates?: string
  requiresAction: boolean
  isResolved: boolean
  isRead: boolean
  acknowledgedAt?: string
  resolvedAt?: string
  resolution?: string
  
  // Audit fields
  createdAt: string
  updatedAt: string
}

// DTO for creating a new event
export interface EventCreate {
  eventType: string
  title: string
  description: string
  eventTime: string
  severity: string
  
  // Optional foreign keys
  containerId?: number
  shipId?: number
  berthId?: number
  portId?: number
  userId?: number
  
  // Optional fields
  source?: string
  additionalData?: Record<string, any>
  category?: string
  priority?: string
  coordinates?: string
  requiresAction?: boolean
}

// DTO for updating an event
export interface EventUpdate {
  title?: string
  description?: string
  status?: string
  priority?: string
  assignedToUserId?: number
  resolution?: string
  eventData?: Record<string, any>
  metadata?: Record<string, any>
}

// Filter/search DTO for querying events
export interface EventFilter {
  // Search and filtering
  searchTerm?: string
  eventType?: string
  category?: string
  priority?: string
  severity?: string
  status?: string
  isResolved?: boolean
  requiresAction?: boolean
  isRead?: boolean
  
  // Related entities
  containerId?: number
  shipId?: number
  berthId?: number
  portId?: number
  assignedToUserId?: number
  
  // Date range filtering
  startDate?: string
  endDate?: string
  eventTimeFrom?: string
  eventTimeTo?: string
  
  // Pagination
  page?: number
  pageSize?: number
  
  // Sorting
  sortBy?: string
  sortDescending?: boolean
}

// Event statistics DTO
export interface EventStats {
  totalEvents: number
  unresolvedEvents: number
  criticalEvents: number
  highPriorityEvents: number
  eventsRequiringAction: number
  averageResolutionTime: number
  
  // Breakdown by type
  eventsByType: Record<string, number>
  eventsBySeverity: Record<string, number>
  eventsByStatus: Record<string, number>
  eventsByPriority: Record<string, number>
}

// Event trend data DTO
export interface EventTrend {
  date: string
  count: number
  eventType?: string
  severity?: string
}

// Paginated response wrapper (matches backend PaginatedResponse<T>)
export interface EventPagedResponse {
  data: Event[]  // Backend uses "Data" property (lowercase in JSON)
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

// Event acknowledgement request
export interface EventAcknowledgeRequest {
  eventId: number
  notes?: string
}

// Event resolution request
export interface EventResolveRequest {
  eventId: number
  resolution: string
  notes?: string
}

// Event type enum values (for reference and validation)
export const EventTypes = {
  CONTAINER_ARRIVAL: 'container_arrival',
  CONTAINER_DEPARTURE: 'container_departure',
  CONTAINER_DAMAGED: 'container_damaged',
  SHIP_ARRIVAL: 'ship_arrival',
  SHIP_DEPARTURE: 'ship_departure',
  SHIP_DELAYED: 'ship_delayed',
  BERTH_ASSIGNED: 'berth_assigned',
  BERTH_RELEASED: 'berth_released',
  BERTH_MAINTENANCE: 'berth_maintenance',
  PORT_CONGESTION: 'port_congestion',
  SECURITY_ALERT: 'security_alert',
  WEATHER_ALERT: 'weather_alert',
  SYSTEM_ALERT: 'system_alert',
  CUSTOM_EVENT: 'custom_event'
} as const

export type EventType = typeof EventTypes[keyof typeof EventTypes]

// Event severity enum values
export const EventSeverities = {
  LOW: 'low',
  MEDIUM: 'medium',
  HIGH: 'high',
  CRITICAL: 'critical'
} as const

export type EventSeverity = typeof EventSeverities[keyof typeof EventSeverities]

// Event status enum values
export const EventStatuses = {
  PENDING: 'pending',
  ACKNOWLEDGED: 'acknowledged',
  IN_PROGRESS: 'in_progress',
  RESOLVED: 'resolved',
  CLOSED: 'closed'
} as const

export type EventStatus = typeof EventStatuses[keyof typeof EventStatuses]

// Event priority enum values
export const EventPriorities = {
  LOW: 'low',
  NORMAL: 'normal',
  HIGH: 'high',
  URGENT: 'urgent'
} as const

export type EventPriority = typeof EventPriorities[keyof typeof EventPriorities]

// Event category enum values
export const EventCategories = {
  OPERATIONAL: 'operational',
  SECURITY: 'security',
  MAINTENANCE: 'maintenance',
  ADMINISTRATIVE: 'administrative',
  SYSTEM: 'system',
  ENVIRONMENTAL: 'environmental'
} as const

export type EventCategory = typeof EventCategories[keyof typeof EventCategories]

// Helper type for event severity colors (for UI)
export interface EventSeverityConfig {
  color: string
  bgColor: string
  icon: string
  label: string
}

export const EventSeverityConfigs: Record<EventSeverity, EventSeverityConfig> = {
  low: {
    color: 'text-blue-600',
    bgColor: 'bg-blue-100',
    icon: 'fa-info-circle',
    label: 'Low'
  },
  medium: {
    color: 'text-yellow-600',
    bgColor: 'bg-yellow-100',
    icon: 'fa-exclamation-triangle',
    label: 'Medium'
  },
  high: {
    color: 'text-orange-600',
    bgColor: 'bg-orange-100',
    icon: 'fa-exclamation',
    label: 'High'
  },
  critical: {
    color: 'text-red-600',
    bgColor: 'bg-red-100',
    icon: 'fa-fire',
    label: 'Critical'
  }
}
