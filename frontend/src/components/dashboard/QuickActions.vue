<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
    <div class="border-b border-slate-200 p-6">
      <h3 class="text-lg font-semibold text-slate-900">Quick Actions</h3>
    </div>
    <div class="p-6 space-y-3">
      <button 
        v-for="action in actions"
        :key="action.id"
        @click="$emit('action-clicked', action.id)"
        :disabled="action.disabled || loading"
        class="w-full flex items-center gap-3 p-3 text-left rounded-lg hover:bg-slate-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        :class="{ 'bg-slate-50': loading }"
      >
        <component 
          :is="action.icon" 
          :size="18" 
          class="text-slate-600 flex-shrink-0"
          :class="{ 'animate-spin': loading && action.loading }"
        />
        <div class="flex-1 min-w-0">
          <div class="text-sm font-medium text-slate-700">{{ action.label }}</div>
          <div v-if="action.description" class="text-xs text-slate-500 mt-1">{{ action.description }}</div>
        </div>
        <span v-if="action.badge" class="ml-auto inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 flex-shrink-0">
          {{ action.badge }}
        </span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Ship, Users, AlertTriangle, Container } from 'lucide-vue-next';

interface QuickAction {
  id: string;
  label: string;
  description?: string;
  icon: any;
  disabled?: boolean;
  loading?: boolean;
  badge?: string;
}

interface Props {
  loading?: boolean;
  actions?: QuickAction[];
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  actions: undefined
});

const defaultActions: QuickAction[] = [
  {
    id: 'add-ship',
    label: 'Register Incoming Vessel',
    description: 'Add a new ship to the port system',
    icon: Ship,
    disabled: false
  },
  {
    id: 'manage-staff',
    label: 'Workforce Management',
    description: 'View and manage staff schedules',
    icon: Users,
    disabled: false
  },
  {
    id: 'emergency-protocols',
    label: 'Safety & Emergency',
    description: 'Access emergency procedures',
    icon: AlertTriangle,
    disabled: false
  },
  {
    id: 'container-search',
    label: 'Find Container',
    description: 'Track and locate containers',
    icon: Container,
    disabled: false
  }
];

const actions = computed(() => props.actions || defaultActions);

defineEmits<{
  'action-clicked': [actionId: string];
}>();
</script>
