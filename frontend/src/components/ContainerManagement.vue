<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Page Header -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
          <div class="flex items-center gap-4">
            <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
              <Container :size="28" class="text-white" />
            </div>
            <div>
              <h1 class="text-3xl font-bold text-slate-900">Container Management</h1>
              <p class="text-slate-600 mt-1">Track and manage container lifecycle operations</p>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <div class="flex items-center gap-2 bg-slate-100 px-4 py-2 rounded-lg">
              <Package :size="16" class="text-blue-600" />
              <span class="font-medium text-slate-700">{{ containers.length }} Active</span>
            </div>
            <button class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-6 rounded-lg transition-colors duration-200 flex items-center gap-2">
              <Plus :size="16" />
              Add Container
            </button>
          </div>
        </div>
      </div>
      <!-- Search and Filter Bar -->
      <section class="mb-8">
        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6">
          <div class="flex flex-col md:flex-row gap-4">
            <div class="flex-1 relative">
              <Search :size="20" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400" />
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search containers by ID, type, or status..."
                class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              />
            </div>
            <div class="flex gap-3">
              <button class="px-4 py-3 border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors flex items-center gap-2">
                <Filter :size="16" class="text-slate-600" />
                <span class="text-slate-700">Filter</span>
              </button>
              <button class="px-4 py-3 border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors flex items-center gap-2">
                <Download :size="16" class="text-slate-600" />
                <span class="text-slate-700">Export</span>
              </button>
            </div>
          </div>
        </div>
      </section>

      <!-- Container Grid -->
      <section>
        <div class="mb-6">
          <h2 class="text-2xl font-bold text-slate-900 mb-2">Active Containers</h2>
          <p class="text-slate-600">Monitor real-time status and location of all containers</p>
        </div>
        
        <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          <div
            v-for="(container, index) in containers"
            :key="container.id"
            class="bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-lg transition-all duration-300 hover:-translate-y-1 group animate-slideIn"
            :style="{ animationDelay: `${index * 100}ms` }"
          >
            <!-- Container Header -->
            <div class="p-6 border-b border-slate-200">
              <div class="flex items-start justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 rounded-lg bg-blue-50 border-2 border-blue-200 flex items-center justify-center text-xl group-hover:border-blue-300 transition-colors">
                    {{ getTypeIcon(container.type) }}
                  </div>
                  <div>
                    <h3 class="text-lg font-bold text-slate-900">{{ container.id }}</h3>
                    <div class="flex items-center gap-2 mt-1">
                      <span class="px-2 py-1 text-xs font-medium bg-slate-100 text-slate-700 rounded-full">
                        {{ container.type }}
                      </span>
                      <span 
                        class="px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getStatusColor(container.status)"
                      >
                        {{ container.status }}
                      </span>
                    </div>
                  </div>
                </div>
                <div class="p-2 bg-slate-50 rounded-lg group-hover:bg-blue-50 transition-colors">
                  <Package :size="20" class="text-slate-600 group-hover:text-blue-600" />
                </div>
              </div>
            </div>
            
            <!-- Container Details -->
            <div class="p-6 space-y-4">
              <!-- Location & Destination -->
              <div class="grid grid-cols-1 gap-3">
                <div class="flex items-center gap-3">
                  <div class="p-2 bg-blue-50 rounded-lg">
                    <MapPin :size="16" class="text-blue-600" />
                  </div>
                  <div class="flex-1">
                    <p class="text-sm font-medium text-slate-700">Current Location</p>
                    <p class="text-sm text-slate-900 font-semibold">{{ container.location }}</p>
                  </div>
                </div>
                
                <div class="flex items-center gap-3">
                  <div class="p-2 bg-green-50 rounded-lg">
                    <Navigation :size="16" class="text-green-600" />
                  </div>
                  <div class="flex-1">
                    <p class="text-sm font-medium text-slate-700">Destination</p>
                    <p class="text-sm text-slate-900 font-semibold">{{ container.destination }}</p>
                  </div>
                </div>
              </div>

              <!-- Cargo Information -->
              <div class="p-4 bg-slate-50 rounded-lg">
                <div class="flex items-center gap-2 mb-2">
                  <Package2 :size="16" class="text-slate-600" />
                  <span class="text-sm font-medium text-slate-700">Cargo Details</span>
                </div>
                <p class="text-sm text-slate-900 font-semibold">{{ container.cargo }}</p>
              </div>

              <!-- Temperature Monitor -->
              <div class="p-4 bg-gradient-to-r from-blue-50 to-cyan-50 rounded-lg border border-blue-200">
                <div class="flex justify-between items-center">
                  <div class="flex items-center gap-2">
                    <Thermometer :size="16" class="text-blue-600" />
                    <span class="text-sm font-medium text-blue-800">Temperature</span>
                  </div>
                  <span class="text-lg font-bold text-blue-900">{{ container.temperature }}</span>
                </div>
              </div>

              <!-- Last Update -->
              <div class="flex items-center gap-2 text-xs text-slate-500">
                <Clock :size="14" />
                <span>Last updated: {{ container.lastUpdate }}</span>
              </div>

              <!-- Action Buttons -->
              <div class="flex gap-2 pt-2">
                <button class="flex-1 px-3 py-2 text-sm font-medium border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors">
                  View Details
                </button>
                <button class="flex-1 px-3 py-2 text-sm font-medium bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                  Update Status
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { 
  Container, 
  Package, 
  Package2, 
  Plus, 
  Search, 
  Filter, 
  Download, 
  MapPin, 
  Navigation, 
  Thermometer, 
  Clock 
} from 'lucide-vue-next';

interface Container {
  id: string;
  type: string;
  status: string;
  location: string;
  lastUpdate: string;
  temperature: string;
  destination: string;
  cargo: string;
}

const searchQuery = ref('');

const containers = ref<Container[]>([
  {
    id: "CTR-2024-001",
    type: "Refrigerated",
    status: "At Port",
    location: "Berth B-07",
    lastUpdate: "2 hours ago",
    temperature: "-18°C",
    destination: "Chennai Port",
    cargo: "Frozen Foods"
  },
  {
    id: "CTR-2024-002", 
    type: "Dry",
    status: "In Transit",
    location: "Route A-12",
    lastUpdate: "5 hours ago",
    temperature: "Ambient",
    destination: "Mumbai Port", 
    cargo: "Electronics"
  },
  {
    id: "CTR-2024-003",
    type: "Liquid",
    status: "Loading",
    location: "Berth B-12",
    lastUpdate: "30 min ago",
    temperature: "15°C",
    destination: "Kochi Port",
    cargo: "Chemical Products"
  },
  {
    id: "CTR-2024-004",
    type: "Dry",
    status: "Inspected",
    location: "Berth B-03",
    lastUpdate: "1 hour ago", 
    temperature: "Ambient",
    destination: "Kolkata Port",
    cargo: "Textiles"
  },
  {
    id: "CTR-2024-005",
    type: "Refrigerated",
    status: "Departed",
    location: "En Route",
    lastUpdate: "3 hours ago",
    temperature: "-5°C", 
    destination: "Vizag Port",
    cargo: "Pharmaceuticals"
  }
]);

const getStatusColor = (status: string): string => {
  const statusColors = {
    "At Port": "bg-blue-100 text-blue-800 border-blue-200",
    "In Transit": "bg-orange-100 text-orange-800 border-orange-200",
    "Loading": "bg-purple-100 text-purple-800 border-purple-200",
    "Inspected": "bg-green-100 text-green-800 border-green-200",
    "Departed": "bg-slate-100 text-slate-800 border-slate-200",
  };
  return statusColors[status as keyof typeof statusColors] || "bg-slate-100 text-slate-800 border-slate-200";
};

const getTypeIcon = (type: string): string => {
  const typeIcons = {
    "Refrigerated": "❄️",
    "Liquid": "🌊",
    "Dry": "📦",
  };
  return typeIcons[type as keyof typeof typeIcons] || "📦";
};
</script>

<style scoped>
/* Modern animations */
@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* Animation classes */
.animate-slideIn {
  animation: slideIn 0.6s ease-out forwards;
  opacity: 0;
}

.animate-fadeIn {
  animation: fadeIn 0.8s ease-out;
}

/* Custom hover effects */
.group:hover .group-hover\:scale-105 {
  transform: scale(1.05);
}

/* Responsive utilities */
@media (max-width: 768px) {
  .grid-cols-1.lg\:grid-cols-2.xl\:grid-cols-3 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    gap: 1rem;
  }
  
  .flex-col.md\:flex-row {
    flex-direction: column;
  }
  
  .gap-6 {
    gap: 1rem;
  }
  
  .px-6 {
    padding-left: 1rem;
    padding-right: 1rem;
  }
  
  .py-8 {
    padding-top: 1.5rem;
    padding-bottom: 1.5rem;
  }
  
  .text-3xl {
    font-size: 1.875rem;
    line-height: 2.25rem;
  }
  
  .text-2xl {
    font-size: 1.5rem;
    line-height: 2rem;
  }
}

@media (max-width: 640px) {
  .text-3xl {
    font-size: 1.5rem;
    line-height: 2rem;
  }
  
  .text-2xl {
    font-size: 1.25rem;
    line-height: 1.75rem;
  }
  
  .text-lg {
    font-size: 1.125rem;
    line-height: 1.75rem;
  }
  
  .p-6 {
    padding: 1rem;
  }
  
  .p-4 {
    padding: 0.75rem;
  }
  
  .gap-6 {
    gap: 1rem;
  }
  
  .gap-4 {
    gap: 0.75rem;
  }
  
  .mb-8 {
    margin-bottom: 1.5rem;
  }
  
  .mb-6 {
    margin-bottom: 1rem;
  }
}

/* Enhanced shadows for depth */
.shadow-sm {
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
}

.shadow-lg {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

/* Smooth transitions for all interactive elements */
* {
  transition-property: color, background-color, border-color, text-decoration-color, fill, stroke, opacity, box-shadow, transform, filter, backdrop-filter;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}

/* Focus states for accessibility */
button:focus-visible,
input:focus-visible {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* Custom scrollbar for better UX */
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: #f1f5f9;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}
</style>