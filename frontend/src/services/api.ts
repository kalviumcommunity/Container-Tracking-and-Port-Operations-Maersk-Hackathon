import axios from 'axios';
import type { AxiosInstance, AxiosResponse } from 'axios';

// API Configuration
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? 'https://your-api-domain.com/api' 
  : 'http://localhost:5221/api';

// Create axios instance
export const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Token management
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

// Initialize token on startup
const initializeAuth = (): void => {
  const token = getToken();
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }
};

// Set token on startup if exists
initializeAuth();

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    
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

// Add Authentication API
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

  async register(userData: { username: string; email: string; password: string }) {
    try {
      const response = await api.post('/auth/register', userData);
      return response.data;
    } catch (error) {
      console.error('Registration error:', error);
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
      console.error('Get current user error:', error);
      
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
  }
};

// Add Role Application API
export const roleApplicationApi = {
  async submitApplication(applicationData: {
    userId: string;
    requestedRole: string;
    reason: string;
    companyName?: string;
    companyEmail?: string;
    position?: string;
  }) {
    try {
      const response = await api.post('/role-applications', applicationData);
      return response.data;
    } catch (error) {
      console.error('Submit role application error:', error);
      throw error;
    }
  },

  async getApplications(filter?: string) {
    try {
      const params = filter ? { status: filter } : {};
      const response = await api.get('/role-applications', { params });
      return response.data;
    } catch (error) {
      console.error('Get role applications error:', error);
      throw error;
    }
  },

  async getUserApplications(userId: string) {
    try {
      const response = await api.get(`/role-applications/user/${userId}`);
      return response.data;
    } catch (error) {
      console.error(`Get user applications error for ${userId}:`, error);
      throw error;
    }
  },

  async approveApplication(applicationId: string, reviewNotes?: string) {
    try {
      const response = await api.post(`/role-applications/${applicationId}/approve`, { reviewNotes });
      return response.data;
    } catch (error) {
      console.error(`Approve application error for ${applicationId}:`, error);
      throw error;
    }
  },

  async rejectApplication(applicationId: string, reviewNotes: string) {
    try {
      const response = await api.post(`/role-applications/${applicationId}/reject`, { reviewNotes });
      return response.data;
    } catch (error) {
      console.error(`Reject application error for ${applicationId}:`, error);
      throw error;
    }
  },

  async withdrawApplication(applicationId: string) {
    try {
      const response = await api.post(`/role-applications/${applicationId}/withdraw`);
      return response.data;
    } catch (error) {
      console.error(`Withdraw application error for ${applicationId}:`, error);
      throw error;
    }
  }
};

// Add Berth API
export const berthApi = {
  // Get all berths
  async getAll() {
    try {
      const response = await api.get('/berths');
      return response.data;
    } catch (error) {
      console.error('Error fetching berths:', error);
      throw error;
    }
  },

  // Get berth by ID
  async getById(id: number | string) {
    try {
      const response = await api.get(`/berths/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching berth ${id}:`, error);
      throw error;
    }
  },

  // Get berth details
  async getDetails(id: number | string) {
    try {
      const response = await api.get(`/berths/${id}/details`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching berth details for ${id}:`, error);
      throw error;
    }
  },

  // Get berths by port
  async getByPort(portId: number | string) {
    try {
      const response = await api.get(`/berths/port/${portId}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching berths for port ${portId}:`, error);
      throw error;
    }
  },

  // Get berths by status
  async getByStatus(status: string) {
    try {
      const response = await api.get(`/berths/status/${status}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching berths with status ${status}:`, error);
      throw error;
    }
  },

  // Create new berth
  async create(berthData: {
    name: string;
    portId: number;
    capacity: number;
    length?: number;
    width?: number;
    depth?: number;
    type?: string;
    status?: string;
  }) {
    try {
      const response = await api.post('/berths', berthData);
      return response.data;
    } catch (error) {
      console.error('Error creating berth:', error);
      throw error;
    }
  },

  // Update berth
  async update(id: number | string, berthData: {
    name?: string;
    capacity?: number;
    length?: number;
    width?: number;
    depth?: number;
    type?: string;
    status?: string;
  }) {
    try {
      const response = await api.put(`/berths/${id}`, berthData);
      return response.data;
    } catch (error) {
      console.error(`Error updating berth ${id}:`, error);
      throw error;
    }
  },

  // Delete berth
  async delete(id: number | string) {
    try {
      const response = await api.delete(`/berths/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error deleting berth ${id}:`, error);
      throw error;
    }
  }
};

// Add Berth Assignment API
export const berthAssignmentApi = {
  // Get all berth assignments
  async getAll() {
    try {
      const response = await api.get('/berth-assignments');
      return response.data;
    } catch (error) {
      console.error('Error fetching berth assignments:', error);
      throw error;
    }
  },

  // Get berth assignment by ID
  async getById(id: number | string) {
    try {
      const response = await api.get(`/berth-assignments/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching berth assignment ${id}:`, error);
      throw error;
    }
  },

  // Assign ship to berth
  async create(assignmentData: {
    shipId: number | string;
    berthId: number | string;
    scheduledArrival: string;
    scheduledDeparture: string;
    containerCount?: number;
    priority?: string;
    status?: string;
  }) {
    try {
      const response = await api.post('/berth-assignments', assignmentData);
      return response.data;
    } catch (error) {
      console.error('Error creating berth assignment:', error);
      throw error;
    }
  },

  // Update berth assignment
  async update(id: number | string, updateData: {
    scheduledArrival?: string;
    scheduledDeparture?: string;
    actualArrival?: string;
    actualDeparture?: string;
    containerCount?: number;
    priority?: string;
    status?: string;
  }) {
    try {
      const response = await api.put(`/berth-assignments/${id}`, updateData);
      return response.data;
    } catch (error) {
      console.error(`Error updating berth assignment ${id}:`, error);
      throw error;
    }
  },

  // Delete berth assignment
  async delete(id: number | string) {
    try {
      const response = await api.delete(`/berth-assignments/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error deleting berth assignment ${id}:`, error);
      throw error;
    }
  }
};

// Import services to re-export
import { containerService } from './containerService';
import { portService } from './portService';
import { shipService } from './shipService';

// Export renamed services to avoid naming conflicts
// Direct exports from this file for backward compatibility
export const containerApi = {
  // Get containers with filtering and pagination
  async getContainers(filters = {}) {
    // Delegate to containerService
    return await containerService.getContainers(filters);
  },

  // Legacy method for backward compatibility
  async getAll() {
    // Delegate to containerService
    return await containerService.getAll();
  },

  async getById(id: string) {
    // Delegate to containerService
    return await containerService.getById(id);
  },

  async getStatistics() {
    // Delegate to containerService
    return await containerService.getStatistics();
  },

  async create(containerData) {
    // Delegate to containerService
    return await containerService.create(containerData);
  },

  async update(containerId, containerData) {
    // Delegate to containerService
    return await containerService.update(containerId, containerData);
  },

  async delete(id: string) {
    // Delegate to containerService
    return await containerService.delete(id);
  },

  async bulkUpdateStatus(bulkUpdate) {
    // Delegate to containerService
    return await containerService.bulkUpdateStatus(bulkUpdate);
  },

  async exportContainers(filters) {
    // Delegate to containerService
    return await containerService.exportContainers(filters);
  }
};

// Re-export other services with different names to avoid conflicts
export const portApi = {
  async getAll() {
    return await portService.getAll();
  },
  async getById(id) {
    return await portService.getById(id);
  }
  // Add other port methods as needed
};

export const shipApi = {
  async getAll() {
    return await shipService.getAll();
  },
  async getById(id) {
    return await shipService.getById(id);
  }
  // Add other ship methods as needed
};

// Export the userManagementApi directly
export { userManagementApi } from './userManagementApi';

// Export types
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

// Define Role Application types
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

// Re-export the api for any direct API calls
export { api as apiClient };
export default api;