import { renderHook, act } from '@testing-library/react';
import { useCart } from './useCart';
import { Product } from '../types';

const mockProduct = (overrides?: Partial<Product>): Product => ({
  id: 1,
  name: 'Capinha iPhone 15',
  price: 49.90,
  stock: 5,
  ...overrides,
});

describe('useCart', () => {
  it('addToCart: adds product and returns true', () => {
    const { result } = renderHook(() => useCart());

    let returned: boolean | undefined;
    act(() => { returned = result.current.addToCart(mockProduct()); });

    expect(returned).toBe(true);
    expect(result.current.cart).toHaveLength(1);
    expect(result.current.cart[0].quantity).toBe(1);
  });

  it('addToCart: returns false when stock is 0', () => {
    const { result } = renderHook(() => useCart());

    let returned: boolean | undefined;
    act(() => { returned = result.current.addToCart(mockProduct({ stock: 0 })); });

    expect(returned).toBe(false);
    expect(result.current.cart).toHaveLength(0);
  });

  it('increaseQuantity: increases when below stock', () => {
    const { result } = renderHook(() => useCart());

    act(() => { result.current.addToCart(mockProduct({ stock: 5 })); });
    act(() => { result.current.increaseQuantity(1); });

    expect(result.current.cart[0].quantity).toBe(2);
  });

  it('decreaseQuantity: removes item when quantity is 1', () => {
    const { result } = renderHook(() => useCart());

    act(() => { result.current.addToCart(mockProduct()); });
    act(() => { result.current.decreaseQuantity(1); });

    expect(result.current.cart).toHaveLength(0);
  });
});
