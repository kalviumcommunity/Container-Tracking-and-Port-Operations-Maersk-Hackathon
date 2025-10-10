import axios from 'axios';
import type { AxiosInstance } from 'axios';

// Import all specialized services
import { containerService } from './containerService';
import { portService } from './portService';
import { shipService } from './shipService';
import { userManagementApi } from './userManagementApi';

// API Configuration
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? 'https://your-api-domain.com/api' 
  : 'http://localhost:5221/api';

// Create axios instance (shared by all services)
export const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Token management utilities
const getToken = (): string | null => {
  // First check for real JWT token
  const jwtToken = localStorage.getItem('auth_token');
  if (jwtToken) {
    return jwtToken;
  }
  
  // Then check for localStorage user (create mock token)
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    // Create a mock JWT-like token for localStorage authentication
    const userData = JSON.parse(currentUser);
    return `mock-jwt-${userData.id}-${userData.username}`;
  }
  
  return null;
};

const setToken = (token: string): void => {
  localStorage.setItem('auth_token', token);
  api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
};

const removeToken = (): void => {
  localStorage.removeItem('auth_token');
  localStorage.removeItem('current_user'); // Also clear current user
  delete api.defaults.headers.common['Authorization'];
};

// Initialize authentication
const initializeAuth = (): void => {
  const token = getToken();
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }
};

// Set token on startup if exists
initializeAuth();

// Request/Response interceptors
api.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Don't log 404 errors for development endpoints - they're expected when backend routes aren't fully implemented
    if (error.response?.status !== 404) {
      console.error('API Error:', error.response?.data || error.message);
    } else {
      console.warn('API endpoint not found:', error.config?.url);
    }
    
    // Handle 401 Unauthorized errors
    if (error.response?.status === 401) {
      console.warn('Authentication failed - clearing tokens and redirecting to login');
      removeToken();
      
      // Only redirect if we're not already on the login page
      if (window.location.pathname !== '/login' && window.location.pathname !== '/') {
        window.location.href = '/login';
      }
    }
    
    return Promise.reject(error);
  }
);

// ===== ORCHESTRATION LAYER =====
// Re-export specialized services for clean API access

// Authentication API (minimal, focused)
export const authApi = {
  async login(credentials: { username: string; password: string }) {
    try {
      const response = await api.post('/auth/login', credentials);
      
      // Store the JWT token in local storage
      if (response.data.token) {
        setToken(response.data.token);
      }
      
      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  },

  async logout() {
    try {
      // Call API logout endpoint if available
      await api.post('/auth/logout').catch(() => {
        // Silently fail if the endpoint doesn't exist
        console.warn('Logout endpoint not available, removing token locally');
      });
      
      // Always remove the token locally
      removeToken();
      
      return { success: true };
    } catch (error) {
      console.error('Logout error:', error);
      
      // Still remove token on error
      removeToken();
      
      return { success: true };
    }
  },
  
  async getCurrentUser() {
    try {
      // Try to get user from API first
      const response = await api.get('/auth/current-user');
      return response.data;
    } catch (error) {
      // Remove console.error - don't expose API implementation details
      
      // Fallback to localStorage if API fails
      const currentUser = localStorage.getItem('current_user');
      if (currentUser) {
        return { user: JSON.parse(currentUser) };
      }
      
      throw error;
    }
  },
  
  isAuthenticated() {
    return getToken() !== null;
  },
  
  async changePassword(changePasswordData: {
    currentPassword: string;
    newPassword: string;
  }) {
    try {
      const response = await api.post('/auth/change-password', changePasswordData);
      return response.data;
    } catch (error) {
      // Return mock success for development
      return { success: true, message: 'Password changed successfully' };
    }
  }
};

// Role Application API (lightweight)
export const roleApplicationApi = {
  async submitApplication(applicationData: {
    requestedRole: string;
    justification: string;
  }) {
    try {
      const response = await api.post('/role-applications', applicationData);
      return response.data;
    } catch (error) {
      // Return mock response for development
      return { success: true, message: 'Application submitted successfully' };
    }
  },

  async getMyApplications() {
    try {
      const response = await api.get('/role-applications/my-applications');
      return response.data;
    } catch (error) {
      // Return mock data for development
      return [];
    }
  },

  async getAvailableRoles() {
    try {
      const response = await api.get('/role-applications/available-roles');
      return response.data;
    } catch (error) {
      // Return mock roles for development
      return [
        { roleName: 'PortManager', description: 'Manage port operations', canApply: true },
        { roleName: 'Operator', description: 'Container and ship operations', canApply: true },
        { roleName: 'Admin', description: 'Full system access', canApply: false, reasonCannotApply: 'Admin approval required' }
      ];
    }
  },

  async getPendingApplications() {
    try {
      const response = await api.get('/role-applications/pending');
      return response.data;
    } catch (error) {
      return [];
    }
  },

  async getAllApplications() {
    try {
      const response = await api.get('/role-applications/all');
      return response.data;
    } catch (error) {
      return [];
    }
  },

  async reviewApplication(applicationId: string, status: string, reviewNotes?: string) {
    try {
      const response = await api.post(`/role-applications/${applicationId}/review`, {
        status, reviewNotes
      });
      return response.data;
    } catch (error) {
      return { success: true, message: 'Review completed' };
    }
  },

  async cancelApplication(applicationId: string) {
    try {
      const response = await api.post(`/role-applications/${applicationId}/cancel`);
      return response.data;
    } catch (error) {
      return { success: true, message: 'Application cancelled' };
    }
  }
};

// Crew API (minimal - since backend doesn't exist yet)
export const crewApi = {
  async getAll() {
    try {
      const response = await api.get('/crew');
      return response.data;
    } catch (error) {
      // Fallback mock data
      return {
        data: [
          { id: 1, name: 'John Smith', role: 'Crane Operator' },
          { id: 2, name: 'Maria Garcia', role: 'Dock Supervisor' },
          { id: 3, name: 'David Chen', role: 'Forklift Operator' },
          { id: 4, name: 'Sarah Johnson', role: 'Safety Inspector' },
          { id: 5, name: 'Michael Brown', role: 'Equipment Technician' }
        ]
      };
    }
  }
};

// Berth API (minimal - delegate to dedicated service when created)
export const berthApi = {
  async getAll() {
    const response = await api.get('/berths');
    return response.data;
  },

  async getById(id: number | string) {
    const response = await api.get(`/berths/${id}`);
    return response.data;
  }
};

// Add Berth Assignment API (missing export)
export const berthAssignmentApi = {
  async getAll() {
    try {
      const response = await api.get('/berth-assignments');
      return response.data;
    } catch (error) {
      // Return mock data if API endpoint doesn't exist yet
      return {
        data: [
          { id: 1, berthId: 1, shipId: 1, containerId: 'MAEU1234567', assignedAt: new Date().toISOString() },
          { id: 2, berthId: 2, shipId: 2, containerId: 'MAEU2345678', assignedAt: new Date().toISOString() }
        ]
      };
    }
  },

  async getById(id: number | string) {
    try {
      const response = await api.get(`/berth-assignments/${id}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  async create(assignmentData: {
    shipId?: number;
    berthId: number;
    containerId?: string;
    scheduledArrival?: string;
    scheduledDeparture?: string;
    assignmentType?: string;
    priority?: string;
    status?: string;
  }) {
    try {
      const response = await api.post('/berth-assignments', assignmentData);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  async update(id: number | string, updateData: any) {
    try {
      const response = await api.put(`/berth-assignments/${id}`, updateData);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  async delete(id: number | string) {
    try {
      const response = await api.delete(`/berth-assignments/${id}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  }
};

// ===== DELEGATE TO SPECIALIZED SERVICES =====
// These are the main orchestration exports

// Container operations - delegate to containerService
export const containerApi = {
  getContainers: (filters = {}) => containerService.getContainers(filters),
  getAll: () => containerService.getAll(),
  getById: (id: string) => containerService.getById(id),
  getStatistics: () => containerService.getStatistics(),
  create: (containerData: any) => containerService.create(containerData),
  update: (containerId: string, containerData: any) => containerService.update(containerId, containerData),
  delete: (id: string) => containerService.delete(id),
  bulkUpdateStatus: (bulkUpdate: any) => containerService.bulkUpdateStatus(bulkUpdate),
  exportContainers: (filters: any) => containerService.exportContainers(filters)
};

// Port operations - delegate to portService
export const portApi = {
  getAll: () => portService.getAll(),
  getById: (id: any) => portService.getById(id)
};

// Ship operations - delegate to shipService
export const shipApi = {
  getAll: async () => {
    try {
      const response = await shipService.getAll();
      return response;
    } catch (error) {
      console.error('shipApi.getAll error:', error);
      // Return mock data as fallback
      return {
        data: [
          { id: 1, shipId: 1, name: 'Maersk Edinburgh', status: 'Docked', capacity: 13092 },
          { id: 2, shipId: 2, name: 'MSC Oscar', status: 'At Sea', capacity: 19224 },
          { id: 3, shipId: 3, name: 'CMA CGM Bougainville', status: 'Loading', capacity: 18000 },
          { id: 4, shipId: 4, name: 'Ever Given', status: 'Docked', capacity: 20124 }
        ]
      };
    }
  },
  getById: (id: any) => shipService.getById(id),
  create: (shipData: any) => shipService.create(shipData),
  update: (id: any, shipData: any) => shipService.update(id, shipData)
};

// User management - re-export existing service
export { userManagementApi };

// ===== TYPE EXPORTS =====
export type { Container, ContainerFilters, ContainerStats, PaginatedResponse } from '../types/container';
export type { Ship } from './shipService';
export type { Port } from './portService';
export type { 
  UserListDto, 
  UpdateUserRolesDto, 
  UpdateUserStatusDto, 
  SystemStatsDto, 
  UsersPagedResponse 
} from './userManagementApi';

export interface RoleApplication {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  requestedRole: string;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected' | 'Withdrawn';
  companyName?: string;
  companyEmail?: string;
  position?: string;
  submittedAt: string;
  reviewedAt?: string;
  reviewedBy?: string;
  reviewNotes?: string;
}

// Export the base API client for direct access when needed
export { api as apiClient };
export default api;