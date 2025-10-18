<!-- Reusable Stats Card Component -->
<template>
  <div class="bg-white rounded-xl border border-gray-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
    <!-- Loading State -->
    <div v-if="loading" class="animate-pulse">
      <div class="flex items-start justify-between mb-4">
        <div class="w-12 h-12 bg-gray-200 rounded-lg"></div>
      </div>
      <div class="space-y-2">
        <div class="h-8 bg-gray-200 rounded w-16"></div>
        <div class="h-4 bg-gray-200 rounded w-24"></div>
        <div class="h-3 bg-gray-200 rounded w-20"></div>
      </div>
    </div>

    <!-- Content -->
    <div v-else>
      <!-- Icon -->
      <div class="flex items-start justify-between mb-4">
        <div :class="iconClasses">
          <component :is="iconComponent" :size="24" />
        </div>
      </div>

      <!-- Value and Title -->
      <div class="mb-3">
        <p class="text-3xl font-bold text-gray-900">{{ formattedValue }}</p>
        <p class="text-sm font-medium text-gray-600">{{ title }}</p>
        
        <!-- Trend -->
        <div v-if="trend" class="flex items-center gap-1 mt-2">
          <TrendingUp :size="14" :class="trendColor" />
          <span class="text-sm font-medium" :class="trendColor">{{ trend.value }}{{ trendUnit }}</span>
          <span class="text-sm text-gray-500">{{ trend.label }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { 
  MapPin, 
  CheckCircle, 
  Clock, 
  Ship, 
  TrendingUp, 
  Package,
  Activity
} from 'lucide-vue-next';

interface Trend {
  value: number;
  label: string;
}

interface Props {
  title: string;
  value: number;
  icon: string;
  color: 'blue' | 'green' | 'orange' | 'purple' | 'red' | 'gray';
  trend?: Trend;
  loading?: boolean;
  unit?: string;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  unit: ''
});

const iconMap = {
  MapPin,
  CheckCircle,
  Clock,
  Ship,
  Package,
  Activity
};

const iconComponent = computed(() => {
  return iconMap[props.icon as keyof typeof iconMap] || MapPin;
});

const iconClasses = computed(() => {
  const colorClasses = {
    blue: 'p-3 bg-blue-50 rounded-lg text-blue-600',
    green: 'p-3 bg-green-50 rounded-lg text-green-600',
    orange: 'p-3 bg-orange-50 rounded-lg text-orange-600',
    purple: 'p-3 bg-purple-50 rounded-lg text-purple-600',
    red: 'p-3 bg-red-50 rounded-lg text-red-600',
    gray: 'p-3 bg-gray-50 rounded-lg text-gray-600'
  };
  
  return colorClasses[props.color];
});

const trendColor = computed(() => {
  if (!props.trend) return 'text-gray-600';
  
  // Determine trend color based on value and context
  if (props.trend.value >= 75) return 'text-green-600';
  if (props.trend.value >= 50) return 'text-blue-600';
  if (props.trend.value >= 25) return 'text-yellow-600';
  return 'text-red-600';
});

const trendUnit = computed(() => {
  if (!props.trend) return '';
  if (typeof props.trend.value === 'number' && props.trend.label.includes('%')) return '%';
  if (typeof props.trend.value === 'number' && props.trend.value > 100) return '';
  return typeof props.trend.value === 'number' && props.trend.value <= 100 ? '%' : '';
});

const formattedValue = computed(() => {
  if (props.value >= 1000000) {
    return (props.value / 1000000).toFixed(1) + 'M';
  }
  if (props.value >= 1000) {
    return (props.value / 1000).toFixed(1) + 'K';
  }
  return props.value.toString();
});
</script>

<style scoped>
.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.hover\:-translate-y-1:hover {
  transform: translateY(-0.25rem);
}

.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}
</style>
