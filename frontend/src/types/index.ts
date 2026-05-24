export interface Product {
  id: number;
  name: string;
  price: number;
  stock: number;
}

export interface CartItem extends Product {
  quantity: number;
}

export interface CheckoutResponse {
  message: string;
}
