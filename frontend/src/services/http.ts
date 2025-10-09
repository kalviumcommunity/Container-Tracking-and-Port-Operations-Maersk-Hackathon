import axios, { AxiosInstance, AxiosResponse } from 'axios';

// API Configuration
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? 'https://your-api-domain.com/api' 
  : 'http://localhost:5221/api';

// Create axios instance
export const httpClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Token management
const getToken = (): string | null => {
  const jwtToken = localStorage.getItem('auth_token');
  if (jwtToken) return jwtToken;
  
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    const userData = JSON.parse(currentUser);
    return `mock-jwt-${userData.id}-${userData.username}`;
  }
  
  return null;
};

const setToken = (token: string): void => {
  localStorage.setItem('auth_token', token);
  httpClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
};

const removeToken = (): void => {
  localStorage.removeItem('auth_token');
  localStorage.removeItem('current_user');
  delete httpClient.defaults.headers.common['Authorization'];
};

// Request interceptor
httpClient.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor
httpClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    
    if (error.response?.status === 401) {
      console.warn('Authentication failed - clearing tokens');
      removeToken();
      
      if (window.location.pathname !== '/login' && window.location.pathname !== '/') {
        window.location.href = '/login';
      }
    }
    
    return Promise.reject(error);
  }
);

export { setToken, removeToken };
export default httpClient;
