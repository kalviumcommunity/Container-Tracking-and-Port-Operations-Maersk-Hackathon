<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity z-50" @click="$emit('cancel')">
    <div class="fixed inset-0 z-50 overflow-y-auto">
      <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
        <div @click.stop class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-2xl">
          <!-- Header -->
          <div class="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4">
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <Ship class="w-6 h-6 text-white mr-3" />
                <h3 class="text-lg font-semibold text-white">Assign Berth</h3>
              </div>
              <button @click="$emit('cancel')" class="text-white hover:text-gray-200">
                <X class="w-5 h-5" />
              </button>
            </div>
          </div>

          <!-- Berth Info -->
          <div class="bg-blue-50 px-6 py-4 border-b border-blue-100">
            <div class="flex items-center justify-between">
              <div>
                <h4 class="text-sm font-medium text-gray-700">Berth</h4>
                <p class="text-lg font-semibold text-gray-900">{{ berth?.name }}</p>
              </div>
              <div class="text-right">
                <h4 class="text-sm font-medium text-gray-700">Available Capacity</h4>
                <p class="text-lg font-semibold" :class="availableCapacity > 0 ? 'text-green-600' : 'text-red-600'">
                  {{ availableCapacity }} / {{ berth?.capacity || 0 }} units
                </p>
              </div>
            </div>
          </div>

          <!-- Form -->
          <form @submit.prevent="handleSubmit" class="px-6 py-4">
            <div class="space-y-4">
              <!-- Assignment Type -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Assignment Type <span class="text-red-500">*</span>
                </label>
                <div class="grid grid-cols-2 gap-3">
                  <button
                    type="button"
                    @click="assignmentForm.assignmentType = 'Ship'"
                    class="flex items-center justify-center px-4 py-3 border-2 rounded-lg transition-all"
                    :class="assignmentForm.assignmentType === 'Ship' 
                      ? 'border-blue-500 bg-blue-50 text-blue-700' 
                      : 'border-gray-300 bg-white text-gray-700 hover:border-gray-400'"
                  >
                    <Ship class="w-5 h-5 mr-2" />
                    Ship
                  </button>
                  <button
                    type="button"
                    @click="assignmentForm.assignmentType = 'Container'"
                    class="flex items-center justify-center px-4 py-3 border-2 rounded-lg transition-all"
                    :class="assignmentForm.assignmentType === 'Container' 
                      ? 'border-blue-500 bg-blue-50 text-blue-700' 
                      : 'border-gray-300 bg-white text-gray-700 hover:border-gray-400'"
                  >
                    <Package class="w-5 h-5 mr-2" />
                    Container
                  </button>
                </div>
              </div>

              <!-- Ship/Container Selection -->
              <div v-if="assignmentForm.assignmentType === 'Ship'">
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Select Ship <span class="text-red-500">*</span>
                </label>
                <select
                  v-model="assignmentForm.shipId"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="">-- Select Ship --</option>
                  <option value="1">MV Ocean Pioneer (IMO: 9876543)</option>
                  <option value="2">SS Pacific Star (IMO: 9876544)</option>
                  <option value="3">MS Atlantic Wave (IMO: 9876545)</option>
                </select>
                <p class="mt-1 text-xs text-gray-500">Select the ship to assign to this berth</p>
              </div>

              <div v-else-if="assignmentForm.assignmentType === 'Container'">
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Container ID <span class="text-red-500">*</span>
                </label>
                <input
                  v-model="assignmentForm.containerNumber"
                  type="text"
                  required
                  placeholder="e.g., CONT-2024-001"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
                <p class="mt-1 text-xs text-gray-500">Enter the container number or ID</p>
              </div>

              <!-- Date Range -->
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Start Date <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="assignmentForm.startDate"
                    type="datetime-local"
                    required
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    End Date <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="assignmentForm.endDate"
                    type="datetime-local"
                    required
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
              </div>

              <!-- Capacity Required -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Capacity Required <span class="text-red-500">*</span>
                </label>
                <input
                  v-model.number="assignmentForm.capacityRequired"
                  type="number"
                  min="1"
                  :max="availableCapacity"
                  required
                  placeholder="e.g., 50"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
                <p class="mt-1 text-xs text-gray-500">
                  Maximum available: {{ availableCapacity }} units
                </p>
              </div>

              <!-- Priority -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Priority
                </label>
                <select
                  v-model.number="assignmentForm.priority"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option :value="1">High Priority</option>
                  <option :value="2">Medium Priority</option>
                  <option :value="3">Low Priority</option>
                </select>
              </div>

              <!-- Notes -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Notes (Optional)
                </label>
                <textarea
                  v-model="assignmentForm.notes"
                  rows="3"
                  placeholder="Add any special instructions or notes..."
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                ></textarea>
              </div>
            </div>

            <!-- Actions -->
            <div class="mt-6 flex justify-end space-x-3">
              <button
                type="button"
                @click="$emit('cancel')"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="!isFormValid || isSubmitting"
                class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Loader2 v-if="isSubmitting" class="animate-spin -ml-1 mr-2 h-4 w-4" />
                {{ isSubmitting ? 'Assigning...' : 'Assign Berth' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Ship, Package, X, Loader2 } from 'lucide-vue-next';

interface Berth {
  berthId: number;
  name: string;
  capacity: number;
  currentLoad?: number;
}

interface Props {
  berth: Berth | null;
  isSubmitting?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isSubmitting: false
});

const emit = defineEmits<{
  submit: [data: any];
  cancel: [];
}>();

const assignmentForm = ref({
  assignmentType: 'Ship',
  shipId: '',
  containerNumber: '',
  startDate: '',
  endDate: '',
  capacityRequired: null as number | null,
  priority: 2,
  notes: ''
});

const availableCapacity = computed(() => {
  if (!props.berth) return 0;
  return props.berth.capacity - (props.berth.currentLoad || 0);
});

const isFormValid = computed(() => {
  if (!assignmentForm.value.assignmentType) return false;
  if (!assignmentForm.value.startDate || !assignmentForm.value.endDate) return false;
  if (!assignmentForm.value.capacityRequired || assignmentForm.value.capacityRequired <= 0) return false;
  if (assignmentForm.value.capacityRequired > availableCapacity.value) return false;
  
  if (assignmentForm.value.assignmentType === 'Ship') {
    return !!assignmentForm.value.shipId;
  } else {
    return !!assignmentForm.value.containerNumber;
  }
});

const handleSubmit = () => {
  if (!isFormValid.value) return;
  
  const data = {
    berthId: props.berth?.berthId,
    assignmentType: assignmentForm.value.assignmentType,
    ...(assignmentForm.value.assignmentType === 'Ship' 
      ? { shipId: parseInt(assignmentForm.value.shipId) }
      : { containerNumber: assignmentForm.value.containerNumber }
    ),
    startDate: assignmentForm.value.startDate,
    endDate: assignmentForm.value.endDate,
    capacityRequired: assignmentForm.value.capacityRequired,
    priority: assignmentForm.value.priority,
    notes: assignmentForm.value.notes || null
  };
  
  emit('submit', data);
};
</script>
