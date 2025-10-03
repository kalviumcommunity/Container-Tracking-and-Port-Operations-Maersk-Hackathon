import axios from 'axios';

const API_BASE_URL = 'http://localhost:5221/api';

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Token management
let authToken = localStorage.getItem('authToken');

// Request interceptor to add auth header
api.interceptors.request.use(
  (config) => {
    if (authToken) {
      config.headers.Authorization = `Bearer ${authToken}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      authToken = null;
      localStorage.removeItem('authToken');
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authApi = {
  async login(username, password) {
    try {
      const response = await api.post('/auth/login', {
        username,
        password
      });
      
      if (response.data.success && response.data.data.token) {
        authToken = response.data.data.token;
        localStorage.setItem('authToken', authToken);
      }
      
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  logout() {
    authToken = null;
    localStorage.removeItem('authToken');
  },

  isAuthenticated() {
    return !!authToken;
  }
};

// Container API
export const containerApi = {
  async getAllContainers() {
    try {
      const response = await api.get('/containers');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getContainer(id) {
    try {
      const response = await api.get(`/containers/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getContainerDetails(id) {
    try {
      const response = await api.get(`/containers/${id}/details`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getContainersByLocation(location) {
    try {
      const response = await api.get(`/containers/location/${location}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getContainersByStatus(status) {
    try {
      const response = await api.get(`/containers/status/${status}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getContainersByShip(shipId) {
    try {
      const response = await api.get(`/containers/ship/${shipId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
};

export default api;