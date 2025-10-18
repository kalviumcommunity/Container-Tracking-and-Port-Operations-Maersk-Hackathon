<!-- Simple Port Settings Component -->
<template>
  <div class="space-y-6">
    <div class="bg-white rounded-lg border border-gray-200 p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Port Settings</h3>
      <p class="text-gray-600">Configure port settings and preferences.</p>
      
      <div class="mt-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Port Configuration -->
          <div>
            <h4 class="text-md font-medium text-gray-900 mb-3">Port Configuration</h4>
            <div class="space-y-3">
              <div v-for="port in ports" :key="port.portId" class="p-3 border border-gray-200 rounded-lg">
                <div class="flex items-center justify-between">
                  <div>
                    <div class="font-medium text-gray-900">{{ port.name }}</div>
                    <div class="text-sm text-gray-600">{{ port.location }}</div>
                  </div>
                  <div class="text-sm text-gray-500">
                    {{ port.totalContainerCapacity }} capacity
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- User Permissions -->
          <div>
            <h4 class="text-md font-medium text-gray-900 mb-3">User Permissions</h4>
            <div class="space-y-2">
              <div v-for="(permission, key) in userPermissions" :key="key" class="flex items-center justify-between">
                <span class="text-sm text-gray-700">{{ formatPermissionName(key) }}</span>
                <span :class="permission ? 'text-green-600' : 'text-red-600'" class="text-sm font-medium">
                  {{ permission ? 'Enabled' : 'Disabled' }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Port {
  portId: number;
  name: string;
  location: string;
  totalContainerCapacity: number;
}

interface UserPermissions {
  canManageBerths: boolean;
  canCreateOperations: boolean;
  canViewAnalytics: boolean;
  canManageSettings: boolean;
}

interface Props {
  ports: Port[];
  userPermissions: UserPermissions;
}

withDefaults(defineProps<Props>(), {
  ports: () => [],
  userPermissions: () => ({
    canManageBerths: false,
    canCreateOperations: false,
    canViewAnalytics: false,
    canManageSettings: false
  })
});

defineEmits<{
  'update-settings': [settings: any];
}>();

const formatPermissionName = (key: string): string => {
  return key.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
};
</script>
