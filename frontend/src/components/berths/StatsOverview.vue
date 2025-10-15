<!-- Stats Overview Component -->
<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
    <!-- Total Berths Card -->
    <StatsCard
      title="Total Berths"
      :value="stats.totalBerths"
      icon="MapPin"
      color="blue"
      :trend="{ value: 100, label: 'capacity' }"
      :loading="loading"
    />

    <!-- Available Berths Card -->
    <StatsCard
      title="Available Berths"
      :value="stats.availableBerths"
      icon="CheckCircle"
      color="green"
      :trend="{ value: stats.availabilityPercentage, label: 'available' }"
      :loading="loading"
    />

    <!-- Occupied Berths Card -->
    <StatsCard
      title="Occupied Berths"
      :value="stats.occupiedBerths"
      icon="Clock"
      color="orange"
      :trend="{ value: stats.occupancyPercentage, label: 'occupied' }"
      :loading="loading"
    />

    <!-- Ships in Port Card -->
    <StatsCard
      title="Ships in Port"
      :value="stats.totalShips"
      icon="Ship"
      color="purple"
      :trend="{ value: stats.dockedShips, label: 'docked' }"
      :loading="loading"
    />
  </div>
</template>

<script setup lang="ts">
import StatsCard from './StatsCard.vue';

interface Stats {
  totalBerths: number;
  availableBerths: number;
  occupiedBerths: number;
  availabilityPercentage: number;
  occupancyPercentage: number;
  totalShips: number;
  dockedShips: number;
}

interface Props {
  stats: Stats;
  loading: boolean;
}

withDefaults(defineProps<Props>(), {
  loading: false
});
</script>