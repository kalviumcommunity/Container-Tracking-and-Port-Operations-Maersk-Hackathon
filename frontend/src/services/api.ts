import axios from 'axios';
import type { AxiosInstance, AxiosResponse } from 'axios';

// Import all specialized services
import { containerService } from './containerService';
import { portService } from './portService';
import { shipService } from './shipService';
import { userManagementApi } from './userManagementApi';

// API Configuration
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5221/api';

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

// ===== API RESPONSE TYPE =====
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success?: boolean;
  timestamp?: string;
}

// ===== AUTHENTICATION API =====
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

  async register(userData: { username: string; email: string; password: string; fullName: string; phoneNumber?: string; department?: string }) {
    try {
      const response = await api.post('/auth/register', userData);
      
      // Store the JWT token in local storage
      if (response.data.token) {
        setToken(response.data.token);
      }
      
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
  
  async updateProfile(updateData: {
    fullName?: string
    email?: string
    phoneNumber?: string
    department?: string
  }): Promise<ApiResponse<any>> {
    try {
      const response: AxiosResponse<ApiResponse<any>> = await api.put('/auth/profile', updateData)
      return response.data
    } catch (error) {
      console.error('Update profile error:', error)
      throw error
    }
  },

  async changePassword(passwordData: {
    currentPassword: string
    newPassword: string
    confirmNewPassword: string
  }): Promise<ApiResponse<any>> {
    try {
      const response: AxiosResponse<ApiResponse<any>> = await api.put('/auth/change-password', passwordData)
      return response.data
    } catch (error) {
      console.error('Change password error:', error)
      throw error
    }
  },

  async getCurrentUser() {
    try {
      // Try to get user from API first
      const response = await api.get('/auth/current-user');
      return response.data;
    } catch (error) {
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

// ===== ROLE APPLICATION API =====
export const roleApplicationApi = {
  async submitApplication(applicationData: {
    requestedRole: string;
    justification: string;
  }) {
    try {
      const response = await api.post('/role-applications', {
        RequestedRole: applicationData.requestedRole,  // Match backend DTO
        Justification: applicationData.justification
      });
      return response.data;
    } catch (error) {
      console.error('Submit role application error:', error);
      throw error;
    }
  },

  async getMyApplications() {
    try {
      const response = await api.get('/role-applications/my-applications');
      return response.data.data || response.data; // Handle ApiResponse wrapper
    } catch (error) {
      console.error('Get my applications error:', error);
      return [];
    }
  },

  async getAvailableRoles() {
    try {
      const response = await api.get('/role-applications/available-roles');
      return response.data.data || response.data; // Handle ApiResponse wrapper
    } catch (error) {
      console.error('Get available roles error:', error);
      // Return empty array instead of mock data
      return [];
    }
  },

  async getPendingApplications() {
    try {
      const response = await api.get('/role-applications/pending');
      return response.data.data || response.data; // Handle ApiResponse wrapper
    } catch (error) {
      console.error('Get pending applications error:', error);
      return [];
    }
  },

  async getAllApplications() {
    try {
      const response = await api.get('/role-applications/pending');
      return response.data.data || response.data; // Handle ApiResponse wrapper
    } catch (error) {
      console.error('Get all applications error:', error);
      return [];
    }
  },

  async reviewApplication(applicationId: number, reviewData: {
    status: 'Approved' | 'Rejected';
    reviewNotes?: string;
  }) {
    try {
      const response = await api.put(`/role-applications/${applicationId}/review`, {
        Status: reviewData.status,        // Match backend DTO
        ReviewNotes: reviewData.reviewNotes
      });
      return response.data;
    } catch (error) {
      console.error('Review application error:', error);
      throw error;
    }
  },

  async cancelApplication(applicationId: number) {
    try {
      const response = await api.delete(`/role-applications/${applicationId}`);
      return response.data;
    } catch (error) {
      console.error('Cancel application error:', error);
      throw error;
    }
  }
};

// ===== CONTAINER API =====
export { containerApi } from './containerApi';

// ===== SHIP API =====
export { shipApi } from './shipApi';

// ===== BERTH API ===== 
export { berthApi } from './berthApi';

// ===== BERTH ASSIGNMENT API =====
export { berthAssignmentApi } from './berthAssignmentApi';

// ===== PORT API =====
export { portApi } from './portApi';

// ===== CREW API =====
export { crewApi } from './crewApi';

// User management - re-export existing service
export { userManagementApi };

// Analytics service - re-export analytics functionality
export { analyticsService } from './analyticsService';

// ===== TYPE EXPORTS =====
export type { Container, ContainerFilters, ContainerStats, PaginatedResponse } from '../types/container';
export type { Ship } from './shipService';
export type { Port, PortCreateUpdate, PortDetail } from '../types/port';
export type { Berth, BerthCreateUpdate, BerthAssignment } from '../types/berth';
export type { 
  UserListDto, 
  UpdateUserRolesDto, 
  UpdateUserStatusDto, 
  SystemStatsDto, 
  UsersPagedResponse 
} from './userManagementApi';

// ===== INTERFACE DEFINITIONS =====
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

export interface RoleApplicationDto {
  id: number
  userId: number
  userName: string
  userEmail: string
  requestedRole: string
  justification: string
  status: 'Pending' | 'Approved' | 'Rejected'
  requestedAt: string
  reviewedAt?: string
  reviewedBy?: number
  reviewerName?: string
  reviewComments?: string
}


// Export the base API client for direct access when needed
export { api as apiClient };
export default api;