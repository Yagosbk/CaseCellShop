import { render, screen, waitFor } from '@testing-library/react';
import App from './App';

const mockProducts = [
  { id: 1, name: 'Capinha iPhone 15', price: 49.90, stock: 5 },
  { id: 2, name: 'Capinha Galaxy S24', price: 39.90, stock: 3 },
];

beforeEach(() => {
  localStorage.clear();
  jest.resetAllMocks();
});

describe('App', () => {
  it('renders products after successful fetch', async () => {
    global.fetch = jest.fn().mockResolvedValue({
      ok: true,
      json: jest.fn().mockResolvedValue(mockProducts),
    } as unknown as Response);

    render(<App />);

    expect(await screen.findByText('CaseCellShop')).toBeInTheDocument();
    expect(await screen.findByText('Capinha iPhone 15')).toBeInTheDocument();
    expect(screen.getByText('Capinha Galaxy S24')).toBeInTheDocument();
  });

  it('shows error message when fetch fails with network error', async () => {
    global.fetch = jest.fn().mockRejectedValue(new TypeError('Failed to fetch'));

    render(<App />);

    await waitFor(() => {
      expect(screen.getByText(/Sem conexão com o servidor/i)).toBeInTheDocument();
    });
  });
});
