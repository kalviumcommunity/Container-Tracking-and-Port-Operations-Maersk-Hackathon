# 🧩 Component Library Documentation

## Overview

The Maersk Container Tracking frontend features a comprehensive component library built with Vue 3 Composition API, TypeScript, and Tailwind CSS. This documentation covers all reusable components, their props, events, and usage examples.

## 🏗️ Component Architecture

### Design Principles
- **Composition API**: All components use Vue 3 Composition API
- **TypeScript**: Full type safety with interface definitions
- **Accessibility**: WCAG 2.1 AA compliant components
- **Responsive**: Mobile-first responsive design
- **Themeable**: Tailwind CSS with custom design tokens

### Component Categories
```
components/
├── common/          # Reusable UI components
├── forms/          # Form-specific components
├── layout/         # Layout and navigation
├── main/           # Main application sections
└── icons/          # Custom icon components
```

## 🎨 Common Components

### 1. **Button Component** (`components/common/AppButton.vue`)

#### Props Interface:
```typescript
interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost'
  size?: 'sm' | 'md' | 'lg' | 'xl'
  disabled?: boolean
  loading?: boolean
  type?: 'button' | 'submit' | 'reset'
  fullWidth?: boolean
  icon?: string
  iconPosition?: 'left' | 'right'
}
```

#### Usage Examples:
```vue
<template>
  <!-- Basic button -->
  <AppButton>Click me</AppButton>

  <!-- Primary button with icon -->
  <AppButton 
    variant="primary" 
    icon="plus"
    @click="handleClick"
  >
    Add Container
  </AppButton>

  <!-- Loading state -->
  <AppButton 
    :loading="isSubmitting"
    type="submit"
    variant="primary"
  >
    {{ isSubmitting ? 'Saving...' : 'Save' }}
  </AppButton>

  <!-- Disabled button -->
  <AppButton 
    disabled
    variant="secondary"
  >
    Not Available
  </AppButton>
</template>
```

#### CSS Classes:
```css
/* Base styles applied via Tailwind */
.btn-base {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors duration-200;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-900 hover:bg-gray-300 focus:ring-gray-500;
}
```

### 2. **Input Component** (`components/common/AppInput.vue`)

#### Props Interface:
```typescript
interface InputProps {
  modelValue: string | number
  label?: string
  type?: 'text' | 'email' | 'password' | 'number' | 'tel'
  placeholder?: string
  required?: boolean
  disabled?: boolean
  readonly?: boolean
  error?: string
  hint?: string
  icon?: string
  maxlength?: number
  autocomplete?: string
}
```

#### Usage Examples:
```vue
<template>
  <!-- Basic input -->
  <AppInput 
    v-model="containerNumber"
    label="Container Number"
    placeholder="Enter container number"
  />

  <!-- Input with validation -->
  <AppInput 
    v-model="email"
    type="email"
    label="Email Address"
    :required="true"
    :error="emailError"
    hint="We'll use this for notifications"
  />

  <!-- Number input with icon -->
  <AppInput 
    v-model="weight"
    type="number"
    label="Weight (kg)"
    icon="scale"
    placeholder="0.00"
  />
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const containerNumber = ref('')
const email = ref('')
const weight = ref(0)

const emailError = computed(() => {
  if (!email.value) return ''
  const isValid = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)
  return isValid ? '' : 'Please enter a valid email address'
})
</script>
```

### 3. **Modal Component** (`components/common/AppModal.vue`)

#### Props Interface:
```typescript
interface ModalProps {
  show: boolean
  title?: string
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full'
  closable?: boolean
  persistent?: boolean
  maxWidth?: string
}
```

#### Usage Examples:
```vue
<template>
  <!-- Basic modal -->
  <AppModal 
    :show="showModal"
    title="Create New Container"
    @close="showModal = false"
  >
    <template #body>
      <p>Modal content goes here</p>
    </template>
    
    <template #footer>
      <div class="flex justify-end gap-3">
        <AppButton 
          variant="secondary"
          @click="showModal = false"
        >
          Cancel
        </AppButton>
        <AppButton 
          variant="primary"
          @click="saveContainer"
        >
          Save
        </AppButton>
      </div>
    </template>
  </AppModal>

  <!-- Confirmation modal -->
  <AppModal 
    :show="showConfirmation"
    title="Confirm Delete"
    size="sm"
    @close="showConfirmation = false"
  >
    <template #body>
      <p>Are you sure you want to delete this container?</p>
    </template>
    
    <template #footer>
      <div class="flex justify-end gap-3">
        <AppButton 
          variant="secondary"
          @click="showConfirmation = false"
        >
          Cancel
        </AppButton>
        <AppButton 
          variant="danger"
          @click="confirmDelete"
        >
          Delete
        </AppButton>
      </div>
    </template>
  </AppModal>
</template>
```

### 4. **Data Table Component** (`components/common/DataTable.vue`)

#### Props Interface:
```typescript
interface Column {
  key: string
  label: string
  sortable?: boolean
  width?: string
  align?: 'left' | 'center' | 'right'
  render?: (value: any, row: any) => string
}

interface DataTableProps {
  data: any[]
  columns: Column[]
  loading?: boolean
  pagination?: {
    page: number
    size: number
    total: number
  }
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
  selectable?: boolean
  selectedRows?: any[]
}
```

#### Usage Examples:
```vue
<template>
  <DataTable
    :data="containers"
    :columns="containerColumns"
    :loading="isLoading"
    :pagination="pagination"
    @sort="handleSort"
    @page-change="handlePageChange"
    @row-select="handleRowSelect"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import DataTable from '@/components/common/DataTable.vue'

const containers = ref([])
const isLoading = ref(false)

const containerColumns = [
  { 
    key: 'containerNumber', 
    label: 'Container #', 
    sortable: true 
  },
  { 
    key: 'type', 
    label: 'Type', 
    sortable: true 
  },
  { 
    key: 'status', 
    label: 'Status',
    render: (value) => `<span class="badge badge-${value.toLowerCase()}">${value}</span>`
  },
  { 
    key: 'weight', 
    label: 'Weight (kg)', 
    align: 'right',
    render: (value) => value?.toLocaleString() || 'N/A'
  }
]

const pagination = ref({
  page: 1,
  size: 10,
  total: 0
})
</script>
```

## 📝 Form Components

### 1. **Container Form** (`components/forms/ContainerForm.vue`)

#### Props Interface:
```typescript
interface ContainerFormProps {
  container?: Container
  mode: 'create' | 'edit'
  loading?: boolean
}

interface ContainerFormData {
  containerNumber: string
  type: string
  size: string
  weight: number
  cargo: string
  status: string
  location: string
  portId?: number
}
```

#### Usage Examples:
```vue
<template>
  <!-- Create new container -->
  <ContainerForm
    mode="create"
    :loading="isCreating"
    @submit="handleCreateContainer"
    @cancel="closeForm"
  />

  <!-- Edit existing container -->
  <ContainerForm
    mode="edit"
    :container="selectedContainer"
    :loading="isUpdating"
    @submit="handleUpdateContainer"
    @cancel="closeForm"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import ContainerForm from '@/components/forms/ContainerForm.vue'
import type { Container } from '@/types/container'

const selectedContainer = ref<Container>()
const isCreating = ref(false)
const isUpdating = ref(false)

const handleCreateContainer = async (data: ContainerFormData) => {
  isCreating.value = true
  try {
    await containerService.create(data)
    // Handle success
  } catch (error) {
    // Handle error
  } finally {
    isCreating.value = false
  }
}
</script>
```

### 2. **Ship Form** (`components/forms/ShipForm.vue`)

#### Props Interface:
```typescript
interface ShipFormProps {
  ship?: Ship
  mode: 'create' | 'edit'
  loading?: boolean
}

interface ShipFormData {
  name: string
  imoNumber: string
  flag: string
  type: string
  capacity: number
  length?: number
  width?: number
  status: string
}
```

### 3. **Berth Assignment Form** (`components/forms/BerthAssignmentForm.vue`)

#### Usage Examples:
```vue
<template>
  <BerthAssignmentForm
    :ships="availableShips"
    :berths="availableBerths"
    :loading="isAssigning"
    @submit="handleBerthAssignment"
  />
</template>

<script setup lang="ts">
interface BerthAssignmentData {
  shipId: number
  berthId: number
  scheduledArrival: string
  scheduledDeparture: string
  notes?: string
}

const handleBerthAssignment = async (data: BerthAssignmentData) => {
  // Handle berth assignment logic
}
</script>
```

## 🎯 Layout Components

### 1. **Sidebar Navigation** (`components/layout/Sidebar.vue`)

#### Features:
- Collapsible sidebar with icons
- Role-based navigation items
- Active route highlighting
- Mobile-responsive drawer

#### Usage:
```vue
<template>
  <div class="flex h-screen bg-gray-100">
    <Sidebar 
      :collapsed="sidebarCollapsed"
      :navigation-items="navigationItems"
      @toggle="toggleSidebar"
    />
    
    <main class="flex-1 overflow-auto">
      <router-view />
    </main>
  </div>
</template>
```

### 2. **Header Component** (`components/layout/Header.vue`)

#### Features:
- User profile dropdown
- Notifications
- Search functionality
- Theme toggle (if enabled)

### 3. **Breadcrumb Component** (`components/layout/Breadcrumb.vue`)

#### Usage:
```vue
<template>
  <Breadcrumb :items="breadcrumbItems" />
</template>

<script setup lang="ts">
const breadcrumbItems = ref([
  { label: 'Dashboard', to: '/' },
  { label: 'Containers', to: '/containers' },
  { label: 'Container Details', to: `/containers/${id}` }
])
</script>
```

## 📊 Main Application Components

### 1. **Dashboard Cards** (`components/main/DashboardCard.vue`)

#### Props Interface:
```typescript
interface DashboardCardProps {
  title: string
  value: string | number
  subtitle?: string
  icon?: string
  trend?: {
    value: number
    direction: 'up' | 'down' | 'neutral'
    period: string
  }
  color?: 'blue' | 'green' | 'yellow' | 'red'
}
```

#### Usage:
```vue
<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
    <DashboardCard
      title="Total Containers"
      :value="stats.totalContainers"
      icon="container"
      :trend="{
        value: 12,
        direction: 'up',
        period: 'vs last month'
      }"
      color="blue"
    />
    
    <DashboardCard
      title="Available Berths"
      :value="stats.availableBerths"
      :subtitle="`of ${stats.totalBerths} total`"
      icon="anchor"
      color="green"
    />
  </div>
</template>
```

### 2. **Berth Card Component** (`components/main/BerthCard.vue`)

#### Features:
- Visual berth status representation
- Ship assignment details
- Quick action buttons
- Real-time status updates

### 3. **Container Status Badge** (`components/main/StatusBadge.vue`)

#### Usage:
```vue
<template>
  <StatusBadge 
    :status="container.status"
    :variant="getStatusVariant(container.status)"
  />
</template>

<script setup lang="ts">
const getStatusVariant = (status: string) => {
  const variants = {
    'Available': 'success',
    'In Transit': 'warning', 
    'At Port': 'info',
    'Maintenance': 'danger'
  }
  return variants[status] || 'default'
}
</script>
```

## 🎨 Icon Components

### Custom Icon System (`components/icons/`)

#### Available Icons:
```typescript
// Icon types
type IconName = 
  | 'container' | 'ship' | 'anchor' | 'port' 
  | 'user' | 'settings' | 'dashboard' | 'analytics'
  | 'plus' | 'edit' | 'delete' | 'search'
  | 'arrow-up' | 'arrow-down' | 'chevron-left' | 'chevron-right'
```

#### Usage:
```vue
<template>
  <!-- Using Lucide icons -->
  <Container class="w-5 h-5 text-blue-600" />
  <Ship class="w-6 h-6 text-gray-700" />
  
  <!-- Dynamic icon -->
  <component 
    :is="iconComponents[iconName]"
    class="w-4 h-4"
  />
</template>

<script setup lang="ts">
import { Container, Ship, Anchor } from 'lucide-vue-next'

const iconComponents = {
  container: Container,
  ship: Ship,
  anchor: Anchor
}
</script>
```

## 🎭 Animation Components

### 1. **Loading Spinner** (`components/common/LoadingSpinner.vue`)

#### Variants:
```vue
<template>
  <!-- Small spinner -->
  <LoadingSpinner size="sm" />
  
  <!-- Page loading -->
  <LoadingSpinner 
    size="lg" 
    message="Loading containers..."
  />
  
  <!-- Inline with text -->
  <LoadingSpinner 
    inline 
    message="Saving..."
  />
</template>
```

### 2. **Fade Transition** (`components/common/FadeTransition.vue`)

#### Usage:
```vue
<template>
  <FadeTransition>
    <div v-if="showContent" key="content">
      Content that fades in/out
    </div>
  </FadeTransition>
</template>
```

## 🔧 Utility Components

### 1. **Copy to Clipboard** (`components/common/CopyToClipboard.vue`)

#### Usage:
```vue
<template>
  <CopyToClipboard 
    :text="container.containerNumber"
    success-message="Container number copied!"
  />
</template>
```

### 2. **Tooltip Component** (`components/common/Tooltip.vue`)

#### Usage:
```vue
<template>
  <Tooltip content="This is a helpful tooltip">
    <AppButton>Hover me</AppButton>
  </Tooltip>
</template>
```

## 📱 Responsive Design

### Breakpoint System:
```css
/* Tailwind CSS breakpoints */
sm: 640px   /* Small devices */
md: 768px   /* Medium devices */
lg: 1024px  /* Large devices */
xl: 1280px  /* Extra large devices */
2xl: 1536px /* 2X large devices */
```

### Responsive Patterns:
```vue
<template>
  <!-- Mobile-first grid -->
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
    <div v-for="item in items" :key="item.id">
      <!-- Card content -->
    </div>
  </div>

  <!-- Responsive text sizing -->
  <h1 class="text-2xl md:text-3xl lg:text-4xl font-bold">
    Dashboard
  </h1>

  <!-- Conditional rendering -->
  <div class="block md:hidden">
    <!-- Mobile menu -->
  </div>
  <div class="hidden md:block">
    <!-- Desktop navigation -->
  </div>
</template>
```

## ♿ Accessibility Features

### ARIA Labels and Roles:
```vue
<template>
  <!-- Semantic HTML -->
  <nav aria-label="Main navigation">
    <ul role="menu">
      <li role="menuitem">
        <a href="/dashboard" aria-current="page">
          Dashboard
        </a>
      </li>
    </ul>
  </nav>

  <!-- Form accessibility -->
  <label for="container-number" class="sr-only">
    Container Number
  </label>
  <input
    id="container-number"
    aria-describedby="container-help"
    aria-required="true"
  />
  <div id="container-help" class="text-sm text-gray-600">
    Enter the unique container identifier
  </div>
</template>
```

### Focus Management:
```css
/* Focus styles */
.focus-visible {
  @apply focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2;
}

/* Skip links */
.skip-link {
  @apply sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-blue-600 text-white px-4 py-2 rounded;
}
```

## 🧪 Component Testing

### Unit Test Example:
```typescript
// ContainerForm.spec.ts
import { mount } from '@vue/test-utils'
import ContainerForm from '@/components/forms/ContainerForm.vue'

describe('ContainerForm', () => {
  it('renders form fields correctly', () => {
    const wrapper = mount(ContainerForm, {
      props: {
        mode: 'create'
      }
    })

    expect(wrapper.find('[data-testid="container-number"]').exists()).toBe(true)
    expect(wrapper.find('[data-testid="container-type"]').exists()).toBe(true)
  })

  it('emits submit event with form data', async () => {
    const wrapper = mount(ContainerForm, {
      props: { mode: 'create' }
    })

    await wrapper.find('[data-testid="container-number"]').setValue('TEST123')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('submit')).toBeTruthy()
  })
})
```

## 📚 Best Practices

### 1. **Component Naming**
- Use PascalCase for component names
- Prefix reusable components with 'App'
- Use descriptive, domain-specific names

### 2. **Props Validation**
```typescript
// Always define prop interfaces
interface ComponentProps {
  requiredProp: string
  optionalProp?: number
  enumProp: 'option1' | 'option2' | 'option3'
}

// Use default values
const props = withDefaults(defineProps<ComponentProps>(), {
  optionalProp: 0,
  enumProp: 'option1'
})
```

### 3. **Event Naming**
```typescript
// Use kebab-case for custom events
const emit = defineEmits<{
  'item-selected': [item: Item]
  'form-submit': [data: FormData]
  'modal-close': []
}>()
```

### 4. **Slot Usage**
```vue
<template>
  <div class="card">
    <header v-if="$slots.header" class="card-header">
      <slot name="header" />
    </header>
    
    <main class="card-body">
      <slot />
    </main>
    
    <footer v-if="$slots.footer" class="card-footer">
      <slot name="footer" />
    </footer>
  </div>
</template>
```

This component library provides a solid foundation for building consistent, accessible, and maintainable user interfaces for the Maersk Container Tracking system.