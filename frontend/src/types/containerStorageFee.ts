/**
 * ContainerStorageFee type definitions aligned with backend DTOs
 */

export interface ContainerStorageFee {
  id: number
  containerId: string
  portId: number
  storageStartDate: string
  storageEndDate: string | null
  dailyStorageRate: number
  totalDays: number
  totalFees: number
  feeStatus: string
}

export interface ContainerStorageFeeCreateUpdate {
  containerId: string
  portId: number
  storageStartDate: string
  storageEndDate: string | null
  dailyStorageRate: number
  feeStatus: string
}

export type FeeStatus = 'Pending' | 'Paid' | 'Overdue' | 'Waived'
