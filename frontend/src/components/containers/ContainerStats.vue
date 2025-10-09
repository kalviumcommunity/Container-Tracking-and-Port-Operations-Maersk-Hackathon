<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-6 gap-4">
    <div class="bg-gradient-to-r from-blue-500 to-blue-600 rounded-lg p-4 text-white">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90">Total Containers</p>
          <p class="text-2xl font-bold">{{ stats.totalContainers || 0 }}</p>
        </div>
        <div class="w-8 h-8 bg-blue-400 rounded-full flex items-center justify-center">
          <BoxIcon class="w-4 h-4" />
        </div>
      </div>
    </div>
    
    <div class="bg-gradient-to-r from-green-500 to-green-600 rounded-lg p-4 text-white">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90">Available</p>
          <p class="text-2xl font-bold">{{ stats.availableContainers || 0 }}</p>
        </div>
        <div class="w-8 h-8 bg-green-400 rounded-full flex items-center justify-center">
          <CheckCircleIcon class="w-4 h-4" />
        </div>
      </div>
    </div>
    
    <div class="bg-gradient-to-r from-yellow-500 to-yellow-600 rounded-lg p-4 text-white">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90">In Transit</p>
          <p class="text-2xl font-bold">{{ stats.inTransitContainers || 0 }}</p>
        </div>
        <div class="w-8 h-8 bg-yellow-400 rounded-full flex items-center justify-center">
          <TruckIcon class="w-4 h-4" />
        </div>
      </div>
    </div>
    
    <div class="bg-gradient-to-r from-purple-500 to-purple-600 rounded-lg p-4 text-white">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90">At Port</p>
          <p class="text-2xl font-bold">{{ stats.atPortContainers || 0 }}</p>
        </div>
        <div class="w-8 h-8 bg-purple-400 rounded-full flex items-center justify-center">
          <AnchorIcon class="w-4 h-4" />
        </div>
      </div>
    </div>
    
    <div class="bg-gradient-to-r from-orange-500 to-orange-600 rounded-lg p-4 text-white">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90">Loading</p>
          <p class="text-2xl font-bold">{{ stats.loadingContainers || 0 }}</p>
        </div>
        <div class="w-8 h-8 bg-orange-400 rounded-full flex items-center justify-center">
          <ArrowUpIcon class="w-4 h-4" />
        </div>
      </div>
    </div>
    
    <div class="bg-gradient-to-r from-red-500 to-red-600 rounded-lg p-4 text-white">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90">Unloading</p>
          <p class="text-2xl font-bold">{{ stats.unloadingContainers || 0 }}</p>
        </div>
        <div class="w-8 h-8 bg-red-400 rounded-full flex items-center justify-center">
          <ArrowDownIcon class="w-4 h-4" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { BoxIcon, CheckCircleIcon, TruckIcon, AnchorIcon, ArrowUpIcon, ArrowDownIcon } from 'lucide-vue-next';
import type { ContainerStats } from '../../types/container';

interface Props {
  stats: ContainerStats;
}

defineProps<Props>();
</script>
