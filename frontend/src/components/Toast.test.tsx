import { render, screen, act } from '@testing-library/react';
import Toast from './Toast';

describe('Toast', () => {
  it('renders the message with the correct type class', () => {
    const onClose = jest.fn();
    render(<Toast message="Compra concluída!" type="success" onClose={onClose} />);

    const el = screen.getByRole('alert');
    expect(el).toHaveTextContent('Compra concluída!');
    expect(el).toHaveClass('toast', 'success');
  });

  it('calls onClose after 3 seconds', () => {
    jest.useFakeTimers();
    const onClose = jest.fn();

    render(<Toast message="Mensagem" onClose={onClose} />);

    expect(onClose).not.toHaveBeenCalled();

    act(() => { jest.advanceTimersByTime(3000); });

    expect(onClose).toHaveBeenCalledTimes(1);
    jest.useRealTimers();
  });
});
