<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen && container" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Backdrop -->
        <div 
          class="fixed inset-0 bg-black/50 backdrop-blur-sm transition-opacity"
          @click="emit('close')"
        ></div>

        <!-- Modal Container -->
        <div class="flex min-h-screen items-center justify-center p-4">
          <div 
            class="relative bg-white rounded-2xl shadow-2xl w-full max-w-5xl transform transition-all max-h-[90vh] overflow-y-auto"
            @click.stop
          >
            <!-- Header -->
            <div class="sticky top-0 bg-gradient-to-r from-indigo-600 to-indigo-700 px-6 py-4 rounded-t-2xl z-10">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
                    <PackageIcon :size="24" class="text-white" />
                  </div>
                  <div>
                    <h2 class="text-2xl font-bold text-white">{{ container.containerId }}</h2>
                    <p class="text-indigo-100 text-sm">{{ container.cargoType }} - {{ container.type }} Container</p>
                  </div>
                </div>
                <button 
                  @click="emit('close')"
                  class="p-2 hover:bg-white/10 rounded-lg transition-colors"
                >
                  <X :size="24" class="text-white" />
                </button>
              </div>
            </div>

            <!-- Content -->
            <div class="p-6">
              <!-- Status Banner -->
              <div class="mb-6 p-4 rounded-xl border-2" :class="getStatusBannerClass(container.status)">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <component :is="getStatusIcon(container.status)" :size="24" />
                    <div>
                      <p class="font-semibold text-lg">Current Status: {{ container.status }}</p>
                      <p class="text-sm opacity-90">{{ getStatusDescription(container.status) }}</p>
                    </div>
                  </div>
                  <span 
                    class="px-4 py-2 rounded-full text-sm font-semibold"
                    :class="getStatusClass(container.status)"
                  >
                    {{ container.status }}
                  </span>
                </div>
              </div>

              <!-- Key Metrics Grid -->
              <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                <div class="bg-gradient-to-br from-blue-50 to-blue-100 rounded-xl p-4 border border-blue-200">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-blue-800">Type</span>
                    <BoxIcon :size="20" class="text-blue-600" />
                  </div>
                  <p class="text-xl font-bold text-blue-900">{{ container.type }}</p>
                  <p class="text-xs text-blue-700 mt-1">Container type</p>
                </div>

                <div class="bg-gradient-to-br from-green-50 to-green-100 rounded-xl p-4 border border-green-200" v-if="container.weight">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-green-800">Weight</span>
                    <ScaleIcon :size="20" class="text-green-600" />
                  </div>
                  <p class="text-xl font-bold text-green-900">{{ container.weight.toLocaleString() }} kg</p>
                  <p class="text-xs text-green-700 mt-1" v-if="container.maxWeight">Max: {{ container.maxWeight.toLocaleString() }} kg</p>
                </div>

                <div class="bg-gradient-to-br from-purple-50 to-purple-100 rounded-xl p-4 border border-purple-200" v-if="container.condition">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-purple-800">Condition</span>
                    <CheckCircleIcon :size="20" class="text-purple-600" />
                  </div>
                  <p class="text-xl font-bold text-purple-900">{{ container.condition }}</p>
                  <p class="text-xs text-purple-700 mt-1">Physical condition</p>
                </div>

                <div class="bg-gradient-to-br from-amber-50 to-amber-100 rounded-xl p-4 border border-amber-200" v-if="container.size">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-amber-800">Size</span>
                    <RulerIcon :size="20" class="text-amber-600" />
                  </div>
                  <p class="text-xl font-bold text-amber-900">{{ container.size }}</p>
                  <p class="text-xs text-amber-700 mt-1">Container size</p>
                </div>
              </div>

              <!-- Details Sections -->
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Location Information -->
                <div class="bg-white border-2 border-gray-200 rounded-xl p-5">
                  <div class="flex items-center gap-2 mb-4">
                    <MapPinIcon :size="20" class="text-indigo-600" />
                    <h3 class="text-lg font-semibold text-gray-900">Location Information</h3>
                  </div>
                  <div class="space-y-3">
                    <div>
                      <p class="text-sm text-gray-500">Current Location</p>
                      <p class="text-base font-medium text-gray-900">{{ container.currentLocation || 'Unknown' }}</p>
                    </div>
                    <div v-if="container.destination">
                      <p class="text-sm text-gray-500">Destination</p>
                      <p class="text-base font-medium text-gray-900">{{ container.destination }}</p>
                    </div>
                    <div v-if="container.coordinates">
                      <p class="text-sm text-gray-500">GPS Coordinates</p>
                      <p class="text-base font-medium text-gray-900 font-mono">{{ container.coordinates }}</p>
                    </div>
                  </div>
                </div>

                <!-- Cargo Information -->
                <div class="bg-white border-2 border-gray-200 rounded-xl p-5">
                  <div class="flex items-center gap-2 mb-4">
                    <PackageIcon :size="20" class="text-indigo-600" />
                    <h3 class="text-lg font-semibold text-gray-900">Cargo Information</h3>
                  </div>
                  <div class="space-y-3">
                    <div>
                      <p class="text-sm text-gray-500">Cargo Type</p>
                      <p class="text-base font-medium text-gray-900">{{ container.cargoType }}</p>
                    </div>
                    <div v-if="container.cargoDescription">
                      <p class="text-sm text-gray-500">Description</p>
                      <p class="text-base font-medium text-gray-900">{{ container.cargoDescription }}</p>
                    </div>
                    <div v-if="container.temperature !== null && container.temperature !== undefined">
                      <p class="text-sm text-gray-500">Temperature</p>
                      <p class="text-base font-medium text-gray-900 flex items-center gap-2">
                        <ThermometerIcon :size="16" class="text-blue-600" />
                        {{ container.temperature }}°C
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Ship Assignment -->
                <div v-if="container.shipName || container.shipId" class="bg-white border-2 border-gray-200 rounded-xl p-5">
                  <div class="flex items-center gap-2 mb-4">
                    <ShipIcon :size="20" class="text-indigo-600" />
                    <h3 class="text-lg font-semibold text-gray-900">Ship Assignment</h3>
                  </div>
                  <div class="space-y-3">
                    <div>
                      <p class="text-sm text-gray-500">Assigned Ship</p>
                      <p class="text-base font-medium text-gray-900">{{ container.shipName || `Ship #${container.shipId}` }}</p>
                    </div>
                  </div>
                </div>

                <!-- Port Assignment -->
                <div v-if="container.portName || container.portId" class="bg-white border-2 border-gray-200 rounded-xl p-5">
                  <div class="flex items-center gap-2 mb-4">
                    <AnchorIcon :size="20" class="text-indigo-600" />
                    <h3 class="text-lg font-semibold text-gray-900">Port Assignment</h3>
                  </div>
                  <div class="space-y-3">
                    <div>
                      <p class="text-sm text-gray-500">Current Port</p>
                      <p class="text-base font-medium text-gray-900">{{ container.portName || `Port #${container.portId}` }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Timestamps -->
              <div class="mt-6 p-4 bg-gray-50 rounded-xl border border-gray-200">
                <h3 class="text-sm font-semibold text-gray-700 mb-3 flex items-center gap-2">
                  <ClockIcon :size="16" />
                  Timeline
                </h3>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                  <div>
                    <p class="text-gray-500">Created</p>
                    <p class="text-gray-900 font-medium">{{ formatDateTime(container.createdAt) }}</p>
                  </div>
                  <div>
                    <p class="text-gray-500">Last Updated</p>
                    <p class="text-gray-900 font-medium">{{ formatDateTime(container.updatedAt) }}</p>
                  </div>
                  <div v-if="container.estimatedArrival">
                    <p class="text-gray-500">Estimated Arrival</p>
                    <p class="text-gray-900 font-medium">{{ formatDateTime(container.estimatedArrival) }}</p>
                  </div>
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="mt-6 flex justify-end gap-3">
                <button
                  @click="emit('close')"
                  class="px-6 py-2.5 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-lg font-medium transition-colors"
                >
                  Close
                </button>
                <button
                  v-if="canManage"
                  @click="emit('edit', container)"
                  class="px-6 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg font-medium transition-colors flex items-center gap-2"
                >
                  <PencilIcon :size="16" />
                  Edit Container
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import {
  X,
  PackageIcon,
  BoxIcon,
  ScaleIcon,
  CheckCircleIcon,
  RulerIcon,
  MapPinIcon,
  ShipIcon,
  AnchorIcon,
  ThermometerIcon,
  ClockIcon,
  PencilIcon,
  CheckCircle,
  TruckIcon,
  WarehouseIcon,
  AlertTriangleIcon,
  Loader2Icon
} from 'lucide-vue-next';

interface Container {
  containerId: string;
  cargoType: string;
  cargoDescription?: string;
  type: string;
  status: string;
  condition?: string;
  currentLocation: string;
  destination?: string;
  weight?: number;
  maxWeight?: number;
  size?: string;
  temperature?: number;
  coordinates?: string;
  estimatedArrival?: string;
  shipId?: number;
  shipName?: string;
  portId?: number;
  portName?: string;
  createdAt: string;
  updatedAt: string;
}

interface Props {
  isOpen: boolean;
  container: Container | null;
  canManage?: boolean;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  close: [];
  edit: [container: Container];
}>();

const getStatusClass = (status: string): string => {
  const statusClasses: Record<string, string> = {
    'Available': 'bg-green-100 text-green-800',
    'In Transit': 'bg-blue-100 text-blue-800',
    'At Port': 'bg-yellow-100 text-yellow-800',
    'Loading': 'bg-orange-100 text-orange-800',
    'Unloading': 'bg-purple-100 text-purple-800',
    'Maintenance': 'bg-red-100 text-red-800',
  };
  return statusClasses[status] || 'bg-gray-100 text-gray-800';
};

const getStatusBannerClass = (status: string): string => {
  const bannerClasses: Record<string, string> = {
    'Available': 'bg-green-50 border-green-300 text-green-900',
    'In Transit': 'bg-blue-50 border-blue-300 text-blue-900',
    'At Port': 'bg-yellow-50 border-yellow-300 text-yellow-900',
    'Loading': 'bg-orange-50 border-orange-300 text-orange-900',
    'Unloading': 'bg-purple-50 border-purple-300 text-purple-900',
    'Maintenance': 'bg-red-50 border-red-300 text-red-900',
  };
  return bannerClasses[status] || 'bg-gray-50 border-gray-300 text-gray-900';
};

const getStatusIcon = (status: string) => {
  const statusIcons: Record<string, any> = {
    'Available': CheckCircle,
    'In Transit': TruckIcon,
    'At Port': WarehouseIcon,
    'Loading': Loader2Icon,
    'Unloading': Loader2Icon,
    'Maintenance': AlertTriangleIcon,
  };
  return statusIcons[status] || CheckCircle;
};

const getStatusDescription = (status: string): string => {
  const descriptions: Record<string, string> = {
    'Available': 'Container is available for assignment',
    'In Transit': 'Container is currently in transit',
    'At Port': 'Container is at the port facility',
    'Loading': 'Container is being loaded onto a ship',
    'Unloading': 'Container is being unloaded from a ship',
    'Maintenance': 'Container is under maintenance',
  };
  return descriptions[status] || 'Status information not available';
};

const formatDateTime = (dateString: string): string => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active > div > div,
.modal-leave-active > div > div {
  transition: transform 0.3s ease;
}

.modal-enter-from > div > div,
.modal-leave-to > div > div {
  transform: scale(0.9);
}
</style>
