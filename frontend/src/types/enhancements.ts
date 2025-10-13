/**
 * Container movement history tracking
 */
export interface ContainerMovementHistory {
  id: number;
  containerId: string;
  fromLocation?: string;
  toLocation?: string;
  movementType: 'Load' | 'Unload' | 'Transfer' | 'Storage';
  movedAt: string;
  movedBy?: string; // Operator/System name
  duration?: number; // Duration in hours
  equipment?: string; // Crane, Truck, etc.
  notes?: string;
  createdAt: string;
}

/**
 * Port operational enhancements
 */
export interface PortOperationalMetrics {
  portId: number;
  averageProcessingTime?: number; // hours
  peakHoursStart?: string; // Time format
  peakHoursEnd?: string; // Time format
  weatherConditions?: string;
  operationalEfficiencyRating?: number; // 0.00-5.00
}

/**
 * Container storage fee tracking
 */
export interface ContainerStorageFee {
  id: number;
  containerId: string;
  portId: number;
  storageStartDate: string;
  storageEndDate?: string;
  dailyStorageRate: number;
  totalDays: number;
  totalFees: number;
  feeStatus: 'Calculating' | 'Finalized' | 'Paid';
  createdAt: string;
  updatedAt: string;
}

/**
 * Analytics and reporting interfaces
 */
export interface PortAnalytics {
  portId: number;
  period: 'daily' | 'weekly' | 'monthly' | 'yearly';
  
  // Container metrics
  containersProcessed: number;
  averageProcessingTime: number;
  
  // Ship metrics
  shipsServed: number;
  averageTurnaroundTime: number;
  
  // Berth metrics
  averageBerthOccupancy: number; // percentage
  totalBerthRevenue: number;
  
  // Efficiency metrics
  onTimePerformance: number; // percentage
  equipmentUtilization: number; // percentage
  
  // Financial metrics
  totalRevenue: number;
  operatingCosts: number;
  profitMargin: number;
  
  // Timestamp
  generatedAt: string;
}

/**
 * Real-time dashboard metrics
 */
export interface DashboardMetrics {
  // Current operations
  activeOperations: number;
  containersMoved: number;
  shipsInPort: number;
  
  // Capacity utilization
  berthOccupancyRate: number; // percentage
  storageUtilization: number; // percentage
  craneUtilization: number; // percentage
  
  // Performance indicators
  averageWaitTime: number; // hours
  onTimeDeliveries: number; // percentage
  customerSatisfaction: number; // rating out of 5
  
  // Financial summary
  dailyRevenue: number;
  costPerContainer: number;
  
  // Alerts and notifications
  criticalAlerts: number;
  warningAlerts: number;
  
  // Environmental
  weatherStatus: string;
  tideLevel: string;
  
  // Last updated
  lastUpdated: string;
}

/**
 * Cost management and billing
 */
export interface BillingStatement {
  id: number;
  customerId: string;
  customerName: string;
  billingPeriodStart: string;
  billingPeriodEnd: string;
  
  // Berth charges
  berthUsageCharges: number;
  serviceCharges: number;
  
  // Storage fees
  containerStorageFees: number;
  
  // Additional services
  craneCharges: number;
  securityCharges: number;
  utilityCharges: number;
  
  // Totals
  subtotal: number;
  taxes: number;
  totalAmount: number;
  
  // Payment info
  paymentStatus: 'Pending' | 'Paid' | 'Overdue' | 'Cancelled';
  paymentDueDate: string;
  paymentDate?: string;
  
  // Metadata
  generatedAt: string;
  generatedByUserId?: number;
}