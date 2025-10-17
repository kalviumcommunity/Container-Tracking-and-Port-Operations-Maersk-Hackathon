# Component Development Guide

This guide establishes standards and best practices for developing Vue 3 components in the **PortTrack Container Operations** frontend application.

## 🧩 Component Architecture

### Component Types

#### 1. **Page Components** (Views)
- Located in `src/components/`
- Represent full page views
- Handle routing and high-level state
- Examples: `Dashboard.vue`, `ContainerManagement.vue`, `AdminDashboard.vue`

#### 2. **Form Components**
- Located in `src/forms/`
- Handle user input and validation
- Reusable across different views
- Examples: Login forms, container creation forms

#### 3. **UI Components** (Shared)
- Reusable UI elements
- Should be generic and configurable
- Examples: `Navbar.vue`, buttons, modals, cards

## 📝 Vue 3 Composition API Standards

### Component Structure Template
```vue
<template>
  <div class="component-name">
    <!-- Component content -->
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import type { ComponentProps, ComponentEmits } from './types'

// Props definition
interface Props {
  title: string
  data?: Array<any>
  isLoading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  data: () => [],
  isLoading: false
})

// Emits definition
interface Emits {
  update: [value: any]
  delete: [id: string]
}

const emit = defineEmits<Emits>()

// Reactive state
const localData = ref<any[]>([])
const isProcessing = ref(false)

// Computed properties
const filteredData = computed(() => {
  return localData.value.filter(item => item.active)
})

// Methods
const handleUpdate = (value: any) => {
  emit('update', value)
}

const processData = async () => {
  isProcessing.value = true
  try {
    // Process data logic
  } finally {
    isProcessing.value = false
  }
}

// Lifecycle hooks
onMounted(() => {
  // Component initialization
})

// Watchers
watch(() => props.data, (newData) => {
  localData.value = [...newData]
}, { immediate: true })
</script>

<style scoped>
/* Component-specific styles using Tailwind CSS */
.component-name {
  @apply flex flex-col space-y-4;
}
</style>
```

## 🎨 Styling Guidelines

### Tailwind CSS Best Practices

#### 1. **Use Utility Classes**
```vue
<template>
  <!-- Good: Utility classes -->
  <div class="bg-blue-500 text-white px-4 py-2 rounded-lg shadow-md">
    Content
  </div>
  
  <!-- Avoid: Custom CSS when utilities exist -->
  <div class="custom-button">
    Content
  </div>
</template>
```

#### 2. **Responsive Design**
```vue
<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
    <!-- Responsive grid -->
  </div>
</template>
```

#### 3. **Component Variants**
```vue
<template>
  <button :class="buttonClasses">
    <slot />
  </button>
</template>

<script setup lang="ts">
interface Props {
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md'
})

const buttonClasses = computed(() => {
  const base = 'font-medium rounded-lg transition-colors'
  
  const variants = {
    primary: 'bg-blue-600 hover:bg-blue-700 text-white',
    secondary: 'bg-gray-200 hover:bg-gray-300 text-gray-900',
    danger: 'bg-red-600 hover:bg-red-700 text-white'
  }
  
  const sizes = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2',
    lg: 'px-6 py-3 text-lg'
  }
  
  return `${base} ${variants[props.variant]} ${sizes[props.size]}`
})
</script>
```

## 🔧 State Management Integration

### Using Pinia Stores

#### 1. **Store Composition**
```vue
<script setup lang="ts">
import { useContainerStore } from '@/stores/containerStore'
import { useAuthStore } from '@/stores/authStore'

const containerStore = useContainerStore()
const authStore = useAuthStore()

// Access state
const containers = computed(() => containerStore.containers)
const isAuthenticated = computed(() => authStore.isAuthenticated)

// Call actions
const loadContainers = () => {
  containerStore.fetchContainers()
}
</script>
```

#### 2. **Local vs Global State**
```vue
<script setup lang="ts">
// Local component state
const isModalOpen = ref(false)
const formData = ref({})

// Global state from store
const { user, permissions } = storeToRefs(authStore)
</script>
```

## 🔗 API Integration

### Service Integration Pattern
```vue
<script setup lang="ts">
import { containerService } from '@/services/containerService'
import { useAsyncState } from '@vueuse/core'

// Async data fetching
const { data: containers, isLoading, error, execute: refetch } = useAsyncState(
  () => containerService.getContainers(),
  [],
  { immediate: true }
)

// Handle API calls
const updateContainer = async (id: string, data: any) => {
  try {
    await containerService.updateContainer(id, data)
    await refetch() // Refresh data
  } catch (error) {
    console.error('Update failed:', error)
  }
}
</script>
```

## 🧪 Component Testing

### Testing Template
```typescript
// Component.test.ts
import { describe, it, expect, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import Component from './Component.vue'

describe('Component', () => {
  let wrapper: any

  beforeEach(() => {
    wrapper = mount(Component, {
      props: {
        title: 'Test Title'
      }
    })
  })

  it('renders correctly', () => {
    expect(wrapper.find('h1').text()).toBe('Test Title')
  })

  it('emits update event', async () => {
    await wrapper.find('button').trigger('click')
    expect(wrapper.emitted()).toHaveProperty('update')
  })
})
```

## 📋 Component Checklist

### Before Creating a Component

- [ ] **Single Responsibility**: Component has one clear purpose
- [ ] **Reusability**: Consider if component can be reused
- [ ] **Props Interface**: Define clear TypeScript interfaces
- [ ] **Event Emissions**: Document all emitted events
- [ ] **Accessibility**: Include ARIA attributes where needed

### Development Standards

- [ ] **TypeScript**: Use TypeScript for props and emits
- [ ] **Composition API**: Use `<script setup>` syntax
- [ ] **Reactive Data**: Use `ref()` and `reactive()` appropriately
- [ ] **Computed Properties**: Use for derived state
- [ ] **Scoped Styles**: Use scoped CSS with Tailwind utilities

### Testing Requirements

- [ ] **Unit Tests**: Test component behavior and events
- [ ] **Props Testing**: Verify prop validation and defaults
- [ ] **Accessibility**: Test keyboard navigation and screen readers
- [ ] **Error Handling**: Test error states and edge cases

## 🎯 Role-Based Components

### Role Access Control
```vue
<template>
  <div v-if="hasPermission">
    <!-- Admin only content -->
    <AdminPanel v-if="isAdmin" />
    
    <!-- Port Manager content -->
    <PortManagement v-if="isPortManager || isAdmin" />
    
    <!-- Operator content -->
    <OperatorDashboard v-if="isOperator || isPortManager || isAdmin" />
    
    <!-- Viewer content (available to all) -->
    <ViewerContent />
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'

const authStore = useAuthStore()

const isAdmin = computed(() => authStore.hasRole('Admin'))
const isPortManager = computed(() => authStore.hasRole('PortManager'))
const isOperator = computed(() => authStore.hasRole('Operator'))
const hasPermission = computed(() => authStore.isAuthenticated)
</script>
```

## 🚀 Performance Best Practices

### 1. **Lazy Loading**
```vue
<script setup lang="ts">
import { defineAsyncComponent } from 'vue'

// Lazy load heavy components
const HeavyComponent = defineAsyncComponent(() => import('./HeavyComponent.vue'))
</script>
```

### 2. **Memo and Optimization**
```vue
<script setup lang="ts">
import { shallowRef, markRaw } from 'vue'

// Use shallowRef for large objects
const largeData = shallowRef(markRaw({}))

// Optimize watchers
watch(
  () => props.data,
  (newData) => {
    // Handle change
  },
  { deep: false } // Avoid deep watching when not needed
)
</script>
```

## 📚 Related Documentation

- [Frontend Architecture](./frontend-architecture.md)
- [State Management Guide](./state-management-guide.md)
- [API Integration Guide](./api-integration-guide.md)
- [Testing Strategy Guide](./testing-strategy-guide.md)

---

**Remember**: Always follow the established patterns and maintain consistency across components for better maintainability and team collaboration.