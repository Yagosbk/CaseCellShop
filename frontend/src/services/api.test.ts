import { getProducts } from './api';

afterEach(() => jest.resetAllMocks());

describe('api', () => {
  it('getProducts: returns product array on 200', async () => {
    const products = [{ id: 1, name: 'Case', price: 49.9, stock: 5 }];
    global.fetch = jest.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: jest.fn().mockResolvedValue(products),
    } as unknown as Response);

    const result = await getProducts();

    expect(result).toEqual(products);
  });

  it('getProducts: throws with status and message on HTTP error, and connection message on network error', async () => {
    global.fetch = jest.fn().mockResolvedValue({
      ok: false,
      status: 503,
      json: jest.fn().mockResolvedValue({ message: 'Service unavailable' }),
    } as unknown as Response);

    await expect(getProducts()).rejects.toMatchObject({
      status: 503,
      message: expect.stringContaining('Service unavailable'),
    });

    global.fetch = jest.fn().mockRejectedValue(new TypeError('Failed to fetch'));

    await expect(getProducts()).rejects.toThrow(/Sem conexão com o servidor/i);
  });
});
