import { useState, useEffect } from 'react';
import './App.css';
import { getProducts, createCartCheckout } from './services/api';
import { Product } from './types';
import { useCart } from './hooks/useCart';
import CheckoutPage from './pages/CheckoutPage';
import Toast from './components/Toast';

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [message, setMessage] = useState<string>('');
  const [messageType, setMessageType] = useState<'success' | 'error' | 'info'>('info');
  const [apiError, setApiError] = useState<string>('');
  const [currentPage, setCurrentPage] = useState<'home' | 'checkout'>('home');
  const { cart, addToCart, increaseQuantity, decreaseQuantity, clearCart, isLoaded } = useCart();

  const showToast = (text: string, type: 'success' | 'error' | 'info' = 'info') => {
    setMessage(text);
    setMessageType(type);
  };

  const productImages: Record<number, string> = {
    1:  'https://images.unsplash.com/photo-1601593346740-925612772716?w=500',
    2:  'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=500',
    3:  'https://images.unsplash.com/photo-1574944985070-8f3ebc6b79d2?w=500',
    4:  'https://images.unsplash.com/photo-1580910051074-3eb694886505?w=500',
    5:  'https://images.unsplash.com/photo-1585399000684-d2f72660f092?w=500',
    6:  'https://images.unsplash.com/photo-1556656793-08538906a9f8?w=500',
    7:  'https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=500',
    8:  'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=500',
    9:  'https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=500',
    10: 'https://images.unsplash.com/photo-1548438294-1ad5d5f4f063?w=500',
    11: 'https://images.unsplash.com/photo-1518770660439-4636190af475?w=500',
    12: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=500',
  };

  const getAvailableStock = (product: Product): number => {
    const cartItem = cart.find(item => item.id === product.id);
    return product.stock - (cartItem?.quantity ?? 0);
  };

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        setApiError('');
        const data = await getProducts();
        setProducts(data);
      } catch (error) {
        setApiError((error as Error).message);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  const openCheckout = (product: Product) => {
    const existingItem = cart.find(item => item.id === product.id);

    if (!existingItem) {
      addToCart(product);
    }

    setCurrentPage('checkout');
  };

  const goToCheckout = () => {
    if (cart.length === 0) {
      showToast('Seu carrinho está vazio. Adicione produtos antes de finalizar a compra.', 'error');
      return;
    }

    setCurrentPage('checkout');
  };

  const handleBackToHome = () => {
    setCurrentPage('home');
  };

  const handleConfirmPurchase = async () => {
    if (cart.length === 0) {
      showToast('Seu carrinho está vazio. Adicione produtos antes de finalizar a compra.', 'error');
      return;
    }

    setMessage('');
    setApiError('');
    setLoading(true);

    try {
      const idempotencyKey = `ck_${Date.now()}_${Math.random().toString(16).slice(2)}`;

      try {
        localStorage.setItem(`casecellshop_pending_${idempotencyKey}`, JSON.stringify(cart));
      } catch {}

      await createCartCheckout(cart.map(item => ({ productId: item.id, quantity: item.quantity })), idempotencyKey);

      const updatedProducts = await getProducts();
      setProducts(updatedProducts);

      try {
        localStorage.removeItem(`casecellshop_pending_${idempotencyKey}`);
      } catch {}

      clearCart();
      setCurrentPage('home');
      showToast('Compra concluída com sucesso!', 'success');
    } catch (error) {
      const err = error as Error & { status?: number };

      if (err.status === 404)
        showToast('Produto não encontrado no carrinho. Atualize a página e tente novamente. (Status code: 404)', 'error');
      else if (err.status === 422)
        showToast('Estoque insuficiente para um ou mais itens do carrinho. Ajuste a quantidade e tente novamente. (Status code: 422)', 'error');
      else
        showToast(err.message, 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = (product: Product) => {
    const success = addToCart(product);
    if (success) {
      showToast(`${product.name} adicionado ao carrinho!`, 'success');
    } else {
      showToast(`Quantidade máxima de ${product.name} atingida.`, 'error');
    }
  };

  const cartTotal = cart.reduce((total, item) => total + item.quantity, 0);

  if (!isLoaded) {
    return <div className="loading">Carregando...</div>;
  }

  return (
    <div className="App">
      <header className="header">
        <h1>CaseCellShop</h1>
        <div className="header-right">
          <div className="cart">
            <img src="/cart-icon.png" alt="Carrinho" className="cart-icon" />
            {cartTotal}
          </div>
          {cartTotal > 0 && (
            <button className="checkout-button" onClick={goToCheckout}>
              Finalizar compra
            </button>
          )}
        </div>
      </header>

      {apiError && (
        <div className="message error">{apiError}</div>
      )}

      {currentPage === 'checkout' ? (
        <CheckoutPage
          cart={cart}
          loading={loading}
          onConfirmPurchase={handleConfirmPurchase}
          onBack={handleBackToHome}
          onIncreaseQuantity={increaseQuantity}
          onDecreaseQuantity={decreaseQuantity}
        />
      ) : loading ? (
        <div className="loading">Carregando produtos...</div>
      ) : (
        <div className="products-grid">
          {products.length > 0 ? (
            products.map(product => (
              <div className="product-card" key={product.id}>
                <img
                  src={productImages[product.id] ?? 'https://images.unsplash.com/photo-1601593346740-925612772716?w=500'}
                  alt={product.name}
                />
                <h2>{product.name}</h2>
                <p>R$ {product.price.toFixed(2)}</p>
                <div className="stock-info">
                  <span className="stock-label">Disponível:</span>
                  <span className={`stock-number ${getAvailableStock(product) <= 3 ? 'low' : 'available'}`}>
                    {getAvailableStock(product)} {getAvailableStock(product) === 1 ? 'unidade' : 'unidades'}
                  </span>
                </div>

                <button
                  onClick={() => openCheckout(product)}
                  disabled={getAvailableStock(product) <= 0}
                >
                  {getAvailableStock(product) > 0 ? 'Comprar' : 'Sem estoque'}
                </button>

                <button className="secondary-button" onClick={() => handleAddToCart(product)} disabled={getAvailableStock(product) <= 0}>
                  Adicionar ao carrinho
                </button>

                {getAvailableStock(product) <= 3 && getAvailableStock(product) > 0 && (
                  <span className="low-stock">Últimas unidades!</span>
                )}
              </div>
            ))
          ) : (
            <p>Nenhum produto disponível</p>
          )}
        </div>
      )}

      {message && (
        <Toast
          message={message}
          type={messageType}
          onClose={() => setMessage('')}
        />
      )}
    </div>
  );
}

export default App;
