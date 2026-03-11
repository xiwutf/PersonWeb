/**
 * AI Assistant Module Types
 */

export interface AIMessage {
  id: string
  role: 'user' | 'assistant' | 'system'
  content: string
  timestamp: Date
  loading?: boolean
}

export interface AIConfig {
  apiKey: string
  model: string
  temperature: number
  maxTokens: number
  systemPrompt: string
  enableQuickActions: boolean
  enableHistory: boolean
  maxHistoryLength: number
  theme: string
  position: string
}

export interface QuickAction {
  text: string
  icon: string
}