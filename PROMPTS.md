# PROMPTS.md — Registro de uso de IA

Prompts relevantes utilizados durante o desenvolvimento, conforme solicitado no documento do desafio.
A IA utilizada foi o **Cursor (Claude Sonnet 4.5)** integrado ao IDE.

---

**Logs**

> Qual o plano para usar o ILogger com injeção de dependência? e o que vale a pena logar em cada situação?

A IA explicou que o ASP.NET Core já vem com `ILogger<T>` pronto para uso — basta adicioná-lo no construtor da classe. Para o nível de cada log: `Information` para operações normais (requisição recebida, checkout concluído), `Warning` para falhas esperadas de negócio (produto não encontrado, estoque insuficiente) e `Error` para exceções inesperadas. Também sugeriu incluir o `TraceId` da requisição em cada log para facilitar a rastreabilidade.

---

**Validação de entrada**

> Toda validação do meu projeto está espalhada dentro dos controllers com vários `if`. Como eu centralizo isso de um jeito mais limpo sem precisar instalar nada novo?

A IA sugeriu usar atributos de validação diretamente nos DTOs, como `[Required]` para campos obrigatórios e `[Range(1, 100)]` para limitar valores numéricos. Combinado com o atributo `[ApiController]` no controller, o próprio framework verifica esses atributos antes de executar qualquer código e já retorna um `400 Bad Request` com a descrição do problema caso algo esteja errado — sem precisar escrever nenhuma validação manual.

---

**Duplicação de pedido**

> Como eu testo a garantia da idempotência tanto no frontend quanto no backend?

A IA explicou o conceito de idempotência: o frontend gera uma chave única (`Idempotency-Key`) por tentativa de compra e envia no header da requisição. O backend armazena o resultado da primeira vez que essa chave foi processada. Se a mesma chave chegar de novo, devolve o resultado já armazenado sem processar novamente. Foi implementado com um `IIdempotencyStore` usando `ConcurrentDictionary` em memória.

---

**Cobertura dos testes**

> Qual a porcentagem do código que os testes automatizados estão validando? Gere um plano para que eu consiga aumentar o alcance dos testes sem aumentar o número deles.

A IA rodou a coleta de cobertura (`dotnet test --collect:"XPlat Code Coverage"` e `npm test -- --coverage`) e identificou os caminhos não cobertos. O plano foi consolidar testes existentes para cobrir mais branches dentro do mesmo teste, em vez de criar novos arquivos — por exemplo, um único teste de integração que valida sucesso, `404` e `422` em sequência cobre mais código do que três testes unitários separados.

---

**Migração para TypeScript**

> Meu projeto React está em JavaScript puro. Quais são as vantagens de migrar para TypeScript e como eu faria essa migração sem quebrar o que já está funcionando?

A IA explicou que a principal vantagem é detectar erros antes de rodar o código — por exemplo, tentar acessar `produto.qtd` quando a propriedade se chama `produto.quantity` já aparece como erro no editor. Para migrar, renomear os arquivos de `.js` para `.ts` e `.jsx` para `.tsx` e ir corrigindo os erros que o compilador apontar gradualmente. Os tipos compartilhados foram centralizados em `src/types/index.ts`.

---
