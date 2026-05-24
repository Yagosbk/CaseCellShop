import { render, screen, fireEvent } from '@testing-library/react';
import CheckoutPage from './CheckoutPage';
import { CartItem } from '../types';

const mockItem = (overrides?: Partial<CartItem>): CartItem => ({
  id: 1,
  name: 'Capinha iPhone 15',
  price: 49.90,
  stock: 5,
  quantity: 2,
  ...overrides,
});

const defaultProps = {
  cart: [mockItem()],
  loading: false,
  onConfirmPurchase: jest.fn(),
  onBack: jest.fn(),
  onIncreaseQuantity: jest.fn(),
  onDecreaseQuantity: jest.fn(),
};

afterEach(() => jest.clearAllMocks());

describe('CheckoutPage', () => {
  it('renders cart items with name, price and total correctly', () => {
    render(<CheckoutPage {...defaultProps} />);

    expect(screen.getByText('Capinha iPhone 15')).toBeInTheDocument();
    expect(screen.getByText('R$ 49.90')).toBeInTheDocument();
    expect(screen.getByText('R$ 99.80')).toBeInTheDocument();

    render(<CheckoutPage {...defaultProps} cart={[]} />);
    expect(screen.getByText('Seu carrinho está vazio.')).toBeInTheDocument();
  });

  it('buttons call correct callbacks and disable at limits', () => {
    const { rerender } = render(<CheckoutPage {...defaultProps} />);

    fireEvent.click(screen.getByText('Confirmar compra'));
    expect(defaultProps.onConfirmPurchase).toHaveBeenCalledTimes(1);

    fireEvent.click(screen.getByText('Voltar'));
    expect(defaultProps.onBack).toHaveBeenCalledTimes(1);

    fireEvent.click(screen.getByText('+'));
    expect(defaultProps.onIncreaseQuantity).toHaveBeenCalledWith(1);

    fireEvent.click(screen.getByText('−'));
    expect(defaultProps.onDecreaseQuantity).toHaveBeenCalledWith(1);

    rerender(<CheckoutPage {...defaultProps} cart={[mockItem({ quantity: 5, stock: 5 })]} />);
    expect(screen.getByText('+')).toBeDisabled();

    rerender(<CheckoutPage {...defaultProps} loading={true} />);
    expect(screen.getByText('Processando...')).toBeDisabled();
  });
});
