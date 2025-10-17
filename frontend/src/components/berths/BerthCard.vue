<template>
  <div class="berth-card">
    <!-- Compact Header -->
    <div class="berth-card__header">
      <div class="berth-card__main-info">
        <div class="berth-card__identity">
          <div class="berth-card__badge">
            <span class="berth-card__identifier">{{ berth.identifier || `B${berth.berthId}` }}</span>
          </div>
          <div class="berth-card__title">
            <h3 class="berth-card__name">{{ berth.name }}</h3>
            <div class="berth-card__type">{{ berth.type }}</div>
          </div>
        </div>
        <div class="berth-card__status-section">
          <span class="berth-card__status" :class="`berth-card__status--${getStatusClass(berth.status)}`">
            {{ berth.status }}
          </span>
          <div v-if="showActions" class="berth-card__menu">
            <button
              @click="showMenu = !showMenu"
              class="berth-card__menu-button"
              :class="{ 'berth-card__menu-button--active': showMenu }"
            >
              <MoreVertical class="berth-card__menu-icon" />
            </button>
            <div
              v-if="showMenu"
              v-click-outside="() => showMenu = false"
              class="berth-card__dropdown"
            >
              <button @click="handleAction('view')" class="berth-card__dropdown-item">
                <Eye class="berth-card__dropdown-icon" />
                View Details
              </button>
              <button @click="handleAction('edit')" class="berth-card__dropdown-item">
                <Edit class="berth-card__dropdown-icon" />
                Edit
              </button>
              <button @click="handleAction('assignments')" class="berth-card__dropdown-item">
                <Users class="berth-card__dropdown-icon" />
                Assignments
              </button>
              <div class="berth-card__dropdown-divider"></div>
              <button @click="handleAction('delete')" class="berth-card__dropdown-item berth-card__dropdown-item--danger">
                <Trash2 class="berth-card__dropdown-icon" />
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Quick Stats -->
      <div class="berth-card__quick-stats">
        <div class="berth-card__stat">
          <Building class="berth-card__stat-icon" />
          <span class="berth-card__stat-value">{{ berth.portName }}</span>
        </div>
        <div v-if="berth.hourlyRate" class="berth-card__stat">
          <DollarSign class="berth-card__stat-icon" />
          <span class="berth-card__stat-value">${{ berth.hourlyRate }}/hr</span>
        </div>
        <div v-if="berth.priority" class="berth-card__stat">
          <span class="berth-card__priority" :class="`berth-card__priority--${berth.priority}`">
            P{{ berth.priority }}
          </span>
        </div>
      </div>
    </div>

    <!-- Capacity Overview -->
    <div class="berth-card__capacity">
      <div class="berth-card__capacity-header">
        <span class="berth-card__capacity-title">Capacity</span>
        <span class="berth-card__utilization">{{ utilizationPercentage }}%</span>
      </div>
      
      <div class="berth-card__capacity-bars">
        <div class="berth-card__capacity-bar">
          <div class="berth-card__bar-labels">
            <span>Used: {{ berth.currentLoad || 0 }}</span>
            <span>Total: {{ berth.capacity }}</span>
          </div>
          <div class="berth-card__progress-bar">
            <div 
              class="berth-card__progress-fill"
              :class="`berth-card__progress-fill--${getUtilizationLevel(utilizationPercentage)}`"
              :style="{ width: `${Math.min(utilizationPercentage, 100)}%` }"
            ></div>
          </div>
        </div>
      </div>
      
      <div class="berth-card__capacity-numbers">
        <div class="berth-card__capacity-number">
          <Package class="berth-card__capacity-icon" />
          <div>
            <div class="berth-card__capacity-value">{{ formatNumber(berth.capacity) }}</div>
            <div class="berth-card__capacity-label">Max</div>
          </div>
        </div>
        <div class="berth-card__capacity-number">
          <TrendingUp class="berth-card__capacity-icon" />
          <div>
            <div class="berth-card__capacity-value">{{ berth.currentLoad || 0 }}</div>
            <div class="berth-card__capacity-label">Current</div>
          </div>
        </div>
        <div class="berth-card__capacity-number">
          <CheckCircle class="berth-card__capacity-icon" />
          <div>
            <div class="berth-card__capacity-value">{{ berth.capacity - (berth.currentLoad || 0) }}</div>
            <div class="berth-card__capacity-label">Available</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Compact Specifications -->
    <div class="berth-card__specs">
      <div class="berth-card__specs-grid">
        <div class="berth-card__spec">
          <span class="berth-card__spec-label">Length</span>
          <span class="berth-card__spec-value">{{ berth.maxShipLength ? `${berth.maxShipLength}m` : 'N/A' }}</span>
        </div>
        <div class="berth-card__spec">
          <span class="berth-card__spec-label">Draft</span>
          <span class="berth-card__spec-value">{{ berth.maxDraft ? `${berth.maxDraft}m` : 'N/A' }}</span>
        </div>
        <div class="berth-card__spec">
          <span class="berth-card__spec-label">Cranes</span>
          <span class="berth-card__spec-value">{{ berth.craneCount || 0 }}</span>
        </div>
        <div class="berth-card__spec">
          <span class="berth-card__spec-label">Assignments</span>
          <span class="berth-card__spec-value">{{ berth.activeAssignmentCount || 0 }}</span>
        </div>
      </div>
    </div>

    <!-- Services & Notes -->
    <div class="berth-card__footer">
      <div v-if="berth.availableServices" class="berth-card__services">
        <div class="berth-card__services-list">
          <Wrench class="berth-card__services-icon" />
          <span class="berth-card__services-text">
            {{ getServicesList(berth.availableServices).slice(0, 2).join(', ') }}
            <span v-if="getServicesList(berth.availableServices).length > 2" class="berth-card__services-more">
              +{{ getServicesList(berth.availableServices).length - 2 }} more
            </span>
          </span>
        </div>
      </div>
      
      <div v-if="berth.notes" class="berth-card__notes">
        <AlertCircle class="berth-card__notes-icon" />
        <span class="berth-card__notes-text">{{ berth.notes }}</span>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="berth-card__actions-bar">
      <button @click="$emit('view', berth)" class="berth-card__action-button">
        <Eye class="berth-card__action-icon" />
        View
      </button>
      <button @click="$emit('edit', berth)" class="berth-card__action-button">
        <Edit class="berth-card__action-icon" />
        Edit
      </button>
      <button @click="$emit('assignments', berth)" class="berth-card__action-button">
        <Users class="berth-card__action-icon" />
        Assign
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { 
  Building, 
  Package, 
  Eye, 
  Edit, 
  Users, 
  Trash2, 
  MoreVertical,
  TrendingUp,
  CheckCircle,
  DollarSign,
  Wrench,
  AlertCircle
} from 'lucide-vue-next';

interface Berth {
  berthId: number;
  name: string;
  identifier?: string;
  type?: string;
  status: string;
  capacity: number;
  currentLoad?: number;
  maxShipLength?: number;
  maxDraft?: number;
  availableServices?: string;
  craneCount?: number;
  hourlyRate?: number;
  priority?: number;
  notes?: string;
  portId: number;
  portName: string;
  activeAssignmentCount?: number;
  createdAt?: string;
  updatedAt?: string;
}

interface Props {
  berth: Berth;
  compact?: boolean;
  showActions?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  compact: false,
  showActions: true
});

const emit = defineEmits<{
  view: [berth: Berth];
  edit: [berth: Berth];
  assignments: [berth: Berth];
  delete: [berth: Berth];
}>();

const showMenu = ref(false);

const utilizationPercentage = computed(() => {
  if (!props.berth.capacity) return 0;
  return Math.round(((props.berth.currentLoad || 0) / props.berth.capacity) * 100);
});

const getStatusClass = (status: string): string => {
  const statusMap: Record<string, string> = {
    'Available': 'available',
    'Occupied': 'occupied',
    'Under Maintenance': 'maintenance',
    'Reserved': 'reserved',
    'Full': 'full',
    'Partially Occupied': 'partial',
    'Inactive': 'inactive',
    'Emergency': 'emergency'
  };
  
  return statusMap[status] || 'default';
};

const getUtilizationLevel = (percentage: number): string => {
  if (percentage >= 90) return 'high';
  if (percentage >= 75) return 'medium';
  if (percentage >= 50) return 'low';
  return 'very-low';
};

const formatNumber = (num: number): string => {
  if (!num) return '0';
  return new Intl.NumberFormat('en-US').format(num);
};

const getServicesList = (services: string): string[] => {
  if (!services) return [];
  return services.split(',').map(s => s.trim()).filter(s => s.length > 0);
};

const handleAction = (action: string) => {
  showMenu.value = false;
  emit(action as any, props.berth);
};

// Click outside directive
const vClickOutside = {
  beforeMount(el: any, binding: any) {
    el.clickOutsideEvent = (event: Event) => {
      if (!(el === event.target || el.contains(event.target))) {
        binding.value();
      }
    };
    document.addEventListener('click', el.clickOutsideEvent);
  },
  unmounted(el: any) {
    document.removeEventListener('click', el.clickOutsideEvent);
  }
};
</script>

<style scoped>
.berth-card {
  background: white;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
  overflow: hidden;
  width: 100%;
  max-width: 400px;
  min-height: 380px;
  display: flex;
  flex-direction: column;
}

.berth-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
  border-color: #bfdbfe;
}

/* Header Styles */
.berth-card__header {
  padding: 16px;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-bottom: 1px solid #e2e8f0;
}

.berth-card__main-info {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 12px;
}

.berth-card__identity {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.berth-card__badge {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #3b82f6, #1d4ed8);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  box-shadow: 0 2px 4px rgba(59, 130, 246, 0.2);
}

.berth-card__identifier {
  color: white;
  font-weight: 700;
  font-size: 14px;
}

.berth-card__title {
  min-width: 0;
}

.berth-card__name {
  font-size: 16px;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 4px 0;
  line-height: 1.2;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.berth-card__type {
  font-size: 12px;
  color: #64748b;
  font-weight: 500;
}

.berth-card__status-section {
  display: flex;
  align-items: center;
  gap: 8px;
}

.berth-card__status {
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 600;
  border: 1px solid;
  white-space: nowrap;
}

.berth-card__status--available {
  background: #dcfce7;
  color: #166534;
  border-color: #bbf7d0;
}

.berth-card__status--occupied {
  background: #fef3c7;
  color: #92400e;
  border-color: #fde68a;
}

.berth-card__status--maintenance {
  background: #fee2e2;
  color: #991b1b;
  border-color: #fecaca;
}

.berth-card__status--reserved {
  background: #dbeafe;
  color: #1e40af;
  border-color: #93c5fd;
}

.berth-card__status--default {
  background: #f1f5f9;
  color: #475569;
  border-color: #cbd5e1;
}

/* Quick Stats */
.berth-card__quick-stats {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.berth-card__stat {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: #64748b;
}

.berth-card__stat-icon {
  width: 12px;
  height: 12px;
}

.berth-card__stat-value {
  font-weight: 500;
}

.berth-card__priority {
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 10px;
  font-weight: 700;
  border: 1px solid;
}

.berth-card__priority--1,
.berth-card__priority--2,
.berth-card__priority--3 {
  background: #fef2f2;
  color: #dc2626;
  border-color: #fecaca;
}

.berth-card__priority--4,
.berth-card__priority--5,
.berth-card__priority--6 {
  background: #fffbeb;
  color: #d97706;
  border-color: #fed7aa;
}

.berth-card__priority--7,
.berth-card__priority--8,
.berth-card__priority--9 {
  background: #dbeafe;
  color: #1d4ed8;
  border-color: #93c5fd;
}

/* Menu Styles */
.berth-card__menu {
  position: relative;
}

.berth-card__menu-button {
  padding: 6px;
  border-radius: 4px;
  color: #64748b;
  border: 1px solid transparent;
  background: transparent;
  cursor: pointer;
  transition: all 0.2s ease;
}

.berth-card__menu-button:hover {
  background: white;
  color: #374151;
  border-color: #e2e8f0;
}

.berth-card__menu-icon {
  width: 14px;
  height: 14px;
}

.berth-card__dropdown {
  position: absolute;
  right: 0;
  top: 100%;
  margin-top: 4px;
  width: 160px;
  background: white;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  border: 1px solid #e2e8f0;
  z-index: 10;
  overflow: hidden;
}

.berth-card__dropdown-item {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  padding: 8px 12px;
  background: transparent;
  border: none;
  font-size: 12px;
  color: #374151;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.berth-card__dropdown-item:hover {
  background: #f8fafc;
}

.berth-card__dropdown-item--danger {
  color: #dc2626;
}

.berth-card__dropdown-item--danger:hover {
  background: #fef2f2;
}

.berth-card__dropdown-icon {
  width: 12px;
  height: 12px;
  flex-shrink: 0;
}

.berth-card__dropdown-divider {
  height: 1px;
  background: #e2e8f0;
  margin: 2px 0;
}

/* Capacity Section */
.berth-card__capacity {
  padding: 16px;
  border-bottom: 1px solid #f1f5f9;
}

.berth-card__capacity-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.berth-card__capacity-title {
  font-size: 12px;
  font-weight: 600;
  color: #374151;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.berth-card__utilization {
  font-size: 11px;
  color: #64748b;
  font-weight: 500;
}

.berth-card__capacity-bars {
  margin-bottom: 12px;
}

.berth-card__capacity-bar {
  margin-bottom: 8px;
}

.berth-card__bar-labels {
  display: flex;
  justify-content: space-between;
  font-size: 10px;
  color: #64748b;
  margin-bottom: 4px;
}

.berth-card__progress-bar {
  width: 100%;
  height: 4px;
  background: #e2e8f0;
  border-radius: 2px;
  overflow: hidden;
}

.berth-card__progress-fill {
  height: 100%;
  border-radius: 2px;
  transition: width 0.5s ease;
}

.berth-card__progress-fill--very-low {
  background: linear-gradient(90deg, #10b981, #34d399);
}

.berth-card__progress-fill--low {
  background: linear-gradient(90deg, #3b82f6, #60a5fa);
}

.berth-card__progress-fill--medium {
  background: linear-gradient(90deg, #f59e0b, #fbbf24);
}

.berth-card__progress-fill--high {
  background: linear-gradient(90deg, #ef4444, #f87171);
}

.berth-card__capacity-numbers {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
}

.berth-card__capacity-number {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px;
  background: #f8fafc;
  border-radius: 6px;
  border: 1px solid #e2e8f0;
}

.berth-card__capacity-icon {
  width: 14px;
  height: 14px;
  flex-shrink: 0;
}

.berth-card__capacity-number:nth-child(1) .berth-card__capacity-icon {
  color: #3b82f6;
}

.berth-card__capacity-number:nth-child(2) .berth-card__capacity-icon {
  color: #f59e0b;
}

.berth-card__capacity-number:nth-child(3) .berth-card__capacity-icon {
  color: #10b981;
}

.berth-card__capacity-value {
  font-size: 14px;
  font-weight: 700;
  line-height: 1;
  margin-bottom: 2px;
}

.berth-card__capacity-number:nth-child(1) .berth-card__capacity-value {
  color: #1d4ed8;
}

.berth-card__capacity-number:nth-child(2) .berth-card__capacity-value {
  color: #c2410c;
}

.berth-card__capacity-number:nth-child(3) .berth-card__capacity-value {
  color: #15803d;
}

.berth-card__capacity-label {
  font-size: 10px;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

/* Specifications */
.berth-card__specs {
  padding: 12px 16px;
  border-bottom: 1px solid #f1f5f9;
}

.berth-card__specs-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;
}

.berth-card__spec {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 8px;
  background: #f8fafc;
  border-radius: 4px;
  border: 1px solid #e2e8f0;
}

.berth-card__spec-label {
  font-size: 10px;
  color: #64748b;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.berth-card__spec-value {
  font-size: 11px;
  font-weight: 600;
  color: #1e293b;
}

/* Footer */
.berth-card__footer {
  padding: 12px 16px;
  border-bottom: 1px solid #f1f5f9;
  flex: 1;
}

.berth-card__services {
  margin-bottom: 8px;
}

.berth-card__services-list {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 11px;
  color: #4338ca;
}

.berth-card__services-icon {
  width: 12px;
  height: 12px;
  flex-shrink: 0;
}

.berth-card__services-text {
  font-weight: 500;
}

.berth-card__services-more {
  color: #64748b;
  font-weight: 400;
}

.berth-card__notes {
  display: flex;
  align-items: flex-start;
  gap: 6px;
  padding: 8px;
  background: #fffbeb;
  border: 1px solid #fcd34d;
  border-radius: 4px;
}

.berth-card__notes-icon {
  width: 12px;
  height: 12px;
  color: #d97706;
  flex-shrink: 0;
  margin-top: 1px;
}

.berth-card__notes-text {
  font-size: 11px;
  color: #92400e;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Actions Bar */
.berth-card__actions-bar {
  display: flex;
  padding: 12px 16px;
  background: #f8fafc;
  border-top: 1px solid #e2e8f0;
}

.berth-card__action-button {
  display: flex;
  align-items: center;
  gap: 4px;
  flex: 1;
  padding: 6px;
  border: none;
  background: transparent;
  font-size: 11px;
  font-weight: 600;
  color: #64748b;
  cursor: pointer;
  border-radius: 4px;
  transition: all 0.2s ease;
  justify-content: center;
}

.berth-card__action-button:hover {
  background: white;
  color: #374151;
}

.berth-card__action-icon {
  width: 12px;
  height: 12px;
}

/* Responsive Design */
@media (max-width: 640px) {
  .berth-card {
    max-width: 100%;
  }
  
  .berth-card__main-info {
    flex-direction: column;
    gap: 8px;
  }
  
  .berth-card__status-section {
    align-self: flex-end;
  }
}
</style>