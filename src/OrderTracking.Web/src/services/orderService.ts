import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export interface CreateOrderRequest {
  customerName: string;
  amount: number;
  orderDate: string;
}

export interface OrderResponse {
  id: string;
  customerName: string;
  amount: number;
  orderDate: string;
  status: string;
}

export interface CreateOrderResponse {
  id: string;
  message: string;
}

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const orderService = {
  createOrder: async (request: CreateOrderRequest): Promise<CreateOrderResponse> => {
    const response = await api.post<CreateOrderResponse>('/api/orders', request);
    return response.data;
  },

  getOrderById: async (id: string): Promise<OrderResponse> => {
    const response = await api.get<OrderResponse>(`/api/orders/${id}`);
    return response.data;
  },

  getAllOrders: async (): Promise<OrderResponse[]> => {
    const response = await api.get<OrderResponse[]>('/api/orders');
    return response.data;
  },
};

