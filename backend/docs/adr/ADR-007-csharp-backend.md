# ADR-007: C# / ASP.NET Core para o Backend

**Status:** Aceito
**Data:** 2026-05-24

## Contexto

Era necessário escolher uma linguagem e framework para o servidor da API REST. As alternativas consideradas foram Node.js (Express/Fastify), Python (FastAPI) e C# (ASP.NET Core). O principal critério foi a produtividade da equipe, seguida de ecossistema de testes e suporte a tipagem estática.

## Decisão

Adotar **C# com ASP.NET Core 9**. A escolha foi motivada principalmente pela familiaridade prévia com a linguagem, o que reduz o tempo de onboarding e o risco de erros conceituais na estruturação do projeto.

## Trade-offs

**Positivo:**
- Familiaridade com a linguagem acelera o desenvolvimento e a resolução de problemas.
- Tipagem estática forte previne classes inteiras de erros em tempo de compilação.
- Container de DI nativo (`WebApplication.CreateBuilder`) elimina a necessidade de bibliotecas externas para injeção de dependência.
- Ecossistema de testes maduro: xUnit + Moq + `WebApplicationFactory` para testes de integração sem infraestrutura externa.
- Swagger/OpenAPI gerado automaticamente via `Swashbuckle` com suporte a comentários XML.

**Negativo:**
- Runtime mais pesado em comparação a Node.js ou Go para uma API de escala pequena.
- Requer instalação do SDK .NET na máquina de desenvolvimento e no servidor.
- Menos comum em times com perfil predominantemente front-end, o que pode dificultar contribuições de outros membros.
