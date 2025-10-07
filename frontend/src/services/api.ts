import axios, { AxiosResponse } from 'axios';

// API Configuration
const API_BASE_URL = window.location.hostname === 'localhost' 
  ? 'http://localhost:5221/api'
  : '/api';

// TypeScript interfaces for API responses
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

// Authentication interfaces
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  fullName: string;
  phoneNumber?: string;
  department?: string;
  portId?: number;
  roles: string[];
}

export interface AuthResponse {
  token: string;
  expires: string;
  user: User;
}

export interface User {
  userId: number;
  username: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  department?: string;
  portId?: number;
  roles: string[];
  permissions: string[];
  isActive: boolean;
  lastLoginAt?: string;
  createdAt: string;
}

export interface Container {
  containerId: string;
  name: string;
  type: string;
  status: string;
  currentLocation: string;
  createdAt: string;
  updatedAt: string;
  shipId?: number;
  shipName?: string;
  weight?: number;
  destination?: string;
}

export interface Port {
  id: number;
  name: string;
  code: string;
  country: string;
  location?: string;
  capacity?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface Ship {
  id: number;
  name: string;
  imoNumber?: string;
  flag?: string;
  type?: string;
  capacity?: number;
  status?: string;
  portId?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface Berth {
  id: number;
  identifier: string;
  type: string;
  status: string;
  capacity?: number;
  portId: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface BerthAssignment {
  id: number;
  berthId: number;
  shipId: number;
  assignedAt: string;
  scheduledDeparture?: string;
  actualDeparture?: string;
  status: string;
  createdAt?: string;
  updatedAt?: string;
}

// Create axios instance with TypeScript support
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000, // 10 seconds timeout
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
    return Promise.reject(error);
  }
);

// Container API
export const containerApi = {
  async getAll(): Promise<{ data: Container[] }> {
    try {
      const response = await api.get('/containers');
      // Handle both direct array and ApiResponse wrapper
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('Error fetching containers:', error);
      return { data: [] };
    }
  },

  async getContainers(): Promise<Container[]> {
    try {
      const response = await api.get('/Containers');
      // Handle the API response structure
      return response.data.data || response.data || [];
    } catch (error) {
      console.error('Error fetching containers:', error);
      return [];
    }
  },

  async getById(id: number): Promise<Container | null> {
    try {
      const response = await api.get(`/containers/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching container ${id}:`, error);
      return null;
    }
  },

  async getDetails(id: number): Promise<Container | null> {
    try {
      const response = await api.get(`/containers/${id}/details`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching container details ${id}:`, error);
      return null;
    }
  },

  async getByLocation(location: string): Promise<Container[]> {
    try {
      const response = await api.get(`/containers/location/${location}`);
      return response.data.data || response.data || [];
    } catch (error) {
      console.error(`Error fetching containers by location ${location}:`, error);
      return [];
    }
  },

  async getByStatus(status: string): Promise<Container[]> {
    try {
      const response = await api.get(`/containers/status/${status}`);
      return response.data.data || response.data || [];
    } catch (error) {
      console.error(`Error fetching containers by status ${status}:`, error);
      return [];
    }
  },

  async getByShip(shipId: number): Promise<Container[]> {
    try {
      const response = await api.get(`/containers/ship/${shipId}`);
      return response.data.data || response.data || [];
    } catch (error) {
      console.error(`Error fetching containers by ship ${shipId}:`, error);
      return [];
    }
  },

  async create(containerData: Partial<Container>): Promise<Container | null> {
    try {
      const response = await api.post('/containers', containerData);
      return response.data.data || response.data;
    } catch (error) {
      console.error('Error creating container:', error);
      throw error;
    }
  },

  async update(id: number, containerData: Partial<Container>): Promise<Container | null> {
    try {
      const response = await api.put(`/containers/${id}`, containerData);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error updating container ${id}:`, error);
      throw error;
    }
  },

  async delete(id: number): Promise<boolean> {
    try {
      await api.delete(`/containers/${id}`);
      return true;
    } catch (error) {
      console.error(`Error deleting container ${id}:`, error);
      throw error;
    }
  }
};

// Port API
export const portApi = {
  async getAll(): Promise<{ data: Port[] }> {
    try {
      const response = await api.get('/ports');
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('Error fetching ports:', error);
      return { data: [] };
    }
  },

  async getById(id: number): Promise<Port | null> {
    try {
      const response = await api.get(`/ports/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching port ${id}:`, error);
      return null;
    }
  },

  async getDetails(id: number): Promise<Port | null> {
    try {
      const response = await api.get(`/ports/${id}/details`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching port details ${id}:`, error);
      return null;
    }
  },

  async create(portData: Partial<Port>): Promise<Port | null> {
    try {
      const response = await api.post('/ports', portData);
      return response.data.data || response.data;
    } catch (error) {
      console.error('Error creating port:', error);
      throw error;
    }
  },

  async update(id: number, portData: Partial<Port>): Promise<Port | null> {
    try {
      const response = await api.put(`/ports/${id}`, portData);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error updating port ${id}:`, error);
      throw error;
    }
  }
};

// Ship API
export const shipApi = {
  async getAll(): Promise<{ data: Ship[] }> {
    try {
      const response = await api.get('/ships');
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('Error fetching ships:', error);
      return { data: [] };
    }
  },

  async getById(id: number): Promise<Ship | null> {
    try {
      const response = await api.get(`/ships/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching ship ${id}:`, error);
      return null;
    }
  },

  async getDetails(id: number): Promise<Ship | null> {
    try {
      const response = await api.get(`/ships/${id}/details`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching ship details ${id}:`, error);
      return null;
    }
  },

  async create(shipData: Partial<Ship>): Promise<Ship | null> {
    try {
      const response = await api.post('/ships', shipData);
      return response.data.data || response.data;
    } catch (error) {
      console.error('Error creating ship:', error);
      throw error;
    }
  },

  async update(id: number, shipData: Partial<Ship>): Promise<Ship | null> {
    try {
      const response = await api.put(`/ships/${id}`, shipData);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error updating ship ${id}:`, error);
      throw error;
    }
  }
};

// Berth API
export const berthApi = {
  async getAll(): Promise<{ data: Berth[] }> {
    try {
      const response = await api.get('/berths');
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('Error fetching berths:', error);
      return { data: [] };
    }
  },

  async getById(id: number): Promise<Berth | null> {
    try {
      const response = await api.get(`/berths/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching berth ${id}:`, error);
      return null;
    }
  },

  async getByPort(portId: number): Promise<Berth[]> {
    try {
      const response = await api.get(`/berths/port/${portId}`);
      return response.data.data || response.data || [];
    } catch (error) {
      console.error(`Error fetching berths by port ${portId}:`, error);
      return [];
    }
  },

  async create(berthData: Partial<Berth>): Promise<Berth | null> {
    try {
      const response = await api.post('/berths', berthData);
      return response.data.data || response.data;
    } catch (error) {
      console.error('Error creating berth:', error);
      throw error;
    }
  },

  async update(id: number, berthData: Partial<Berth>): Promise<Berth | null> {
    try {
      const response = await api.put(`/berths/${id}`, berthData);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error updating berth ${id}:`, error);
      throw error;
    }
  }
};

// Berth Assignment API
export const berthAssignmentApi = {
  async getAll(): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await api.get('/berthassignments');
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('Error fetching berth assignments:', error);
      return { data: [] };
    }
  },

  async getById(id: number): Promise<BerthAssignment | null> {
    try {
      const response = await api.get(`/berthassignments/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error fetching berth assignment ${id}:`, error);
      return null;
    }
  },

  async getByBerth(berthId: number): Promise<BerthAssignment[]> {
    try {
      const response = await api.get(`/berthassignments/berth/${berthId}`);
      return response.data.data || response.data || [];
    } catch (error) {
      console.error(`Error fetching berth assignments by berth ${berthId}:`, error);
      return [];
    }
  },

  async create(assignmentData: Partial<BerthAssignment>): Promise<BerthAssignment | null> {
    try {
      const response = await api.post('/berthassignments', assignmentData);
      return response.data.data || response.data;
    } catch (error) {
      console.error('Error creating berth assignment:', error);
      throw error;
    }
  },

  async update(id: number, assignmentData: Partial<BerthAssignment>): Promise<BerthAssignment | null> {
    try {
      const response = await api.put(`/berthassignments/${id}`, assignmentData);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`Error updating berth assignment ${id}:`, error);
      throw error;
    }
  },

  async delete(id: number): Promise<boolean> {
    try {
      await api.delete(`/berthassignments/${id}`);
      return true;
    } catch (error) {
      console.error(`Error deleting berth assignment ${id}:`, error);
      throw error;
    }
  }
};

// Authentication API functions
export const authApi = {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await api.post<AuthResponse>('/auth/login', credentials);
      if (response.data.token) {
        setToken(response.data.token);
      }
      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  },

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await api.post<AuthResponse>('/auth/register', userData);
      if (response.data.token) {
        setToken(response.data.token);
      }
      return response.data;
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  },

  async logout(): Promise<void> {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      removeToken();
    }
  },

  async getCurrentUser(): Promise<User | null> {
    try {
      const token = getToken();
      if (!token) return null;
      
      const response = await api.get<User>('/auth/me');
      return response.data;
    } catch (error) {
      console.error('Get current user error:', error);
      removeToken(); // Clear invalid token
      return null;
    }
  },

  isAuthenticated(): boolean {
    return !!getToken();
  }
};

// Export the main axios instance for custom requests
export default api;