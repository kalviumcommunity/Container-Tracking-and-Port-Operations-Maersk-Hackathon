# 🎨 Frontend Setup Guide

## Project Overview

The Maersk Container Tracking frontend is built with **Vue.js 3.5.22**, **TypeScript**, and **Tailwind CSS**, providing a modern, responsive interface for maritime operations management. This guide covers everything needed to set up and run the frontend development environment.

## 🛠️ Prerequisites

### Required Software
- **Node.js 20.19.0+** - [Download LTS version](https://nodejs.org/)
- **npm 10.2.0+** or **yarn 1.22.0+** - Package manager
- **Git** - Version control
- **VS Code** - Recommended IDE

### Recommended VS Code Extensions
```json
{
  "recommendations": [
    "Vue.volar",
    "Vue.vscode-typescript-vue-plugin",
    "bradlc.vscode-tailwindcss",
    "esbenp.prettier-vscode",
    "ms-vscode.vscode-typescript-next",
    "formulahendry.auto-rename-tag",
    "christian-kohler.path-intellisense"
  ]
}
```

## 🚀 Quick Start Setup

### 1. **Environment Verification**
```bash
# Verify Node.js version
node --version  # Should be 20.19.0+

# Verify npm version
npm --version   # Should be 10.2.0+

# Verify Git
git --version
```

### 2. **Project Setup**
```bash
# Navigate to frontend directory
cd Container-Tracking-and-Port-Operations-Maersk-Hackathon/frontend

# Install dependencies
npm install

# Verify installation
npm list --depth=0
```

### 3. **Environment Configuration**

#### Create Environment Files:
```bash
# Copy example environment file
cp .env.example .env

# Or create manually
touch .env
```

#### Configure `.env` file:
```env
# API Configuration
VITE_API_BASE_URL=http://localhost:5221
VITE_API_TIMEOUT=10000

# Application Configuration
VITE_APP_TITLE=Maersk Container Tracking
VITE_APP_VERSION=1.0.0
VITE_APP_DESCRIPTION=Container Tracking & Berth Operations System

# Environment
NODE_ENV=development

# Features
VITE_ENABLE_ANALYTICS=true
VITE_ENABLE_DARK_MODE=false
VITE_ENABLE_PWA=false

# Debug
VITE_DEBUG_MODE=true
VITE_LOG_LEVEL=info
```

#### Additional Environment Files:
```bash
# Development environment
# .env.development
VITE_API_BASE_URL=http://localhost:5221
VITE_DEBUG_MODE=true

# Production environment
# .env.production
VITE_API_BASE_URL=https://api-container-tracking.azurewebsites.net
VITE_DEBUG_MODE=false
```

### 4. **Development Server**
```bash
# Start development server
npm run dev

# Server will start on:
# Local:   http://localhost:5173/
# Network: http://192.168.x.x:5173/

# Alternative port
npm run dev -- --port 3000
```

### 5. **Verify Setup**
Open browser to `http://localhost:5173` and verify:
- [ ] Application loads without errors
- [ ] Login page is displayed
- [ ] Can authenticate with `admin` / `Admin123!`
- [ ] Dashboard displays after login
- [ ] Navigation works properly

## 📦 Package Dependencies

### Core Dependencies
```json
{
  "vue": "^3.5.22",           // Vue.js framework
  "vue-router": "^4.5.0",     // Client-side routing
  "pinia": "^2.3.0",          // State management
  "axios": "^1.7.9",          // HTTP client
  "@vueuse/core": "^11.3.0"   // Vue composition utilities
}
```

### UI & Styling Dependencies
```json
{
  "tailwindcss": "^4.1.13",        // Utility-first CSS
  "lucide-vue-next": "^0.468.0",   // Icon library
  "@headlessui/vue": "^1.7.23"     // Accessible UI components
}
```

### Development Dependencies
```json
{
  "vite": "^7.1.7",                    // Build tool
  "typescript": "^5.6.3",             // Type safety
  "@vitejs/plugin-vue": "^5.2.1",     // Vue plugin for Vite
  "@vue/tsconfig": "^0.7.0",          // TypeScript config
  "autoprefixer": "^10.4.20"          // CSS prefixing
}
```

### Install Additional Packages:
```bash
# Add new dependencies
npm install package-name

# Add development dependencies
npm install --save-dev package-name

# Update all packages
npm update

# Audit for vulnerabilities
npm audit
npm audit fix
```

## ⚙️ Development Configuration

### 1. **Vite Configuration** (`vite.config.js`)
```javascript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
      '@components': path.resolve(__dirname, 'src/components'),
      '@stores': path.resolve(__dirname, 'src/stores'),
      '@services': path.resolve(__dirname, 'src/services'),
      '@types': path.resolve(__dirname, 'src/types')
    }
  },
  server: {
    port: 5173,
    host: true,
    open: true,
    cors: true
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['vue', 'vue-router', 'pinia'],
          ui: ['lucide-vue-next', '@headlessui/vue']
        }
      }
    }
  },
  css: {
    devSourcemap: true
  }
})
```

### 2. **TypeScript Configuration** (`tsconfig.json`)
```json
{
  "extends": "@vue/tsconfig/tsconfig.dom.json",
  "include": [
    "env.d.ts",
    "src/**/*",
    "src/**/*.vue"
  ],
  "exclude": [
    "src/**/__tests__/*"
  ],
  "compilerOptions": {
    "composite": true,
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.app.tsbuildinfo",
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"],
      "@components/*": ["./src/components/*"],
      "@stores/*": ["./src/stores/*"],
      "@services/*": ["./src/services/*"],
      "@types/*": ["./src/types/*"]
    },
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true
  }
}
```

### 3. **Tailwind CSS Configuration** (`tailwind.config.js`)
```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
        },
        maersk: {
          blue: '#00A3E0',
          navy: '#003F7F',
          gray: '#6B7280'
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
  ],
}
```

### 4. **Vue Router Configuration** (`src/router/index.ts`)
```typescript
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      name: 'Dashboard',
      component: () => import('@/views/DashboardView.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

// Navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
```

## 🔧 Development Tools Setup

### 1. **VS Code Workspace Configuration** (`.vscode/settings.json`)
```json
{
  "typescript.preferences.includePackageJsonAutoImports": "auto",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  },
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "emmet.includeLanguages": {
    "vue-html": "html"
  },
  "files.associations": {
    "*.vue": "vue"
  },
  "tailwindCSS.includeLanguages": {
    "vue": "html",
    "vue-html": "html"
  },
  "tailwindCSS.experimental.classRegex": [
    ["class:\\s*?[\"'`]([^\"'`]*).*?[\"'`]", "[\"'`]([^\"'`]*)[\"'`]"]
  ]
}
```

### 2. **Prettier Configuration** (`.prettierrc`)
```json
{
  "semi": false,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 80,
  "bracketSpacing": true,
  "arrowParens": "avoid",
  "endOfLine": "lf",
  "vueIndentScriptAndStyle": false
}
```

### 3. **ESLint Configuration** (`.eslintrc.cjs`)
```javascript
module.exports = {
  root: true,
  env: {
    node: true,
    'vue/setup-compiler-macros': true,
  },
  extends: [
    'plugin:vue/vue3-recommended',
    '@vue/eslint-config-typescript',
    '@vue/eslint-config-prettier',
  ],
  parser: 'vue-eslint-parser',
  parserOptions: {
    parser: '@typescript-eslint/parser',
    ecmaVersion: 2022,
    sourceType: 'module',
  },
  rules: {
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'vue/multi-word-component-names': 'off',
    'vue/no-reserved-component-names': 'off',
  },
}
```

## 🏗️ Project Structure Overview

```
frontend/
├── public/                     # Static assets
│   ├── favicon.ico            # Application icon
│   └── robots.txt             # SEO configuration
├── src/                       # Source code
│   ├── components/            # Vue components
│   │   ├── common/           # Reusable components
│   │   ├── forms/            # Form components
│   │   └── main/             # Main layout components
│   ├── stores/               # Pinia state stores
│   │   ├── auth.ts           # Authentication store
│   │   ├── containers.ts     # Container management
│   │   └── ships.ts          # Ship management
│   ├── services/             # API service layer
│   │   ├── api.ts            # Axios configuration
│   │   ├── auth.service.ts   # Authentication services
│   │   └── container.service.ts # Container services
│   ├── types/                # TypeScript type definitions
│   ├── router/               # Vue Router configuration
│   ├── assets/               # CSS and images
│   ├── views/                # Page components
│   ├── App.vue               # Root component
│   └── main.ts               # Application entry point
├── .env                      # Environment variables
├── .env.example              # Environment template
├── package.json              # Dependencies and scripts
├── vite.config.js            # Vite configuration
├── tailwind.config.js        # Tailwind CSS configuration
└── tsconfig.json             # TypeScript configuration
```

## 🔄 Development Workflow

### 1. **Daily Development Routine**
```bash
# 1. Pull latest changes
git pull origin main

# 2. Install any new dependencies
npm install

# 3. Start development server
npm run dev

# 4. Open browser to http://localhost:5173
```

### 2. **Making Changes**
```bash
# Create new component
mkdir src/components/NewFeature
touch src/components/NewFeature/NewFeature.vue

# Add new route
# Edit src/router/index.ts

# Add new store
touch src/stores/newFeature.ts

# Add new service
touch src/services/newFeature.service.ts
```

### 3. **Building for Production**
```bash
# Build production version
npm run build

# Preview production build
npm run preview

# Test build locally
npx serve dist
```

## 🧪 Testing & Quality

### 1. **Type Checking**
```bash
# Run TypeScript type checking
npm run type-check

# Watch mode for type checking
npm run type-check -- --watch
```

### 2. **Linting**
```bash
# Run ESLint
npm run lint

# Fix ESLint issues automatically
npm run lint -- --fix
```

### 3. **Code Formatting**
```bash
# Format code with Prettier
npm run format

# Check formatting
npm run format -- --check
```

## 🔍 Troubleshooting

### Common Issues & Solutions

#### 1. **Node.js Version Issues**
```bash
# Use Node Version Manager (nvm)
nvm install 20.19.0
nvm use 20.19.0

# Verify version
node --version
```

#### 2. **Package Installation Issues**
```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Use yarn as alternative
npm install -g yarn
yarn install
```

#### 3. **Port Conflicts**
```bash
# Kill process on port 5173 (Windows)
netstat -ano | findstr :5173
taskkill /PID <process_id> /F

# Kill process on port 5173 (macOS/Linux)
lsof -ti:5173 | xargs kill -9

# Use different port
npm run dev -- --port 3000
```

#### 4. **TypeScript Errors**
```bash
# Restart TypeScript service in VS Code
# Ctrl+Shift+P -> "TypeScript: Restart TS Server"

# Clear TypeScript cache
rm -rf node_modules/.cache
```

#### 5. **API Connection Issues**
```bash
# Verify backend is running
curl http://localhost:5221/api/health/live

# Check environment variables
cat .env

# Verify CORS configuration in backend
```

## 📊 Performance Optimization

### 1. **Bundle Analysis**
```bash
# Install bundle analyzer
npm install --save-dev rollup-plugin-visualizer

# Build with analysis
npm run build -- --analyze

# View bundle report
open dist/stats.html
```

### 2. **Development Performance**
```bash
# Use faster development server
npm run dev -- --host 0.0.0.0

# Enable debugging
export DEBUG=vite:*
npm run dev
```

## 🚀 Deployment Preparation

### 1. **Environment Variables for Production**
```env
# .env.production
VITE_API_BASE_URL=https://your-production-api.com
VITE_DEBUG_MODE=false
VITE_LOG_LEVEL=error
```

### 2. **Build Optimization**
```javascript
// vite.config.js production settings
export default defineConfig(({ mode }) => ({
  build: {
    minify: 'terser',
    terserOptions: {
      compress: {
        drop_console: mode === 'production',
        drop_debugger: mode === 'production',
      },
    },
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['vue', 'vue-router', 'pinia'],
          utils: ['axios', '@vueuse/core']
        }
      }
    }
  }
}))
```

## ✅ Setup Verification Checklist

- [ ] Node.js 20.19.0+ installed
- [ ] npm/yarn installed and working
- [ ] Project dependencies installed
- [ ] Environment variables configured
- [ ] Development server starts without errors
- [ ] Application loads in browser
- [ ] Can authenticate with backend API
- [ ] TypeScript compilation works
- [ ] Tailwind CSS styles apply correctly
- [ ] Hot module replacement works
- [ ] Vue DevTools browser extension installed

## 📚 Additional Resources

- [Vue.js 3 Documentation](https://vuejs.org/)
- [Vite Documentation](https://vitejs.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/)
- [Tailwind CSS Documentation](https://tailwindcss.com/)
- [Pinia Documentation](https://pinia.vuejs.org/)
- [Vue Router Documentation](https://router.vuejs.org/)

## 🆘 Getting Help

If you encounter issues during setup:

1. **Check Console Logs**: Browser DevTools and terminal output
2. **Verify Dependencies**: Ensure all packages are installed correctly
3. **Environment**: Check .env file configuration
4. **Backend Connection**: Verify API server is running
5. **Documentation**: Refer to this guide and official docs
6. **Team Support**: Ask team members or create GitHub issues