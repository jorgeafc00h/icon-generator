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
      const token = localStorage.getItem('accessToken')
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
          localStorage.removeItem('accessToken')
          localStorage.removeItem('userId')
          localStorage.removeItem('userEmail')
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  // Icon Generation
  async generateIcon(request: IconGenerationRequest): Promise<IconGenerationResponse> {
    const { data } = await this.client.post<IconGenerationResponse>(
      '/icons/generate',
      request
    )
    return data
  }

  async enhancePrompt(request: Pick<IconGenerationRequest, 'keywords' | 'style' | 'colors'>): Promise<string> {
    const { data } = await this.client.post<{ enhancedPrompt: string }>(
      '/icons/enhance-prompt',
      request
    )
    return data.enhancedPrompt
  }

  // App Resources
  async generateAppResources(request: AppResourcesRequest): Promise<AppResourcesResponse> {
    const { data } = await this.client.post<AppResourcesResponse>(
      '/resources/generate',
      request
    )
    return data
  }

  async analyzeIcon(iconId: string): Promise<any> {
    const { data } = await this.client.get(`/icons/${iconId}/analyze`)
    return data
  }

  // User Management
  async getUser(): Promise<User> {
    const userId = localStorage.getItem('userId')
    if (!userId) {
      throw new Error('User ID not found')
    }
    const { data } = await this.client.get<User>(`/users/${userId}`)
    return data
  }

  async getUserIcons(): Promise<IconGenerationResponse[]> {
    const userId = localStorage.getItem('userId')
    if (!userId) {
      throw new Error('User ID not found')
    }
    const { data } = await this.client.get<IconGenerationResponse[]>(`/users/${userId}/icons`)
    return data
  }

  // Payments
  async createCheckoutSession(planId: string): Promise<{ url: string }> {
    const { data } = await this.client.post<{ url: string }>(
      '/payments/checkout',
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

  // App Resources Generation
  async generateAppResourcesV2(request: any): Promise<any> {
    const { data } = await this.client.post('/app-resources/generate', request)
    return data
  }

  // Chat Interface for Iterative Screen Generation
  async chatGenerateScreen(request: { sessionId: string; message: string }): Promise<any> {
    const { data} = await this.client.post('/app-resources/chat', request)
    return data
  }

  async getChatSession(sessionId: string): Promise<any> {
    const { data } = await this.client.get(`/app-resources/sessions/${sessionId}`)
    return data
  }

  async getUserChatSessions(): Promise<any[]> {
    const { data } = await this.client.get('/app-resources/sessions')
    return data
  }

  // Story Image Generation
  async getImageStyles(): Promise<any[]> {
    const { data } = await this.client.get('/images/styles')
    return data
  }

  async enhanceImagePrompt(request: any): Promise<any> {
    const { data } = await this.client.post('/images/enhance-prompt', request)
    return data
  }

  async generatePreviewImages(request: any): Promise<any> {
    const { data } = await this.client.post('/images/generate-preview', request)
    return data
  }

  async generateFinalImages(request: any): Promise<any> {
    const { data } = await this.client.post('/images/generate-final', request)
    return data
  }
}

export const api = new ApiClient()
