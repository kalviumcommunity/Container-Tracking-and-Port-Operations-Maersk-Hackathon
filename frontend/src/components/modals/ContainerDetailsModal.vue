<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-container">
      <!-- Header -->
      <div class="modal-header">
        <div class="header-content">
          <div class="icon-wrapper">
            <i class="fas fa-box"></i>
          </div>
          <div>
            <h2 class="modal-title">Container Details</h2>
            <p class="modal-subtitle">{{ container.containerId }}</p>
          </div>
        </div>
        <button @click="closeModal" class="close-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <!-- Status Bar -->
      <div class="status-bar">
        <span :class="['status-badge', getStatusClass(container.status)]">
          <i :class="getStatusIcon(container.status)"></i>
          {{ container.status }}
        </span>
        <span class="location-badge">
          <i class="fas fa-map-marker-alt"></i>
          {{ container.currentLocation }}
        </span>
      </div>

      <!-- Tabs -->
      <div class="tabs">
        <button 
          v-for="tab in tabs" 
          :key="tab.id"
          :class="['tab-btn', { active: activeTab === tab.id }]"
          @click="activeTab = tab.id"
        >
          <i :class="tab.icon"></i>
          {{ tab.label }}
          <span v-if="tab.count !== undefined" class="tab-count">{{ tab.count }}</span>
        </button>
      </div>

      <!-- Modal Body -->
      <div class="modal-body">
        <!-- Overview Tab -->
        <div v-if="activeTab === 'overview'" class="tab-content">
          <div class="info-grid">
            <div class="info-card">
              <h4><i class="fas fa-info-circle"></i> Basic Information</h4>
              <div class="info-row">
                <span class="label">Container ID:</span>
                <span class="value">{{ container.containerId }}</span>
              </div>
              <div class="info-row">
                <span class="label">Type:</span>
                <span class="value">{{ container.type || 'Standard' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Cargo Type:</span>
                <span class="value">{{ container.cargoType }}</span>
              </div>
              <div class="info-row">
                <span class="label">Status:</span>
                <span :class="['value', 'status-' + container.status.toLowerCase()]">
                  {{ container.status }}
                </span>
              </div>
            </div>

            <div class="info-card">
              <h4><i class="fas fa-box-open"></i> Cargo Details</h4>
              <div class="info-row">
                <span class="label">Description:</span>
                <span class="value">{{ container.cargoDescription || 'N/A' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Weight:</span>
                <span class="value">{{ container.weight ? `${container.weight} kg` : 'N/A' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Declared Value:</span>
                <span class="value">{{ container.declaredValue ? `$${container.declaredValue.toLocaleString()}` : 'N/A' }}</span>
              </div>
            </div>

            <div class="info-card">
              <h4><i class="fas fa-map-marked-alt"></i> Location & Route</h4>
              <div class="info-row">
                <span class="label">Current Location:</span>
                <span class="value">{{ container.currentLocation }}</span>
              </div>
              <div class="info-row">
                <span class="label">Origin:</span>
                <span class="value">{{ container.origin || 'N/A' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Destination:</span>
                <span class="value">{{ container.destination || 'N/A' }}</span>
              </div>
            </div>

            <div class="info-card">
              <h4><i class="fas fa-ship"></i> Current Assignment</h4>
              <div class="info-row">
                <span class="label">Ship:</span>
                <span class="value">{{ container.shipName || 'Not assigned' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Port:</span>
                <span class="value">{{ container.portName || 'N/A' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Last Updated:</span>
                <span class="value">{{ formatDate(container.updatedAt) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Movement History Tab -->
        <div v-if="activeTab === 'movements'" class="tab-content">
          <div v-if="loadingMovements" class="loading-state">
            <i class="fas fa-spinner fa-spin"></i> Loading movement history...
          </div>
          <div v-else-if="movements.length === 0" class="empty-state">
            <i class="fas fa-route"></i>
            <p>No movement history available</p>
          </div>
          <div v-else class="timeline">
            <div v-for="(movement, index) in movements" :key="movement.id" class="timeline-item">
              <div class="timeline-marker">
                <i :class="getMovementIcon(movement.movementType)"></i>
              </div>
              <div class="timeline-content">
                <div class="timeline-header">
                  <h5>{{ movement.movementType }}</h5>
                  <span class="timeline-date">{{ formatDate(movement.movementTimestamp || movement.movedAt) }}</span>
                </div>
                <div class="timeline-details">
                  <p>
                    <i class="fas fa-arrow-right"></i>
                    From: <strong>{{ movement.fromLocation }}</strong> 
                    → To: <strong>{{ movement.toLocation }}</strong>
                  </p>
                  <p v-if="movement.notes" class="timeline-notes">
                    <i class="fas fa-sticky-note"></i> {{ movement.notes }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Ship Cargo History Tab -->
        <div v-if="activeTab === 'ships'" class="tab-content">
          <div v-if="loadingShipHistory" class="loading-state">
            <i class="fas fa-spinner fa-spin"></i> Loading ship history...
          </div>
          <div v-else-if="shipHistory.length === 0" class="empty-state">
            <i class="fas fa-ship"></i>
            <p>No ship cargo history available</p>
          </div>
          <table v-else class="data-table">
            <thead>
              <tr>
                <th>Ship Name</th>
                <th>Loaded At</th>
                <th>Duration</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="record in shipHistory" :key="record.id">
                <td>
                  <div class="ship-cell">
                    <i class="fas fa-ship"></i>
                    <strong>{{ record.shipName }}</strong>
                  </div>
                </td>
                <td>{{ formatDate(record.loadedAt) }}</td>
                <td>{{ calculateDuration(record.loadedAt) }}</td>
                <td>
                  <span :class="['badge', record.id === currentShipRecord?.id ? 'badge-success' : 'badge-secondary']">
                    {{ record.id === currentShipRecord?.id ? 'Current' : 'Completed' }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Storage Fees Tab -->
        <div v-if="activeTab === 'fees'" class="tab-content">
          <div v-if="loadingFees" class="loading-state">
            <i class="fas fa-spinner fa-spin"></i> Loading storage fees...
          </div>
          <div v-else-if="storageFees.length === 0" class="empty-state">
            <i class="fas fa-dollar-sign"></i>
            <p>No storage fees recorded</p>
          </div>
          <div v-else>
            <!-- Fee Summary Cards -->
            <div class="fee-summary">
              <div class="fee-card">
                <div class="fee-icon total">
                  <i class="fas fa-calculator"></i>
                </div>
                <div class="fee-details">
                  <span class="fee-label">Total Fees</span>
                  <span class="fee-amount">${{ totalFees.toFixed(2) }}</span>
                </div>
              </div>
              <div class="fee-card">
                <div class="fee-icon paid">
                  <i class="fas fa-check-circle"></i>
                </div>
                <div class="fee-details">
                  <span class="fee-label">Paid</span>
                  <span class="fee-amount">${{ paidFees.toFixed(2) }}</span>
                </div>
              </div>
              <div class="fee-card">
                <div class="fee-icon pending">
                  <i class="fas fa-clock"></i>
                </div>
                <div class="fee-details">
                  <span class="fee-label">Pending</span>
                  <span class="fee-amount">${{ pendingFees.toFixed(2) }}</span>
                </div>
              </div>
            </div>

            <!-- Fee Details Table -->
            <table class="data-table">
              <thead>
                <tr>
                  <th>Port</th>
                  <th>Storage Period</th>
                  <th>Days</th>
                  <th>Daily Rate</th>
                  <th>Total</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="fee in storageFees" :key="fee.id">
                  <td>Port #{{ fee.portId }}</td>
                  <td>
                    {{ formatDate(fee.storageStartDate) }}
                    <br>
                    <small>to {{ fee.storageEndDate ? formatDate(fee.storageEndDate) : 'Present' }}</small>
                  </td>
                  <td>{{ fee.totalDays }} days</td>
                  <td>${{ fee.dailyStorageRate }}</td>
                  <td><strong>${{ fee.totalFees.toFixed(2) }}</strong></td>
                  <td>
                    <span :class="['badge', 'badge-' + fee.feeStatus.toLowerCase()]">
                      {{ fee.feeStatus }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Footer Actions -->
      <div class="modal-footer">
        <button @click="closeModal" class="btn btn-secondary">
          <i class="fas fa-times"></i> Close
        </button>
        <button @click="editContainer" class="btn btn-primary">
          <i class="fas fa-edit"></i> Edit Container
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type { Container } from '../../types/container'
import type { ContainerMovement } from '../../types/containerMovement'
import type { ShipContainer } from '../../types/shipContainer'
import type { ContainerStorageFee } from '../../types/containerStorageFee'
import { containerMovementApi } from '../../services/containerMovementApi'
import { shipContainerApi } from '../../services/shipContainerApi'
import { containerStorageFeeApi } from '../../services/containerStorageFeeApi'

interface Props {
  isOpen: boolean
  container: Container
}

const props = defineProps<Props>()
const emit = defineEmits(['close', 'edit'])

// State
const activeTab = ref('overview')
const loadingMovements = ref(false)
const loadingShipHistory = ref(false)
const loadingFees = ref(false)

const movements = ref<ContainerMovement[]>([])
const shipHistory = ref<ShipContainer[]>([])
const storageFees = ref<ContainerStorageFee[]>([])

// Tabs configuration
const tabs = computed(() => [
  { id: 'overview', label: 'Overview', icon: 'fas fa-info-circle' },
  { id: 'movements', label: 'Movement History', icon: 'fas fa-route', count: movements.value.length },
  { id: 'ships', label: 'Ship History', icon: 'fas fa-ship', count: shipHistory.value.length },
  { id: 'fees', label: 'Storage Fees', icon: 'fas fa-dollar-sign', count: storageFees.value.length }
])

// Computed
const currentShipRecord = computed(() => 
  shipHistory.value.find(record => record.shipName === props.container.shipName)
)

const totalFees = computed(() => 
  storageFees.value.reduce((sum, fee) => sum + fee.totalFees, 0)
)

const paidFees = computed(() => 
  storageFees.value
    .filter(fee => fee.feeStatus === 'Paid')
    .reduce((sum, fee) => sum + fee.totalFees, 0)
)

const pendingFees = computed(() => 
  storageFees.value
    .filter(fee => fee.feeStatus === 'Pending' || fee.feeStatus === 'Overdue')
    .reduce((sum, fee) => sum + fee.totalFees, 0)
)

// Methods
const closeModal = () => {
  emit('close')
}

const editContainer = () => {
  emit('edit', props.container)
}

const loadMovementHistory = async () => {
  if (!props.container.containerId) return
  
  loadingMovements.value = true
  try {
    const response = await containerMovementApi.getByContainer(props.container.containerId)
    movements.value = response.data
  } catch (error) {
    console.error('Error loading movement history:', error)
    movements.value = []
  } finally {
    loadingMovements.value = false
  }
}

const loadShipHistory = async () => {
  if (!props.container.containerId) return
  
  loadingShipHistory.value = true
  try {
    const response = await shipContainerApi.getContainerHistory(props.container.containerId)
    shipHistory.value = response.data
  } catch (error) {
    console.error('Error loading ship history:', error)
    shipHistory.value = []
  } finally {
    loadingShipHistory.value = false
  }
}

const loadStorageFees = async () => {
  if (!props.container.containerId) return
  
  loadingFees.value = true
  try {
    const response = await containerStorageFeeApi.getByContainer(props.container.containerId)
    storageFees.value = response.data
  } catch (error) {
    console.error('Error loading storage fees:', error)
    storageFees.value = []
  } finally {
    loadingFees.value = false
  }
}

const getStatusClass = (status: string) => {
  const statusMap: Record<string, string> = {
    'Empty': 'status-empty',
    'In Transit': 'status-transit',
    'At Port': 'status-port',
    'On Ship': 'status-ship',
    'Loaded': 'status-loaded',
    'Delivered': 'status-delivered'
  }
  return statusMap[status] || 'status-default'
}

const getStatusIcon = (status: string) => {
  const iconMap: Record<string, string> = {
    'Empty': 'fas fa-box-open',
    'In Transit': 'fas fa-truck-moving',
    'At Port': 'fas fa-warehouse',
    'On Ship': 'fas fa-ship',
    'Loaded': 'fas fa-box',
    'Delivered': 'fas fa-check-circle'
  }
  return iconMap[status] || 'fas fa-box'
}

const getMovementIcon = (type: string) => {
  const iconMap: Record<string, string> = {
    'Load': 'fas fa-upload',
    'Unload': 'fas fa-download',
    'Transfer': 'fas fa-exchange-alt',
    'Arrival': 'fas fa-map-marker-alt',
    'Departure': 'fas fa-plane-departure'
  }
  return iconMap[type] || 'fas fa-arrows-alt'
}

const formatDate = (dateString: string) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const calculateDuration = (loadedAt: string) => {
  const loaded = new Date(loadedAt)
  const now = new Date()
  const diffMs = now.getTime() - loaded.getTime()
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
  const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
  
  if (diffDays > 0) {
    return `${diffDays} day${diffDays !== 1 ? 's' : ''}`
  }
  return `${diffHours} hour${diffHours !== 1 ? 's' : ''}`
}

// Watch for modal open to load data
watch(() => props.isOpen, (isOpen) => {
  if (isOpen) {
    activeTab.value = 'overview'
    loadMovementHistory()
    loadShipHistory()
    loadStorageFees()
  }
})

// Watch tab changes
watch(activeTab, (newTab) => {
  if (newTab === 'movements' && movements.value.length === 0 && !loadingMovements.value) {
    loadMovementHistory()
  }
  if (newTab === 'ships' && shipHistory.value.length === 0 && !loadingShipHistory.value) {
    loadShipHistory()
  }
  if (newTab === 'fees' && storageFees.value.length === 0 && !loadingFees.value) {
    loadStorageFees()
  }
})
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 20px;
}

.modal-container {
  background: white;
  border-radius: 16px;
  max-width: 1000px;
  width: 100%;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px;
  border-bottom: 1px solid #e5e7eb;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.icon-wrapper {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 20px;
}

.modal-title {
  font-size: 24px;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0;
}

.modal-subtitle {
  font-size: 14px;
  color: #666;
  margin: 4px 0 0;
}

.close-btn {
  width: 36px;
  height: 36px;
  border: none;
  background: #f3f4f6;
  border-radius: 8px;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.close-btn:hover {
  background: #e5e7eb;
  color: #1f2937;
}

.status-bar {
  display: flex;
  gap: 12px;
  padding: 16px 24px;
  background: #f9fafb;
  border-bottom: 1px solid #e5e7eb;
}

.status-badge,
.location-badge {
  padding: 6px 12px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.status-badge {
  background: #dbeafe;
  color: #1e40af;
}

.status-badge.status-delivered { background: #d1fae5; color: #065f46; }
.status-badge.status-transit { background: #fef3c7; color: #92400e; }
.status-badge.status-ship { background: #ddd6fe; color: #5b21b6; }

.location-badge {
  background: white;
  color: #374151;
  border: 1px solid #e5e7eb;
}

.tabs {
  display: flex;
  padding: 0 24px;
  border-bottom: 2px solid #e5e7eb;
  overflow-x: auto;
}

.tab-btn {
  padding: 16px 20px;
  border: none;
  background: none;
  font-size: 14px;
  font-weight: 500;
  color: #6b7280;
  cursor: pointer;
  border-bottom: 3px solid transparent;
  transition: all 0.2s;
  white-space: nowrap;
  display: flex;
  align-items: center;
  gap: 8px;
}

.tab-btn:hover {
  color: #667eea;
}

.tab-btn.active {
  color: #667eea;
  border-bottom-color: #667eea;
}

.tab-count {
  background: #e5e7eb;
  color: #6b7280;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
}

.tab-btn.active .tab-count {
  background: #dbeafe;
  color: #1e40af;
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
}

.tab-content {
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
}

.info-card {
  background: #f9fafb;
  border-radius: 12px;
  padding: 20px;
}

.info-card h4 {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 16px 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #e5e7eb;
}

.info-row:last-child {
  border-bottom: none;
}

.info-row .label {
  font-size: 14px;
  color: #6b7280;
  font-weight: 500;
}

.info-row .value {
  font-size: 14px;
  color: #1f2937;
  font-weight: 600;
  text-align: right;
}

.timeline {
  position: relative;
  padding-left: 40px;
}

.timeline::before {
  content: '';
  position: absolute;
  left: 17px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: #e5e7eb;
}

.timeline-item {
  position: relative;
  margin-bottom: 24px;
}

.timeline-marker {
  position: absolute;
  left: -40px;
  width: 36px;
  height: 36px;
  background: white;
  border: 3px solid #667eea;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #667eea;
  font-size: 14px;
}

.timeline-content {
  background: #f9fafb;
  border-radius: 12px;
  padding: 16px;
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.timeline-header h5 {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.timeline-date {
  font-size: 12px;
  color: #6b7280;
}

.timeline-details p {
  font-size: 14px;
  color: #374151;
  margin: 4px 0;
}

.timeline-notes {
  color: #6b7280;
  font-style: italic;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 8px;
  overflow: hidden;
}

.data-table thead {
  background: #f9fafb;
}

.data-table th {
  padding: 12px 16px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  color: #6b7280;
  text-transform: uppercase;
  border-bottom: 2px solid #e5e7eb;
}

.data-table td {
  padding: 14px 16px;
  font-size: 14px;
  color: #374151;
  border-bottom: 1px solid #f3f4f6;
}

.ship-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  display: inline-block;
}

.badge-success { background: #d1fae5; color: #065f46; }
.badge-secondary { background: #e5e7eb; color: #374151; }
.badge-pending { background: #fef3c7; color: #92400e; }
.badge-paid { background: #d1fae5; color: #065f46; }
.badge-overdue { background: #fee2e2; color: #991b1b; }

.fee-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.fee-card {
  background: linear-gradient(135deg, #f9fafb 0%, #ffffff 100%);
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 16px;
}

.fee-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 20px;
}

.fee-icon.total { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
.fee-icon.paid { background: linear-gradient(135deg, #10b981 0%, #059669 100%); }
.fee-icon.pending { background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); }

.fee-details {
  flex: 1;
}

.fee-label {
  display: block;
  font-size: 12px;
  color: #6b7280;
  margin-bottom: 4px;
}

.fee-amount {
  display: block;
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
}

.loading-state,
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #9ca3af;
}

.loading-state i,
.empty-state i {
  font-size: 48px;
  margin-bottom: 16px;
  display: block;
}

.empty-state p {
  font-size: 16px;
  margin: 0;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 20px 24px;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
  border-radius: 0 0 16px 16px;
}

.btn {
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.btn-secondary {
  background: white;
  color: #374151;
  border: 1px solid #d1d5db;
}

.btn-secondary:hover {
  background: #f9fafb;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}
</style>
