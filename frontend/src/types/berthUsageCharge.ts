/**
 * BerthUsageCharge type definitions aligned with backend DTOs
 */

export interface BerthUsageCharge {
  id: number
  berthAssignmentId: number
  hourlyRate: number
  totalHours: number
  baseCharges: number
  serviceCharges: number
  totalCharges: number
  chargedAt: string
  paymentStatus: string
}

export interface BerthUsageChargeCreateUpdate {
  berthAssignmentId: number
  hourlyRate: number
  totalHours: number
  serviceCharges: number
  paymentStatus: string
}

export type PaymentStatus = 'Pending' | 'Paid' | 'Overdue' | 'Cancelled'
