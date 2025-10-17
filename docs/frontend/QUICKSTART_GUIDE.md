# 🚀 Frontend Quick Start Guide

Get the Vue.js 3 + TypeScript frontend running in under 3 minutes.

## 📋 Prerequisites

- **Node.js 20.x LTS** - [Download here](https://nodejs.org/)
- **npm** - Comes with Node.js (or use yarn/pnpm)
- **Git** - For repository management
- **VS Code** - Recommended IDE with Vue and TypeScript extensions

## ⚡ 30-Second Setup

### 1. Navigate to Frontend
```bash
cd Container-Tracking-and-Port-Operations-Maersk-Hackathon/frontend
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Start Development Server
```bash
npm run dev
```

**🎉 Frontend ready at:** http://localhost:5173

## 🔗 Connect to Backend

### Backend Requirements
Ensure the backend is running at http://localhost:5221

### Environment Configuration
The frontend is pre-configured to connect to the local backend:
```bash
# .env.development (auto-loaded)
VITE_API_BASE_URL=http://localhost:5221/api
```

### Test Connection
1. Open http://localhost:5173
2. Click "Login" 
3. Use credentials: `admin` / `Admin123!`
4. Dashboard should load with data from backend

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **Vue.js** | 3.5.22 | Progressive JavaScript framework |
| **TypeScript** | 5.6.3 | Type safety and developer experience |
| **Vite** | 7.1.7 | Fast build tool and dev server |
| **Tailwind CSS** | 4.1.13 | Utility-first CSS framework |
| **Pinia** | 2.3.0 | State management |
| **Vue Router** | 4.5.0 | Client-side routing |
| **Axios** | 1.7.9 | HTTP client for API calls |

## 🎨 Key Features

### 🔐 Authentication System
- JWT token-based authentication
- Role-based access control (Admin, Port Manager, Operator, Viewer)
- Automatic token refresh and session management
- Secure route protection

### 📊 Dashboard Components
- **Real-time Metrics** - Container counts, ship status, berth utilization
- **Interactive Charts** - Container throughput, operational trends
- **Data Visualization** - Charts using Chart.js integration
- **Responsive Design** - Mobile-friendly layouts

### 🚢 Core Management Features
- **Container Management** - Track 300+ containers with real-time status
- **Ship Operations** - Manage 60+ ships and their schedules
- **Berth Assignment** - Visual berth allocation and scheduling
- **Port Operations** - Multi-port management across 25 global locations

## 🧪 Development Commands

### Development Server
```bash
# Start dev server with hot reload
npm run dev

# Start dev server on specific port
npm run dev -- --port 3000

# Start with network access
npm run dev -- --host
```

### Building & Production
```bash
# Build for production
npm run build

# Preview production build
npm run preview

# Build with custom base path
npm run build -- --base=/container-tracking/
```

### Code Quality
```bash
# Type checking
npm run type-check

# Lint code
npm run lint

# Format code
npm run format
```

## 🎯 Project Structure

```
frontend/src/
├── components/           # Reusable Vue components
│   ├── common/          # Generic UI components
│   ├── containers/      # Container-specific components
│   ├── ships/          # Ship management components
│   └── berths/         # Berth operation components
├── views/              # Page-level components
├── stores/             # Pinia state management
├── services/           # API service layer
├── router/            # Vue Router configuration
├── composables/       # Composition API utilities
├── types/             # TypeScript type definitions
└── assets/            # Static assets and styles
```

## 🔧 Configuration Files

### Environment Variables
```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5221/api
VITE_APP_TITLE=Container Tracking - Development
VITE_ENABLE_DEBUG=true

# .env.production  
VITE_API_BASE_URL=https://api.containertracking.com/api
VITE_APP_TITLE=Container Tracking System
VITE_ENABLE_DEBUG=false
```

### Vite Configuration
```javascript
// vite.config.js - Key settings
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 5173,
    proxy: {
      '/api': 'http://localhost:5221'  // Proxy API calls in dev
    }
  }
})
```

## 🔍 Troubleshooting

### Common Issues

#### npm install fails
```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Check Node.js version
node --version  # Should be 20.x LTS
```

#### Frontend can't connect to backend
```bash
# Check backend is running
curl http://localhost:5221/api/health/live

# Check CORS settings in backend
# Verify VITE_API_BASE_URL in .env files

# Check browser console for CORS errors
```

#### Build errors
```bash
# Clear Vite cache
rm -rf node_modules/.vite

# Check TypeScript errors
npm run type-check

# Update dependencies if needed
npm update
```

#### Port 5173 already in use
```bash
# Check what's using the port
netstat -ano | findstr :5173

# Start on different port
npm run dev -- --port 3000

# Kill process using port (Windows)
taskkill /PID <process_id> /F
```

## 🎨 Component Development

### Creating New Components
```vue
<!-- Example: src/components/containers/ContainerCard.vue -->
<template>
  <div class="container-card">
    <h3>{{ container.containerNumber }}</h3>
    <p>Status: {{ container.status }}</p>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'
import type { Container } from '@/types/container.types'

defineProps<{
  container: Container
}>()
</script>
```

### Using Pinia Stores
```typescript
// Example: Using the container store
import { useContainerStore } from '@/stores/containers'

export default defineComponent({
  setup() {
    const containerStore = useContainerStore()
    
    onMounted(() => {
      containerStore.fetchContainers()
    })
    
    return {
      containers: computed(() => containerStore.containers),
      loading: computed(() => containerStore.loading)
    }
  }
})
```

## 📚 Next Steps

### For Development
1. **Explore Components** - Check out the component library at `/components`
2. **Review State Management** - Understand Pinia stores in `/stores`
3. **API Integration** - Learn service layer patterns in `/services`
4. **UI Guidelines** - [UI/UX Guidelines](./UI_UX_GUIDELINES.md)

### For Customization
1. **Styling** - Customize Tailwind CSS configuration
2. **Components** - Add new reusable components
3. **Routes** - Extend Vue Router configuration
4. **State** - Create new Pinia stores for features

### For Production
1. **Environment Setup** - Configure production environment variables
2. **Build Optimization** - Review Vite build configuration
3. **Deployment** - Static hosting setup (Netlify, Vercel, Azure)
4. **Performance** - Code splitting and optimization

## 🎯 Quick Verification Checklist

- [ ] Frontend dev server starts without errors
- [ ] Application loads at http://localhost:5173
- [ ] Login page displays correctly
- [ ] Can authenticate with admin credentials
- [ ] Dashboard loads with data from backend API
- [ ] Navigation between pages works
- [ ] Responsive design works on mobile

**Ready to build amazing maritime interfaces!** ⚓

For detailed development information, see the [Frontend Setup Guide](./FRONTEND_SETUP.md).