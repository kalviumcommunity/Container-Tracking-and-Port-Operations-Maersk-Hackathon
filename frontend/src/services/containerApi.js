import axios from 'axios'

// Simple API service just for fetching container data
const API_BASE_URL = 'http://localhost:5221/api'

/**
 * Simple service to fetch container data from the backend
 */
export const containerApi = {
  /**
   * Get all containers from the backend
   * @returns {Promise} Response with containers data
   */
  async getContainers() {
    try {
      const response = await axios.get(`${API_BASE_URL}/Containers`, {
        headers: {
          'Content-Type': 'application/json'
        },
        timeout: 5000 // 5 seconds timeout
      })
      
      // Handle the API response structure
      // Backend likely returns data in ApiResponse<T> format with a 'data' property
      return response.data.data || response.data || []
    } catch (error) {
      console.error('Error fetching containers:', error)
      
      // Return empty array on error so the form still works
      return []
    }
  }
}