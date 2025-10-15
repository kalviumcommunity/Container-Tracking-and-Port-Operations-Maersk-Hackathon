import type { Component } from 'vue'

// Main Event Types
export interface EventDto {
  id: number
  eventType: string
  title: string
  description: string
  eventTime: string
  severity: string
  containerId?: string
  shipId?: number
  shipName?: string
  berthId?: number
  berthName?: string
  portId?: number
  portName?: string
  userId?: number
  userName?: string
  source: string
  isRead: boolean
  createdAt: string
}

export interface EventStatsDto {
  totalEvents: number
  unreadEvents: number
  todayEvents: number
  weekEvents: number
  eventsBySeverity: { [key: string]: number }
  eventsByType: { [key: string]: number }
  eventsBySource: { [key: string]: number }
  recentTrends: EventTrendDto[]
}

export interface EventTrendDto {
  date: string
  count: number
  eventType: string
}

export interface EventFilterDto {
  eventType?: string
  severity?: string
  source?: string
  containerId?: string
  shipId?: number
  berthId?: number
  portId?: number
  userId?: number
  eventAfter?: string
  eventBefore?: string
  isRead?: boolean
  searchTerm?: string
  sortBy?: string
  sortDirection?: string
  page?: number
  pageSize?: number
}

// UI Component Types
export interface EventStat {
  label: string
  value: string
  color: string
  bgColor: string
  iconColor: string
  icon: Component
  severity: string
  trend: 'up' | 'down'
  change: number
  period: string
}

export interface EventCategory {
  type: string
  count: number
  percentage: number
  color: string
  barColor: string
  icon: Component
}

export interface SeverityStat {
  level: string
  count: number
  description: string
  borderColor: string
  dotColor: string
  textColor: string
}

export interface StreamStats {
  eventsPerSecond: number
  avgLatency: number
  isActive: boolean
}

export interface QuickFilter {
  id: string
  label: string
  active: boolean
}

// Event Feed Types
export interface EventFeedProps {
  events: EventDto[]
  viewMode: 'list' | 'grid'
  autoRefresh: boolean
  filters: QuickFilter[]
}

// Modal Types
export interface EventModalProps {
  isOpen: boolean
  event: EventDto | null
  mode: 'create' | 'edit' | 'view'
}

export interface EventFormData {
  eventType: string
  title: string
  description: string
  severity: string
  containerId?: string
  shipId?: number
  berthId?: number
  portId?: number
  source: string
}