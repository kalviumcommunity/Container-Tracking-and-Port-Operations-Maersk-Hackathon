
<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Hero Section -->
    <section class="relative py-20 px-6">
      <div class="max-w-7xl mx-auto">
        <div class="text-center mb-16">
          <div class="flex justify-center mb-8">
            <div class="p-6 bg-gradient-to-r from-blue-600 to-blue-700 rounded-2xl shadow-xl animate-slideIn">
              <Anchor :size="48" class="text-white" />
            </div>
          </div>
          <h1 class="text-5xl sm:text-6xl lg:text-7xl font-bold text-slate-900 mb-6 animate-slideIn" style="animation-delay: 200ms">
            Modern Port
            <br />
            <span class="text-blue-600">Operations System</span>
          </h1>
          <p class="text-xl text-slate-600 max-w-4xl mx-auto mb-10 leading-relaxed animate-slideIn" style="animation-delay: 400ms">
            Revolutionize your port operations with cutting-edge technology. Our comprehensive platform 
            delivers real-time tracking, intelligent automation, and complete visibility across all container 
            movements, vessels, and port activities. Built for efficiency, designed for scale.
          </p>
          <div class="flex flex-col sm:flex-row gap-4 justify-center animate-slideIn" style="animation-delay: 600ms">
            <button 
              @click="navigateToDashboard"
              class="px-8 py-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg shadow-lg text-lg font-medium transition-all duration-200 hover:-translate-y-1 flex items-center justify-center gap-2"
            >
              Get Started
              <ArrowRight :size="20" />
            </button>
            <button 
              class="px-8 py-4 border-2 border-slate-300 hover:bg-slate-50 text-slate-700 rounded-lg text-lg font-medium transition-colors duration-200 flex items-center justify-center gap-2"
            >
              <Play :size="20" />
              Watch Demo
            </button>
          </div>
        </div>

        <!-- Hero Stats -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-8 mt-20">
          <div 
            v-for="(stat, index) in heroStats" 
            :key="index"
            class="text-center p-6 bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-lg transition-all duration-300 hover:-translate-y-1 animate-slideIn"
            :style="{ animationDelay: `${800 + index * 100}ms` }"
          >
            <div class="p-3 rounded-lg mx-auto mb-4 w-fit" :class="stat.bgColor">
              <component :is="stat.icon" :size="24" :class="stat.iconColor" />
            </div>
            <div class="text-3xl font-bold text-slate-900 mb-2">{{ stat.value }}</div>
            <p class="text-slate-600 text-sm">{{ stat.label }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="py-20 px-6 bg-white">
      <div class="max-w-7xl mx-auto">
        <div class="text-center mb-16">
          <div class="inline-flex items-center gap-2 bg-blue-50 text-blue-600 px-4 py-2 rounded-full mb-6">
            <Zap :size="16" />
            <span class="font-semibold">Core Features</span>
          </div>
          <h2 class="text-4xl sm:text-5xl font-bold text-slate-900 mb-6">
            Everything You Need for Port Excellence
          </h2>
          <p class="text-xl text-slate-600 max-w-3xl mx-auto">
            Comprehensive tools and features designed to streamline every aspect of your port operations
          </p>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Dashboard Feature -->
          <div class="bg-slate-50 rounded-xl p-8 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 group">
            <div class="p-4 bg-blue-600 rounded-xl w-fit mb-6 group-hover:scale-110 transition-transform duration-300">
              <BarChart3 :size="32" class="text-white" />
            </div>
            <h3 class="text-2xl font-bold text-slate-900 mb-4">Real-Time Dashboard</h3>
            <p class="text-slate-600 mb-6 leading-relaxed">
              Monitor all port activities from a centralized dashboard with live metrics, 
              vessel tracking, and operational insights.
            </p>
            <ul class="space-y-3 mb-6">
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Live Performance Metrics</span>
              </li>
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Advanced Analytics</span>
              </li>
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Predictive Insights</span>
              </li>
            </ul>
            <button 
              @click="navigateTo('dashboard')"
              class="text-blue-600 hover:text-blue-700 font-medium flex items-center gap-2 group-hover:gap-3 transition-all"
            >
              Explore Dashboard
              <ArrowRight :size="16" />
            </button>
          </div>

          <!-- Container Management Feature -->
          <div class="bg-slate-50 rounded-xl p-8 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 group">
            <div class="p-4 bg-green-600 rounded-xl w-fit mb-6 group-hover:scale-110 transition-transform duration-300">
              <Container :size="32" class="text-white" />
            </div>
            <h3 class="text-2xl font-bold text-slate-900 mb-4">Container Management</h3>
            <p class="text-slate-600 mb-6 leading-relaxed">
              Complete container lifecycle management with tracking, inventory control, 
              and automated logistics coordination.
            </p>
            <ul class="space-y-3 mb-6">
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Precise Location Tracking</span>
              </li>
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Inventory Management</span>
              </li>
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Logistics Integration</span>
              </li>
            </ul>
            <button 
              @click="navigateTo('container-management')"
              class="text-green-600 hover:text-green-700 font-medium flex items-center gap-2 group-hover:gap-3 transition-all"
            >
              Manage Containers
              <ArrowRight :size="16" />
            </button>
          </div>

          <!-- Port Operations Feature -->
          <div class="bg-slate-50 rounded-xl p-8 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 group">
            <div class="p-4 bg-purple-600 rounded-xl w-fit mb-6 group-hover:scale-110 transition-transform duration-300">
              <Anchor :size="32" class="text-white" />
            </div>
            <h3 class="text-2xl font-bold text-slate-900 mb-4">Port Operations</h3>
            <p class="text-slate-600 mb-6 leading-relaxed">
              Streamline vessel scheduling, berth management, and cargo operations 
              with intelligent resource allocation.
            </p>
            <ul class="space-y-3 mb-6">
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Vessel Schedule Management</span>
              </li>
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Berth Allocation</span>
              </li>
              <li class="flex items-center gap-3 text-slate-700">
                <CheckCircle :size="16" class="text-green-600" />
                <span>Safety Compliance</span>
              </li>
            </ul>
            <button 
              @click="navigateTo('port-operation-management')"
              class="text-purple-600 hover:text-purple-700 font-medium flex items-center gap-2 group-hover:gap-3 transition-all"
            >
              Manage Operations
              <ArrowRight :size="16" />
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Capabilities Section -->
    <section id="capabilities" class="py-20 px-6">
      <div class="max-w-7xl mx-auto">
        <div class="grid lg:grid-cols-2 gap-16 items-center mb-20">
          <div>
            <div class="inline-flex items-center gap-2 bg-orange-50 text-orange-600 px-4 py-2 rounded-full mb-6">
              <Activity :size="16" />
              <span class="font-semibold">Event Streaming</span>
            </div>
            <h2 class="text-4xl font-bold text-slate-900 mb-6">
              Real-Time Activity Monitoring & Alerts
            </h2>
            <p class="text-lg text-slate-600 mb-8 leading-relaxed">
              Stay informed with live event streams of all port activities. Receive instant 
              notifications about critical events, track transaction histories, and maintain 
              complete audit trails for compliance and operational excellence.
            </p>
            <div class="space-y-4 mb-8">
              <div class="flex items-start gap-4">
                <div class="p-2 bg-orange-50 rounded-lg">
                  <Radio :size="20" class="text-orange-600" />
                </div>
                <div>
                  <h4 class="font-semibold text-slate-900 mb-1">Live Event Feed</h4>
                  <p class="text-slate-600">Real-time updates on all port activities and operations</p>
                </div>
              </div>
              <div class="flex items-start gap-4">
                <div class="p-2 bg-orange-50 rounded-lg">
                  <Clock :size="20" class="text-orange-600" />
                </div>
                <div>
                  <h4 class="font-semibold text-slate-900 mb-1">Historical Data Access</h4>
                  <p class="text-slate-600">Complete audit trails and comprehensive event history</p>
                </div>
              </div>
              <div class="flex items-start gap-4">
                <div class="p-2 bg-orange-50 rounded-lg">
                  <Globe :size="20" class="text-orange-600" />
                </div>
                <div>
                  <h4 class="font-semibold text-slate-900 mb-1">Global Visibility</h4>
                  <p class="text-slate-600">Monitor operations from anywhere in the world</p>
                </div>
              </div>
            </div>
            <button 
              @click="navigateTo('event-streaming')"
              class="bg-orange-600 hover:bg-orange-700 text-white font-medium py-3 px-6 rounded-lg transition-colors duration-200 flex items-center gap-2"
            >
              View Event Stream
              <ArrowRight :size="16" />
            </button>
          </div>
          <div class="bg-white rounded-2xl shadow-xl p-8">
            <div class="bg-gradient-to-br from-orange-50 to-orange-100 rounded-xl p-8 text-center">
              <Activity :size="64" class="text-orange-600 mx-auto mb-4" />
              <h3 class="text-lg font-semibold text-slate-900 mb-2">Live Event Monitoring</h3>
              <p class="text-slate-600 text-sm">Processing 1,200+ events per minute</p>
              <div class="mt-6 flex items-center justify-center gap-2">
                <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                <span class="text-sm text-green-700 font-medium">Connected</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Technology Stack -->
        <div class="bg-white rounded-2xl border border-slate-200 p-8 lg:p-12">
          <div class="text-center mb-12">
            <h2 class="text-3xl font-bold text-slate-900 mb-4">Built with Modern Technology</h2>
            <p class="text-lg text-slate-600">Powered by industry-leading tools and frameworks</p>
          </div>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-8">
            <div class="text-center">
              <div class="p-4 bg-blue-50 rounded-xl w-fit mx-auto mb-3">
                <Database :size="32" class="text-blue-600" />
              </div>
              <h4 class="font-semibold text-slate-900">Real-Time Data</h4>
              <p class="text-sm text-slate-600">Live processing</p>
            </div>
            <div class="text-center">
              <div class="p-4 bg-green-50 rounded-xl w-fit mx-auto mb-3">
                <Shield :size="32" class="text-green-600" />
              </div>
              <h4 class="font-semibold text-slate-900">Enterprise Security</h4>
              <p class="text-sm text-slate-600">Bank-level encryption</p>
            </div>
            <div class="text-center">
              <div class="p-4 bg-purple-50 rounded-xl w-fit mx-auto mb-3">
                <Cloud :size="32" class="text-purple-600" />
              </div>
              <h4 class="font-semibold text-slate-900">Cloud Native</h4>
              <p class="text-sm text-slate-600">Scalable architecture</p>
            </div>
            <div class="text-center">
              <div class="p-4 bg-orange-50 rounded-xl w-fit mx-auto mb-3">
                <Smartphone :size="32" class="text-orange-600" />
              </div>
              <h4 class="font-semibold text-slate-900">Mobile Ready</h4>
              <p class="text-sm text-slate-600">Access anywhere</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="py-20 px-6 bg-blue-600">
      <div class="max-w-4xl mx-auto text-center">
        <h2 class="text-4xl sm:text-5xl font-bold text-white mb-6">
          Ready to Transform Your Port Operations?
        </h2>
        <p class="text-xl text-blue-100 mb-10">
          Join leading ports worldwide who trust our platform for their critical operations
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <button 
            @click="navigateToDashboard"
            class="bg-white text-blue-600 hover:bg-blue-50 font-semibold py-4 px-8 rounded-lg text-lg transition-colors duration-200 flex items-center justify-center gap-2"
          >
            Start Free Trial
            <ArrowRight :size="20" />
          </button>
          <button 
            class="border-2 border-white text-white hover:bg-white hover:text-blue-600 font-semibold py-4 px-8 rounded-lg text-lg transition-colors duration-200 flex items-center justify-center gap-2"
          >
            <Phone :size="20" />
            Contact Sales
          </button>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="bg-slate-900 text-white py-12 px-6">
      <div class="max-w-7xl mx-auto">
        <div class="grid md:grid-cols-4 gap-8">
          <div>
            <div class="flex items-center gap-3 mb-4">
              <div class="p-2 bg-blue-600 rounded-lg">
                <Ship :size="24" class="text-white" />
              </div>
              <h3 class="text-xl font-bold">PortTrack</h3>
            </div>
            <p class="text-slate-400 mb-4">
              Modern container operations platform for efficient port management.
            </p>
          </div>
          <div>
            <h4 class="font-semibold mb-4">Features</h4>
            <ul class="space-y-2 text-slate-400">
              <li><a href="#" class="hover:text-white transition-colors">Dashboard</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Container Management</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Port Operations</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Event Streaming</a></li>
            </ul>
          </div>
          <div>
            <h4 class="font-semibold mb-4">Company</h4>
            <ul class="space-y-2 text-slate-400">
              <li><a href="#" class="hover:text-white transition-colors">About</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Careers</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Contact</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Support</a></li>
            </ul>
          </div>
          <div>
            <h4 class="font-semibold mb-4">Legal</h4>
            <ul class="space-y-2 text-slate-400">
              <li><a href="#" class="hover:text-white transition-colors">Privacy Policy</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Terms of Service</a></li>
              <li><a href="#" class="hover:text-white transition-colors">Security</a></li>
            </ul>
          </div>
        </div>
        <div class="border-t border-slate-800 mt-8 pt-8 text-center text-slate-400">
          <p>&copy; 2025 PortTrack. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { 
  Ship, 
  Anchor, 
  ArrowRight, 
  Play, 
  Container, 
  BarChart3, 
  CheckCircle, 
  Activity,
  Zap,
  Radio,
  Clock,
  Globe,
  Database,
  Shield,
  Cloud,
  Smartphone,
  Phone
} from 'lucide-vue-next'

const router = useRouter()

// Hero statistics data
const heroStats = ref([
  {
    value: '10K+',
    label: 'Containers Tracked',
    icon: Container,
    bgColor: 'bg-blue-50',
    iconColor: 'text-blue-600'
  },
  {
    value: '99.9%',
    label: 'System Uptime',
    icon: Activity,
    bgColor: 'bg-green-50',
    iconColor: 'text-green-600'
  },
  {
    value: '500+',
    label: 'Vessels Managed',
    icon: Ship,
    bgColor: 'bg-purple-50',
    iconColor: 'text-purple-600'
  },
  {
    value: '24/7',
    label: 'Live Monitoring',
    icon: Clock,
    bgColor: 'bg-orange-50',
    iconColor: 'text-orange-600'
  }
])

// Navigation methods
const navigateToDashboard = () => {
  router.push('/dashboard')
}

const navigateTo = (route: string) => {
  router.push(`/${route}`)
}
</script>

<style scoped>
@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideIn {
  animation: slideIn 0.6s ease-out forwards;
  opacity: 0;
}
</style>