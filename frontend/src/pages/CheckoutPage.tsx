import { CartItem } from '../types';

interface CheckoutPageProps {
  cart: CartItem[];
  loading: boolean;
  onConfirmPurchase: () => void;
  onBack: () => void;
  onIncreaseQuantity: (productId: number) => void;
  onDecreaseQuantity: (productId: number) => void;
}

const CheckoutPage = ({ 
  cart, 
  loading, 
  onConfirmPurchase,
  onBack,
  onIncreaseQuantity,
  onDecreaseQuantity
}: CheckoutPageProps) => {
  const cartTotal = cart.reduce((total, item) => total + item.quantity, 0);
  const cartTotalValue = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <div className="checkout-container">
      <h1>Resumo do pedido</h1>

      {cart.length > 0 ? (
        <>
          <div className="checkout-items">
            {cart.map(item => (
              <div className="checkout-item" key={item.id}>
                <div className="checkout-item-info">
                  <strong>{item.name}</strong>
                  <p>R$ {item.price.toFixed(2)}</p>
                </div>
                <div className="checkout-item-controls">
                  <button 
                    className="quantity-button"
                    onClick={() => onDecreaseQuantity(item.id)}
                    disabled={loading}
                  >
                    −
                  </button>
                  <span className="quantity-display">{item.quantity}</span>
                  <button 
                    className="quantity-button"
                    onClick={() => onIncreaseQuantity(item.id)}
                    disabled={loading || item.quantity >= item.stock}
                  >
                    +
                  </button>
                </div>
                <div className="checkout-item-total">
                  <span className={`stock-number ${item.stock <= 3 ? 'low' : 'available'}`}>
                    Estoque: {item.stock}
                  </span>
                  <span className="item-total">R$ {(item.price * item.quantity).toFixed(2)}</span>
                </div>
              </div>
            ))}
          </div>

          <div className="checkout-summary">
            <p>Itens: {cartTotal}</p>
            <p>Total: R$ {cartTotalValue.toFixed(2)}</p>
          </div>

          <div className="checkout-actions">
            <button onClick={onConfirmPurchase} disabled={loading}>
              {loading ? 'Processando...' : 'Confirmar compra'}
            </button>
            <button className="secondary-button" onClick={onBack}>
              Voltar
            </button>
          </div>
        </>
      ) : (
        <>
          <p>Seu carrinho está vazio.</p>
          <div className="checkout-actions">
            <button className="secondary-button" onClick={onBack}>
              Voltar
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default CheckoutPage;
