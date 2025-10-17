# 🎨 UI/UX Guidelines

## Design System Overview

The Maersk Container Tracking system follows a comprehensive design system that ensures consistency, accessibility, and excellent user experience across all maritime operations interfaces.

## 🎯 Design Principles

### 1. **Maritime-First Design**
- Clean, professional interface suitable for port operations
- High contrast for outdoor/bright environments
- Weather-resistant color palette
- Industrial-grade usability standards

### 2. **Operational Efficiency**
- Minimal clicks to complete tasks
- Keyboard shortcuts for power users
- Bulk operations support
- Quick access to critical information

### 3. **Role-Based Experience**
- Adaptive interfaces based on user permissions
- Contextual information display
- Streamlined workflows per user type
- Progressive disclosure of advanced features

### 4. **Accessibility First**
- WCAG 2.1 AA compliance
- Screen reader optimization
- Keyboard navigation support
- High contrast mode availability

## 🎨 Brand Identity

### Color Palette

#### Primary Colors
```css
/* Maersk Brand Colors */
--maersk-blue: #00A3E0      /* Primary brand color */
--maersk-navy: #003F7F      /* Deep navy for headers */
--maersk-light: #E6F7FF     /* Light blue for backgrounds */

/* Functional Colors */
--primary: #3B82F6          /* Action buttons */
--secondary: #6B7280        /* Secondary elements */
--accent: #10B981           /* Success indicators */
```

#### Status Colors
```css
/* Container Status Colors */
--status-available: #10B981     /* Green - Available */
--status-transit: #F59E0B       /* Orange - In Transit */
--status-port: #3B82F6          /* Blue - At Port */
--status-maintenance: #EF4444   /* Red - Maintenance */
--status-assigned: #8B5CF6      /* Purple - Assigned */

/* Berth Status Colors */
--berth-free: #10B981           /* Green - Free */
--berth-occupied: #F59E0B       /* Orange - Occupied */
--berth-reserved: #3B82F6       /* Blue - Reserved */
--berth-maintenance: #EF4444    /* Red - Maintenance */
```

#### Neutral Colors
```css
/* Gray Scale */
--gray-50: #F9FAFB
--gray-100: #F3F4F6
--gray-200: #E5E7EB
--gray-300: #D1D5DB
--gray-400: #9CA3AF
--gray-500: #6B7280
--gray-600: #4B5563
--gray-700: #374151
--gray-800: #1F2937
--gray-900: #111827
```

### Typography

#### Font System
```css
/* Font Stack */
--font-primary: 'Inter', system-ui, -apple-system, sans-serif;
--font-mono: 'JetBrains Mono', 'Consolas', monospace;

/* Font Weights */
--font-light: 300;
--font-normal: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;

/* Font Sizes */
--text-xs: 0.75rem;     /* 12px */
--text-sm: 0.875rem;    /* 14px */
--text-base: 1rem;      /* 16px */
--text-lg: 1.125rem;    /* 18px */
--text-xl: 1.25rem;     /* 20px */
--text-2xl: 1.5rem;     /* 24px */
--text-3xl: 1.875rem;   /* 30px */
--text-4xl: 2.25rem;    /* 36px */
```

#### Typography Scale
```vue
<template>
  <!-- Headings -->
  <h1 class="text-3xl font-bold text-gray-900">Page Title</h1>
  <h2 class="text-2xl font-semibold text-gray-800">Section Header</h2>
  <h3 class="text-xl font-medium text-gray-700">Subsection</h3>
  <h4 class="text-lg font-medium text-gray-700">Component Title</h4>

  <!-- Body Text -->
  <p class="text-base text-gray-600">Regular body text</p>
  <p class="text-sm text-gray-500">Secondary text</p>
  <p class="text-xs text-gray-400">Caption text</p>

  <!-- Emphasis -->
  <strong class="font-semibold text-gray-900">Important text</strong>
  <em class="italic text-gray-700">Emphasized text</em>
  <code class="font-mono text-sm bg-gray-100 px-2 py-1 rounded">Code text</code>
</template>
```

## 🧩 Component Design Guidelines

### 1. **Button System**

#### Button Variants
```vue
<template>
  <!-- Primary Actions -->
  <AppButton variant="primary">Create Container</AppButton>
  
  <!-- Secondary Actions -->
  <AppButton variant="secondary">Cancel</AppButton>
  
  <!-- Destructive Actions -->
  <AppButton variant="danger">Delete Ship</AppButton>
  
  <!-- Ghost/Minimal Actions -->
  <AppButton variant="ghost">View Details</AppButton>
  
  <!-- Icon Buttons -->
  <AppButton variant="icon" icon="edit" aria-label="Edit container" />
</template>
```

#### Button States
```css
/* Button Base Styles */
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed;
}

/* Primary Button */
.btn-primary {
  @apply bg-primary-600 text-white hover:bg-primary-700 focus:ring-primary-500 active:bg-primary-800;
}

/* Loading State */
.btn-loading {
  @apply relative text-transparent pointer-events-none;
}

.btn-loading::after {
  @apply absolute inset-0 flex items-center justify-center;
  content: '';
  background: url('data:image/svg+xml,...') center/20px no-repeat;
}
```

### 2. **Input System**

#### Input Variants
```vue
<template>
  <!-- Standard Text Input -->
  <AppInput
    v-model="containerNumber"
    label="Container Number"
    placeholder="MAEU1234567"
    required
  />

  <!-- Number Input with Validation -->
  <AppInput
    v-model="weight"
    type="number"
    label="Weight (kg)"
    :min="0"
    :max="50000"
    step="0.1"
    suffix="kg"
  />

  <!-- Select Dropdown -->
  <AppSelect
    v-model="containerType"
    label="Container Type"
    :options="containerTypes"
    placeholder="Select type..."
  />

  <!-- Multi-select -->
  <AppMultiSelect
    v-model="selectedPorts"
    label="Ports"
    :options="availablePorts"
    searchable
  />
</template>
```

#### Input States
```css
/* Input Base Styles */
.input-base {
  @apply block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm transition-colors duration-200;
}

/* Input States */
.input-error {
  @apply border-red-300 text-red-900 placeholder-red-300 focus:border-red-500 focus:ring-red-500;
}

.input-success {
  @apply border-green-300 focus:border-green-500 focus:ring-green-500;
}

.input-disabled {
  @apply bg-gray-50 text-gray-500 cursor-not-allowed;
}
```

### 3. **Card System**

#### Card Variants
```vue
<template>
  <!-- Basic Card -->
  <div class="card">
    <div class="card-header">
      <h3 class="card-title">Container Details</h3>
    </div>
    <div class="card-body">
      <p>Card content goes here</p>
    </div>
    <div class="card-footer">
      <AppButton variant="primary">Action</AppButton>
    </div>
  </div>

  <!-- Status Card -->
  <div class="card card-status" :class="`card-${status}`">
    <StatusIndicator :status="status" />
    <div class="card-content">
      <h4>{{ title }}</h4>
      <p>{{ description }}</p>
    </div>
  </div>

  <!-- Interactive Card -->
  <div class="card card-interactive" @click="handleClick">
    <div class="card-hover-overlay">
      <Icon name="arrow-right" />
    </div>
    <div class="card-content">
      <!-- Card content -->
    </div>
  </div>
</template>
```

#### Card Styles
```css
/* Card Base */
.card {
  @apply bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden;
}

.card-header {
  @apply px-6 py-4 border-b border-gray-200 bg-gray-50;
}

.card-title {
  @apply text-lg font-medium text-gray-900;
}

.card-body {
  @apply px-6 py-4;
}

.card-footer {
  @apply px-6 py-4 bg-gray-50 border-t border-gray-200;
}

/* Interactive Card */
.card-interactive {
  @apply cursor-pointer transition-all duration-200 hover:shadow-md hover:-translate-y-1;
}

/* Status Cards */
.card-available { @apply border-l-4 border-l-green-500; }
.card-transit { @apply border-l-4 border-l-orange-500; }
.card-maintenance { @apply border-l-4 border-l-red-500; }
```

## 📊 Data Visualization

### 1. **Status Indicators**

```vue
<template>
  <!-- Badge Status -->
  <span :class="statusClass">{{ status }}</span>

  <!-- Dot Indicator -->
  <div class="flex items-center">
    <div :class="dotClass"></div>
    <span class="ml-2 text-sm text-gray-600">{{ status }}</span>
  </div>

  <!-- Progress Bar -->
  <div class="progress-bar">
    <div 
      class="progress-fill"
      :style="{ width: `${percentage}%` }"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  status: string
  percentage?: number
}>()

const statusClass = computed(() => ({
  'badge badge-green': props.status === 'Available',
  'badge badge-orange': props.status === 'In Transit',
  'badge badge-blue': props.status === 'At Port',
  'badge badge-red': props.status === 'Maintenance'
}))

const dotClass = computed(() => ({
  'status-dot bg-green-500': props.status === 'Available',
  'status-dot bg-orange-500': props.status === 'In Transit',
  'status-dot bg-blue-500': props.status === 'At Port',
  'status-dot bg-red-500': props.status === 'Maintenance'
}))
</script>

<style scoped>
.badge {
  @apply inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium;
}

.badge-green { @apply bg-green-100 text-green-800; }
.badge-orange { @apply bg-orange-100 text-orange-800; }
.badge-blue { @apply bg-blue-100 text-blue-800; }
.badge-red { @apply bg-red-100 text-red-800; }

.status-dot {
  @apply w-2 h-2 rounded-full;
}

.progress-bar {
  @apply w-full bg-gray-200 rounded-full h-2;
}

.progress-fill {
  @apply bg-blue-600 h-2 rounded-full transition-all duration-300;
}
</style>
```

### 2. **Data Tables**

```vue
<template>
  <div class="table-container">
    <table class="data-table">
      <thead class="table-header">
        <tr>
          <th class="table-header-cell sortable" @click="sort('containerNumber')">
            Container Number
            <SortIcon :direction="getSortDirection('containerNumber')" />
          </th>
          <th class="table-header-cell">Type</th>
          <th class="table-header-cell">Status</th>
          <th class="table-header-cell text-right">Weight</th>
          <th class="table-header-cell">Actions</th>
        </tr>
      </thead>
      <tbody class="table-body">
        <tr 
          v-for="container in containers" 
          :key="container.id"
          class="table-row"
        >
          <td class="table-cell font-mono">{{ container.containerNumber }}</td>
          <td class="table-cell">{{ container.type }}</td>
          <td class="table-cell">
            <StatusBadge :status="container.status" />
          </td>
          <td class="table-cell text-right">{{ formatWeight(container.weight) }}</td>
          <td class="table-cell">
            <div class="flex space-x-2">
              <AppButton variant="ghost" size="sm" icon="edit" />
              <AppButton variant="ghost" size="sm" icon="trash" />
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.table-container {
  @apply overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg;
}

.data-table {
  @apply min-w-full divide-y divide-gray-300;
}

.table-header {
  @apply bg-gray-50;
}

.table-header-cell {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider;
}

.sortable {
  @apply cursor-pointer hover:bg-gray-100 select-none;
}

.table-body {
  @apply bg-white divide-y divide-gray-200;
}

.table-row {
  @apply hover:bg-gray-50 transition-colors duration-150;
}

.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm text-gray-900;
}
</style>
```

## 📱 Responsive Design

### Breakpoint System
```css
/* Tailwind CSS Breakpoints */
/* sm: 640px */
/* md: 768px */  
/* lg: 1024px */
/* xl: 1280px */
/* 2xl: 1536px */

/* Mobile First Approach */
.responsive-grid {
  @apply grid grid-cols-1 gap-4;
  
  /* Tablet */
  @apply md:grid-cols-2 md:gap-6;
  
  /* Desktop */
  @apply lg:grid-cols-3 lg:gap-8;
  
  /* Large Desktop */
  @apply xl:grid-cols-4;
}
```

### Mobile Navigation
```vue
<template>
  <!-- Mobile Menu Button -->
  <div class="md:hidden">
    <AppButton 
      variant="ghost"
      icon="menu"
      @click="toggleMobileMenu"
      aria-label="Toggle menu"
    />
  </div>

  <!-- Mobile Menu Overlay -->
  <Transition name="slide-over">
    <div v-if="showMobileMenu" class="mobile-menu-overlay">
      <div class="mobile-menu">
        <div class="mobile-menu-header">
          <h2>Navigation</h2>
          <AppButton 
            variant="ghost"
            icon="x"
            @click="closeMobileMenu"
          />
        </div>
        
        <nav class="mobile-nav">
          <router-link
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            class="mobile-nav-item"
            @click="closeMobileMenu"
          >
            <Icon :name="item.icon" />
            {{ item.label }}
          </router-link>
        </nav>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.mobile-menu-overlay {
  @apply fixed inset-0 z-50 lg:hidden;
  @apply bg-gray-600 bg-opacity-75;
}

.mobile-menu {
  @apply fixed inset-y-0 left-0 flex flex-col w-full max-w-xs bg-white;
}

.mobile-menu-header {
  @apply flex items-center justify-between px-4 py-3 border-b border-gray-200;
}

.mobile-nav {
  @apply flex-1 px-4 py-4 space-y-2;
}

.mobile-nav-item {
  @apply flex items-center px-3 py-2 text-sm font-medium text-gray-700 rounded-md hover:bg-gray-100;
}

/* Slide Over Animation */
.slide-over-enter-active,
.slide-over-leave-active {
  transition: transform 0.3s ease-in-out;
}

.slide-over-enter-from,
.slide-over-leave-to {
  transform: translateX(-100%);
}
</style>
```

## ♿ Accessibility Guidelines

### 1. **Keyboard Navigation**
```css
/* Focus Styles */
.focus-visible {
  @apply focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2;
}

/* Skip Links */
.skip-link {
  @apply sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-primary-600 text-white px-4 py-2 rounded-md z-50;
}
```

### 2. **Screen Reader Support**
```vue
<template>
  <!-- Semantic HTML -->
  <main role="main" aria-label="Container management">
    <h1>Container Management</h1>
    
    <!-- Screen reader only content -->
    <div class="sr-only">
      This page allows you to manage shipping containers
    </div>
    
    <!-- ARIA labels -->
    <button 
      aria-label="Create new container"
      aria-describedby="create-help"
    >
      <Icon name="plus" aria-hidden="true" />
      Create
    </button>
    
    <div id="create-help" class="sr-only">
      Opens a form to create a new container entry
    </div>
    
    <!-- Live regions for dynamic content -->
    <div 
      aria-live="polite" 
      aria-atomic="true"
      class="sr-only"
    >
      {{ statusMessage }}
    </div>
  </main>
</template>

<style>
/* Screen reader only utility */
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}

.sr-only.focus:not(.sr-only) {
  position: static;
  width: auto;
  height: auto;
  padding: inherit;
  margin: inherit;
  overflow: visible;
  clip: auto;
  white-space: normal;
}
</style>
```

### 3. **High Contrast Mode**
```css
/* High contrast mode support */
@media (prefers-contrast: high) {
  .btn-primary {
    @apply bg-black text-white border-2 border-white;
  }
  
  .card {
    @apply border-2 border-black;
  }
  
  .table-row:hover {
    @apply bg-yellow-200;
  }
}
```

## 🎭 Animation & Interactions

### 1. **Micro-Interactions**
```css
/* Subtle animations for better UX */
.animate-fade-in {
  animation: fadeIn 0.3s ease-out;
}

.animate-slide-up {
  animation: slideUp 0.3s ease-out;
}

.animate-bounce-in {
  animation: bounceIn 0.4s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { transform: translateY(20px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

@keyframes bounceIn {
  0% { transform: scale(0.8); opacity: 0; }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); opacity: 1; }
}

/* Hover effects */
.hover-lift {
  @apply transition-transform duration-200 hover:-translate-y-1;
}

.hover-glow {
  @apply transition-shadow duration-200 hover:shadow-lg;
}
```

### 2. **Loading States**
```vue
<template>
  <!-- Skeleton Loading -->
  <div v-if="loading" class="animate-pulse">
    <div class="skeleton-line h-4 mb-2"></div>
    <div class="skeleton-line h-4 mb-2 w-3/4"></div>
    <div class="skeleton-line h-4 w-1/2"></div>
  </div>

  <!-- Spinner Loading -->
  <div v-if="loading" class="flex justify-center">
    <div class="spinner"></div>
  </div>

  <!-- Progress Loading -->
  <div v-if="loading" class="progress-container">
    <div class="progress-bar indeterminate"></div>
  </div>
</template>

<style scoped>
.skeleton-line {
  @apply bg-gray-300 rounded;
}

.spinner {
  @apply w-6 h-6 border-2 border-gray-200 border-t-primary-600 rounded-full;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.progress-container {
  @apply w-full bg-gray-200 rounded-full h-2;
}

.progress-bar.indeterminate {
  @apply h-2 bg-primary-600 rounded-full;
  animation: indeterminate 2s ease-in-out infinite;
  transform-origin: left;
}

@keyframes indeterminate {
  0% { transform: scaleX(0); }
  40% { transform: scaleX(0.4); }
  100% { transform: scaleX(1); }
}
</style>
```

## 🔍 Icon System

### Icon Usage Guidelines
```vue
<template>
  <!-- Functional Icons -->
  <Icon name="container" class="w-5 h-5 text-blue-600" />
  <Icon name="ship" class="w-6 h-6 text-gray-700" />
  <Icon name="port" class="w-4 h-4 text-green-600" />

  <!-- Status Icons -->
  <Icon name="check-circle" class="w-5 h-5 text-green-500" />
  <Icon name="exclamation-circle" class="w-5 h-5 text-yellow-500" />
  <Icon name="x-circle" class="w-5 h-5 text-red-500" />

  <!-- Navigation Icons -->
  <Icon name="home" class="w-5 h-5" />
  <Icon name="cog" class="w-5 h-5" />
  <Icon name="user" class="w-5 h-5" />

  <!-- Action Icons -->
  <Icon name="plus" class="w-4 h-4" />
  <Icon name="pencil" class="w-4 h-4" />
  <Icon name="trash" class="w-4 h-4" />
</template>
```

### Icon Sizes & States
```css
/* Icon Size Classes */
.icon-xs { @apply w-3 h-3; }      /* 12px */
.icon-sm { @apply w-4 h-4; }      /* 16px */
.icon-md { @apply w-5 h-5; }      /* 20px */
.icon-lg { @apply w-6 h-6; }      /* 24px */
.icon-xl { @apply w-8 h-8; }      /* 32px */

/* Icon States */
.icon-interactive {
  @apply transition-colors duration-200 cursor-pointer hover:text-primary-600;
}

.icon-disabled {
  @apply text-gray-300 cursor-not-allowed;
}
```

## 📋 Form Design Patterns

### Form Layout
```vue
<template>
  <form class="form-container">
    <!-- Form Header -->
    <div class="form-header">
      <h2 class="form-title">Create Container</h2>
      <p class="form-description">
        Add a new container to the tracking system
      </p>
    </div>

    <!-- Form Body -->
    <div class="form-body">
      <!-- Two Column Layout -->
      <div class="form-grid">
        <div class="form-group">
          <AppInput
            v-model="formData.containerNumber"
            label="Container Number"
            required
          />
        </div>
        
        <div class="form-group">
          <AppSelect
            v-model="formData.type"
            label="Container Type"
            :options="containerTypes"
            required
          />
        </div>
      </div>

      <!-- Full Width Field -->
      <div class="form-group">
        <AppTextarea
          v-model="formData.notes"
          label="Notes"
          rows="3"
        />
      </div>
    </div>

    <!-- Form Footer -->
    <div class="form-footer">
      <AppButton variant="secondary" @click="cancel">
        Cancel
      </AppButton>
      <AppButton 
        type="submit" 
        variant="primary"
        :loading="isSubmitting"
      >
        Create Container
      </AppButton>
    </div>
  </form>
</template>

<style scoped>
.form-container {
  @apply bg-white rounded-lg shadow-sm border border-gray-200 max-w-2xl mx-auto;
}

.form-header {
  @apply px-6 py-4 border-b border-gray-200;
}

.form-title {
  @apply text-lg font-medium text-gray-900;
}

.form-description {
  @apply mt-1 text-sm text-gray-600;
}

.form-body {
  @apply px-6 py-6 space-y-6;
}

.form-grid {
  @apply grid grid-cols-1 md:grid-cols-2 gap-6;
}

.form-group {
  @apply space-y-1;
}

.form-footer {
  @apply px-6 py-4 bg-gray-50 border-t border-gray-200 flex justify-end space-x-3;
}
</style>
```

This comprehensive UI/UX guide ensures consistent, accessible, and user-friendly interfaces throughout the Maersk Container Tracking system, optimized for maritime operations environments and workflows.