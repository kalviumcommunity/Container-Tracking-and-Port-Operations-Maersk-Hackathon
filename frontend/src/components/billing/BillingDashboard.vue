<template>
  <div class="billing-dashboard">
    <div class="dashboard-header">
      <h2>Billing & Revenue Dashboard</h2>
      <div class="header-actions">
        <button @click="refreshData" class="btn btn-secondary">
          <i class="fas fa-sync-alt"></i> Refresh
        </button>
        <button @click="exportData" class="btn btn-primary">
          <i class="fas fa-download"></i> Export Report
        </button>
      </div>
    </div>

    <!-- Revenue Summary Cards -->
    <div class="revenue-cards">
      <div class="card revenue-card">
        <div class="card-icon berth-revenue">
          <i class="fas fa-anchor"></i>
        </div>
        <div class="card-content">
          <h3>Berth Usage Revenue</h3>
          <p class="amount">${{ berthRevenue.toLocaleString() }}</p>
          <span class="period">This Period</span>
        </div>
      </div>

      <div class="card revenue-card">
        <div class="card-icon storage-revenue">
          <i class="fas fa-warehouse"></i>
        </div>
        <div class="card-content">
          <h3>Storage Fee Revenue</h3>
          <p class="amount">${{ storageRevenue.toLocaleString() }}</p>
          <span class="period">This Period</span>
        </div>
      </div>

      <div class="card revenue-card">
        <div class="card-icon total-revenue">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="card-content">
          <h3>Total Revenue</h3>
          <p class="amount">${{ totalRevenue.toLocaleString() }}</p>
          <span class="period">This Period</span>
        </div>
      </div>

      <div class="card revenue-card">
        <div class="card-icon pending">
          <i class="fas fa-clock"></i>
        </div>
        <div class="card-content">
          <h3>Pending Payments</h3>
          <p class="amount">${{ pendingAmount.toLocaleString() }}</p>
          <span class="count">{{ pendingCount }} invoices</span>
        </div>
      </div>
    </div>

    <!-- Date Range Filter -->
    <div class="filter-section">
      <div class="date-filters">
        <div class="form-group">
          <label>Start Date</label>
          <input v-model="startDate" type="date" class="form-control" @change="applyFilters" />
        </div>
        <div class="form-group">
          <label>End Date</label>
          <input v-model="endDate" type="date" class="form-control" @change="applyFilters" />
        </div>
        <button @click="setQuickFilter('today')" class="btn btn-sm">Today</button>
        <button @click="setQuickFilter('week')" class="btn btn-sm">This Week</button>
        <button @click="setQuickFilter('month')" class="btn btn-sm">This Month</button>
      </div>
    </div>

    <!-- Tabs for Different Billing Types -->
    <div class="billing-tabs">
      <button 
        :class="['tab-btn', { active: activeTab === 'berth' }]"
        @click="activeTab = 'berth'"
      >
        <i class="fas fa-anchor"></i> Berth Usage Charges
      </button>
      <button 
        :class="['tab-btn', { active: activeTab === 'storage' }]"
        @click="activeTab = 'storage'"
      >
        <i class="fas fa-warehouse"></i> Storage Fees
      </button>
    </div>

    <!-- Berth Usage Charges Table -->
    <div v-if="activeTab === 'berth'" class="billing-table-container">
      <div class="table-header">
        <h3>Berth Usage Charges</h3>
        <div class="status-filter">
          <select v-model="berthStatusFilter" @change="filterBerthCharges" class="form-control">
            <option value="">All Status</option>
            <option value="Pending">Pending</option>
            <option value="Paid">Paid</option>
            <option value="Overdue">Overdue</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      </div>

      <div v-if="loading" class="loading">
        <i class="fas fa-spinner fa-spin"></i> Loading charges...
      </div>

      <table v-else-if="filteredBerthCharges.length > 0" class="billing-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Assignment ID</th>
            <th>Hourly Rate</th>
            <th>Total Hours</th>
            <th>Base Charges</th>
            <th>Service Charges</th>
            <th>Total</th>
            <th>Charged At</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="charge in filteredBerthCharges" :key="charge.id">
            <td>{{ charge.id }}</td>
            <td>{{ charge.berthAssignmentId }}</td>
            <td>${{ charge.hourlyRate }}</td>
            <td>{{ charge.totalHours.toFixed(2) }}h</td>
            <td>${{ charge.baseCharges.toFixed(2) }}</td>
            <td>${{ charge.serviceCharges.toFixed(2) }}</td>
            <td class="amount">${{ charge.totalCharges.toFixed(2) }}</td>
            <td>{{ formatDate(charge.chargedAt) }}</td>
            <td>
              <span :class="['status-badge', charge.paymentStatus.toLowerCase()]">
                {{ charge.paymentStatus }}
              </span>
            </td>
            <td>
              <button @click="viewDetails('berth', charge)" class="btn-icon" title="View Details">
                <i class="fas fa-eye"></i>
              </button>
              <button 
                v-if="charge.paymentStatus === 'Pending'"
                @click="markAsPaid('berth', charge.id)" 
                class="btn-icon" 
                title="Mark as Paid"
              >
                <i class="fas fa-check"></i>
              </button>
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr class="total-row">
            <td colspan="6"><strong>Total</strong></td>
            <td class="amount"><strong>${{ berthChargesTotal.toFixed(2) }}</strong></td>
            <td colspan="3"></td>
          </tr>
        </tfoot>
      </table>

      <div v-else class="no-data">
        <i class="fas fa-receipt"></i>
        <p>No berth usage charges found</p>
      </div>
    </div>

    <!-- Container Storage Fees Table -->
    <div v-if="activeTab === 'storage'" class="billing-table-container">
      <div class="table-header">
        <h3>Container Storage Fees</h3>
        <div class="status-filter">
          <select v-model="storageStatusFilter" @change="filterStorageFees" class="form-control">
            <option value="">All Status</option>
            <option value="Pending">Pending</option>
            <option value="Paid">Paid</option>
            <option value="Overdue">Overdue</option>
            <option value="Waived">Waived</option>
          </select>
        </div>
      </div>

      <div v-if="loading" class="loading">
        <i class="fas fa-spinner fa-spin"></i> Loading fees...
      </div>

      <table v-else-if="filteredStorageFees.length > 0" class="billing-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Container ID</th>
            <th>Port ID</th>
            <th>Storage Start</th>
            <th>Storage End</th>
            <th>Daily Rate</th>
            <th>Total Days</th>
            <th>Total Fees</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="fee in filteredStorageFees" :key="fee.id">
            <td>{{ fee.id }}</td>
            <td>{{ fee.containerId }}</td>
            <td>{{ fee.portId }}</td>
            <td>{{ formatDate(fee.storageStartDate) }}</td>
            <td>{{ fee.storageEndDate ? formatDate(fee.storageEndDate) : 'Ongoing' }}</td>
            <td>${{ fee.dailyStorageRate }}</td>
            <td>{{ fee.totalDays }}</td>
            <td class="amount">${{ fee.totalFees.toFixed(2) }}</td>
            <td>
              <span :class="['status-badge', fee.feeStatus.toLowerCase()]">
                {{ fee.feeStatus }}
              </span>
            </td>
            <td>
              <button @click="viewDetails('storage', fee)" class="btn-icon" title="View Details">
                <i class="fas fa-eye"></i>
              </button>
              <button 
                v-if="fee.feeStatus === 'Pending'"
                @click="markAsPaid('storage', fee.id)" 
                class="btn-icon" 
                title="Mark as Paid"
              >
                <i class="fas fa-check"></i>
              </button>
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr class="total-row">
            <td colspan="7"><strong>Total</strong></td>
            <td class="amount"><strong>${{ storageFeesTotal.toFixed(2) }}</strong></td>
            <td colspan="2"></td>
          </tr>
        </tfoot>
      </table>

      <div v-else class="no-data">
        <i class="fas fa-warehouse"></i>
        <p>No storage fees found</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { berthUsageChargeApi } from '../../services/berthUsageChargeApi'
import { containerStorageFeeApi } from '../../services/containerStorageFeeApi'
import type { BerthUsageCharge } from '../../types/berthUsageCharge'
import type { ContainerStorageFee } from '../../types/containerStorageFee'

// State
const loading = ref(false)
const activeTab = ref<'berth' | 'storage'>('berth')

// Date filters
const startDate = ref('')
const endDate = ref('')

// Berth charges
const berthCharges = ref<BerthUsageCharge[]>([])
const berthStatusFilter = ref('')
const filteredBerthCharges = ref<BerthUsageCharge[]>([])

// Storage fees
const storageFees = ref<ContainerStorageFee[]>([])
const storageStatusFilter = ref('')
const filteredStorageFees = ref<ContainerStorageFee[]>([])

// Revenue data
const berthRevenue = ref(0)
const storageRevenue = ref(0)

// Computed
const totalRevenue = computed(() => berthRevenue.value + storageRevenue.value)

const pendingBerthCharges = computed(() => 
  berthCharges.value.filter(c => c.paymentStatus === 'Pending')
)

const pendingStorageFees = computed(() => 
  storageFees.value.filter(f => f.feeStatus === 'Pending')
)

const pendingAmount = computed(() => {
  const berthPending = pendingBerthCharges.value.reduce((sum, c) => sum + c.totalCharges, 0)
  const storagePending = pendingStorageFees.value.reduce((sum, f) => sum + f.totalFees, 0)
  return berthPending + storagePending
})

const pendingCount = computed(() => 
  pendingBerthCharges.value.length + pendingStorageFees.value.length
)

const berthChargesTotal = computed(() => 
  filteredBerthCharges.value.reduce((sum, c) => sum + c.totalCharges, 0)
)

const storageFeesTotal = computed(() => 
  filteredStorageFees.value.reduce((sum, f) => sum + f.totalFees, 0)
)

// Methods
const loadData = async () => {
  loading.value = true
  try {
    // Load berth charges
    const berthResponse = await berthUsageChargeApi.getAll()
    berthCharges.value = berthResponse.data
    filteredBerthCharges.value = berthResponse.data

    // Load storage fees
    const storageResponse = await containerStorageFeeApi.getAll()
    storageFees.value = storageResponse.data
    filteredStorageFees.value = storageResponse.data

    // Calculate revenue
    await calculateRevenue()
  } catch (error) {
    console.error('Error loading billing data:', error)
  } finally {
    loading.value = false
  }
}

const calculateRevenue = async () => {
  try {
    const berthRevenueResponse = await berthUsageChargeApi.calculateRevenue(startDate.value, endDate.value)
    berthRevenue.value = berthRevenueResponse.data

    const storageRevenueResponse = await containerStorageFeeApi.calculateRevenue(startDate.value, endDate.value)
    storageRevenue.value = storageRevenueResponse.data
  } catch (error) {
    console.error('Error calculating revenue:', error)
  }
}

const filterBerthCharges = () => {
  if (berthStatusFilter.value) {
    filteredBerthCharges.value = berthCharges.value.filter(
      c => c.paymentStatus === berthStatusFilter.value
    )
  } else {
    filteredBerthCharges.value = berthCharges.value
  }
}

const filterStorageFees = () => {
  if (storageStatusFilter.value) {
    filteredStorageFees.value = storageFees.value.filter(
      f => f.feeStatus === storageStatusFilter.value
    )
  } else {
    filteredStorageFees.value = storageFees.value
  }
}

const applyFilters = async () => {
  await calculateRevenue()
  await loadData()
}

const setQuickFilter = (period: string) => {
  const today = new Date()
  endDate.value = today.toISOString().split('T')[0]

  switch (period) {
    case 'today':
      startDate.value = endDate.value
      break
    case 'week':
      const weekAgo = new Date(today)
      weekAgo.setDate(today.getDate() - 7)
      startDate.value = weekAgo.toISOString().split('T')[0]
      break
    case 'month':
      const monthAgo = new Date(today)
      monthAgo.setMonth(today.getMonth() - 1)
      startDate.value = monthAgo.toISOString().split('T')[0]
      break
  }

  applyFilters()
}

const refreshData = () => {
  loadData()
}

const viewDetails = (type: string, item: any) => {
  // TODO: Open details modal
  console.log('View details:', type, item)
}

const markAsPaid = async (type: 'berth' | 'storage', id: number) => {
  try {
    if (type === 'berth') {
      const charge = berthCharges.value.find(c => c.id === id)
      if (charge) {
        await berthUsageChargeApi.update(id, {
          ...charge,
          paymentStatus: 'Paid'
        })
      }
    } else {
      const fee = storageFees.value.find(f => f.id === id)
      if (fee) {
        await containerStorageFeeApi.update(id, {
          ...fee,
          feeStatus: 'Paid'
        })
      }
    }
    await loadData()
  } catch (error) {
    console.error('Error marking as paid:', error)
  }
}

const exportData = () => {
  // TODO: Implement export functionality
  console.log('Export data')
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Lifecycle
onMounted(() => {
  // Set default date range to this month
  setQuickFilter('month')
})
</script>

<style scoped>
.billing-dashboard {
  padding: 24px;
  background: #f8f9fa;
  min-height: 100vh;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.dashboard-header h2 {
  font-size: 28px;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 12px;
}

/* Revenue Cards */
.revenue-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.revenue-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 20px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.card-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  color: white;
}

.card-icon.berth-revenue { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
.card-icon.storage-revenue { background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); }
.card-icon.total-revenue { background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); }
.card-icon.pending { background: linear-gradient(135deg, #fa709a 0%, #fee140 100%); }

.card-content h3 {
  font-size: 14px;
  font-weight: 500;
  color: #666;
  margin: 0 0 8px 0;
}

.card-content .amount {
  font-size: 28px;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0;
}

.card-content .period,
.card-content .count {
  font-size: 12px;
  color: #999;
}

/* Filters */
.filter-section {
  background: white;
  padding: 20px;
  border-radius: 12px;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.date-filters {
  display: flex;
  align-items: end;
  gap: 12px;
}

.date-filters .form-group {
  flex: 1;
  max-width: 200px;
}

.date-filters label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 8px;
}

/* Tabs */
.billing-tabs {
  display: flex;
  gap: 8px;
  margin-bottom: 24px;
}

.tab-btn {
  padding: 12px 24px;
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  color: #666;
  cursor: pointer;
  transition: all 0.3s;
}

.tab-btn:hover {
  border-color: #667eea;
  color: #667eea;
}

.tab-btn.active {
  background: #667eea;
  border-color: #667eea;
  color: white;
}

.tab-btn i {
  margin-right: 8px;
}

/* Table */
.billing-table-container {
  background: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.table-header h3 {
  font-size: 20px;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0;
}

.status-filter select {
  min-width: 150px;
}

.billing-table {
  width: 100%;
  border-collapse: collapse;
}

.billing-table thead {
  background: #f8f9fa;
}

.billing-table th {
  padding: 12px 16px;
  text-align: left;
  font-size: 14px;
  font-weight: 600;
  color: #333;
  border-bottom: 2px solid #e0e0e0;
}

.billing-table td {
  padding: 16px;
  font-size: 14px;
  color: #666;
  border-bottom: 1px solid #f0f0f0;
}

.billing-table td.amount {
  font-weight: 600;
  color: #1a1a1a;
}

.billing-table tfoot {
  background: #f8f9fa;
}

.total-row td {
  padding: 16px;
  font-size: 16px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  text-transform: capitalize;
}

.status-badge.pending {
  background: #fff3cd;
  color: #856404;
}

.status-badge.paid {
  background: #d4edda;
  color: #155724;
}

.status-badge.overdue {
  background: #f8d7da;
  color: #721c24;
}

.status-badge.cancelled,
.status-badge.waived {
  background: #e2e3e5;
  color: #383d41;
}

.btn-icon {
  background: none;
  border: none;
  color: #667eea;
  cursor: pointer;
  padding: 4px 8px;
  font-size: 16px;
  transition: color 0.3s;
}

.btn-icon:hover {
  color: #5568d3;
}

.loading,
.no-data {
  text-align: center;
  padding: 60px 20px;
  color: #999;
}

.loading i,
.no-data i {
  font-size: 48px;
  margin-bottom: 16px;
  display: block;
}

.no-data p {
  font-size: 16px;
  margin: 0;
}
</style>
