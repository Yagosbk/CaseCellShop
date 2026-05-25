import { Product, CheckoutResponse } from '../types';

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5111';

async function handleResponse(response: Response): Promise<any> {
  if (response.ok) return response.json();

  const body = await response.json().catch(() => null);
  const message = body?.message ?? 'Erro desconhecido';
  const error: any = new Error(`${message} (Status code: ${response.status})`);
  error.status = response.status;
  throw error;
}

export const getProducts = async (): Promise<Product[]> => {
  try {
    const response = await fetch(`${API_URL}/products`);
    return handleResponse(response);
  } catch (error) {
    if (error instanceof TypeError)
      throw new Error('Sem conexão com o servidor. StatusCode: 500');
    throw error;
  }
};

export const getProductById = async (id: number): Promise<Product> => {
  try {
    const response = await fetch(`${API_URL}/products/${id}`);
    return handleResponse(response);
  } catch (error) {
    if (error instanceof TypeError)
      throw new Error('Sem conexão com o servidor. StatusCode: 500');
    throw error;
  }
};

export const createCartCheckout = async (
  items: { productId: number; quantity: number }[],
  idempotencyKey?: string
): Promise<CheckoutResponse> => {
  try {
    const headers: Record<string, string> = { 'Content-Type': 'application/json' };
    if (idempotencyKey) headers['Idempotency-Key'] = idempotencyKey;

    const response = await fetch(`${API_URL}/checkout/cart`, {
      method: 'POST',
      headers,
      body: JSON.stringify({ items }),
    });
    return handleResponse(response);
  } catch (error) {
    if (error instanceof TypeError)
      throw new Error('Sem conexão com o servidor. StatusCode: 500');
    throw error;
  }
};
