# CaseCellShop — Desafio Técnico (Nível Júnior | Fullstack)

Implementação do fluxo de checkout de capinhas de celular proposto no desafio técnico da CaseCellShop.

---

## Sobre a stack utilizada

O desafio sugere **Node.js + TypeScript** no back-end. Optei por **C# + ASP.NET Core 9** por ter maior familiaridade com a linguagem, o que me permitiu focar na qualidade da solução em vez de lutar contra a ferramenta. O raciocínio completo está documentado no [ADR-007](#adr-007--c--aspnet-core-para-o-back-end).

O front-end segue a stack sugerida: **React + TypeScript**.

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 16+](https://nodejs.org/) e npm

---

## Como executar

Execute back-end e front-end em terminais separados.

### Terminal 1 — Back-end

```bash
cd backend
dotnet run
```

Aguarde até aparecer no terminal uma linha como:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5111
```

Quando essa linha aparecer, abra `http://localhost:5111/swagger` no navegador para ver a documentação interativa da API.

### Terminal 2 — Front-end

```bash
cd frontend
npm install   # apenas na primeira vez
npm start
```

- Interface: `http://localhost:3000`

> A URL do back-end é configurada em `frontend/.env` via `REACT_APP_API_URL`. O valor padrão já aponta para `http://localhost:5111`.

---

## Rotas da API

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/products` | Lista todos os produtos e o estoque disponível |
| `POST` | `/checkout/cart` | Processa o checkout de um ou mais itens |

### Exemplo de requisição — `POST /checkout/cart`

```json
{
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 1 }
  ]
}
```

### Respostas possíveis

| Status | Situação |
|--------|----------|
| `200 OK` | Compra realizada com sucesso |
| `400 Bad Request` | Payload inválido (carrinho vazio, quantidade ≤ 0, etc.) |
| `404 Not Found` | Produto não encontrado |
| `422 Unprocessable Entity` | Estoque insuficiente |
| `500 Internal Server Error` | Erro inesperado no servidor |

Todos os erros retornam um `traceId` que permite correlacionar a resposta com o log do servidor:

```json
{
  "success": false,
  "error": "Estoque insuficiente.",
  "traceId": "0HN8ABC:00000001"
}
```

---

## Testes automatizados

### Back-end — 14 testes

```bash
cd backend.Tests
dotnet test
```

| Tipo | Arquivo | Testes |
|------|---------|--------|
| Unitário | `CheckoutServiceTests.cs` | 2 |
| Unitário | `ProductServiceTests.cs` | 4 |
| Unitário | `IdempotencyStoreTests.cs` | 1 |
| Unitário | `ErrorHandlingMiddlewareTests.cs` | 2 |
| Integração | `CheckoutControllerTests.cs` | 4 |
| Integração | `ProductsControllerTests.cs` | 1 |

**Cobertura:** ~85,9% de linhas · ~78,6% de branches

### Front-end — 12 testes

```bash
cd frontend
npm test
```

| Arquivo | Testes |
|---------|--------|
| `App.test.tsx` | 2 |
| `CheckoutPage.test.tsx` | 2 |
| `Toast.test.tsx` | 2 |
| `api.test.ts` | 2 |
| `useCart.test.ts` | 4 |

**Cobertura:** ~40,3% de statements

---

## Estrutura do projeto

```
CaseCellShop/
├── backend/
│   ├── Domain/           Entidades, interfaces, exceções
│   ├── Application/      Serviços de negócio e DTOs
│   ├── Infrastructure/   Banco em memória, middleware, repositórios
│   ├── Presentation/     Controllers HTTP
│   └── docs/adr/         Architecture Decision Records
│
├── backend.Tests/
│   ├── Unit/             Testes unitários
│   └── Integration/      Testes de integração (WebApplicationFactory)
│
└── frontend/
    └── src/
        ├── components/   Toast — notificações globais
        ├── hooks/        useCart — estado do carrinho
        ├── pages/        CheckoutPage
        ├── services/     api.ts — comunicação com a API
        └── types/        Interfaces TypeScript
```

---

## Decisões arquiteturais (ADR)

Cada decisão relevante está documentada com Contexto, Decisão tomada e Trade-offs. Os arquivos completos estão em [`backend/docs/adr/`](backend/docs/adr/).

---

### ADR-001 — Arquitetura em Camadas (Clean Architecture)

**Contexto:** Precisava de uma estrutura que separasse responsabilidades e permitisse testar cada parte de forma independente.

**Decisão:** Quatro camadas — `Domain`, `Application`, `Infrastructure`, `Presentation`. Dependências sempre fluem para dentro; a infraestrutura implementa interfaces definidas no domínio.

**Trade-offs:**
- Positivo: fica fácil testar cada parte separada e trocar detalhes técnicos (ex: banco de dados) sem mexer na lógica de negócio.
- Negativo: para um projeto pequeno, cria mais arquivos e pastas do que seria necessário.

---

### ADR-002 — ILogger via Injeção de Dependência

**Contexto:** Precisava de logs rastreáveis sem acoplar o código a uma biblioteca específica.

**Decisão:** `ILogger<T>` nativo do ASP.NET Core.

| Nível | Quando usar |
|-------|-------------|
| `Information` | Requisição recebida, operação concluída |
| `Warning` | Falhas de negócio esperadas (produto não encontrado, estoque insuficiente) |
| `Error` | Exceções não tratadas |

**Trade-offs:**
- Positivo: não precisa instalar nenhuma biblioteca extra; já vem com o ASP.NET Core.
- Negativo: para rastrear uma requisição entre vários serviços diferentes, precisaria de uma configuração adicional.

---

### ADR-003 — Data Annotations para Validação de Entrada

**Contexto:** Os DTOs precisam de validação antes de chegar à lógica de negócio.

**Decisão:** Atributos `[Required]`, `[Range]`, `[MinLength]` nos DTOs + `[ApiController]` nos controllers. O framework retorna `400 Bad Request` automaticamente quando a validação falha, sem código manual.

**Trade-offs:**
- Positivo: a validação fica declarada direto no modelo, sem precisar escrever `if` nos controllers.
- Negativo: não funciona bem para regras mais complexas, como comparar dois campos entre si.

---

### ADR-004 — Middleware de Tratamento de Erros + ApiException

**Contexto:** Sem tratamento centralizado, exceções não tratadas vazariam stack traces para o cliente.

**Decisão:** `ErrorHandlingMiddleware` no início da pipeline captura todas as exceções. `ApiException` carrega o `HttpStatusCode` da falha de negócio; exceções genéricas sempre retornam `500`. Todas as respostas incluem `traceId`.

**Trade-offs:**
- Positivo: todos os erros da API seguem o mesmo formato; o `traceId` ajuda a encontrar o erro no log sem mostrar informações sensíveis ao cliente.
- Negativo: se o próprio middleware tiver um erro, não tem como capturá-lo da mesma forma.

---

### ADR-005 — CheckoutResult Enum como Retorno Tipado do Serviço

**Contexto:** O serviço retornava `bool`. Um `false` poderia ser "produto não encontrado" ou "estoque insuficiente" — o controller não conseguia distinguir sem uma segunda consulta.

**Decisão:** Enum `CheckoutResult { Success, ProductNotFound, InsufficientStock }`. O controller mapeia cada valor para o status HTTP correto via `if/else`. Adicionar um novo valor ao enum sem tratar no controller é visível imediatamente.

**Trade-offs:**
- Positivo: o controller sabe exatamente o que aconteceu e retorna o status HTTP certo (404 vs 422); se esquecer de tratar um novo caso, o compilador avisa.
- Negativo: para adicionar um novo tipo de erro, precisa alterar o enum, o serviço, o controller e os testes.

---

### ADR-006 — TypeScript no lugar de JavaScript no Front-end

**Contexto:** O projeto começou em JavaScript. Com a adição de hooks, serviços e interfaces de produto/carrinho, erros de contrato entre API e front-end só eram detectados em tempo de execução.

**Decisão:** Migração para TypeScript com `strict: true`. Contratos formalizados em `src/types/index.ts`.

**Trade-offs:**
- Positivo: erros como acessar uma propriedade que não existe aparecem antes de rodar o código; renomear algo no IDE atualiza todos os lugares automaticamente.
- Negativo: alguns pacotes mais antigos tiveram conflito de versão na instalação; as anotações de tipo deixam o código um pouco mais longo.

---

### ADR-007 — C# / ASP.NET Core para o Back-end

**Contexto:** O desafio sugere Node.js + TypeScript. Era necessário justificar a escolha diferente.

**Decisão:** ASP.NET Core 9 com C#. A motivação principal foi a familiaridade prévia — isso permitiu focar nos critérios do desafio (tratamento de erros, validação, testes, documentação) em vez de aprender a stack ao mesmo tempo.

**Trade-offs:**
- Positivo: por já conhecer a linguagem, consegui focar nos critérios do desafio em vez de aprender a stack ao mesmo tempo.
- Negativo: é mais pesado que Node.js para uma API simples e exige instalar o SDK .NET na máquina.
