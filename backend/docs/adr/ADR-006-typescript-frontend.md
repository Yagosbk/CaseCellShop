# ADR-006: TypeScript no lugar de JavaScript no Frontend

**Status:** Aceito
**Data:** 2026-05-24

## Contexto

O frontend foi iniciado em JavaScript puro com React. À medida que o projeto cresceu — adicionando hooks, serviços de API, tipos de produto e carrinho — a ausência de tipagem estática tornou difícil detectar incompatibilidades entre o contrato da API e os objetos consumidos pelo frontend em tempo de desenvolvimento, exigindo testes manuais para encontrar erros de propriedade.

## Decisão

Migrar o frontend para TypeScript com `strict: true` ativado no `tsconfig.json`. Todos os arquivos `.js` e `.jsx` foram convertidos para `.ts` e `.tsx`. Os contratos entre API e frontend foram formalizados em `src/types/index.ts`:

```ts
export interface Product {
  id: number;
  name: string;
  price: number;
  stock: number;
}

export interface CartItem extends Product {
  quantity: number;
}
```

## Trade-offs

**Positivo:**
- Erros de contrato (ex: acessar `product.qtd` em vez de `product.quantity`) são capturados em tempo de compilação, antes de chegar ao browser.
- Autocompletar e navegação de código no IDE melhoram significativamente.
- Refatorações (renomear propriedades, mudar assinaturas de funções) ficam seguras — o compilador aponta todos os pontos afetados.

**Negativo:**
- Etapa adicional de compilação; projetos pequenos podem não justificar o overhead de configuração.
- Alguns pacotes mais antigos exigiram `--legacy-peer-deps` para instalar os `@types/*` correspondentes.
- Anotações de tipo adicionam pequeno volume ao código (especialmente em generics como `useState<Product[]>`).
