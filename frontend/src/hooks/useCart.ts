import { useState } from 'react';
import { CartItem, Product } from '../types';

export const useCart = () => {
  const [cart, setCart] = useState<CartItem[]>([]);
  const [isLoaded] = useState(true);

  const addToCart = (product: Product): boolean => {
    if (product.stock <= 0) {
      return false;
    }

    const existingItem = cart.find(item => item.id === product.id);

    if (existingItem) {
      if (existingItem.quantity < product.stock) {
        setCart(cart.map(item =>
          item.id === product.id ? { ...item, quantity: item.quantity + 1 } : item
        ));
        return true;
      }
      return false;
    } else {
      setCart([...cart, { ...product, quantity: 1 }]);
      return true;
    }
  };

  const increaseQuantity = (productId: number) => {
    setCart(cart.map(item => {
      if (item.id === productId && item.quantity < item.stock) {
        return { ...item, quantity: item.quantity + 1 };
      }
      return item;
    }));
  };

  const decreaseQuantity = (productId: number) => {
    setCart(cart.filter(item => {
      if (item.id === productId) {
        return item.quantity > 1 ? true : false;
      }
      return true;
    }).map(item =>
      item.id === productId ? { ...item, quantity: item.quantity - 1 } : item
    ));
  };

  const clearCart = () => {
    setCart([]);
  };

  return {
    cart,
    setCart,
    addToCart,
    increaseQuantity,
    decreaseQuantity,
    clearCart,
    isLoaded,
  };
};
