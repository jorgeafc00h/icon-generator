import axios, { type AxiosInstance } from 'axios'
import type {
  IconGenerationRequest,
  IconGenerationResponse,
  AppResourcesRequest,
  AppResourcesResponse,
  User,
} from '../types'

const API_BASE_URL = import.meta.env.VITE_API_ENDPOINT || 'http://localhost:7071/api'

class ApiClient {
  private client: AxiosInstance

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    // Add request interceptor for auth
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('auth_token')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    })

    // Add response interceptor for error handling
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Handle unauthorized
          localStorage.removeItem('auth_token')
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  // Icon Generation
  async generateIcon(request: IconGenerationRequest): Promise<IconGenerationResponse> {
    const { data } = await this.client.post<IconGenerationResponse>(
      '/generate-icon',
      request
    )
    return data
  }

  async enhancePrompt(request: Pick<IconGenerationRequest, 'keywords' | 'style' | 'colors'>): Promise<string> {
    const { data } = await this.client.post<{ enhancedPrompt: string }>(
      '/enhance-prompt',
      request
    )
    return data.enhancedPrompt
  }

  // App Resources
  async generateAppResources(request: AppResourcesRequest): Promise<AppResourcesResponse> {
    const { data } = await this.client.post<AppResourcesResponse>(
      '/generate-app-resources',
      request
    )
    return data
  }

  async analyzeIcon(iconId: string): Promise<any> {
    const { data } = await this.client.get(`/analyze-icon/${iconId}`)
    return data
  }

  // User Management
  async getUser(): Promise<User> {
    const { data } = await this.client.get<User>('/user')
    return data
  }

  async getUserIcons(): Promise<IconGenerationResponse[]> {
    const { data } = await this.client.get<IconGenerationResponse[]>('/user/icons')
    return data
  }

  // Payments
  async createCheckoutSession(planId: string): Promise<{ url: string }> {
    const { data } = await this.client.post<{ url: string }>(
      '/create-checkout-session',
      { planId }
    )
    return data
  }

  // File Downloads
  async downloadZip(url: string): Promise<Blob> {
    const { data } = await this.client.get(url, {
      responseType: 'blob',
    })
    return data
  }
}

export const api = new ApiClient()
