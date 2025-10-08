# Frontend Development Setup Guide

This guide provides step-by-step instructions for setting up the **PortTrack Container Operations** frontend development environment.

## 📋 Prerequisites

### Required Software
- **Node.js**: Version 20.19.0 or 22.12.0+ (as specified in package.json)
- **npm**: Version 8.0.0+ (comes with Node.js)
- **Git**: Latest version
- **VS Code**: Recommended IDE with Vue extensions

### Recommended VS Code Extensions
```json
{
  "recommendations": [
    "Vue.volar",
    "Vue.vscode-typescript-vue-plugin", 
    "bradlc.vscode-tailwindcss",
    "ms-vscode.vscode-typescript-next",
    "esbenp.prettier-vscode",
    "ms-vscode.vscode-eslint"
  ]
}
```

## 🚀 Installation Steps

### 1. Clone the Repository
```bash
git clone https://github.com/kalviumcommunity/Container-Tracking-and-Port-Operations-Maersk-Hackathon.git
cd Container-Tracking-and-Port-Operations-Maersk-Hackathon/frontend
```

### 2. Install Dependencies
```bash
# Install all dependencies
npm install

# Verify installation
npm list --depth=0
```

### 3. Environment Configuration
```bash
# Copy environment template
cp .env.production.example .env

# Edit environment variables
# Update API_BASE_URL to match your backend
```

**Required Environment Variables:**
```env
# API Configuration
API_BASE_URL=http://localhost:5000/api
VITE_API_BASE_URL=http://localhost:5000/api

# Authentication
VITE_JWT_SECRET_KEY=your-jwt-secret
VITE_TOKEN_EXPIRY=24h

# Development
NODE_ENV=development
VITE_APP_TITLE="PortTrack Container Operations"
```

### 4. Start Development Server
```bash
# Start Vite development server
npm run dev

# Server will start at http://localhost:5173
```

## 🔧 Development Commands

### Core Commands
```bash
# Development server with hot reload
npm run dev

# Build for production
npm run build

# Preview production build locally
npm run preview

# Type checking (if TypeScript is configured)
npm run type-check
```

### Package Management
```bash
# Install new dependency
npm install <package-name>

# Install dev dependency
npm install -D <package-name>

# Update dependencies
npm update

# Audit for vulnerabilities
npm audit
npm audit fix
```

## 🏗️ Project Structure Overview

```
frontend/
├── public/                 # Static assets
│   ├── favicon.ico
│   └── index.html
├── src/
│   ├── components/         # Vue components
│   │   ├── Dashboard.vue
│   │   ├── ContainerManagement.vue
│   │   ├── AdminDashboard.vue
│   │   └── ...
│   ├── forms/             # Form components
│   ├── router/            # Vue Router configuration
│   ├── services/          # API services and utilities
│   ├── stores/            # Pinia state management
│   ├── assets/            # Assets (images, styles)
│   ├── App.vue           # Root component
│   └── main.ts           # Application entry point
├── .env                   # Environment variables
├── package.json          # Dependencies and scripts
├── vite.config.js        # Vite configuration
└── jsconfig.json         # JavaScript/TypeScript config
```

## 🛠️ Development Tools Configuration

### Vite Configuration
The project uses Vite for fast development and building:
- **Hot Module Replacement (HMR)**: Instant updates during development
- **TypeScript Support**: Built-in TypeScript processing
- **Vue SFC Support**: Single File Component processing
- **Tailwind CSS Integration**: Automatic CSS processing

### Vue DevTools
Install the Vue DevTools browser extension for enhanced debugging:
- **Component Inspector**: View component hierarchy and props
- **State Management**: Monitor Pinia store changes
- **Performance Profiling**: Analyze component render times
- **Events Timeline**: Track Vue events and lifecycle hooks

## 🔍 Troubleshooting

### Common Issues

#### Node Version Mismatch
```bash
# Check current Node version
node --version

# Use Node Version Manager (nvm)
nvm install 20.19.0
nvm use 20.19.0
```

#### Port Already in Use
```bash
# If port 5173 is occupied, Vite will automatically use next available port
# Or specify custom port:
npm run dev -- --port 3000
```

#### Module Resolution Issues
```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

#### Build Errors
```bash
# Check for TypeScript errors
npm run type-check

# Clean dist folder
rm -rf dist
npm run build
```

### Environment Issues
- Ensure `.env` file is properly configured
- Verify API backend is running on specified URL
- Check CORS configuration if API calls fail

## 🚦 Development Workflow

### 1. Feature Development
```bash
# Create feature branch
git checkout -b feature/new-component

# Start development server
npm run dev

# Make changes and test locally
# Commit changes with descriptive messages
git add .
git commit -m "feat: add new container tracking component"
```

### 2. Code Quality Checks
```bash
# Lint code (if ESLint is configured)
npm run lint

# Format code (if Prettier is configured)
npm run format

# Type checking
npm run type-check
```

### 3. Testing
```bash
# Run unit tests (when configured)
npm run test

# Run E2E tests (when configured)
npm run test:e2e
```

## 📚 Next Steps

After successful setup:
1. Review [Component Development Guide](./component-development-guide.md)
2. Understand [Frontend Architecture](./frontend-architecture.md)  
3. Learn about [API Integration](./api-integration-guide.md)
4. Explore [State Management](./state-management-guide.md)

---

**Need Help?** Check the [main README](../README.md) or refer to specific documentation guides in this folder.