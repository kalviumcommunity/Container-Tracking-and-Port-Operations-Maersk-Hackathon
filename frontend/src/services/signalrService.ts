import * as signalR from '@microsoft/signalr';
import type { Event, EventStats } from '@/types/event';

export interface EventSubscription {
  categories?: string[];
  severities?: string[];
}

export class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private eventHandlers: Map<string, Function[]> = new Map();
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 10;
  private reconnectDelay = 2000;
  private isConnecting = false;

  constructor(private baseUrl: string = 'http://localhost:5221') {}

  /**
   * Initialize and start SignalR connection
   */
  async connect(): Promise<void> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      console.log('SignalR already connected');
      return;
    }

    if (this.isConnecting) {
      console.log('SignalR connection in progress...');
      return;
    }

    this.isConnecting = true;

    try {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${this.baseUrl}/hubs/events`, {
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling,
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            // Exponential backoff: 2s, 4s, 8s, 16s, 30s, 30s...
            if (retryContext.previousRetryCount < 4) {
              return Math.min(2000 * Math.pow(2, retryContext.previousRetryCount), 30000);
            }
            return 30000;
          },
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // Set up connection event handlers
      this.setupConnectionHandlers();

      // Set up message handlers
      this.setupMessageHandlers();

      // Start the connection
      await this.connection.start();
      console.log('✅ SignalR Connected successfully');
      this.reconnectAttempts = 0;
      this.isConnecting = false;
    } catch (error) {
      console.error('❌ SignalR Connection failed:', error);
      this.isConnecting = false;
      await this.handleReconnect();
    }
  }

  /**
   * Set up connection lifecycle event handlers
   */
  private setupConnectionHandlers(): void {
    if (!this.connection) return;

    this.connection.onclose(async (error) => {
      console.warn('SignalR connection closed', error);
      await this.handleReconnect();
    });

    this.connection.onreconnecting((error) => {
      console.warn('SignalR reconnecting...', error);
      this.triggerEvent('reconnecting', error);
    });

    this.connection.onreconnected((connectionId) => {
      console.log('✅ SignalR reconnected with connectionId:', connectionId);
      this.reconnectAttempts = 0;
      this.triggerEvent('reconnected', connectionId);
      
      // Re-subscribe to groups after reconnection
      this.resubscribeToGroups();
    });
  }

  /**
   * Set up message handlers for different event types
   */
  private setupMessageHandlers(): void {
    if (!this.connection) return;

    // Handle incoming events
    this.connection.on('ReceiveEvent', (event: Event) => {
      console.log('📨 Received event:', event);
      this.triggerEvent('event', event);
    });

    // Handle event statistics updates
    this.connection.on('ReceiveEventStats', (stats: EventStats) => {
      console.log('📊 Received event stats:', stats);
      this.triggerEvent('stats', stats);
    });

    // Handle connection count updates
    this.connection.on('ReceiveConnectionCount', (count: number) => {
      console.log('👥 Active connections:', count);
      this.triggerEvent('connectionCount', count);
    });
  }

  /**
   * Handle reconnection with exponential backoff
   */
  private async handleReconnect(): Promise<void> {
    if (this.reconnectAttempts >= this.maxReconnectAttempts) {
      console.error('❌ Max reconnection attempts reached. Please refresh the page.');
      this.triggerEvent('maxReconnectAttemptsReached');
      return;
    }

    this.reconnectAttempts++;
    const delay = Math.min(this.reconnectDelay * Math.pow(2, this.reconnectAttempts - 1), 30000);
    
    console.log(`Attempting to reconnect (${this.reconnectAttempts}/${this.maxReconnectAttempts}) in ${delay}ms...`);
    
    await new Promise(resolve => setTimeout(resolve, delay));
    await this.connect();
  }

  /**
   * Subscribe to specific event categories
   */
  async subscribeToCategories(categories: string[]): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.warn('Cannot subscribe: SignalR not connected');
      return;
    }

    try {
      await this.connection.invoke('SubscribeToCategories', categories);
      console.log('✅ Subscribed to categories:', categories);
    } catch (error) {
      console.error('Failed to subscribe to categories:', error);
    }
  }

  /**
   * Unsubscribe from specific event categories
   */
  async unsubscribeFromCategories(categories: string[]): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.warn('Cannot unsubscribe: SignalR not connected');
      return;
    }

    try {
      await this.connection.invoke('UnsubscribeFromCategories', categories);
      console.log('✅ Unsubscribed from categories:', categories);
    } catch (error) {
      console.error('Failed to unsubscribe from categories:', error);
    }
  }

  /**
   * Subscribe to specific severity levels
   */
  async subscribeToSeverities(severities: string[]): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.warn('Cannot subscribe: SignalR not connected');
      return;
    }

    try {
      await this.connection.invoke('SubscribeToSeverities', severities);
      console.log('✅ Subscribed to severities:', severities);
    } catch (error) {
      console.error('Failed to subscribe to severities:', error);
    }
  }

  /**
   * Re-subscribe to groups after reconnection
   */
  private async resubscribeToGroups(): Promise<void> {
    // You can store subscriptions and re-apply them here
    console.log('Re-subscribing to groups after reconnection...');
    // Implementation depends on how you want to persist subscriptions
  }

  /**
   * Register an event handler
   */
  on(eventName: string, handler: Function): void {
    if (!this.eventHandlers.has(eventName)) {
      this.eventHandlers.set(eventName, []);
    }
    this.eventHandlers.get(eventName)!.push(handler);
  }

  /**
   * Unregister an event handler
   */
  off(eventName: string, handler?: Function): void {
    if (!handler) {
      this.eventHandlers.delete(eventName);
      return;
    }

    const handlers = this.eventHandlers.get(eventName);
    if (handlers) {
      const index = handlers.indexOf(handler);
      if (index > -1) {
        handlers.splice(index, 1);
      }
    }
  }

  /**
   * Trigger event handlers
   */
  private triggerEvent(eventName: string, data?: any): void {
    const handlers = this.eventHandlers.get(eventName);
    if (handlers) {
      handlers.forEach(handler => handler(data));
    }
  }

  /**
   * Get connection state
   */
  getState(): signalR.HubConnectionState {
    return this.connection?.state ?? signalR.HubConnectionState.Disconnected;
  }

  /**
   * Check if connected
   */
  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  /**
   * Disconnect from SignalR hub
   */
  async disconnect(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop();
        console.log('SignalR disconnected');
      } catch (error) {
        console.error('Error disconnecting from SignalR:', error);
      }
    }
  }
}

// Export a singleton instance
const signalRService = new SignalRService(import.meta.env.VITE_API_BASE_URL || 'http://localhost:5221');

export default signalRService;
