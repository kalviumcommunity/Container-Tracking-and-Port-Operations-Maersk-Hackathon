<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Enhanced Page Header with Live Metrics -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
          <div class="flex items-center gap-4">
            <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
              <Activity :size="28" class="text-white" />
            </div>
            <div>
              <h1 class="text-3xl font-bold text-slate-900">Advanced Event Streaming Dashboard</h1>
              <p class="text-slate-600 mt-1">Real-time port operations monitoring • {{ currentTime }}</p>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <div class="flex items-center gap-2 bg-green-50 px-4 py-2 rounded-lg border border-green-200">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-green-700 font-medium">Stream Active</span>
            </div>
            <div class="flex items-center gap-2 bg-blue-50 px-4 py-2 rounded-lg border border-blue-200">
              <Zap :size="16" class="text-blue-600" />
              <span class="font-medium text-blue-700">{{ streamStats.eventsPerSecond }}/sec</span>
            </div>
            <div class="flex items-center gap-2 bg-purple-50 px-4 py-2 rounded-lg border border-purple-200">
              <Clock :size="16" class="text-purple-600" />
              <span class="font-medium text-purple-700">{{ streamStats.avgLatency }}ms</span>
            </div>
          </div>
        </div>
      </div>
      <!-- Enhanced Event Analytics Grid -->
      <section class="mb-8">
        <div class="mb-6">
          <h2 class="text-2xl font-bold text-slate-900 mb-2">Live Event Analytics</h2>
          <p class="text-slate-600">Real-time streaming analytics and operational insights</p>
        </div>
        
        <!-- Primary Stats Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
          <div
            v-for="(stat, index) in eventStats"
            :key="index"
            class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 animate-slideIn"
            :style="{ animationDelay: `${index * 100}ms` }"
          >
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 rounded-lg" :class="stat.bgColor">
                <component :is="stat.icon" :size="24" :class="stat.iconColor" />
              </div>
              <div class="text-right">
                <span class="text-xs font-medium px-2 py-1 rounded-full" :class="getSeverityColor(stat.severity)">
                  {{ stat.severity }}
                </span>
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stat.value }}</p>
              <p class="text-sm font-medium text-slate-600">{{ stat.label }}</p>
              <div class="flex items-center gap-1 mt-2">
                <TrendingUp :size="14" :class="stat.trend === 'up' ? 'text-green-600' : 'text-red-600'" />
                <span class="text-sm font-medium" :class="stat.trend === 'up' ? 'text-green-600' : 'text-red-600'">
                  {{ stat.trend === 'up' ? '+' : '-' }}{{ stat.change }}%
                </span>
                <span class="text-sm text-slate-500">{{ stat.period }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Event Category Breakdown -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
          <!-- Event Types Distribution -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-slate-900">Event Categories</h3>
              <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">View Details</button>
            </div>
            <div class="space-y-3">
              <div v-for="category in eventCategories" :key="category.type" class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <component :is="category.icon" :size="20" :class="category.color" />
                  <span class="font-medium text-slate-700">{{ category.type }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <div class="w-16 bg-slate-200 rounded-full h-2">
                    <div 
                      class="h-2 rounded-full transition-all duration-500" 
                      :class="category.barColor"
                      :style="{ width: `${category.percentage}%` }"
                    ></div>
                  </div>
                  <span class="text-sm font-medium text-slate-600 w-12">{{ category.count }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Severity Level Analysis -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-slate-900">Severity Distribution</h3>
              <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">Configure Alerts</button>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div v-for="severity in severityStats" :key="severity.level" 
                   class="p-4 rounded-lg border-2 transition-all hover:shadow-md" :class="severity.borderColor">
                <div class="flex items-center gap-2 mb-2">
                  <div class="w-3 h-3 rounded-full" :class="severity.dotColor"></div>
                  <span class="font-medium text-slate-700">{{ severity.level }}</span>
                </div>
                <div class="text-2xl font-bold" :class="severity.textColor">{{ severity.count }}</div>
                <div class="text-xs text-slate-500">{{ severity.description }}</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Main Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Enhanced Live Event Feed -->
        <div 
          ref="eventContainerRef"
          class="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm"
          :style="isScrollableMode ? `height: ${initialHeight}; display: flex; flex-direction: column;` : ''"
        >
          <div class="border-b border-slate-200 p-6">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="p-2 bg-blue-50 rounded-lg">
                  <Radio :size="20" class="text-blue-600" />
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-slate-900">Live Event Stream</h3>
                  <p class="text-sm text-slate-600">Real-time events • {{ filteredEvents.length }} events loaded</p>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <button 
                  @click="toggleEventView"
                  class="px-3 py-2 text-sm font-medium text-slate-600 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors flex items-center gap-2"
                >
                  <component :is="viewMode === 'list' ? Grid : List" :size="16" />
                  {{ viewMode === 'list' ? 'Grid' : 'List' }}
                </button>
                <button 
                  @click="openEventModal(null)"
                  class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
                >
                  <Plus :size="16" />
                  Create Event
                </button>
                <button class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors flex items-center gap-2">
                  <Eye :size="16" />
                  View All
                </button>
              </div>
            </div>
            
            <!-- Quick Filters -->
            <div class="flex items-center gap-3 mt-4 flex-wrap">
              <div class="flex items-center gap-2">
                <span class="text-sm font-medium text-slate-700">Quick Filter:</span>
                <button 
                  v-for="filter in quickFilters" 
                  :key="filter.id"
                  @click="toggleQuickFilter(filter.id)"
                  class="px-3 py-1 text-xs font-medium rounded-full transition-colors"
                  :class="filter.active 
                    ? 'bg-blue-100 text-blue-700 border border-blue-200' 
                    : 'bg-slate-100 text-slate-600 hover:bg-slate-200'"
                >
                  {{ filter.label }}
                </button>
              </div>
              <div class="ml-auto flex items-center gap-2">
                <span class="text-xs text-slate-500">Auto-refresh:</span>
                <button 
                  @click="toggleAutoRefresh"
                  class="flex items-center gap-1 px-2 py-1 text-xs rounded-full"
                  :class="autoRefresh 
                    ? 'bg-green-100 text-green-700' 
                    : 'bg-slate-100 text-slate-600'"
                >
                  <div class="w-2 h-2 rounded-full" 
                       :class="autoRefresh ? 'bg-green-500 animate-pulse' : 'bg-slate-400'"></div>
                  {{ autoRefresh ? 'On' : 'Off' }}
                </button>
              </div>
            </div>
          </div>
          
          <div 
            class="p-6"
            :style="isScrollableMode ? 'flex: 1 1 auto; overflow-y: auto;' : ''"
          >
            <!-- Event List View -->
            <div v-if="viewMode === 'list'" class="space-y-4">
              <div
                v-for="(event, index) in filteredEvents"
                :key="event.id"
                class="p-4 rounded-lg transition-all duration-300 hover:shadow-md group animate-slideIn cursor-pointer"
                :class="getEventCardClass(event)"
                :style="{ animationDelay: `${(index + 4) * 100}ms` }"
                @click="openEventModal(event)"
              >
                <div class="flex items-start justify-between mb-3">
                  <div class="flex items-center gap-3">
                    <div class="relative">
                      <div class="w-12 h-12 rounded-lg flex items-center justify-center" :class="getEventIconBg(event.eventType)">
                        <component :is="getEventIcon(event.eventType)" :size="20" :class="getEventIconColor(event.eventType)" />
                      </div>
                      <div v-if="event.severity === 'Critical'" class="absolute -top-1 -right-1 w-4 h-4 bg-red-500 rounded-full animate-pulse"></div>
                    </div>
                    <div>
                      <div class="font-semibold text-slate-900">{{ event.title || event.eventType }}</div>
                      <div class="text-sm text-slate-600">
                        <span v-if="event.containerId">Container: {{ event.containerId }}</span>
                        <span v-else-if="event.shipName">Ship: {{ event.shipName }}</span>
                        <span v-else-if="event.berthName">Berth: {{ event.berthName }}</span>
                        <span v-else>{{ event.source }}</span>
                      </div>
                      <div class="text-xs text-slate-500 mt-1">{{ formatEventTime(event.eventTime) }}</div>
                    </div>
                  </div>
                  <div class="text-right">
                    <div class="flex items-center gap-2 mb-2">
                      <span 
                        class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getSeverityColor(event.severity)"
                      >
                        {{ event.severity }}
                      </span>
                      <button 
                        v-if="!event.isRead"
                        @click.stop="markAsRead(event.id)"
                        class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 text-blue-700 hover:bg-blue-200 transition-colors"
                      >
                        <Eye :size="10" class="mr-1" />
                        Mark Read
                      </button>
                    </div>
                    <div class="flex items-center gap-1 text-xs text-slate-500">
                      <Clock :size="12" />
                      <span>{{ getTimeAgo(event.eventTime) }}</span>
                    </div>
                  </div>
                </div>
                
                <p class="text-sm text-slate-700 mb-3 leading-relaxed">{{ event.description }}</p>
                
                <!-- Event Metadata -->
                <div class="flex items-center justify-between text-xs">
                  <div class="flex items-center gap-4 text-slate-500">
                    <span v-if="event.portName">Port: {{ event.portName }}</span>
                    <span v-if="event.userName">Reporter: {{ event.userName }}</span>
                    <span>ID: {{ event.id }}</span>
                  </div>
                  <div class="flex gap-2">
                    <button
                      @click.stop="acknowledgeEvent(event.id)"
                      v-if="!event.isRead"
                      class="px-3 py-1 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                      Acknowledge
                    </button>
                    <button 
                      @click.stop="openEventModal(event)"
                      class="px-3 py-1 text-sm border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors"
                    >
                      Details
                    </button>
                  </div>
                </div>
              </div>
              
              <!-- Load More Button -->
              <div v-if="hasMoreEvents" class="text-center pt-4">
                <button 
                  @click="loadMoreEvents"
                  class="px-6 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
                >
                  Load More Events
                </button>
              </div>
            </div>

            <!-- Event Grid View -->
            <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div
                v-for="event in filteredEvents"
                :key="event.id"
                class="p-4 rounded-lg border transition-all duration-300 hover:shadow-md cursor-pointer"
                :class="getEventCardClass(event)"
                @click="openEventModal(event)"
              >
                <div class="flex items-start gap-3 mb-3">
                  <div class="w-10 h-10 rounded-lg flex items-center justify-center" :class="getEventIconBg(event.eventType)">
                    <component :is="getEventIcon(event.eventType)" :size="18" :class="getEventIconColor(event.eventType)" />
                  </div>
                  <div class="flex-1">
                    <h4 class="font-semibold text-slate-900 text-sm">{{ event.title || event.eventType }}</h4>
                    <p class="text-xs text-slate-600 mt-1 line-clamp-2">{{ event.description }}</p>
                  </div>
                  <span class="text-xs px-2 py-1 rounded-full font-medium" :class="getSeverityColor(event.severity)">
                    {{ event.severity }}
                  </span>
                </div>
                <div class="flex items-center justify-between text-xs text-slate-500">
                  <span>{{ getTimeAgo(event.eventTime) }}</span>
                  <span>{{ event.source }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Enhanced Stream Controls & Analytics -->
        <div class="space-y-6">
          <!-- Stream Connection Status -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <h3 class="text-lg font-semibold text-slate-900">Stream Health</h3>
            </div>
            <div class="p-6 space-y-4">
              <div class="p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg border border-green-200">
                <div class="flex items-center justify-between mb-2">
                  <div class="flex items-center gap-2">
                    <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                    <span class="text-sm font-semibold text-green-800">Event Stream</span>
                  </div>
                  <span class="text-xs text-green-600 font-medium">{{ streamStats.uptime }}</span>
                </div>
                <p class="text-lg font-bold text-green-900">Online</p>
                <p class="text-sm text-green-700 mt-1">Last event: {{ streamStats.lastEventTime }}</p>
              </div>

              <div class="grid grid-cols-2 gap-3 text-center">
                <div class="p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
                  <p class="text-lg font-bold text-slate-900">{{ streamStats.eventsPerMinute }}</p>
                  <p class="text-xs text-slate-600">Events/min</p>
                </div>
                <div class="p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
                  <p class="text-lg font-bold text-slate-900">{{ streamStats.avgLatency }}ms</p>
                  <p class="text-xs text-slate-600">Latency</p>
                </div>
                <div class="p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
                  <p class="text-lg font-bold text-slate-900">{{ streamStats.errorRate }}%</p>
                  <p class="text-xs text-slate-600">Error Rate</p>
                </div>
                <div class="p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
                  <p class="text-lg font-bold text-slate-900">{{ streamStats.throughput }}</p>
                  <p class="text-xs text-slate-600">MB/sec</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Advanced Event Analytics -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-semibold text-slate-900">Event Insights</h3>
                <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">Configure</button>
              </div>
            </div>
            <div class="p-6 space-y-4">
              <!-- Processing Time Chart -->
              <div class="p-4 bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg border border-blue-200">
                <h4 class="font-semibold text-blue-900 mb-2">Processing Performance</h4>
                <div class="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <span class="text-blue-600">Avg Response:</span>
                    <span class="font-bold ml-1">{{ analytics.avgResponseTime }}ms</span>
                  </div>
                  <div>
                    <span class="text-blue-600">SLA Met:</span>
                    <span class="font-bold ml-1 text-green-600">{{ analytics.slaCompliance }}%</span>
                  </div>
                </div>
              </div>

              <!-- Alert Summary -->
              <div class="space-y-2">
                <h4 class="font-semibold text-slate-700">Alert Summary</h4>
                <div v-for="alert in alertSummary" :key="alert.type" 
                     class="flex items-center justify-between p-2 rounded-lg" :class="alert.bgClass">
                  <div class="flex items-center gap-2">
                    <div class="w-2 h-2 rounded-full" :class="alert.dotClass"></div>
                    <span class="text-sm font-medium" :class="alert.textClass">{{ alert.type }}</span>
                  </div>
                  <span class="text-sm font-bold" :class="alert.textClass">{{ alert.count }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Advanced Filtering Panel -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-semibold text-slate-900">Advanced Filters</h3>
                <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">Reset</button>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <!-- Event Type Selection -->
              <div class="space-y-3">
                <h4 class="font-medium text-slate-700">Event Categories</h4>
                <div class="space-y-2">
                  <label v-for="category in eventCategories" :key="category.type" 
                         class="flex items-center gap-2 text-sm cursor-pointer hover:bg-slate-50 p-2 rounded-lg transition-colors">
                    <input
                      type="checkbox"
                      class="rounded text-blue-600 focus:ring-blue-500"
                      :checked="true"
                    />
                    <component :is="category.icon" :size="16" :class="category.color" />
                    <span class="text-slate-700">{{ category.type }}</span>
                    <span class="ml-auto text-xs text-slate-500">({{ category.count }})</span>
                  </label>
                </div>
              </div>

              <!-- Severity Level Filter -->
              <div class="space-y-3">
                <h4 class="font-medium text-slate-700">Severity Levels</h4>
                <div class="grid grid-cols-2 gap-2">
                  <button
                    v-for="severity in severityStats"
                    :key="severity.level"
                    class="flex items-center justify-between p-2 text-xs rounded-lg border-2 transition-colors hover:shadow-sm"
                    :class="severity.borderColor"
                  >
                    <div class="flex items-center gap-2">
                      <div class="w-2 h-2 rounded-full" :class="severity.dotColor"></div>
                      <span :class="severity.textColor">{{ severity.level }}</span>
                    </div>
                    <span :class="severity.textColor" class="font-semibold">{{ severity.count }}</span>
                  </button>
                </div>
              </div>

              <!-- Time Range Filter -->
              <div class="space-y-3">
                <h4 class="font-medium text-slate-700">Time Range</h4>
                <div class="grid grid-cols-2 gap-2 text-xs">
                  <button class="px-3 py-2 bg-blue-100 text-blue-700 rounded-lg font-medium">Last Hour</button>
                  <button class="px-3 py-2 bg-slate-100 text-slate-600 rounded-lg hover:bg-slate-200 transition-colors">Today</button>
                  <button class="px-3 py-2 bg-slate-100 text-slate-600 rounded-lg hover:bg-slate-200 transition-colors">This Week</button>
                  <button class="px-3 py-2 bg-slate-100 text-slate-600 rounded-lg hover:bg-slate-200 transition-colors">Custom</button>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <h3 class="text-lg font-semibold text-slate-900">Quick Actions</h3>
            </div>
            <div class="p-6 space-y-3">
              <button class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-4 rounded-lg transition-colors duration-200 flex items-center justify-center gap-2">
                <Download :size="16" />
                Export Events
              </button>
              <button class="w-full border border-slate-300 rounded-lg hover:bg-slate-50 font-medium py-3 px-4 transition-colors flex items-center justify-center gap-2">
                <Trash2 :size="16" />
                Clear Acknowledged
              </button>
              <button class="w-full border border-slate-300 rounded-lg hover:bg-slate-50 font-medium py-3 px-4 transition-colors flex items-center justify-center gap-2">
                <Settings :size="16" />
                Stream Settings
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Event Analytics Graphs Section -->
      <section class="mb-8">
        <div class="mb-6">
          <h2 class="text-2xl font-bold text-slate-900 mb-2">Event Trends & Analytics</h2>
          <p class="text-slate-600">Visual insights into event patterns and operational performance</p>
        </div>
        
        <!-- 4 Graph Grid Layout -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Event Trend Line Chart -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center justify-between mb-4">
              <div>
                <h3 class="text-lg font-semibold text-slate-900">Event Trends</h3>
                <p class="text-sm text-slate-600">Events over time (Last 24 hours)</p>
              </div>
              <div class="flex items-center gap-2">
                <button class="text-xs px-2 py-1 bg-blue-100 text-blue-700 rounded-full font-medium">24h</button>
                <button class="text-xs px-2 py-1 bg-slate-100 text-slate-600 rounded-full hover:bg-slate-200 transition-colors">7d</button>
              </div>
            </div>
            
            <!-- Line Chart Visualization -->
            <div class="relative h-48">
              <div class="absolute inset-0 flex items-end justify-between">
                <!-- Y-axis labels -->
                <div class="flex flex-col justify-between h-full text-xs text-slate-400 mr-3">
                  <span>50</span>
                  <span>40</span>
                  <span>30</span>
                  <span>20</span>
                  <span>10</span>
                  <span>0</span>
                </div>
                
                <!-- Chart area -->
                <div class="flex-1 h-full relative">
                  <!-- Grid lines -->
                  <div class="absolute inset-0">
                    <div v-for="i in 5" :key="i" class="absolute w-full border-t border-slate-100" :style="{ top: `${(i-1) * 25}%` }"></div>
                  </div>
                  
                  <!-- Line chart -->
                  <svg class="absolute inset-0 w-full h-full">
                    <defs>
                      <linearGradient id="lineGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                        <stop offset="0%" style="stop-color:#3b82f6;stop-opacity:0.3" />
                        <stop offset="100%" style="stop-color:#3b82f6;stop-opacity:0" />
                      </linearGradient>
                    </defs>
                    
                    <!-- Area fill -->
                    <path d="M 0 150 L 50 120 L 100 140 L 150 100 L 200 110 L 250 80 L 300 90 L 350 60 L 400 70 L 400 192 L 0 192 Z" 
                          fill="url(#lineGradient)" />
                    
                    <!-- Line -->
                    <path d="M 0 150 L 50 120 L 100 140 L 150 100 L 200 110 L 250 80 L 300 90 L 350 60 L 400 70" 
                          stroke="#3b82f6" stroke-width="3" fill="none" />
                    
                    <!-- Data points -->
                    <circle v-for="(point, i) in 9" :key="`point-${i}`" 
                            :cx="i * 50" 
                            :cy="150 - (Math.random() * 100)"
                            r="4" 
                            fill="#3b82f6" 
                            class="cursor-pointer hover:r-6 transition-all">
                      <title>{{ Math.floor(Math.random() * 50) }} events at {{ i * 3 }}:00</title>
                    </circle>
                  </svg>
                </div>
              </div>
              
              <!-- X-axis labels -->
              <div class="flex justify-between text-xs text-slate-400 mt-2 ml-8">
                <span v-for="hour in 9" :key="hour">{{ (hour - 1) * 3 }}:00</span>
              </div>
            </div>
            
            <!-- Chart stats -->
            <div class="grid grid-cols-3 gap-4 mt-4 pt-4 border-t border-slate-100 text-center">
              <div>
                <div class="text-lg font-bold text-blue-600">{{ chartStats.peak }}</div>
                <div class="text-xs text-slate-600">Peak Hour</div>
              </div>
              <div>
                <div class="text-lg font-bold text-green-600">{{ chartStats.average }}</div>
                <div class="text-xs text-slate-600">Avg/Hour</div>
              </div>
              <div>
                <div class="text-lg font-bold text-purple-600">{{ chartStats.total }}</div>
                <div class="text-xs text-slate-600">Total Today</div>
              </div>
            </div>
          </div>

          <!-- Event Types Pie Chart -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center justify-between mb-4">
              <div>
                <h3 class="text-lg font-semibold text-slate-900">Event Distribution</h3>
                <p class="text-sm text-slate-600">By event category</p>
              </div>
              <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">Details</button>
            </div>
            
            <!-- Pie Chart -->
            <div class="flex items-center justify-center">
              <div class="relative w-48 h-48">
                <svg class="w-full h-full" viewBox="0 0 200 200">
                  <!-- Pie segments -->
                  <circle cx="100" cy="100" r="80" fill="none" stroke="#3b82f6" stroke-width="40" 
                          stroke-dasharray="126 188" stroke-dashoffset="0" transform="rotate(-90 100 100)" />
                  <circle cx="100" cy="100" r="80" fill="none" stroke="#10b981" stroke-width="40" 
                          stroke-dasharray="94 220" stroke-dashoffset="-126" transform="rotate(-90 100 100)" />
                  <circle cx="100" cy="100" r="80" fill="none" stroke="#8b5cf6" stroke-width="40" 
                          stroke-dasharray="63 251" stroke-dashoffset="-220" transform="rotate(-90 100 100)" />
                  <circle cx="100" cy="100" r="80" fill="none" stroke="#f59e0b" stroke-width="40" 
                          stroke-dasharray="31 283" stroke-dashoffset="-283" transform="rotate(-90 100 100)" />
                  
                  <!-- Center text -->
                  <text x="100" y="95" text-anchor="middle" class="text-sm font-bold fill-slate-700">{{ pieChartStats.total }}</text>
                  <text x="100" y="110" text-anchor="middle" class="text-xs fill-slate-500">Total Events</text>
                </svg>
              </div>
            </div>
            
            <!-- Legend -->
            <div class="grid grid-cols-2 gap-3 mt-4">
              <div v-for="segment in pieChartSegments" :key="segment.label" class="flex items-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="segment.color"></div>
                <div class="flex-1">
                  <div class="text-sm font-medium text-slate-700">{{ segment.label }}</div>
                  <div class="text-xs text-slate-500">{{ segment.percentage }}% ({{ segment.value }})</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Severity Distribution Bar Chart -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center justify-between mb-4">
              <div>
                <h3 class="text-lg font-semibold text-slate-900">Severity Analysis</h3>
                <p class="text-sm text-slate-600">Event priority distribution</p>
              </div>
              <div class="text-xs px-2 py-1 bg-red-100 text-red-700 rounded-full font-medium">{{ severityStats.reduce((sum, s) => sum + s.count, 0) }} Total</div>
            </div>
            
            <!-- Horizontal Bar Chart -->
            <div class="space-y-4">
              <div v-for="severity in severityStats" :key="severity.level" class="group">
                <div class="flex items-center justify-between mb-2">
                  <div class="flex items-center gap-2">
                    <div class="w-3 h-3 rounded-full" :class="severity.dotColor"></div>
                    <span class="text-sm font-medium text-slate-700">{{ severity.level }}</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <span class="text-sm font-bold" :class="severity.textColor">{{ severity.count }}</span>
                    <span class="text-xs text-slate-500">({{ Math.round((severity.count / severityStats.reduce((sum, s) => sum + s.count, 0)) * 100) }}%)</span>
                  </div>
                </div>
                
                <!-- Animated bar -->
                <div class="w-full bg-slate-200 rounded-full h-3 group-hover:h-4 transition-all duration-200">
                  <div 
                    class="h-3 group-hover:h-4 rounded-full transition-all duration-500 relative overflow-hidden"
                    :class="severity.dotColor"
                    :style="{ width: `${(severity.count / severityStats.reduce((sum, s) => sum + s.count, 0)) * 100}%` }"
                  >
                    <div class="absolute inset-0 bg-white opacity-20 animate-pulse"></div>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Severity insights -->
            <div class="mt-6 pt-4 border-t border-slate-100">
              <div class="grid grid-cols-2 gap-4 text-center">
                <div class="p-3 bg-red-50 rounded-lg">
                  <div class="text-lg font-bold text-red-600">{{ Math.round((severityStats.find(s => s.level === 'Critical')?.count || 0) / severityStats.reduce((sum, s) => sum + s.count, 0) * 100) }}%</div>
                  <div class="text-xs text-red-600">Critical Events</div>
                </div>
                <div class="p-3 bg-green-50 rounded-lg">
                  <div class="text-lg font-bold text-green-600">{{ Math.round(((severityStats.find(s => s.level === 'Low')?.count || 0) + (severityStats.find(s => s.level === 'Medium')?.count || 0)) / severityStats.reduce((sum, s) => sum + s.count, 0) * 100) }}%</div>
                  <div class="text-xs text-green-600">Non-Critical</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Real-time Performance Metrics -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center justify-between mb-4">
              <div>
                <h3 class="text-lg font-semibold text-slate-900">Performance Metrics</h3>
                <p class="text-sm text-slate-600">Real-time system performance</p>
              </div>
              <div class="flex items-center gap-1">
                <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                <span class="text-xs text-green-600 font-medium">Live</span>
              </div>
            </div>
            
            <!-- Performance gauges -->
            <div class="space-y-6">
              <!-- Response Time Gauge -->
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-sm font-medium text-slate-700">Avg Response Time</span>
                  <span class="text-sm font-bold text-blue-600">{{ analytics.avgResponseTime }}ms</span>
                </div>
                <div class="w-full bg-slate-200 rounded-full h-2">
                  <div class="bg-blue-500 h-2 rounded-full transition-all duration-1000" 
                       :style="{ width: `${Math.min((analytics.avgResponseTime / 500) * 100, 100)}%` }"></div>
                </div>
                <div class="flex justify-between text-xs text-slate-500 mt-1">
                  <span>0ms</span>
                  <span>500ms</span>
                </div>
              </div>
              
              <!-- SLA Compliance Gauge -->
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-sm font-medium text-slate-700">SLA Compliance</span>
                  <span class="text-sm font-bold text-green-600">{{ analytics.slaCompliance }}%</span>
                </div>
                <div class="w-full bg-slate-200 rounded-full h-2">
                  <div class="bg-green-500 h-2 rounded-full transition-all duration-1000" 
                       :style="{ width: `${analytics.slaCompliance}%` }"></div>
                </div>
                <div class="flex justify-between text-xs text-slate-500 mt-1">
                  <span>0%</span>
                  <span>100%</span>
                </div>
              </div>
              
              <!-- Throughput Gauge -->
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-sm font-medium text-slate-700">Event Throughput</span>
                  <span class="text-sm font-bold text-purple-600">{{ streamStats.eventsPerMinute }}/min</span>
                </div>
                <div class="w-full bg-slate-200 rounded-full h-2">
                  <div class="bg-purple-500 h-2 rounded-full transition-all duration-1000" 
                       :style="{ width: `${Math.min((streamStats.eventsPerMinute / 100) * 100, 100)}%` }"></div>
                </div>
                <div class="flex justify-between text-xs text-slate-500 mt-1">
                  <span>0/min</span>
                  <span>100/min</span>
                </div>
              </div>
            </div>
            
            <!-- Performance summary -->
            <div class="mt-6 pt-4 border-t border-slate-100">
              <div class="flex items-center justify-center">
                <div class="text-center">
                  <div class="text-2xl font-bold text-green-600">{{ performanceScore }}%</div>
                  <div class="text-sm text-slate-600">Overall Performance</div>
                  <div class="text-xs text-green-600 font-medium mt-1">Excellent</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>

    <!-- Enhanced Event Detail Modal -->
    <div v-if="selectedEvent" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-2xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-hidden">
        <!-- Modal Header -->
        <div class="border-b border-slate-200 p-6">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-12 h-12 rounded-lg flex items-center justify-center" :class="getEventIconBg(selectedEvent.eventType)">
                <component :is="getEventIcon(selectedEvent.eventType)" :size="24" :class="getEventIconColor(selectedEvent.eventType)" />
              </div>
              <div>
                <h2 class="text-2xl font-bold text-slate-900">{{ selectedEvent.title || selectedEvent.eventType }}</h2>
                <p class="text-slate-600">Event ID: {{ selectedEvent.id }}</p>
              </div>
            </div>
            <button 
              @click="selectedEvent = null"
              class="p-2 hover:bg-slate-100 rounded-lg transition-colors"
            >
              <X :size="20" class="text-slate-500" />
            </button>
          </div>
        </div>
        
        <!-- Modal Content -->
        <div class="p-6 overflow-y-auto max-h-[calc(90vh-200px)]">
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <!-- Main Event Information -->
            <div class="lg:col-span-2 space-y-6">
              <!-- Event Details -->
              <div class="bg-slate-50 rounded-lg p-4">
                <h3 class="font-semibold text-slate-900 mb-3">Event Details</h3>
                <div class="space-y-3 text-sm">
                  <div class="flex justify-between">
                    <span class="text-slate-600">Type:</span>
                    <span class="font-medium">{{ selectedEvent.eventType }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-slate-600">Severity:</span>
                    <span class="px-2 py-1 rounded-full text-xs font-medium" :class="getSeverityColor(selectedEvent.severity)">
                      {{ selectedEvent.severity }}
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-slate-600">Time:</span>
                    <span class="font-medium">{{ formatEventTime(selectedEvent.eventTime) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-slate-600">Source:</span>
                    <span class="font-medium">{{ selectedEvent.source }}</span>
                  </div>
                  <div v-if="selectedEvent.userName" class="flex justify-between">
                    <span class="text-slate-600">Reporter:</span>
                    <span class="font-medium">{{ selectedEvent.userName }}</span>
                  </div>
                </div>
              </div>

              <!-- Event Description -->
              <div>
                <h3 class="font-semibold text-slate-900 mb-3">Description</h3>
                <p class="text-slate-700 leading-relaxed bg-white p-4 rounded-lg border">
                  {{ selectedEvent.description }}
                </p>
              </div>

              <!-- Related Entities -->
              <div v-if="hasRelatedEntities(selectedEvent)" class="space-y-4">
                <h3 class="font-semibold text-slate-900">Related Information</h3>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div v-if="selectedEvent.containerId" class="bg-blue-50 p-4 rounded-lg border border-blue-200">
                    <div class="flex items-center gap-2 mb-2">
                      <Package :size="16" class="text-blue-600" />
                      <span class="font-semibold text-blue-900">Container</span>
                    </div>
                    <p class="text-blue-700">{{ selectedEvent.containerId }}</p>
                  </div>
                  <div v-if="selectedEvent.shipName" class="bg-green-50 p-4 rounded-lg border border-green-200">
                    <div class="flex items-center gap-2 mb-2">
                      <Ship :size="16" class="text-green-600" />
                      <span class="font-semibold text-green-900">Ship</span>
                    </div>
                    <p class="text-green-700">{{ selectedEvent.shipName }}</p>
                  </div>
                  <div v-if="selectedEvent.berthName" class="bg-purple-50 p-4 rounded-lg border border-purple-200">
                    <div class="flex items-center gap-2 mb-2">
                      <Anchor :size="16" class="text-purple-600" />
                      <span class="font-semibold text-purple-900">Berth</span>
                    </div>
                    <p class="text-purple-700">{{ selectedEvent.berthName }}</p>
                  </div>
                  <div v-if="selectedEvent.portName" class="bg-orange-50 p-4 rounded-lg border border-orange-200">
                    <div class="flex items-center gap-2 mb-2">
                      <MapPin :size="16" class="text-orange-600" />
                      <span class="font-semibold text-orange-900">Port</span>
                    </div>
                    <p class="text-orange-700">{{ selectedEvent.portName }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Event Actions & Status -->
            <div class="space-y-6">
              <!-- Status & Actions -->
              <div class="bg-slate-50 rounded-lg p-4">
                <h3 class="font-semibold text-slate-900 mb-3">Actions</h3>
                <div class="space-y-3">
                  <button 
                    v-if="!selectedEvent.isRead"
                    @click="markAsRead(selectedEvent.id)"
                    class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
                  >
                    Mark as Read
                  </button>
                  <button class="w-full border border-slate-300 hover:bg-slate-50 font-medium py-2 px-4 rounded-lg transition-colors">
                    Assign to User
                  </button>
                  <button class="w-full border border-slate-300 hover:bg-slate-50 font-medium py-2 px-4 rounded-lg transition-colors">
                    Add Comment
                  </button>
                  <button class="w-full border border-slate-300 hover:bg-slate-50 font-medium py-2 px-4 rounded-lg transition-colors">
                    Export Details
                  </button>
                </div>
              </div>

              <!-- Event Timeline -->
              <div class="bg-slate-50 rounded-lg p-4">
                <h3 class="font-semibold text-slate-900 mb-3">Event Timeline</h3>
                <div class="space-y-3 text-sm">
                  <div class="flex items-start gap-3">
                    <div class="w-2 h-2 bg-blue-500 rounded-full mt-2"></div>
                    <div>
                      <p class="font-medium">Event Created</p>
                      <p class="text-slate-500">{{ formatEventTime(selectedEvent.createdAt) }}</p>
                    </div>
                  </div>
                  <div class="flex items-start gap-3">
                    <div class="w-2 h-2 bg-green-500 rounded-full mt-2"></div>
                    <div>
                      <p class="font-medium">Event Detected</p>
                      <p class="text-slate-500">{{ formatEventTime(selectedEvent.eventTime) }}</p>
                    </div>
                  </div>
                  <div v-if="selectedEvent.isRead" class="flex items-start gap-3">
                    <div class="w-2 h-2 bg-purple-500 rounded-full mt-2"></div>
                    <div>
                      <p class="font-medium">Acknowledged</p>
                      <p class="text-slate-500">{{ getTimeAgo(selectedEvent.eventTime) }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Related Events -->
              <div v-if="relatedEvents.length > 0" class="bg-slate-50 rounded-lg p-4">
                <h3 class="font-semibold text-slate-900 mb-3">Related Events</h3>
                <div class="space-y-2">
                  <div 
                    v-for="event in relatedEvents.slice(0, 3)" 
                    :key="event.id"
                    class="p-2 bg-white rounded border text-sm cursor-pointer hover:bg-slate-50 transition-colors"
                    @click="selectedEvent = event"
                  >
                    <p class="font-medium text-slate-900">{{ event.title || event.eventType }}</p>
                    <p class="text-slate-500 text-xs">{{ getTimeAgo(event.eventTime) }}</p>
                  </div>
                  <button v-if="relatedEvents.length > 3" class="text-blue-600 hover:text-blue-700 text-sm font-medium">
                    View All Related ({{ relatedEvents.length }})
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { 
  Activity, 
  Radio, 
  Eye, 
  Clock, 
  TrendingUp, 
  Zap, 
  Download, 
  Trash2, 
  Settings,
  Container as ContainerIcon,
  Ship,
  Anchor,
  AlertTriangle,
  CheckCircle,
  Grid,
  List,
  Plus,
  X,
  Package,
  MapPin,
  Truck,
  Database,
  Users,
  Shield,
  Wrench,
  Bell,
  Filter,
  Search,
  RefreshCw,
  BarChart3,
  Calendar,
  User
} from 'lucide-vue-next';

// TypeScript interfaces based on backend DTOs
interface EventDto {
  id: number;
  eventType: string;
  title: string;
  description: string;
  eventTime: string;
  severity: string;
  containerId?: string;
  shipId?: number;
  shipName?: string;
  berthId?: number;
  berthName?: string;
  portId?: number;
  portName?: string;
  userId?: number;
  userName?: string;
  source: string;
  isRead: boolean;
  createdAt: string;
}

interface EventStatsDto {
  totalEvents: number;
  unreadEvents: number;
  todayEvents: number;
  weekEvents: number;
  eventsBySeverity: { [key: string]: number };
  eventsByType: { [key: string]: number };
  eventsBySource: { [key: string]: number };
  recentTrends: EventTrendDto[];
}

interface EventTrendDto {
  date: string;
  count: number;
  eventType: string;
}

interface EventFilterDto {
  eventType?: string;
  severity?: string;
  source?: string;
  containerId?: string;
  shipId?: number;
  berthId?: number;
  portId?: number;
  userId?: number;
  eventAfter?: string;
  eventBefore?: string;
  isRead?: boolean;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: string;
  page?: number;
  pageSize?: number;
}

interface EventStat {
  label: string;
  value: string;
  color: string;
  bgColor: string;
  iconColor: string;
  icon: any;
  change: string;
  trend: 'up' | 'down';
  period: string;
  severity?: string;
}

interface EventCategory {
  type: string;
  count: number;
  percentage: number;
  icon: any;
  color: string;
  barColor: string;
}

interface SeverityStats {
  level: string;
  count: number;
  description: string;
  dotColor: string;
  textColor: string;
  borderColor: string;
}

interface StreamStats {
  eventsPerSecond: number;
  eventsPerMinute: number;
  avgLatency: number;
  uptime: string;
  lastEventTime: string;
  errorRate: number;
  throughput: number;
}

interface Analytics {
  avgResponseTime: number;
  slaCompliance: number;
}

interface AlertSummary {
  type: string;
  count: number;
  bgClass: string;
  dotClass: string;
  textClass: string;
}

interface QuickFilter {
  id: string;
  label: string;
  active: boolean;
}

// Reactive state
const currentTime = ref(new Date().toLocaleTimeString());
const viewMode = ref<'list' | 'grid'>('list');
const selectedEvent = ref<EventDto | null>(null);
const autoRefresh = ref(true);
const hasMoreEvents = ref(true);
const isScrollableMode = ref(false);
const initialHeight = ref<string>('');
const eventContainerRef = ref<HTMLElement | null>(null);

// Stream performance stats
const streamStats = ref<StreamStats>({
  eventsPerSecond: 3.2,
  eventsPerMinute: 45,
  avgLatency: 85,
  uptime: '99.9%',
  lastEventTime: 'Just now',
  errorRate: 0.1,
  throughput: 2.3
});

// Analytics data
const analytics = ref<Analytics>({
  avgResponseTime: 245,
  slaCompliance: 98.7
});

// Alert summary
const alertSummary = ref<AlertSummary[]>([
  { type: 'Critical', count: 2, bgClass: 'bg-red-50', dotClass: 'bg-red-500', textClass: 'text-red-700' },
  { type: 'High', count: 8, bgClass: 'bg-orange-50', dotClass: 'bg-orange-500', textClass: 'text-orange-700' },
  { type: 'Medium', count: 23, bgClass: 'bg-yellow-50', dotClass: 'bg-yellow-500', textClass: 'text-yellow-700' },
  { type: 'Low', count: 45, bgClass: 'bg-green-50', dotClass: 'bg-green-500', textClass: 'text-green-700' }
]);

// Enhanced mock events based on backend schema
const liveEvents = ref<EventDto[]>([
  {
    id: 1,
    eventType: "Container Arrival",
    title: "Container Arrival at Chennai Port",
    description: "Container CTR-2024-156 arrived at Chennai Port via MV Ocean Star. Customs inspection required before processing.",
    eventTime: new Date().toISOString(),
    severity: "High",
    containerId: "CTR-2024-156",
    shipId: 1,
    shipName: "MV Ocean Star",
    portId: 1,
    portName: "Chennai Port",
    userId: 1,
    userName: "Port Officer",
    source: "Port Management System",
    isRead: false,
    createdAt: new Date().toISOString()
  },
  {
    id: 2,
    eventType: "Customs Inspection",
    title: "Customs Inspection Completed",
    description: "Container CTR-2024-143 successfully passed customs inspection. All documents verified and approved.",
    eventTime: new Date(Date.now() - 300000).toISOString(),
    severity: "Medium",
    containerId: "CTR-2024-143",
    berthId: 2,
    berthName: "Berth B-05",
    portId: 1,
    portName: "Chennai Port",
    source: "Customs System",
    isRead: true,
    createdAt: new Date(Date.now() - 300000).toISOString()
  },
  {
    id: 3,
    eventType: "Loading Operation",
    title: "Container Loading Started",
    description: "Loading operation initiated for container CTR-2024-139 on MV Blue Wave. Estimated completion in 2 hours.",
    eventTime: new Date(Date.now() - 600000).toISOString(),
    severity: "High",
    containerId: "CTR-2024-139",
    shipId: 2,
    shipName: "MV Blue Wave",
    berthId: 3,
    berthName: "Berth A-12",
    portId: 1,
    portName: "Chennai Port",
    source: "Loading Management",
    isRead: false,
    createdAt: new Date(Date.now() - 600000).toISOString()
  },
  {
    id: 4,
    eventType: "Equipment Malfunction",
    title: "Crane Equipment Issue",
    description: "Crane #5 at Berth A-10 experiencing mechanical issues. Maintenance team dispatched for immediate repair.",
    eventTime: new Date(Date.now() - 900000).toISOString(),
    severity: "Critical",
    berthId: 4,
    berthName: "Berth A-10",
    portId: 1,
    portName: "Chennai Port",
    userId: 2,
    userName: "Maintenance Chief",
    source: "Equipment Monitoring",
    isRead: false,
    createdAt: new Date(Date.now() - 900000).toISOString()
  },
  {
    id: 5,
    eventType: "Ship Departure",
    title: "Ship Departure Notification",
    description: "MV Atlantic departed successfully with 45 containers bound for Mumbai Port. All loading operations completed on schedule.",
    eventTime: new Date(Date.now() - 1200000).toISOString(),
    severity: "Low",
    shipId: 3,
    shipName: "MV Atlantic",
    portId: 1,
    portName: "Chennai Port",
    source: "Traffic Control",
    isRead: true,
    createdAt: new Date(Date.now() - 1200000).toISOString()
  },
  {
    id: 6,
    eventType: "Security Alert",
    title: "Unauthorized Access Detected",
    description: "Security system detected unauthorized access attempt at Gate 3. Security personnel notified and investigating.",
    eventTime: new Date(Date.now() - 1800000).toISOString(),
    severity: "Critical",
    portId: 1,
    portName: "Chennai Port",
    userId: 3,
    userName: "Security Officer",
    source: "Security System",
    isRead: false,
    createdAt: new Date(Date.now() - 1800000).toISOString()
  },
  {
    id: 7,
    eventType: "Weather Alert",
    title: "Heavy Rain Warning",
    description: "Meteorological department issued heavy rain warning for the next 4 hours. All outdoor operations should be suspended.",
    eventTime: new Date(Date.now() - 2100000).toISOString(),
    severity: "High",
    portId: 1,
    portName: "Chennai Port",
    source: "Weather Service",
    isRead: true,
    createdAt: new Date(Date.now() - 2100000).toISOString()
  },
  {
    id: 8,
    eventType: "System Maintenance",
    title: "Scheduled System Maintenance",
    description: "Port management system will undergo scheduled maintenance from 2:00 AM to 4:00 AM. Limited functionality expected.",
    eventTime: new Date(Date.now() - 2400000).toISOString(),
    severity: "Medium",
    portId: 1,
    portName: "Chennai Port",
    userId: 4,
    userName: "System Administrator",
    source: "IT Operations",
    isRead: true,
    createdAt: new Date(Date.now() - 2400000).toISOString()
  }
]);

// Enhanced event statistics
const eventStats = ref<EventStat[]>([
  { 
    label: "Events Today", 
    value: "342", 
    color: "text-blue-600",
    bgColor: "bg-blue-50",
    iconColor: "text-blue-600",
    icon: Activity,
    change: "12",
    trend: "up",
    period: "vs yesterday",
    severity: "Info"
  },
  { 
    label: "Unread Events", 
    value: "23", 
    color: "text-orange-600",
    bgColor: "bg-orange-50",
    iconColor: "text-orange-600",
    icon: Bell,
    change: "8",
    trend: "up",
    period: "vs last hour",
    severity: "Warning"
  },
  { 
    label: "Processed", 
    value: "319", 
    color: "text-green-600",
    bgColor: "bg-green-50",
    iconColor: "text-green-600",
    icon: CheckCircle,
    change: "15",
    trend: "up",
    period: "vs yesterday",
    severity: "Success"
  },
  { 
    label: "Critical Events", 
    value: "2", 
    color: "text-red-600",
    bgColor: "bg-red-50",
    iconColor: "text-red-600",
    icon: AlertTriangle,
    change: "4",
    trend: "down",
    period: "vs last week",
    severity: "Critical"
  }
]);

// Event categories with enhanced data
const eventCategories = ref<EventCategory[]>([
  { 
    type: "Container Operations", 
    count: 156, 
    percentage: 45, 
    icon: ContainerIcon, 
    color: "text-blue-600", 
    barColor: "bg-blue-500" 
  },
  { 
    type: "Ship Operations", 
    count: 89, 
    percentage: 26, 
    icon: Ship, 
    color: "text-green-600", 
    barColor: "bg-green-500" 
  },
  { 
    type: "Port Management", 
    count: 67, 
    percentage: 19, 
    icon: Anchor, 
    color: "text-purple-600", 
    barColor: "bg-purple-500" 
  },
  { 
    type: "System Alerts", 
    count: 34, 
    percentage: 10, 
    icon: AlertTriangle, 
    color: "text-orange-600", 
    barColor: "bg-orange-500" 
  }
]);

// Severity statistics
const severityStats = ref<SeverityStats[]>([
  { 
    level: "Critical", 
    count: 2, 
    description: "Immediate attention", 
    dotColor: "bg-red-500", 
    textColor: "text-red-700", 
    borderColor: "border-red-200" 
  },
  { 
    level: "High", 
    count: 8, 
    description: "Priority handling", 
    dotColor: "bg-orange-500", 
    textColor: "text-orange-700", 
    borderColor: "border-orange-200" 
  },
  { 
    level: "Medium", 
    count: 23, 
    description: "Standard process", 
    dotColor: "bg-yellow-500", 
    textColor: "text-yellow-700", 
    borderColor: "border-yellow-200" 
  },
  { 
    level: "Low", 
    count: 45, 
    description: "Information only", 
    dotColor: "bg-green-500", 
    textColor: "text-green-700", 
    borderColor: "border-green-200" 
  }
]);

// Quick filters
const quickFilters = ref<QuickFilter[]>([
  { id: 'unread', label: 'Unread', active: true },
  { id: 'critical', label: 'Critical', active: false },
  { id: 'container', label: 'Container', active: false },
  { id: 'ship', label: 'Ship', active: false },
  { id: 'today', label: 'Today', active: true }
]);

// Event filtering
const eventFilters = ref<EventFilterDto>({
  sortBy: 'eventTime',
  sortDirection: 'desc',
  page: 1,
  pageSize: 20
});

// Computed properties
const filteredEvents = computed(() => {
  let filtered = liveEvents.value;
  
  // Apply quick filters
  const activeFilters = quickFilters.value.filter(f => f.active);
  if (activeFilters.length > 0) {
    filtered = filtered.filter(event => {
      return activeFilters.some(filter => {
        switch (filter.id) {
          case 'unread': return !event.isRead;
          case 'critical': return event.severity === 'Critical';
          case 'container': return event.eventType.toLowerCase().includes('container');
          case 'ship': return event.eventType.toLowerCase().includes('ship') || event.shipName;
          case 'today': return new Date(event.eventTime).toDateString() === new Date().toDateString();
          default: return true;
        }
      });
    });
  }
  
  return filtered.sort((a, b) => 
    new Date(b.eventTime).getTime() - new Date(a.eventTime).getTime()
  );
});

const relatedEvents = computed(() => {
  if (!selectedEvent.value) return [];
  
  return liveEvents.value.filter(event => 
    event.id !== selectedEvent.value!.id && (
      event.containerId === selectedEvent.value!.containerId ||
      event.shipId === selectedEvent.value!.shipId ||
      event.berthId === selectedEvent.value!.berthId ||
      event.portId === selectedEvent.value!.portId
    )
  ).slice(0, 5);
});

// Utility functions
const getSeverityColor = (severity: string): string => {
  const severityColors = {
    "Critical": "bg-red-100 text-red-800 border-red-200",
    "High": "bg-orange-100 text-orange-800 border-orange-200",
    "Medium": "bg-yellow-100 text-yellow-800 border-yellow-200",
    "Low": "bg-green-100 text-green-800 border-green-200",
  };
  return severityColors[severity as keyof typeof severityColors] || "bg-slate-100 text-slate-800 border-slate-200";
};

const getEventIcon = (eventType: string) => {
  const iconMap = {
    "Container Arrival": ContainerIcon,
    "Container Departure": ContainerIcon,
    "Customs Inspection": CheckCircle,
    "Loading Operation": Activity,
    "Unloading Operation": Activity,
    "Berth Assignment": Anchor,
    "Ship Arrival": Ship,
    "Ship Departure": Ship,
    "Equipment Malfunction": Wrench,
    "Security Alert": Shield,
    "Weather Alert": AlertTriangle,
    "System Maintenance": Settings,
    "User Action": User,
    "Port Management": Database
  };
  return iconMap[eventType as keyof typeof iconMap] || Activity;
};

const getEventIconBg = (eventType: string): string => {
  const bgMap = {
    "Container Arrival": "bg-blue-100",
    "Container Departure": "bg-blue-100", 
    "Customs Inspection": "bg-green-100",
    "Loading Operation": "bg-purple-100",
    "Unloading Operation": "bg-purple-100",
    "Berth Assignment": "bg-indigo-100",
    "Ship Arrival": "bg-teal-100",
    "Ship Departure": "bg-teal-100",
    "Equipment Malfunction": "bg-red-100",
    "Security Alert": "bg-red-100",
    "Weather Alert": "bg-orange-100",
    "System Maintenance": "bg-yellow-100",
    "User Action": "bg-gray-100",
    "Port Management": "bg-slate-100"
  };
  return bgMap[eventType as keyof typeof bgMap] || "bg-blue-100";
};

const getEventIconColor = (eventType: string): string => {
  const colorMap = {
    "Container Arrival": "text-blue-600",
    "Container Departure": "text-blue-600",
    "Customs Inspection": "text-green-600",
    "Loading Operation": "text-purple-600", 
    "Unloading Operation": "text-purple-600",
    "Berth Assignment": "text-indigo-600",
    "Ship Arrival": "text-teal-600",
    "Ship Departure": "text-teal-600",
    "Equipment Malfunction": "text-red-600",
    "Security Alert": "text-red-600",
    "Weather Alert": "text-orange-600",
    "System Maintenance": "text-yellow-600",
    "User Action": "text-gray-600",
    "Port Management": "text-slate-600"
  };
  return colorMap[eventType as keyof typeof colorMap] || "text-blue-600";
};

const getEventCardClass = (event: EventDto): string => {
  const baseClass = "border";
  if (!event.isRead) {
    return `${baseClass} border-2 border-blue-200 bg-blue-50`;
  }
  if (event.severity === 'Critical') {
    return `${baseClass} border-red-200 bg-red-50`;
  }
  return `${baseClass} border-slate-200 bg-white`;
};

const formatEventTime = (timeString: string): string => {
  const date = new Date(timeString);
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short', 
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const getTimeAgo = (timeString: string): string => {
  const date = new Date(timeString);
  const now = new Date();
  const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / (1000 * 60));
  
  if (diffInMinutes < 1) return 'Just now';
  if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
  
  const diffInHours = Math.floor(diffInMinutes / 60);
  if (diffInHours < 24) return `${diffInHours}h ago`;
  
  const diffInDays = Math.floor(diffInHours / 24);
  return `${diffInDays}d ago`;
};

const hasRelatedEntities = (event: EventDto): boolean => {
  return !!(event.containerId || event.shipName || event.berthName || event.portName);
};

// Event management methods
const toggleEventView = () => {
  viewMode.value = viewMode.value === 'list' ? 'grid' : 'list';
};

const openEventModal = (event: EventDto | null) => {
  selectedEvent.value = event;
};

const toggleQuickFilter = (filterId: string) => {
  const filter = quickFilters.value.find(f => f.id === filterId);
  if (filter) {
    filter.active = !filter.active;
  }
};

const toggleAutoRefresh = () => {
  autoRefresh.value = !autoRefresh.value;
};

const markAsRead = (eventId: number) => {
  const event = liveEvents.value.find(e => e.id === eventId);
  if (event) {
    event.isRead = true;
  }
};

const acknowledgeEvent = (eventId: number) => {
  markAsRead(eventId);
  // Here you would make API call to acknowledge the event
  console.log('Acknowledging event:', eventId);
};

const loadMoreEvents = () => {
  // Capture the current height before enabling scrollable mode
  if (eventContainerRef.value && !isScrollableMode.value) {
    initialHeight.value = eventContainerRef.value.offsetHeight + 'px';
  }
  // Activate scrollable mode when load more is clicked
  isScrollableMode.value = true;
  // Simulate loading more events
  console.log('Loading more events...');
  // Here you would make API call to fetch more events
};

// Lifecycle hooks
let timeInterval: ReturnType<typeof setInterval> | undefined;

onMounted(() => {
  // Start time interval
  timeInterval = setInterval(() => {
    currentTime.value = new Date().toLocaleTimeString();
  }, 1000);

  // Simulate real-time event updates
  if (autoRefresh.value) {
    setInterval(() => {
      // Simulate new events coming in
      updateStreamStats();
    }, 5000);
  }
});

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval);
  }
});

// Chart data for visualizations
const chartStats = ref({
  peak: '14:30',
  average: 28,
  total: 342
});

const pieChartStats = ref({
  total: 342
});

const pieChartSegments = ref([
  { label: 'Container Ops', value: 156, percentage: 45, color: 'bg-blue-500' },
  { label: 'Ship Ops', value: 89, percentage: 26, color: 'bg-green-500' },
  { label: 'Port Mgmt', value: 67, percentage: 19, color: 'bg-purple-500' },
  { label: 'System Alerts', value: 30, percentage: 9, color: 'bg-yellow-500' }
]);

const performanceScore = computed(() => {
  // Calculate overall performance based on multiple metrics
  const responseScore = Math.max(0, 100 - (analytics.value.avgResponseTime / 5));
  const slaScore = analytics.value.slaCompliance;
  const throughputScore = Math.min(100, (streamStats.value.eventsPerMinute / 60) * 100);
  
  return Math.round((responseScore + slaScore + throughputScore) / 3);
});

// Simulate stream stats updates
const updateStreamStats = () => {
  streamStats.value.eventsPerSecond = Math.random() * 5 + 1;
  streamStats.value.eventsPerMinute = Math.floor(streamStats.value.eventsPerSecond * 60);
  streamStats.value.avgLatency = Math.floor(Math.random() * 200 + 50);
  streamStats.value.lastEventTime = 'Just now';
  
  // Update chart stats
  chartStats.value.average = Math.floor(Math.random() * 20 + 25);
  chartStats.value.total = Math.floor(Math.random() * 50 + 320);
  
  // Update analytics
  analytics.value.avgResponseTime = Math.floor(Math.random() * 100 + 200);
  analytics.value.slaCompliance = Math.floor(Math.random() * 5 + 95);
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

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.8;
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

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

/* Custom hover effects */
.group:hover .group-hover\:scale-105 {
  transform: scale(1.05);
}

/* Responsive utilities */
@media (max-width: 768px) {
  .grid-cols-1.md\:grid-cols-2.lg\:grid-cols-4 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    gap: 1rem;
  }
  
  .grid-cols-1.lg\:grid-cols-3 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
  }
  
  .lg\:col-span-2 {
    grid-column: span 1;
  }
  
  .flex-col.md\:flex-row {
    flex-direction: column;
  }
  
  .gap-8 {
    gap: 1.5rem;
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
  
  .text-xl {
    font-size: 1.125rem;
    line-height: 1.75rem;
  }
  
  .text-lg {
    font-size: 1rem;
    line-height: 1.5rem;
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
  
  .gap-3 {
    gap: 0.5rem;
  }
  
  .mb-8 {
    margin-bottom: 1.5rem;
  }
  
  .mb-6 {
    margin-bottom: 1rem;
  }
  
  .space-y-6 > :not([hidden]) ~ :not([hidden]) {
    --tw-space-y-reverse: 0;
    margin-top: calc(1rem * calc(1 - var(--tw-space-y-reverse)));
    margin-bottom: calc(1rem * var(--tw-space-y-reverse));
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

/* Event feed specific styles */
.max-h-\[600px\] {
  max-height: 600px;
}

/* Custom checkbox styles */
input[type="checkbox"] {
  accent-color: #3b82f6;
}

/* Hover effects for interactive elements */
.cursor-pointer:hover {
  transform: scale(1.02);
}

/* Loading state placeholder */
.loading-shimmer {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200px 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: -200px 0;
  }
  100% {
    background-position: calc(200px + 100%) 0;
  }
}
</style>