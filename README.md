# BeeKind

BeeKind é uma API ASP.NET Core em desenvolvimento para gerenciamento de usuários, contatos e eventos, com autenticação baseada em JWT, Identity e persistência em PostgreSQL via Entity Framework Core.

Este repositório está em evolução contínua. A estrutura, os contratos de API, as regras de negócio e os detalhes de implementação podem mudar ao longo do tempo conforme o projeto cresce.

## Visão Geral

O projeto está organizado em camadas para separar responsabilidades e manter o código mais fácil de manter:

- **Presentation**: controllers, middlewares e helpers de API.
- **Application**: serviços e DTOs usados pela aplicação.
- **Domain**: entidades de negócio.
- **Infrastructure**: acesso a dados, repositórios e integração com o banco.

Hoje a API cobre os seguintes fluxos principais:

- autenticação e gestão de senha;
- CRUD de usuários;
- CRUD de contatos;
- CRUD de eventos vinculados a contatos;
- recuperação de senha por token.

## Tecnologias Utilizadas

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger / OpenAPI
- Swashbuckle
- Injeção de dependência nativa do ASP.NET Core

## Boas Práticas Aplicadas

- Separação em camadas para reduzir acoplamento.
- Uso de DTOs para entrada e saída da API.
- Repositórios e serviços para isolar regras de negócio e persistência.
- Autenticação via JWT para endpoints protegidos.
- Uso de Identity para gerenciamento de credenciais e tokens.
- Middleware global de tratamento de exceções.
- Mapeamento explícito entre usuário da aplicação e `IdentityUserId` para evitar divergência de identificadores.
- Criação automática das roles `AboveAll` e `User` na inicialização.
- Swagger habilitado em ambiente de desenvolvimento com suporte ao Bearer token.
- Migrations versionadas para o banco de dados.

## Estrutura do Projeto

```text
src/
  Application/
    DTOs/
    Services/
  Domain/
    Entities/
    Exceptions/
  Infrastructure/
    Data/
    Repositories/
  Presentation/
    Controllers/
    Helpers/
    Middlewares/
Migrations/
Properties/
Program.cs
```

## Pré-requisitos

- .NET SDK 10
- PostgreSQL
- Acesso a um banco configurado localmente ou em nuvem
- Configuração de e-mail válida para recuperação de senha, caso esse fluxo seja utilizado

## Configuração

Antes de executar, configure as variáveis de ambiente ou os arquivos de ambiente local com:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `EmailSettings:FromEmail`
- `EmailSettings:SmtpHost`
- `EmailSettings:SmtpPort`
- `EmailSettings:SmtpUser`
- `EmailSettings:SmtpPass`

Observação importante: segredos reais não devem ser versionados. O ideal é usar User Secrets, variáveis de ambiente ou outro cofre seguro para credenciais sensíveis.

## Como Executar

1. Restaurar dependências:

```bash
dotnet restore
```

2. Aplicar as migrations dos dois contextos, se necessário:

```bash
dotnet ef database update --context AppDbContext
dotnet ef database update --context IdentityEfDbContext
```

3. Subir a aplicação:

```bash
dotnet run --project BeeKind.csproj
```

Em desenvolvimento, a API normalmente fica disponível em:

- `http://localhost:5444`
- `https://localhost:7049`

O Swagger fica disponível apenas em ambiente de desenvolvimento.

## Autenticação

Os endpoints protegidos usam Bearer Token. Após fazer login, copie o token retornado e use o botão Authorize no Swagger ou envie o cabeçalho:

```http
Authorization: Bearer SEU_TOKEN_AQUI
```

## Principais Endpoints

### Autenticação

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `POST /api/Auth/change-password`
- `POST /api/Auth/reset-password`
- `POST /api/Auth/forgot-password`
- `DELETE /api/Auth/delete-user`

### Usuários

- `GET /api/User`
- `GET /api/User/{id}`
- `POST /api/User`
- `PUT /api/User/{id}`
- `DELETE /api/User/{id}`

### Contatos

- `GET /api/Contact`
- `GET /api/Contact/{id}`
- `POST /api/Contact`
- `PUT /api/Contact/{id}`
- `DELETE /api/Contact/{id}`

### Eventos

- `GET /api/Event`
- `GET /api/Event/{id}`
- `GET /api/Event/contact/{contactId}`
- `POST /api/Event`
- `PUT /api/Event/{id}`
- `DELETE /api/Event/{id}`

## Exemplos de Uso

### Registro

```json
{
  "name": "Maria Silva",
  "email": "maria@exemplo.com",
  "password": "SenhaForte123!",
  "phoneNumber": "11999999999",
  "confirmPassword": "SenhaForte123!"
}
```

### Login

```json
{
  "email": "maria@exemplo.com",
  "password": "SenhaForte123!"
}
```

### Criar Contato

```json
{
  "name": "João Pereira",
  "email": "joao@exemplo.com",
  "phoneNumber": "11988887777"
}
```

### Criar Evento

```json
{
  "contactId": 1,
  "title": "Reunião de acompanhamento",
  "date": "2026-04-04T10:00:00Z",
  "location": "Online",
  "message": "Lembrar de enviar materiais antes da reunião",
  "description": "Evento vinculado a um contato específico"
}
```

## Observações de Arquitetura

- A aplicação usa dois contextos de banco: um para o domínio principal e outro para o Identity.
- O relacionamento entre usuário, contatos e eventos é tratado com chaves e navegações explícitas.
- A API lê o identificador do usuário autenticado a partir das claims para restringir operações por dono do recurso.
- O tratamento de erro é centralizado para padronizar respostas.

## Estado Atual

Este projeto está em desenvolvimento ativo. Mudanças de regra, estrutura, contratos de endpoint e novas funcionalidades podem ser adicionadas ou ajustadas ao longo do tempo.

## Licença

Ainda não definida.
